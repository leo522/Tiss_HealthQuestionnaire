using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Tiss_HealthQuestionnaire.Models;

namespace Tiss_HealthQuestionnaire.Controllers
{
    public class TemporaryController : Controller
    {
        private HealthQuestionnaireEntities _db = new HealthQuestionnaireEntities();

        #region 保存單個問卷回答到暫存表
        [HttpPost]
        public ActionResult SaveAnswer(string userId, string questionnaireTypeCode, int questionId, string answer, string fillDate)
        {
            try
            {
                if (Session["SessionID"] == null)
                {
                    Session["SessionID"] = Guid.NewGuid();
                }
                var sessionId = (Guid)Session["SessionID"];

                var userExists = _db.AthleteUser.Any(u => u.AthleteNumber == userId);
                if (!userExists)
                {
                    return Json(new { success = false, message = "無效的選手編號" });
                }

                var questionnaireType = _db.QuestionnaireType.FirstOrDefault(q => q.Code == questionnaireTypeCode);
                if (questionnaireType == null)
                {
                    return Json(new { success = false, message = "無效的問卷類型" });
                }

                DateTime? parsedFillDate = null;
                if (!string.IsNullOrEmpty(fillDate))
                {
                    if (DateTime.TryParse(fillDate, out var tempDate))
                    {
                        parsedFillDate = tempDate;
                    }
                    else
                    {
                        return Json(new { success = false, message = "填表日期格式不正確" });
                    }
                }

                var tempAnswer = new TemporaryQuestionnaireData
                {
                    UserID = userId,
                    QuestionnaireType = questionnaireTypeCode,
                    QuestionnaireTypeId = questionnaireType.Id,
                    QuestionID = questionId,
                    Answer = answer,
                    SessionID = sessionId,
                    IsCompleted = false,
                    AnswerDateTime = DateTime.Now
                };

                _db.TemporaryQuestionnaireData.Add(tempAnswer);
                _db.SaveChanges();

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error in SaveAnswer: " + ex.ToString());
                return Json(new { success = false, message = ex.Message });
            }
        }
        #endregion

        #region 從暫存表加載問卷回答
        [HttpGet]
        public ActionResult LoadAnswers(string userId, string questionnaireType)
        {
            try
            {
                if (Session["SessionID"] == null)
                {
                    Session["SessionID"] = Guid.NewGuid();
                }

                var sessionId = (Guid)Session["SessionID"];

                var answers = _db.TemporaryQuestionnaireData
                                 .Where(t => t.UserID == userId && t.SessionID == sessionId && t.QuestionnaireType == questionnaireType)
                                 .ToList();

                return Json(answers, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
        #endregion

        #region 標記問卷為已完成
        [HttpPost]
        public ActionResult MarkAsCompleted(string userId, string questionnaireType)
        {
            try
            {
                if (Session["SessionID"] == null)
                {
                    Session["SessionID"] = Guid.NewGuid();
                }

                var sessionId = (Guid)Session["SessionID"];

                var answers = _db.TemporaryQuestionnaireData
                                 .Where(t => t.UserID == userId && t.SessionID == sessionId && t.QuestionnaireType == questionnaireType).ToList();

                foreach (var answer in answers)
                {
                    answer.IsCompleted = true;
                    _db.Entry(answer).State = EntityState.Modified;
                }

                _db.SaveChanges();
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
        #endregion
    }
}