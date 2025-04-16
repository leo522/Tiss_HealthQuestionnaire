using Microsoft.Ajax.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Tiss_HealthQuestionnaire.Models;
using Tiss_HealthQuestionnaire.ViewModels;

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

        #region 顯示選手問卷完整內容（含各個子表)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ViewFullAthleteResponse(int responseId)
        {
            var response = _db.QuestionnaireResponse.Find(responseId);
            if (response == null) return HttpNotFound();

            var model = new AdminViewAthleteResponseViewModel
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

        #region 載入防護員清單
        public ActionResult SelectTrainer()
        {
            var trainerList = _db.TrainerQuestionnaireResponse.Select(t => new
            {
                t.TrainerID,
                t.FillName
            }).Distinct().ToList().Select(r => new SelectListItem
            {
                Value = r.TrainerID,
                Text = $"{r.TrainerID} - {r.FillName}"
            }).ToList();

            ViewBag.TrainerList = trainerList;
            return View();
        }
        #endregion

        #region 根據防護員 ID 載入填答日期清單
        public JsonResult LoadTrainerFillingDates(string trainerID)
        {
            var dtos = _db.TrainerQuestionnaireResponse.Where(q => q.TrainerID == trainerID).OrderByDescending(q => q.FillingDate).ToList().Select(q => new
            {
                ResponseId = q.ID,
                Display = q.FillingDate.ToString("yyyy-MM-dd")
            }).ToList();

            return Json(dtos, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region 顯示選手問卷完整內容（含各個子表)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ViewFullTrainerResponse(int responseId)
        {
            var response = _db.TrainerQuestionnaireResponse.Find(responseId);

            if (response == null) return HttpNotFound();

            var model = new AdminViewTrainerResponseViewModel
            {
                ResponseId = response.ID,
                TrainerID = response.TrainerID,
                FillName = response.FillName,
                FillingDate = response.FillingDate,

                CognitiveScreeningList = _db.ResponseCognitiveScreening.Where(x => x.TrainerQuestionnaireResponseID == response.ID).ToList(),

                ImmediateMemoryList = _db.ResponseImmediateMemory.Where(x => x.TrainerQuestionnaireResponseID == response.ID).ToList(),

                ConcentrationList = _db.ResponseConcentration.Where(x => x.TrainerQuestionnaireResponseID == response.ID).ToList(),

                CoordinationAndBalanceList = _db.ResponseCoordinationAndBalance.Where(x => x.TrainerQuestionnaireResponseID == response.ID).ToList(),

                DelayedRecallList = _db.ResponseDelayedRecall.Where(x => x.TrainerQuestionnaireResponseID == response.ID).ToList(),

                OrthopaedicScreeningList = _db.ResponseOrthopaedicScreening.Where(x => x.TrainerQuestionnaireResponseID == response.ID).ToList(),
            };

            return View(model);
        }
        #endregion
    }
}