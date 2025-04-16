using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Tiss_HealthQuestionnaire.Models;
using Tiss_HealthQuestionnaire.ViewModels;

namespace Tiss_HealthQuestionnaire.Controllers
{
    public class AdminLogController : BaseAuthController
    {
        protected override string[] AllowedRoles => new[] { "admin" };

        private HealthQuestionnaireEntities _db = new HealthQuestionnaireEntities();

        #region 操作紀錄清單查詢
        [HttpGet]
        public ActionResult LogList()
        {
            return View(new AdminLogFilterViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogList(string userName = null, DateTime? startDate = null, DateTime? endDate = null)
        {
            try
            {
                var model = new AdminLogFilterViewModel
                {
                    UserName = userName,
                    StartDate = startDate,
                    EndDate = endDate,
                    Results = new List<AdminLogItemViewModel>() // 預設空結果
                };

                if (string.IsNullOrWhiteSpace(userName))
                {
                    return View(model);
                }

                var logs = _db.SystemLog.AsQueryable();

                logs = logs.Where(g =>
                    (g.SystemUser.UserName != null && g.SystemUser.UserName.Contains(userName)) ||
                    (g.Target != null && g.Target.Contains(userName)));

                if (startDate.HasValue) logs = logs.Where(g => g.LogDate >= startDate);
                if (endDate.HasValue) logs = logs.Where(g => g.LogDate <= endDate);

                model.Results = logs
                    .OrderByDescending(g => g.LogDate)
                    .Select(g => new AdminLogItemViewModel
                    {
                        UserName = g.SystemUser.UserName,
                        Action = g.Action,
                        Target = g.Target,
                        Message = g.Message,
                        LogDate = g.LogDate
                    }).ToList();

                return View(model);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion
    }
}