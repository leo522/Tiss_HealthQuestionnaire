using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Tiss_HealthQuestionnaire.Models;

namespace Tiss_HealthQuestionnaire.Controllers
{
    public class QuestionnaireController : Controller
    {
        private HealthQuestionnaireEntities _db = new HealthQuestionnaireEntities();

        public ActionResult Main()
        {
            ViewBag.GenderList = _db.Gender.ToList(); //性別
            return View();
        }

        #region 第一部份問卷


        #region 過去健康檢查病史
        public ActionResult PastHealth()
        {
            var pastHealthItems = _db.PastHealth.ToList();
            return PartialView("_PastHealth", pastHealthItems);
        }
        #endregion

        #region 過敏史
        public ActionResult AllergicHistory()
        {
            var allergicHistoryItems = _db.AllergicHistory.ToList();
            return PartialView("_AllergicHistory", allergicHistoryItems);
        }
        #endregion

        #region 家族病史
        public ActionResult FamilyHistory()
        {
            var familyHistory = _db.FamilyHistory.ToList();
            return PartialView("_FamilyHistory", familyHistory);
        }
        #endregion

        #region 過去病史Past history
        public ActionResult PastHistory()
        {
            var pastHistory = _db.PastHistory.ToList();
            return PartialView("_PastHistory", pastHistory);
        }
        #endregion

        #region 開刀史
        public ActionResult SurgeryHistory()
        {
            var surgeryHistory = _db.SurgeryHistory.ToList();
            return PartialView("_SurgeryHistory", surgeryHistory);
        }
        #endregion

        #region 現在病史
        public ActionResult PresentIllness()
        {
            var presentIllness = _db.PresentIllness.ToList();
            return PartialView("_PresentIllness", presentIllness);
        }
        #endregion

        #region 藥物史
        public ActionResult PastDrugs()
        {
            var pastDrugs = _db.PastDrugs.ToList();
            return PartialView("_PastDrugs", pastDrugs);
        }
        #endregion

        #region 營養品
        public ActionResult PastSupplements()
        {
            var pastSupplements = _db.PastSupplements.ToList();
            return PartialView("_PastSupplements", pastSupplements);
        }
        #endregion

        #region 女性問卷
        public ActionResult FemaleQuestionnaire()
        {
            var femaleQuestionnaire = _db.FemaleQuestionnaire.ToList();
            return PartialView("_FemaleQuestionnaire", femaleQuestionnaire);
        }
        #endregion

        #endregion

        #region 第二部份問卷

        #region 過去傷害狀況 (已復原)
        public ActionResult PastInjuryRestored() //過去有的症狀或疼痛部位
        {
            try
            {
                var pastInjury = _db.InjuryStatus.ToList();

                // 根據需求設置一個是否需要單一選項的屬性
                // 將數據轉換成 InjuryStatusViewModel
                var injuryList = pastInjury.Select(injury => new InjuryStatusViewModel
                {
                    Id = injury.Id,
                    InjuryPart = injury.InjuryPart,
                    IsSingleSelect = (injury.InjuryPart == "頭部/臉" || injury.InjuryPart == "頸椎" ||
                                      injury.InjuryPart == "胸椎" || injury.InjuryPart == "腰椎" ||
                                      injury.InjuryPart == "胸骨/肋骨" || injury.InjuryPart == "腹部" ||
                                      injury.InjuryPart == "骨盆/薦椎")
                }).ToList();

                return PartialView("_PastInjury", injuryList);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public ActionResult PastInjuryType() //過去傷勢類型
        {
            var InjuryType = _db.InjuryType.ToList();
            return PartialView("_PastInjuryType", InjuryType);
        }

        public ActionResult PastTreatmentMethod() //過去治療方式
        {
            var treatmentMethod = _db.TreatmentMethod.ToList();
            return PartialView("_PastTreatmentMethod", treatmentMethod);
        }
        #endregion

        #region 目前傷害狀況
        public ActionResult NowInjuryRestored() //目前有的症狀或疼痛部位
        {
            try
            {
                var nowInjury = _db.InjuryStatus.ToList();

                // 將資料轉換為 InjuryStatusViewModel
                var injuryList = nowInjury.Select(injury => new InjuryStatusViewModel
                {
                    Id = injury.Id,
                    InjuryPart = injury.InjuryPart,
                    IsSingleSelect = (injury.InjuryPart == "頭部/臉" || injury.InjuryPart == "頸椎" ||
                                      injury.InjuryPart == "胸椎" || injury.InjuryPart == "腰椎" ||
                                      injury.InjuryPart == "胸骨/肋骨" || injury.InjuryPart == "腹部" ||
                                      injury.InjuryPart == "骨盆/薦椎")
                }).ToList();

                // 傳遞轉換後的 injuryList 到 Partial View
                return PartialView("_NowInjuryRestored", injuryList);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public ActionResult NowInjuryType() //目前傷勢類型
        {
            var InjuryType = _db.InjuryType.ToList();
            return PartialView("_NowInjuryType", InjuryType);
        }

        public ActionResult NowTreatmentMethod() //目前治療方式
        {
            var treatmentMethod = _db.TreatmentMethod.ToList();
            return PartialView("_NowTreatmentMethod", treatmentMethod);
        }
        #endregion

        #region 醫療史與家族史
        public ActionResult MedicalFamilyHistory()
        {
            var viewModel = new List<MedicalFamilyHistoryViewModel>
{
    new MedicalFamilyHistoryViewModel
    {
        Question = "您是否曾經在運動中有胸口疼痛、異常疲累、昏厥、心臟雜音、高血壓症狀？",
        Symptoms = _db.MedicalandFamilyHistory.Where(s => s.Id <= 5).Select(s => s.Symptom).ToList()
    },
    new MedicalFamilyHistoryViewModel
    {
        Question = "您的親人是否曾經於50歲前因心臟問題死亡或失能？",
        Symptoms = new List<string>() // 沒有相關症狀
    },
    new MedicalFamilyHistoryViewModel
    {
        Question = "您是否曾經被檢查出心臟雜音、股動脈脈搏異常、馬凡氏症候群、肱動脈血壓異常？",
        Symptoms = _db.MedicalandFamilyHistory.Where(s => s.Id > 5).Select(s => s.Symptom).ToList()
    }
};
            return View("_MedicalFamilyHistory", viewModel);
        }

        #endregion

        #region 心血管篩檢
        public ActionResult CardiovascularScreening()
        {
            // 從資料庫中讀取所有的問卷問題
            var questions = _db.CardiovascularScreening.ToList();

            // 為每個問題手動生成項次
            var viewModel = questions.Select((q, index) => new CardiovascularScreeningViewModel
            {
                OrderNumber = index + 1,  // 自動遞增項次
                Question = q.Question
            }).ToList();

            return PartialView("_CardiovascularScreening", viewModel);
        }
        #endregion

        #region 腦震盪篩檢
        public ActionResult ConcussionScreening()
        {
            // 從資料庫中讀取問卷問題
            var questions = _db.ConcussionScreening.ToList();

            // 為每個問題手動生成項次
            var viewModel = questions.Select((q, index) => new ConcussionScreeningViewModel
            {
                OrderNumber = index + 1,  // 自動遞增項次
                Question = q.Question
            }).ToList();

            return PartialView("_ConcussionScreening", viewModel);
        }
        #endregion

        #region 骨科篩檢

        #endregion

        #endregion

        #region 問卷存檔

        [HttpPost]
        public ActionResult SaveHealth()
        {
            var dtos = new HealthQuestionnaireEntities
            {

            };

            return RedirectToAction("QuestionnaireDone", "Questionnaire");
        }
        #endregion

        #region 問卷完成頁
        public ActionResult QuestionnaireDone(QuestionnaireResponseViewModel model)
        {
            var response = new QuestionnaireResponse
            {
                AthleteID = model.AthleteID,
                GenderID = model.GenderID,
                FillingDate = DateTime.Now,
                Specialty = model.Specialty,
                FillName = model.FillName
            };

            _db.QuestionnaireResponse.Add(response);
            _db.SaveChanges();

            // 存檔各問卷的回覆
            SaveQuestionnaireDetails(response.ID, model.PastHealthAnswers, "PastHealth");
            SaveQuestionnaireDetails(response.ID, model.AllergicHistoryAnswers, "AllergicHistory");
            SaveQuestionnaireDetails(response.ID, model.FamilyHistoryAnswers, "FamilyHistory");
            SaveQuestionnaireDetails(response.ID, model.PastHistoryAnswers, "PastHistory");
            SaveQuestionnaireDetails(response.ID, model.SurgeryHistoryAnswers, "SurgeryHistory");
            SaveQuestionnaireDetails(response.ID, model.PresentIllnessAnswers, "PresentIllness");
            SaveQuestionnaireDetails(response.ID, model.PastDrugsAnswers, "PastDrugs");
            SaveQuestionnaireDetails(response.ID, model.PastSupplementsAnswers, "PastSupplements");
            SaveQuestionnaireDetails(response.ID, model.FemaleQuestionnaireAnswers, "FemaleQuestionnaire");

            _db.SaveChanges();

            return RedirectToAction("Main");
        }

        private void SaveQuestionnaireDetails(int responseID, List<QuestionnaireAnswer> answers, string questionnaireType)
        {
            foreach (var answer in answers)
            {
                var detail = new QuestionnaireResponseDetails
                {
                    ResponseID = responseID,
                    QuestionnaireType = questionnaireType,
                    QuestionID = answer.QuestionID,
                    Answer = answer.Answer
                };
                _db.QuestionnaireResponseDetails.Add(detail);
            }
        }
        #endregion
    }
}