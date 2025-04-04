﻿using System;
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

                var salt = GenerateSalt();
                var encryptedPwd = ComputeSha256Hash(pwd, salt);

                var newUser = new AthleteUser
                {
                    AthleteID = athleteID,
                    AthleteNumber = athleteNumber,
                    Name = userName,
                    Password = encryptedPwd,
                    Salt = salt,
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
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "系統錯誤: " + ex.Message;
                ViewBag.GenderList = _db.Gender.ToList();
                return View();
            }
        }
        #endregion

        #region 密碼加密 + Salt
        private static string GenerateSalt()
        {
            byte[] saltBytes = new byte[16];
            using (var rng = new RNGCryptoServiceProvider())
            {
                rng.GetBytes(saltBytes);
            }
            return Convert.ToBase64String(saltBytes);
        }

        private static string ComputeSha256Hash(string rawData, string salt)
        {
            using (SHA256 sha256Hash = SHA256.Create())
            {
                byte[] combined = Encoding.UTF8.GetBytes(salt + rawData);
                byte[] bytes = sha256Hash.ComputeHash(combined);
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
                var user = _db.AthleteUser.FirstOrDefault(u => u.AthleteNumber == athleteNumber);
                if (user == null)
                {
                    ViewBag.ErrorMessage = "帳號錯誤";
                    return View();
                }

                var hashedPwd = ComputeSha256Hash(pwd, user.Salt);

                if (user.Password == hashedPwd)
                {
                    Session["LoggedIn"] = true;
                    Session["UserName"] = user.Name;
                    return RedirectToAction("Main", "Questionnaire");
                }
                else
                {
                    ViewBag.ErrorMessage = "密碼錯誤";
                    return View();
                }
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "系統錯誤: " + ex.Message;
                return View("Error");
            }
        }
        #endregion

        #region 重設定密碼
        [HttpGet]
        public ActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        public ActionResult ForgotPassword(string athleteNumber, string newPassword, string confirmPassword)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(athleteNumber) || string.IsNullOrWhiteSpace(newPassword))
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

                var user = _db.AthleteUser.FirstOrDefault(u => u.AthleteNumber == athleteNumber);
                if (user == null)
                {
                    ViewBag.ErrorMessage = "查無此帳號";
                    return View();
                }

                var newSalt = GenerateSalt();
                var newHashedPwd = ComputeSha256Hash(newPassword, newSalt);

                user.Salt = newSalt;
                user.Password = newHashedPwd;
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