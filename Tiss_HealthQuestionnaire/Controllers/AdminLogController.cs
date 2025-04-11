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
        public ActionResult LogList(string userName = null, string action = null, DateTime? startDate = null, DateTime? endDate = null)
        {
            var logs = _db.SystemLog.AsQueryable();

            if (!string.IsNullOrWhiteSpace(userName))
                logs = logs.Where(l => l.Target.Contains(userName));

            if (!string.IsNullOrWhiteSpace(action))
                logs = logs.Where(l => l.Action.Contains(action));

            if (startDate.HasValue)
                logs = logs.Where(l => l.LogDate >= startDate);

            if (endDate.HasValue)
                logs = logs.Where(l => l.LogDate <= endDate);

            var results = logs
                .OrderByDescending(l => l.LogDate)
                .Select(l => new AdminLogItemViewModel
                {
                    UserName = l.SystemUser.UserName,
                    Action = l.Action,
                    Target = l.Target,
                    Message = l.Message,
                    LogDate = l.LogDate
                }).ToList();

            var model = new AdminLogFilterViewModel
            {
                UserName = userName,
                Action = action,
                StartDate = startDate,
                EndDate = endDate,
                Results = results
            };

            return View(model);
        }
        #endregion
    }
}