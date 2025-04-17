using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;
using Tiss_HealthQuestionnaire.Models;
using Tiss_HealthQuestionnaire.ViewModels;

namespace Tiss_HealthQuestionnaire.Controllers
{
    public class AdminUserController : BaseAuthController
    {
        protected override string[] AllowedRoles => new[] { "admin" };

        private HealthQuestionnaireEntities _db = new HealthQuestionnaireEntities();

        #region 使用者清單總覽
        public ActionResult UserList()
        {
            var users = GetFilteredUsers(null);

            ViewBag.TeamList = new SelectList(_db.Team.OrderBy(t => t.TeamName).ToList(), "TeamID", "TeamName");

            return View(users);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UserList(int? teamId)
        {
            try
            {
                var users = GetFilteredUsers(teamId);

                ViewBag.TeamList = new SelectList(_db.Team.OrderBy(t => t.TeamName).ToList(), "TeamID", "TeamName", teamId);

                return View(users);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region 角色清單篩選方法
        private List<AdminUserViewModel> GetFilteredUsers(int? teamId)
        {
            var userQuery = _db.SystemUser.AsQueryable();

            if (teamId.HasValue)
            {
                var athleteIds = _db.AthleteTeam.Where(at => at.TeamID == teamId.Value).Select(at => at.AthleteID).ToList();

                var userIds = _db.AthleteProfile.Where(ap => athleteIds.Contains(ap.AthleteID)).Select(ap => ap.UserID).ToList();

                userQuery = userQuery.Where(u => u.UserRole.RoleName == "athlete" && userIds.Contains(u.UserID));
            }
            else
            {
                userQuery = userQuery.Where(u => u.UserRole.RoleName != "athlete");
            }

            var result = userQuery.Select(u => new AdminUserViewModel
            {
                UserID = u.UserID,
                UserName = u.UserName,
                Email = u.Email,
                RoleName = u.UserRole.RoleName,
                IsActive = u.IsActive,
                CreatedDate = u.CreatedDate
            }).ToList();

            return result;
        }
        #endregion

        #region 切換帳號啟用狀態
        [HttpPost]
        public ActionResult ToggleUserStatus(int id)
        {
            var user = _db.SystemUser.Find(id);
            if (user == null) return HttpNotFound();

            user.IsActive = !user.IsActive;

            if (user.IsActive && user.IsApproved == false)
            {
                user.IsApproved = true;
            }


            _db.SaveChanges();

            _db.SystemLog.Add(new SystemLog
            {
                UserID = user.UserID,
                Action = "切換帳號狀態",
                Target = user.UserName,
                Message = user.IsActive ? "啟用帳號" : "停用帳號",
                LogDate = DateTime.Now
            });
            _db.SaveChanges();

            return RedirectToAction("UserList");
        }
        #endregion

        #region 使用者角色編輯
        public ActionResult EditUserRole(int id)
        {
            var user = _db.SystemUser.Find(id);
            if (user == null) return HttpNotFound();

            ViewBag.RoleList = new SelectList(_db.UserRole.ToList()
                .Select(r => new
                {
                    RoleID = r.RoleID,
                    RoleDisplayName = GetRoleDisplayName(r.RoleName)  // 轉換成中文
                }),
                    "RoleID", "RoleDisplayName", user.RoleID);

            var model = new AdminEditRoleViewModel
            {
                UserID = user.UserID,
                UserName = user.UserName,
                RoleID = user.RoleID
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditUserRole(AdminEditRoleViewModel model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.RoleList = new SelectList(_db.UserRole.ToList()
                    .Select(r => new
                    {
                        RoleID = r.RoleID,
                        RoleDisplayName = GetRoleDisplayName(r.RoleName)
                    }), "RoleID", "RoleDisplayName", model.RoleID);

                return View(model);
            }

            var user = _db.SystemUser.Find(model.UserID);
            if (user == null) return HttpNotFound();

            user.RoleID = model.RoleID;
            _db.SaveChanges();

            _db.SystemLog.Add(new SystemLog
            {
                UserID = user.UserID,
                Action = "修改角色",
                Target = user.UserName,
                Message = $"角色變更為 {_db.UserRole.Find(model.RoleID)?.RoleName}",
                LogDate = DateTime.Now
            });
            _db.SaveChanges();

            return RedirectToAction("UserList");
        }

        private string GetRoleDisplayName(string role)
        {
            switch (role?.ToLower())
            {
                case "athlete": return "選手";
                case "trainer": return "防護員";
                case "admin": return "管理員";
                default: return role;
            }
        }
        #endregion

        #region 審核防護員註冊
        public ActionResult PendingTrainerList()
        {
            var trainerRoleId = _db.UserRole.Where(r => r.RoleName.ToLower() == "trainer")
                                .Select(r => r.RoleID).FirstOrDefault();

            var pendingUsers = _db.SystemUser
                .Where(u => u.RoleID == trainerRoleId && u.IsApproved == false)
                .Select(u => new AdminUserViewModel
                {
                    UserID = u.UserID,
                    UserName = u.UserName,
                    Email = u.Email,
                    CreatedDate = u.CreatedDate
                }).ToList();

            return View(pendingUsers);
        }

        [HttpPost]
        public ActionResult ApproveTrainer(int id)
        {
            var user = _db.SystemUser.Find(id);
            if (user == null) return HttpNotFound();

            user.IsApproved = true;

            _db.SystemLog.Add(new SystemLog
            {
                UserID = user.UserID,
                Action = "審核防護員",
                Message = $"帳號 {user.UserName} 已通過審核",
                Target = user.UserName,
                LogDate = DateTime.Now
            });

            _db.SaveChanges();
            return RedirectToAction("PendingTrainerList");
        }
        #endregion

        #region 批量批准拒絕
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult BatchApproveReject(List<int> SelectedUserIDs, string actionType, string[] RejectReasons)
        {
            if (SelectedUserIDs == null || !SelectedUserIDs.Any())
            {
                TempData["ErrorMessage"] = "請至少勾選一位使用者";
                return RedirectToAction("PendingTrainerList");
            }

            for (int i = 0; i < SelectedUserIDs.Count; i++)
            {
                int userId = SelectedUserIDs[i];
                var user = _db.SystemUser.Find(userId);
                if (user == null) continue;

                if (actionType == "approve")
                {
                    user.IsApproved = true;
                    _db.SystemLog.Add(new SystemLog
                    {
                        UserID = user.UserID,
                        Action = "核准帳號",
                        Target = user.UserName,
                        Message = "防護員帳號已核准",
                        LogDate = DateTime.Now
                    });
                }
                else if (actionType == "reject")
                {
                    string reason = (RejectReasons != null && RejectReasons.Length > i) ? RejectReasons[i] : "未填寫原因";

                    _db.SystemLog.Add(new SystemLog
                    {
                        UserID = user.UserID,
                        Action = "拒絕帳號",
                        Target = user.UserName,
                        Message = $"防護員帳號已拒絕，原因：{reason}",
                        LogDate = DateTime.Now
                    });

                    var trainer = _db.TrainerProfile.FirstOrDefault(t => t.UserID == userId);
                    if (trainer != null)
                    {
                        _db.TrainerProfile.Remove(trainer);
                    }
                    _db.SystemUser.Remove(user);
                }
            }

            _db.SaveChanges();
            TempData["SuccessMessage"] = actionType == "approve" ? "已成功核准帳號" : "已拒絕並刪除帳號";
            return RedirectToAction("PendingTrainerList");
        }
        #endregion

        #region 批次建立選手資料
        public ActionResult CreateAthlete()
        {
            ViewBag.GenderList = _db.Gender
        .ToList()
        .Select(g => new
        {
            GenderID = g.GenderID,
            GenderName = g.Male == true ? "男" : "女"
        }).ToList();

            var model = new BatchAthleteCreateViewModel
            {
                Athletes = Enumerable.Range(0, 5).Select(_ => new SingleAthleteViewModel()).ToList()
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateAthlete(BatchAthleteCreateViewModel model)
        {
            if (!ModelState.IsValid || model.Athletes == null || !model.Athletes.Any())
            {
                ViewBag.GenderList = _db.Gender.ToList().Select(g => new
                {
                    GenderID = g.GenderID,
                    GenderName = g.Male.HasValue ? (g.Male.Value ? "男" : "女") : "未指定"
                }).ToList();

                return View(model);
            }

            var athleteRoleId = _db.UserRole.FirstOrDefault(r => r.RoleName == "Athlete")?.RoleID ?? 1;
            const string defaultPassword = "123456";

            foreach (var a in model.Athletes)
            {
                if (string.IsNullOrWhiteSpace(a.AthleteNumber) || string.IsNullOrWhiteSpace(a.Name))
                    continue;

                var salt = GenerateSalt();
                var pwdToHash = string.IsNullOrWhiteSpace(a.Password) ? defaultPassword : a.Password;
                var hashedPwd = ComputeSha256Hash(pwdToHash, salt);

                var systemUser = new SystemUser
                {
                    UserName = a.Name,
                    Password = hashedPwd,
                    Salt = salt,
                    Email = a.Email,
                    RoleID = athleteRoleId,
                    IsActive = true,
                    CreatedDate = DateTime.Now
                };
                _db.SystemUser.Add(systemUser);
                _db.SaveChanges();

                _db.AthleteProfile.Add(new AthleteProfile
                {
                    UserID = systemUser.UserID,
                    AthleteNumber = a.AthleteNumber,
                    Name = a.Name,
                    BirthDate = a.BirthDate,
                    GenderID = a.GenderID,
                    SportSpecialization = a.SportSpecialization
                });

                _db.SystemLog.Add(new SystemLog
                {
                    UserID = systemUser.UserID,
                    Action = "批次新增選手",
                    Message = $"由管理員新增選手：{a.Name}",
                    Target = systemUser.UserName,
                    LogDate = DateTime.Now
                });
            }

            _db.SaveChanges();
            TempData["SuccessMessage"] = "選手資料已成功建立";
            return RedirectToAction("UserList");
        }
        #endregion

        #region 修改全部角色的密碼
        public ActionResult ChangeUsersPwd()
        {
            ViewBag.RoleList = new List<SelectListItem>
            {
                new SelectListItem { Text = "選手", Value = "athlete" },
                new SelectListItem { Text = "防護員", Value = "trainer" }
            };

            ViewBag.AccountList = new List<SelectListItem>(); // 初始為空
            return View(new ChangeUserPwdViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ChangeUsersPwd(ChangeUserPwdViewModel model, string doChange)
        {
            ViewBag.RoleList = new List<SelectListItem>
    {
        new SelectListItem { Text = "選手", Value = "athlete" },
        new SelectListItem { Text = "防護員", Value = "trainer" }
    };

            // 若只改變角色下拉選單，就回傳帳號清單，不進行密碼變更
            if (string.IsNullOrEmpty(doChange))
            {
                if (model.SelectedRole == "athlete")
                {
                    ViewBag.AccountList = _db.AthleteProfile.ToList()
                    .Select(a => new SelectListItem
                    {
                        Value = a.AthleteNumber,
                        Text = $"{a.AthleteNumber} - {a.Name}"
                    }).ToList();

                }
                else if (model.SelectedRole == "trainer")
                {
                    ViewBag.AccountList = _db.TrainerProfile.ToList()
                    .Select(t => new SelectListItem
                    {
                        Value = t.ATName,
                        Text = $"{t.ATName} - {t.TrainerEmail}"
                    }).ToList();
                }
                else
                {
                    ViewBag.AccountList = new List<SelectListItem>();
                }

                return View(model);
            }

            // 驗證表單欄位
            if (string.IsNullOrWhiteSpace(model.SelectedRole) || string.IsNullOrWhiteSpace(model.SelectedAccount) ||
                string.IsNullOrWhiteSpace(model.NewPassword) || string.IsNullOrWhiteSpace(model.ConfirmPassword))
            {
                ViewBag.ErrorMessage = "請填寫完整欄位";
                return View(model);
            }

            if (model.NewPassword != model.ConfirmPassword)
            {
                ViewBag.ErrorMessage = "兩次密碼不一致";
                return View(model);
            }

            if (model.NewPassword.Length < 6)
            {
                ViewBag.ErrorMessage = "密碼長度至少 6 位";
                return View(model);
            }

            // 執行變更密碼
            var salt = GenerateSalt();
            var hash = ComputeSha256Hash(model.NewPassword, salt);
            SystemUser user = null;

            if (model.SelectedRole == "athlete")
            {
                var athlete = _db.AthleteProfile.FirstOrDefault(a => a.AthleteNumber == model.SelectedAccount);
                if (athlete == null)
                {
                    ViewBag.ErrorMessage = "找不到該選手帳號";
                    return View(model);
                }
                user = _db.SystemUser.Find(athlete.UserID);
            }
            else if (model.SelectedRole == "trainer")
            {
                var trainer = _db.TrainerProfile.FirstOrDefault(t => t.ATName == model.SelectedAccount);
                if (trainer == null)
                {
                    ViewBag.ErrorMessage = "找不到該防護員帳號";
                    return View(model);
                }
                user = _db.SystemUser.Find(trainer.UserID);
            }

            if (user == null)
            {
                ViewBag.ErrorMessage = "找不到對應的使用者";
                return View(model);
            }

            user.Salt = salt;
            user.Password = hash;

            _db.SystemLog.Add(new SystemLog
            {
                UserID = user.UserID,
                Action = "管理員變更密碼",
                Target = user.UserName,
                Message = $"由管理員重設 {model.SelectedRole} 密碼",
                LogDate = DateTime.Now
            });

            _db.SaveChanges();

            TempData["SuccessMessage"] = "密碼重設成功";

            return RedirectToAction("ChangeUsersPwd");
        }
        #endregion

        #region 密碼加密 + Salt
        private static string GenerateSalt()
        {
            var bytes = new byte[16];
            using (var rng = new RNGCryptoServiceProvider())
            {
                rng.GetBytes(bytes);
            }
            return Convert.ToBase64String(bytes);
        }

        private static string ComputeSha256Hash(string input, string salt)
        {
            using (var sha256 = SHA256.Create())
            {
                var raw = Encoding.UTF8.GetBytes(salt + input);
                var bytes = sha256.ComputeHash(raw);
                return BitConverter.ToString(bytes).Replace("-", "").ToLowerInvariant();
            }
        }
        #endregion

        #region 密碼加密
        private static string ComputeSha256Hash(string rawData)
        {
            using (SHA256 sha256Hash = SHA256.Create())
            {
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(rawData));

                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }
        #endregion
    }
}