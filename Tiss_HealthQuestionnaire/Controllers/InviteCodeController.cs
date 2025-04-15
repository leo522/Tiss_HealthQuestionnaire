using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;
using Tiss_HealthQuestionnaire.Models;

namespace Tiss_HealthQuestionnaire.Controllers
{
    public class InviteCodeController : BaseAuthController
    {
        protected override string[] AllowedRoles => new[] { "admin" };
        private HealthQuestionnaireEntities _db = new HealthQuestionnaireEntities();

        #region 邀請碼清單
        public ActionResult InviteCodeList()
        {
            var codes = _db.InviteCode.OrderByDescending(c => c.ID).ToList();
            ViewBag.GeneratedCode = TempData["GeneratedCode"];
            return View("InviteCodeList", codes);
        }
        #endregion

        #region 建立邀請碼（畫面）
        public ActionResult InviteCodeCreate()
        {
            return View();
        }
        #endregion

        #region 建立邀請碼（儲存）
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateInviteCode(int maxUsage = 1, DateTime? expiryDate = null, string note = null)
        {
            var plainCode = GenerateInviteCode();
            var hashed = ComputeSha256Hash(plainCode);

            var entity = new InviteCode
            {
                CodeHash = hashed,
                MaxUsage = maxUsage,
                UsedCount = 0,
                ExpiryDate = expiryDate,
                IsActive = true,
                Note = note
            };

            _db.InviteCode.Add(entity);
            _db.SaveChanges();

            var userId = Convert.ToInt32(Session["UserID"]);
            _db.SystemLog.Add(new SystemLog
            {
                UserID = userId,
                Action = "產生邀請碼",
                Target = plainCode,
                Message = $"管理員產生新邀請碼 (用途: {note ?? "無備註"})",
                LogDate = DateTime.Now
            });
            _db.SaveChanges();

            TempData["GeneratedCode"] = plainCode;
            return RedirectToAction("InviteCodeList");
        }
        #endregion

        #region 停用邀請碼
        [HttpPost]
        public ActionResult DisableInviteCode(int id)
        {
            var code = _db.InviteCode.Find(id);
            if (code != null)
            {
                code.IsActive = false;
                _db.SaveChanges();
            }
            return RedirectToAction("InviteCodeList");
        }
        #endregion

        #region 工具方法
        private string GenerateInviteCode()
        {
            return Guid.NewGuid().ToString("N").Substring(0, 10).ToUpper(); // 10 碼英數
        }

        private string ComputeSha256Hash(string input)
        {
            using (SHA256 sha256Hash = SHA256.Create())
            {
                var bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(input));
                return BitConverter.ToString(bytes).Replace("-", "").ToLowerInvariant();
            }
        }
        #endregion
    }
}