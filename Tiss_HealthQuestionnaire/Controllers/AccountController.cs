using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using Tiss_HealthQuestionnaire.Models;
using static Tiss_HealthQuestionnaire.Models.AccountViewModel;
using System.Security.Principal;
using System.Data;
using System.Text.RegularExpressions;

namespace Tiss_HealthQuestionnaire.Controllers
{
    public class AccountController : Controller
    {
        private HealthQuestionnaireEntities _db = new HealthQuestionnaireEntities();

        #region 註冊身分選擇
        public ActionResult SelectRole(string role)
        {
            switch (role?.ToLower())
            {
                case "athlete":
                    return RedirectToAction("RegisterAthlete");
                case "trainer":
                    return RedirectToAction("RegisterTrainer");
                case "admin":
                    return RedirectToAction("RegisterAdmin");
                default:
                    return View();
            }
        }
        #endregion

        #region 選手註冊
        public ActionResult RegisterAthlete()
        {
            ViewBag.GenderList = _db.Gender.ToList();
            return View();
        }

        [HttpPost]
        public ActionResult RegisterAthlete(string athleteNumber, string userName, string password, string email, string sportSpecialization, DateTime birthDate, int genderID)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(password) || password.Length < 6)
                {
                    ViewBag.ErrorMessage = "密碼長度至少為6位數";
                    ViewBag.GenderList = _db.Gender.ToList();
                    return View();
                }

                if (_db.SystemUser.Any(u => u.UserName == userName))
                {
                    ViewBag.ErrorMessage = "此帳號名稱已被使用";
                    ViewBag.GenderList = _db.Gender.ToList();
                    return View();
                }

                // 電子郵件格式檢查
                var emailPattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
                if (!Regex.IsMatch(email, emailPattern))
                {
                    ViewBag.ErrorMessage = "電子郵件格式不正確";
                    return View();
                }

                var salt = GenerateSalt();
                var hashedPwd = ComputeSha256Hash(password, salt);

                var systemUser = new SystemUser
                {
                    UserName = userName,
                    Password = hashedPwd,
                    Salt = salt,
                    Email = email,
                    RoleID = 1, //選手角色
                    IsActive = true,
                    CreatedDate = DateTime.Now
                };
                _db.SystemUser.Add(systemUser);
                _db.SaveChanges();

                var userId = systemUser.UserID;
                var athlete = new AthleteProfile
                {
                    UserID = userId,
                    AthleteNumber = athleteNumber,
                    Name = userName,
                    BirthDate = birthDate,
                    GenderID = genderID,
                    SportSpecialization = sportSpecialization
                };
                _db.AthleteProfile.Add(athlete);
                _db.SaveChanges();

                _db.SystemLog.Add(new SystemLog
                {
                    UserID = userId,
                    Action = "RegisterAthlete",
                    Message = "新選手註冊成功",
                    LogDate = DateTime.Now
                });

                _db.SaveChanges();

                TempData["RegisterMessage"] = "註冊成功，請登入。";
                return RedirectToAction("Login");
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "系統錯誤: 註冊失敗";
                ViewBag.GenderList = _db.Gender.ToList();     
                return View("RegisterAthlete");
                throw ex;
            }
        }
        #endregion

        #region 防護員註冊
        public ActionResult RegisterTrainer()
        {
            return View();
        }

        [HttpPost]
        public ActionResult RegisterTrainer(string userName, string password, string title, string department, string expertise, string TrainerEmail)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(password) || password.Length < 6)
                {
                    ViewBag.ErrorMessage = "密碼長度至少需6位";
                    return View();
                }

                if (_db.SystemUser.Any(u => u.UserName == userName))
                {
                    ViewBag.ErrorMessage = "帳號已存在";
                    return View();
                }

                // 電子郵件格式檢查
                var emailPattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
                if (!Regex.IsMatch(TrainerEmail, emailPattern))
                {
                    ViewBag.ErrorMessage = "電子郵件格式不正確";
                    return View();
                }

                var salt = GenerateSalt();
                var hashedPwd = ComputeSha256Hash(password, salt);

                var trainerRoleId = _db.UserRole.FirstOrDefault(r => r.RoleName == "Trainer")?.RoleID;
                if (trainerRoleId == null)
                {
                    ViewBag.ErrorMessage = "系統錯誤：找不到防護員角色";
                    return View();
                }

                var systemUser = new SystemUser
                {
                    UserName = userName,
                    Password = hashedPwd,
                    Salt = salt,
                    Email = TrainerEmail,
                    RoleID = trainerRoleId.Value,
                    IsActive = true,
                    IsApproved = false,
                    CreatedDate = DateTime.Now
                };

                _db.SystemUser.Add(systemUser);
                _db.SaveChanges();

                var trainerProfile = new TrainerProfile
                {
                    TrainerID = systemUser.UserID,
                    UserID = systemUser.UserID,
                    ATName = userName,
                    Title = title,
                    Department = department,
                    Expertise = expertise,
                    TrainerEmail = TrainerEmail,
                };

                _db.TrainerProfile.Add(trainerProfile);

                _db.SystemLog.Add(new SystemLog
                {
                    UserID = systemUser.UserID,
                    Action = "RegisterTrainer",
                    Target = userName,
                    Message = "註冊新防護員帳號",
                    LogDate = DateTime.Now
                });

                _db.SaveChanges();

                TempData["RegisterMessage"] = "防護員身份註冊成功，請待管理員審核";
                return RedirectToAction("SelectRole");
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "系統錯誤: 註冊失敗";
                return View("RegisterTrainer");
                throw ex;
            }
        }
        #endregion

        #region 管理員註冊
        public ActionResult RegisterAdmin()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult RegisterAdmin(string userName, string email, string password)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(password) || password.Length < 6)
                {
                    ViewBag.ErrorMessage = "密碼長度至少需6位";
                    return View("RegisterAdmin");
                }

                if (_db.SystemUser.Any(u => u.UserName == userName))
                {
                    ViewBag.ErrorMessage = "帳號已存在";
                    return View("RegisterAdmin");
                }

                var emailPattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
                if (!Regex.IsMatch(email, emailPattern))
                {
                    ViewBag.ErrorMessage = "電子郵件格式不正確";
                    return View();
                }

                var salt = GenerateSalt();
                var hashedPassword = ComputeSha256Hash(password, salt);

                int adminRoleId = _db.UserRole.FirstOrDefault(r => r.RoleName == "Admin")?.RoleID ?? 0;
                if (adminRoleId == 0)
                {
                    ViewBag.ErrorMessage = "尚未設定管理員角色，請聯絡系統管理者";
                    return View("RegisterAdmin");
                }

                var newSystemUser = new SystemUser
                {
                    UserName = userName,
                    Password = hashedPassword,
                    Salt = salt,
                    Email = email,
                    RoleID = adminRoleId,
                    CreatedDate = DateTime.Now,
                    IsActive = true
                };

                _db.SystemUser.Add(newSystemUser);
                _db.SaveChanges();

                _db.AdminProfile.Add(new AdminProfile
                {
                    UserID = newSystemUser.UserID,
                    AdminName = userName
                });
                _db.SaveChanges();

                _db.SystemLog.Add(new SystemLog
                {
                    UserID = newSystemUser.UserID,
                    Action = "註冊管理員",
                    Target = userName,
                    Message = "成功註冊管理員帳號",
                    LogDate = DateTime.Now
                });
                _db.SaveChanges();

                TempData["RegisterMessage"] = "管理員註冊成功，請登入";
                return RedirectToAction("Login", "Account");
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "系統錯誤: 註冊失敗";
                return View("RegisterAdmin");

                throw ex;
            }
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

        #region 登入
        public ActionResult Login(string role)
        {
            if (string.IsNullOrEmpty(role)) return RedirectToAction("SelectRole");

            if (role != "athlete" && role != "trainer" && role != "admin")
                return RedirectToAction("SelectRole");

            ViewBag.Role = role;
            SetRoleViewBag(role);

            return View("Login");
        }

        [HttpPost]
        public ActionResult Login(string role, string account, string pwd)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(role) || string.IsNullOrWhiteSpace(account) || string.IsNullOrWhiteSpace(pwd))
                {
                    ViewBag.ErrorMessage = "請填寫所有欄位";
                    ViewBag.Role = role;
                    SetRoleViewBag(role);
                    return View("Login");
                }

                SystemUser user = null;

                if (role == "athlete")
                {
                    var athlete = _db.AthleteProfile.FirstOrDefault(a => a.AthleteNumber == account);
                    if (athlete != null)
                    {
                        user = _db.SystemUser.FirstOrDefault(u => u.UserID == athlete.UserID);
                    }
                }
                else
                {
                    user = _db.SystemUser.FirstOrDefault(u => u.UserName == account);
                }

                if (user == null)
                {
                    ViewBag.ErrorMessage = "查無此帳號";
                    ViewBag.Role = role;
                    SetRoleViewBag(role);
                    return View("Login");
                }

                if (_db.UserRole.FirstOrDefault(r => r.RoleID == user.RoleID)?.NeedApproval == true && !user.IsApproved)
                {
                    ViewBag.ErrorMessage = "您的帳號尚未通過審核，請聯繫管理員。";
                    ViewBag.Role = role;
                    SetRoleViewBag(role);
                    return View("Login");
                }

                if (!user.IsActive)
                {
                    ViewBag.ErrorMessage = "帳號已被停用，請聯繫管理員。";
                    SetRoleViewBag(role);
                    ViewBag.Role = role;
                    return View("Login");
                }

                var hashedInputPwd = ComputeSha256Hash(pwd, user.Salt);
                if (user.Password != hashedInputPwd)
                {
                    ViewBag.ErrorMessage = "密碼錯誤";
                    SetRoleViewBag(role);
                    ViewBag.Role = role;
                    return View("Login");
                }

                var roleName = _db.UserRole.FirstOrDefault(r => r.RoleID == user.RoleID)?.RoleName;
                Session["UserRole"] = roleName?.ToLower();
                Session["UserName"] = user.UserName;
                Session["UserID"] = user.UserID;

                _db.SystemLog.Add(new SystemLog
                {
                    UserID = user.UserID,
                    Action = "登入",
                    Message = $"使用者 {user.UserName} 成功登入 ({roleName})",
                    LogDate = DateTime.Now
                });
                _db.SaveChanges();

                switch (roleName?.ToLower())
                {
                    case "athlete":
                        return RedirectToAction("Main", "Questionnaire");
                    case "trainer":
                        return RedirectToAction("ConcussionMedicalEvaluation", "MedicalEvaluation");
                    case "admin":
                        return RedirectToAction("UserList", "AdminUser");
                    default:
                        ViewBag.ErrorMessage = "身份錯誤";
                        ViewBag.Role = role;
                        SetRoleViewBag(role);
                        return View("Login");
                }
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "錯誤: 查無此帳號";
                SetRoleViewBag(role);
                return View("Error");

                throw ex;
            }
        }
        #endregion

        #region 登入頁身份欄位顯示唯讀_共用方法
        private void SetRoleViewBag(string role)
        {
            ViewBag.Role = role;
            ViewBag.RoleName = role == "athlete" ? "選手" : role == "trainer" ? "防護員" : "管理員";
        }
        #endregion

        #region 重設定密碼
        [HttpGet]
        public ActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        public ActionResult ForgotPassword(string role, string account, string newPassword, string confirmPassword)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(role) || string.IsNullOrWhiteSpace(account) ||
                    string.IsNullOrWhiteSpace(newPassword) || string.IsNullOrWhiteSpace(confirmPassword))
                {
                    ViewBag.ErrorMessage = "請填寫完整欄位";
                    return View();
                }

                if (newPassword != confirmPassword)
                {
                    ViewBag.ErrorMessage = "兩次輸入的密碼不一致";
                    return View();
                }

                if (newPassword.Length < 6)
                {
                    ViewBag.ErrorMessage = "密碼長度至少需6位";
                    return View();
                }

                var newSalt = GenerateSalt();
                var newHashedPwd = ComputeSha256Hash(newPassword, newSalt);

                if (role == "athlete")
                {
                    var user = _db.AthleteUser.FirstOrDefault(u => u.AthleteNumber == account);
                    if (user == null)
                    {
                        ViewBag.ErrorMessage = "查無此選手帳號";
                        return View();
                    }

                    user.Salt = newSalt;
                    user.Password = newHashedPwd;
                }
                else if (role == "trainer")
                {
                    var trainer = _db.AthleticTrainer.FirstOrDefault(t => t.ATName == account);
                    if (trainer == null)
                    {
                        ViewBag.ErrorMessage = "查無此防護員帳號";
                        return View();
                    }

                    trainer.Salt = newSalt;
                    trainer.Password = newHashedPwd;
                }
                else
                {
                    ViewBag.ErrorMessage = "身份資訊錯誤";
                    return View();
                }

                _db.SaveChanges();
                ViewBag.Message = "密碼重設成功，請重新登入";
                return View();
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "系統錯誤: " + ex.Message;
                return View();
            }
        }
        #endregion

        #region 登出
        public ActionResult Logout()
        {
            Session.Remove("LoggedIn");

            FormsAuthentication.SignOut();

            string returnUrl = Request.UrlReferrer != null ? Request.UrlReferrer.ToString() : Url.Action("Login", "Account");

            return Redirect(returnUrl);
        }
        #endregion
    }
}