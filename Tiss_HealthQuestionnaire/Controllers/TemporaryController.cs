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
        private HealthQuestionnaireEntities _db = new HealthQuestionnaireEntities(); //資料庫

        #region 保存單個問卷回答到暫存表
        [HttpPost]
        public ActionResult SaveAnswer(string userId, string questionnaireTypeCode, int questionId, string answer, string fillDate)
        {
            try
            {
                //檢查 SessionID 是否存在，如果不存在則生成一個新的
                if (Session["SessionID"] == null)
                {
                    Session["SessionID"] = Guid.NewGuid(); // 為當前會話創建一個新的唯一 ID
                }
                var sessionId = (Guid)Session["SessionID"]; // 獲取唯一會話ID

                //檢查 AthleteNumber 是否有效
                var userExists = _db.AthleteUser.Any(u => u.AthleteNumber == userId);
                if (!userExists)
                {
                    return Json(new { success = false, message = "無效的選手編號" });
                }

                //查找問卷類型的 Id
                var questionnaireType = _db.QuestionnaireType.FirstOrDefault(q => q.Code == questionnaireTypeCode);
                if (questionnaireType == null)
                {
                    return Json(new { success = false, message = "無效的問卷類型" });
                }

                //嘗試解析填表日期
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

                //新建暫存回答記錄
                var tempAnswer = new TemporaryQuestionnaireData
                {
                    UserID = userId, // 使用 AthleteNumber 作為 UserID 儲存
                    QuestionnaireType = questionnaireTypeCode, // 設定 `QuestionnaireType` 欄位
                    QuestionnaireTypeId = questionnaireType.Id, // 使用問卷類型的 Id
                    QuestionID = questionId,
                    Answer = answer,
                    SessionID = sessionId,
                    IsCompleted = false,
                    AnswerDateTime = DateTime.Now // 確保有填入時間戳記
                };

                // 添加到資料庫並保存
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
                // 檢查 SessionID 是否存在，如果不存在則生成一個新的
                if (Session["SessionID"] == null)
                {
                    Session["SessionID"] = Guid.NewGuid();
                }

                var sessionId = (Guid)Session["SessionID"]; // 獲取ID

                // 查找暫存的回答
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
                // 檢查 SessionID 是否存在，如果不存在則生成一個新的
                if (Session["SessionID"] == null)
                {
                    Session["SessionID"] = Guid.NewGuid();
                }

                var sessionId = (Guid)Session["SessionID"];

                var answers = _db.TemporaryQuestionnaireData
                                 .Where(t => t.UserID == userId && t.SessionID == sessionId && t.QuestionnaireType == questionnaireType)
                                 .ToList();

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