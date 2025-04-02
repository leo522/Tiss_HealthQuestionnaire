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

            int nextAthleteID = 1;
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
                if (Session["NextAthleteID"] == null)
                {
                    ViewBag.ErrorMessage = "註冊過程中發生錯誤，請重新嘗試。";
                    ViewBag.GenderList = _db.Gender.ToList();
                    return View();
                }

                if (!int.TryParse(Session["NextAthleteID"].ToString(), out int athleteID))
                {
                    ViewBag.ErrorMessage = "編號格式錯誤，請重新註冊。";
                    ViewBag.GenderList = _db.Gender.ToList();
                    return View();
                }

                if (string.IsNullOrWhiteSpace(pwd) || pwd.Length < 6)
                {
                    ViewBag.ErrorMessage = "密碼長度至少要6位數";
                    ViewBag.GenderList = _db.Gender.ToList();
                    return View();
                }

                if (_db.AthleteUser.Any(u => u.Name == userName))
                {
                    ViewBag.ErrorMessage = "該帳號已存在";
                    ViewBag.GenderList = _db.Gender.ToList();
                    return View();
                }

                var emailRegex = new System.Text.RegularExpressions.Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$");
                if (!emailRegex.IsMatch(email))
                {
                    ViewBag.ErrorMessage = "無效的 Email 格式";
                    ViewBag.GenderList = _db.Gender.ToList();
                    return View();
                }

                if (string.IsNullOrWhiteSpace(athleteNumber))
                {
                    ViewBag.ErrorMessage = "選手編號必須填寫。";
                    ViewBag.GenderList = _db.Gender.ToList();
                    return View();
                }

                var encryptedPwd = ComputeSha256Hash(pwd);

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

                _db.AthleteUser.Add(newUser);
                _db.SaveChanges();

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
            Session.Clear();
            return View();
        }

        [HttpPost]
        public ActionResult Login(string athleteNumber, string pwd)
        {
            try
            {
                string hashedPwd = ComputeSha256Hash(pwd);
                var dto = _db.AthleteUser.FirstOrDefault(u => u.AthleteNumber == athleteNumber && u.Password == hashedPwd);

                if (dto != null)
                {
                    _db.SaveChanges();

                    Session["LoggedIn"] = true;
                    Session["UserName"] = dto.Name;
                    return RedirectToAction("Main", "Questionnaire");
                }
                else
                {
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

                var resetToken = Guid.NewGuid().ToString();

                var resetPW = new PasswordReset
                {
                    Email = Email,
                    Token = resetToken,
                    ExpiryDate = DateTime.Now.AddMinutes(10),
                    UserAccount = user.Name,
                    changeDate = DateTime.Now
                };
                _db.PasswordReset.Add(resetPW);
                _db.SaveChanges();

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

            FormsAuthentication.SignOut();

            string returnUrl = Request.UrlReferrer != null ? Request.UrlReferrer.ToString() : Url.Action("Login", "Account");

            return Redirect(returnUrl);
        }
        #endregion

        #region 發送重設密碼信件
        private void SendEmail(string toEmail, string subject, string body, string attachmentPath = null)
        {
            var fromEmail = "@tiss.org.tw";
            var fromPassword = "";
            var displayName = "運科中心資訊組";


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
                Console.WriteLine("郵件發送失敗: " + ex.Message);
            }
        }
        #endregion

        #region 重置密碼
        public ActionResult ResetPassword(string token)
        {
            try
            {
                var resetRequest = _db.PasswordReset.SingleOrDefault(r => r.Token == token && r.ExpiryDate > DateTime.Now);

                if (resetRequest == null)
                {
                    ViewBag.ErrorMessage = "無效或過期的要求";
                    return View("Error");
                }

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

        [HttpPost]
        public ActionResult ResetPassword(ResetPasswordViewModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values.SelectMany(v => v.Errors);
                    foreach (var error in errors)
                    {
                        Console.WriteLine(error.ErrorMessage);
                    }
                    return View(model);
                }

                var resetRequest = _db.PasswordReset
                    .FirstOrDefault(r => r.Token == model.Token && r.ExpiryDate > DateTime.Now);

                if (resetRequest == null)
                {
                    ViewBag.ErrorMessage = "無效或過期的要求";
                    return View("Error");
                }

                var user = _db.AthleteUser
                    .FirstOrDefault(u => u.Email == resetRequest.Email);

                if (user == null)
                {
                    ViewBag.ErrorMessage = "無效的帳號";
                    return View("Error");
                }

                user.Password = ComputeSha256Hash(model.NewPassword);

                resetRequest.UserAccount = user.Name;
                resetRequest.changeDate = DateTime.Now;

                _db.PasswordReset.Remove(resetRequest);
            }
            catch (Exception ex)
            {
                Console.WriteLine("其他錯誤: " + ex.Message);
                return View("Error");
            }

            try
            {
                _db.SaveChanges();
            }
            catch (DbEntityValidationException ex)
            {
                foreach (var validationErrors in ex.EntityValidationErrors)
                {
                    foreach (var validationError in validationErrors.ValidationErrors)
                    {
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