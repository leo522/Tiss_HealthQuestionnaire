using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Tiss_HealthQuestionnaire.Models;

namespace Tiss_HealthQuestionnaire.Controllers
{
    public class AdminQuestionnaireController : BaseAuthController
    {
        protected override string[] AllowedRoles => new[] { "admin" };

        private HealthQuestionnaireEntities _db = new HealthQuestionnaireEntities();

        #region 載入選手清單，提供選擇
        public ActionResult SelectAthlete()
        {
            var athleteList = _db.QuestionnaireResponse.Select(q => new
            {
                q.AthleteID,
                q.FillName
            }).Distinct().ToList().Select(a => new SelectListItem 
            {
                Value = a.AthleteID,
                Text = $"{a.AthleteID} - {a.FillName}"
            }).ToList();

            ViewBag.AthleteList = athleteList;
            return View();
        }
        #endregion

        #region 根據選手 ID 載入填答日期清單
        public JsonResult LoadFillingDates(string athleteId)
        {
            var dtos = _db.QuestionnaireResponse.Where(q => q.AthleteID == athleteId).OrderByDescending(q => q.FillingDate)
                        .ToList()
                        .Select(q => new
                        {
                            ResponseId = q.ID,
                            Display = q.FillingDate.ToString("yyyy-MM-dd")
                        }).ToList();

            return Json(dtos, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region 顯示問卷完整內容（含各個子表)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ViewFullResponse(int responseId)
        {
            var response = _db.QuestionnaireResponse.Find(responseId);
            if (response == null) return HttpNotFound();

            var model = new AdminViewResponseViewModel
            {
                ResponseId = response.ID,
                AthleteID = response.AthleteID,
                FillName = response.FillName,
                FillingDate = response.FillingDate,

                PastHealthList = _db.ResponsePastHealth.Where(x => x.QuestionnaireResponseID == response.ID).ToList(),

                AllergicHistoryList = _db.ResponseAllergicHistory.Where(x => x.QuestionnaireResponseID == response.ID).ToList(),

                FamilyHistoryList = _db.ResponseFamilyHistory.Where(x => x.QuestionnaireResponseID == response.ID).ToList(),

                PastHistoryList = _db.ResponsePastHistory.Where(x => x.QuestionnaireResponseID == response.ID).ToList(),

                PresentIllnessList = _db.ResponsePresentIllness.Where(x => x.QuestionnaireResponseID == response.ID).ToList(),

                PastDrugsList = _db.ResponsePastDrugs.Where(x => x.QuestionnaireResponseID == response.ID).ToList(),

                PastSupplementsList = _db.ResponsePastSupplements.Where(x => x.QuestionnaireResponseID == response.ID).ToList(),

                FemaleQuestionnaireList = _db.ResponseFemaleQuestionnaire.Where(x => x.QuestionnaireResponseID == response.ID).ToList(),

                PastInjuryList = _db.ResponsePastInjuries.Where(x => x.QuestionnaireResponseID == response.ID).ToList(),

                CurrentInjuryList = _db.ResponseCurrentInjuries.Where(x => x.QuestionnaireResponseID == response.ID).ToList(),

                CardiovascularList = _db.ResponseCardiovascularScreening.Where(x => x.QuestionnaireResponseID == response.ID).ToList(),

                ConcussionScreeningList = _db.ResponseConcussionScreening.Where(x => x.QuestionnaireResponseID == response.ID).ToList(),

                SymptomEvaluationList = _db.ResponseSymptomEvaluation.Where(x => x.QuestionnaireResponseID == response.ID).ToList(),
            };

            return View(model);
        }
        #endregion
    }
}