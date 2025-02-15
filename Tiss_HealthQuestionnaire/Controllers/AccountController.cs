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

namespace Tiss_HealthQuestionnaire.Controllers
{
    public class AccountController : Controller
    {
        private HealthQuestionnaireEntities _db = new HealthQuestionnaireEntities();

        #region 註冊
        public ActionResult Register()
        {
            ViewBag.GenderList = _db.Gender.ToList();

            int nextAthleteID = 1; // 初始編號
            var maxAthleteID = _db.AthleteUser.Max(a => (int?)a.AthleteID);
            if (maxAthleteID.HasValue)
            {
                nextAthleteID = maxAthleteID.Value + 1;
            }

            string formattedAthleteID = nextAthleteID.ToString("D5");

            Session["NextAthleteID"] = formattedAthleteID;

            ViewBag.AthleteID = formattedAthleteID;

            return View();
        }

        [HttpPost]
        public ActionResult Register(string athleteNumber, string userName, string pwd, string email, int genderID, string sportSpecialization, DateTime birthDate)
        {
            try
            {
                // 確認從 Session 獲取保存的 AthleteID
                if (Session["NextAthleteID"] == null)
                {
                    ViewBag.ErrorMessage = "註冊過程中發生錯誤，請重新嘗試。";
                    ViewBag.GenderList = _db.Gender.ToList();
                    return View();
                }

                // 檢查從 Session 中獲取的 AthleteID 是否為數字
                if (!int.TryParse(Session["NextAthleteID"].ToString(), out int athleteID))
                {
                    ViewBag.ErrorMessage = "編號格式錯誤，請重新註冊。";
                    ViewBag.GenderList = _db.Gender.ToList();
                    return View();
                }

                // 密碼長度驗證
                if (string.IsNullOrWhiteSpace(pwd) || pwd.Length < 6)
                {
                    ViewBag.ErrorMessage = "密碼長度至少要6位數";
                    ViewBag.GenderList = _db.Gender.ToList();
                    return View();
                }

                // 檢查帳號是否已存在
                if (_db.AthleteUser.Any(u => u.Name == userName))
                {
                    ViewBag.ErrorMessage = "該帳號已存在";
                    ViewBag.GenderList = _db.Gender.ToList();
                    return View();
                }

                // Email 格式驗證
                var emailRegex = new System.Text.RegularExpressions.Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$");
                if (!emailRegex.IsMatch(email))
                {
                    ViewBag.ErrorMessage = "無效的 Email 格式";
                    ViewBag.GenderList = _db.Gender.ToList();
                    return View();
                }

                // 檢查 AthleteNumber 是否有提供
                if (string.IsNullOrWhiteSpace(athleteNumber))
                {
                    ViewBag.ErrorMessage = "選手編號必須填寫。";
                    ViewBag.GenderList = _db.Gender.ToList();
                    return View();
                }

                // 密碼加密處理
                var encryptedPwd = ComputeSha256Hash(pwd);

                // 準備插入新用戶
                var newUser = new AthleteUser
                {
                    AthleteID = athleteID,
                    AthleteNumber = athleteNumber,
                    Name = userName,
                    Password = encryptedPwd,
                    Email = email,
                    SportSpecialization = sportSpecialization,
                    BirthDate = birthDate,
                    CreatedDate = DateTime.Now,
                    IsActive = true,
                    Registered = false,
                    GenderID = genderID,
                };

                // 新增至資料庫並儲存
                _db.AthleteUser.Add(newUser);
                _db.SaveChanges();

                // 註冊完成後清除 Session 中的編號
                Session.Remove("NextAthleteID");

                return RedirectToAction("Login");
            }
            catch (DbEntityValidationException ex)
            {
                foreach (var validationErrors in ex.EntityValidationErrors)
                {
                    foreach (var validationError in validationErrors.ValidationErrors)
                    {
                        Console.WriteLine($"Property: {validationError.PropertyName} Error: {validationError.ErrorMessage}");
                    }
                }
                ViewBag.ErrorMessage = "資料驗證失敗，請檢查所有必填欄位是否正確填寫。";
                ViewBag.GenderList = _db.Gender.ToList();
                return View();
            }
            catch (Exception ex)
            {
                Console.WriteLine("其他錯誤: " + ex.Message);
                ViewBag.GenderList = _db.Gender.ToList();
                return View();
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
        public ActionResult Login()
        {
            Session.Clear(); // 每次進入登入頁面時，清除所有現有的 Session 狀態
            return View();
        }

        [HttpPost]
        public ActionResult Login(string athleteNumber, string pwd)
        {
            try
            {
                // 將使用者輸入的密碼進行SHA256加密
                string hashedPwd = ComputeSha256Hash(pwd);
                var dto = _db.AthleteUser.FirstOrDefault(u => u.AthleteNumber == athleteNumber && u.Password == hashedPwd);

                if (dto != null)
                {
                    // 驗證成功，更新最後登入時間（如需要）
                    _db.SaveChanges();

                    // 設定 Session 狀態為已登入
                    Session["LoggedIn"] = true;
                    Session["UserName"] = dto.Name;
                    return RedirectToAction("Main", "Questionnaire");
                }
                else
                {
                    // 驗證失敗
                    ViewBag.ErrorMessage = "帳號或密碼錯誤";
                    return View();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("其他錯誤: " + ex.Message);
                return View("Error");
            }
        }
        #endregion

        #region 忘記密碼
        public ActionResult ForgotPassword()
        {
            return View();
        }

        // 發送重置密碼鏈接
        [HttpPost]
        public ActionResult SendResetLink(string Email)
        {
            try
            {
                var user = _db.AthleteUser.FirstOrDefault(u => u.Email == Email);

                if (user == null)
                {
                    ViewBag.ErrorMessage = "此Email尚未註冊";
                    return View("ForgotPassword");
                }

                // 生成重置密碼令牌（這裡使用 Guid 作為示例）
                var resetToken = Guid.NewGuid().ToString();

                // 保存重置令牌和過期時間
                var resetPW = new PasswordReset
                {
                    Email = Email,
                    Token = resetToken,
                    ExpiryDate = DateTime.Now.AddMinutes(10), // 設定有效時間為10分鐘
                    UserAccount = user.Name,
                    changeDate = DateTime.Now
                };
                _db.PasswordReset.Add(resetPW);
                _db.SaveChanges();

                // 發送重置密碼郵件
                var resetLink = Url.Action("ResetPassword", "Account", new { token = resetToken }, Request.Url.Scheme);

                var emailBody = $"請點擊以下連結重置您的密碼：{resetLink}，連結有效時間為10分鐘";

                SendEmail(Email, "重置密碼", emailBody);

                ViewBag.Message = "重置密碼連結已發送至您的郵箱";
                return View("ForgotPassword");
            }
            catch (Exception ex)
            {
                Console.WriteLine("其他錯誤: " + ex.Message);
                return View("Error");
            }
        }
        #endregion

        #region 登出
        public ActionResult Logout()
        {
            Session.Remove("LoggedIn");
            // 清除所有的 Forms 認證 Cookies
            FormsAuthentication.SignOut();

            // 取得登出前的頁面路徑，如果沒有則預設為首頁
            string returnUrl = Request.UrlReferrer != null ? Request.UrlReferrer.ToString() : Url.Action("Login", "Account");

            // 重定向到記錄的返回頁面
            return Redirect(returnUrl);
        }
        #endregion

        #region 發送重設密碼信件
        private void SendEmail(string toEmail, string subject, string body, string attachmentPath = null)
        {
            var fromEmail = "00048@tiss.org.tw";
            var fromPassword = "lctm hhfh bubx lwda"; //應用程式密碼
            var displayName = "運科中心資訊組"; //顯示的發件人名稱


            var smtpClient = new SmtpClient("smtp.gmail.com")
            {
                Port = 587,
                Credentials = new NetworkCredential(fromEmail, fromPassword),
                EnableSsl = true,
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress(fromEmail, displayName),
                Subject = subject,
                Body = body,
                IsBodyHtml = true,
            };

            // 分割以逗號分隔的收件人地址並添加到郵件中
            foreach (var email in toEmail.Split(','))
            {
                mailMessage.To.Add(email.Trim());
            }

            if (!string.IsNullOrEmpty(attachmentPath))
            {
                Attachment attachment = new Attachment(attachmentPath);
                mailMessage.Attachments.Add(attachment);
            }

            try
            {
                smtpClient.Send(mailMessage);
            }
            catch (Exception ex)
            {
                // 處理發送郵件的錯誤
                Console.WriteLine("郵件發送失敗: " + ex.Message);
            }
        }
        #endregion

        #region 重置密碼
        public ActionResult ResetPassword(string token)
        {
            try
            {
                // 查找重置請求
                var resetRequest = _db.PasswordReset.SingleOrDefault(r => r.Token == token && r.ExpiryDate > DateTime.Now);

                if (resetRequest == null)
                {
                    ViewBag.ErrorMessage = "無效或過期的要求";
                    return View("Error");
                }

                // 初始化 ResetPasswordViewModel 並傳遞到視圖
                var model = new ResetPasswordViewModel
                {
                    Token = token
                };

                return View(model);
            }
            catch (Exception ex)
            {
                Console.WriteLine("其他錯誤: " + ex.Message);
                return View("Error");
            }

        }

        // 處理重置密碼
        [HttpPost]
        public ActionResult ResetPassword(ResetPasswordViewModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    // 顯示驗證錯誤
                    var errors = ModelState.Values.SelectMany(v => v.Errors);
                    foreach (var error in errors)
                    {
                        Console.WriteLine(error.ErrorMessage);
                    }
                    return View(model);
                }

                // 根據 Token 查找重置請求
                var resetRequest = _db.PasswordReset
                    .FirstOrDefault(r => r.Token == model.Token && r.ExpiryDate > DateTime.Now);

                if (resetRequest == null)
                {
                    ViewBag.ErrorMessage = "無效或過期的要求";
                    return View("Error");
                }

                // 根據 Email 查找用戶
                var user = _db.AthleteUser
                    .FirstOrDefault(u => u.Email == resetRequest.Email);

                if (user == null)
                {
                    ViewBag.ErrorMessage = "無效的帳號";
                    return View("Error");
                }

                // 更新用戶的密碼
                user.Password = ComputeSha256Hash(model.NewPassword);
                //user.changeDate = DateTime.Now;

                // 更新 PasswordResetRequest 表中的 UserAccount 和 ChangeDate
                resetRequest.UserAccount = user.Name;
                resetRequest.changeDate = DateTime.Now;

                // 刪除重置請求
                _db.PasswordReset.Remove(resetRequest);
            }
            catch (Exception ex)
            {
                Console.WriteLine("其他錯誤: " + ex.Message);
                return View("Error");
            }

            try
            {
                // 儲存變更到資料庫
                _db.SaveChanges();
            }
            catch (DbEntityValidationException ex)
            {
                foreach (var validationErrors in ex.EntityValidationErrors)
                {
                    foreach (var validationError in validationErrors.ValidationErrors)
                    {
                        // 記錄錯誤信息
                        Console.WriteLine($"Property: {validationError.PropertyName}, Error: {validationError.ErrorMessage}");
                    }
                }
                throw;
            }

            ViewBag.Message = "您的密碼已成功重置";
            return RedirectToAction("Login");
        }
        #endregion
    }
}