using System;
using System.Collections.Generic;
using System.Linq;
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
            var users = _db.SystemUser
                .Select(u => new AdminUserViewModel
                {
                    UserID = u.UserID,
                    UserName = u.UserName,
                    Email = u.Email,
                    RoleName = u.UserRole.RoleName,
                    IsActive = u.IsActive,
                    CreatedDate = u.CreatedDate
                }).ToList();

            return View(users);
        }
        #endregion

        #region 切換帳號啟用狀態
        [HttpPost]
        public ActionResult ToggleUserStatus(int id)
        {
            var user = _db.SystemUser.Find(id);
            if (user == null) return HttpNotFound();

            user.IsActive = !user.IsActive;
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

            ViewBag.RoleList = new SelectList(_db.UserRole.ToList(), "RoleID", "RoleName", user.RoleID);

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
                ViewBag.RoleList = new SelectList(_db.UserRole.ToList(), "RoleID", "RoleName", model.RoleID);
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
        #endregion
    }
}