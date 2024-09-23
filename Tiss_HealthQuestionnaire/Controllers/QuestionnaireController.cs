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

        #region 主頁
        public ActionResult Main()
        {
            // 從 Session 獲取使用者的名稱
            string loggedInUserName = Session["UserName"] as string;
            if (string.IsNullOrEmpty(loggedInUserName))
            {
                // 如果 Session 中沒有登入資訊，重導向到登入頁面
                return RedirectToAction("Login", "Account");
            }

            // 根據使用者名稱獲取使用者詳細資料
            var user = _db.AthleteUser.FirstOrDefault(u => u.Name == loggedInUserName);
            if (user == null)
            {
                // 如果找不到使用者，重導向到登入頁面
                return RedirectToAction("Login", "Account");
            }

            // 設置 ViewBag 值
            ViewBag.Specialist = user.SportSpecialization; // 運動專項
            ViewBag.FillName = user.Name; // 填表人
            ViewBag.AtheNum = user.AthleteNumber; // 選手編號，顯示為五位數
            ViewBag.GenderID = user.GenderID; // 性別
            ViewBag.ShowFemaleTab = (user.GenderID == 2); // 假設 2 代表女性，決定是否顯示女性問卷頁籤

            return View();
        }
        #endregion

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
                var pastInjury = _db.PastInjuryStatus.ToList();

                // 根據需求設置一個是否需要單一選項的屬性
                var PastInjuryList = pastInjury.Select(injury => new PastInjuryStatusViewModel
                {
                    Id = injury.Id,
                    PastInjuryPart = injury.InjuryPart,
                    PastIsSingleSelect = (injury.InjuryPart == "頭部/臉" || injury.InjuryPart == "頸椎" ||
                                      injury.InjuryPart == "胸椎" || injury.InjuryPart == "腰椎" ||
                                      injury.InjuryPart == "胸骨/肋骨" || injury.InjuryPart == "腹部" ||
                                      injury.InjuryPart == "骨盆/薦椎")
                }).ToList();

                return PartialView("_PastInjury", PastInjuryList);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public ActionResult PastInjuryType() //過去傷勢類型
        {
            var PastInjuryType = _db.PastInjuryType.ToList();
            return PartialView("_PastInjuryType", PastInjuryType);
        }

        public ActionResult PastTreatmentMethod() //過去治療方式
        {
            var PastTreatmentMethod = _db.PastTreatmentMethod.ToList();
            return PartialView("_PastTreatmentMethod", PastTreatmentMethod);
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

        #region 不使用--醫療史與家族史
        //public ActionResult MedicalFamilyHistory()
        //{
        //    var viewModel = new List<MedicalFamilyHistoryViewModel>
        //    {
        //        new MedicalFamilyHistoryViewModel
        //    {
        //        Question = "您是否曾經在運動中有胸口疼痛、異常疲累、昏厥、心臟雜音、高血壓症狀？",
        //        Symptoms = _db.MedicalandFamilyHistory.Where(s => s.Id <= 5).Select(s => s.Symptom).ToList()
        //    },
        //        new MedicalFamilyHistoryViewModel
        //    {
        //        Question = "您的親人是否曾經於50歲前因心臟問題死亡或失能？",
        //        Symptoms = new List<string>() // 沒有相關症狀
        //    },
        //        new MedicalFamilyHistoryViewModel
        //    {
        //        Question = "您是否曾經被檢查出心臟雜音、股動脈脈搏異常、馬凡氏症候群、肱動脈血壓異常？",
        //        Symptoms = _db.MedicalandFamilyHistory.Where(s => s.Id > 5).Select(s => s.Symptom).ToList()
        //    }
        //    };
        //    return View("_MedicalFamilyHistory", viewModel);
        //}

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
        /// <summary>
        /// 第一部分 選手背景
        /// </summary>
        /// <returns></returns>
        public ActionResult ConcussionScreening()
        {
            // 從資料庫中讀取問卷問題
            var questions = _db.ConcussionScreening.ToList();

            //為每個問題手動生成項次
            var viewModel = questions.Select((q, index) => new ConcussionScreeningViewModel
            {
                OrderNumber = index + 1,  // 自動遞增項次
                Question = q.Question
            }).ToList();

            return PartialView("_ConcussionScreening", viewModel);
        }

        /// <summary>
        /// 第二部分 症狀自我評估
        /// </summary>
        /// <returns></returns>
        public ActionResult SymptomEvaluation()
        {
            // 從資料庫中讀取問卷問題
            var questions = _db.SymptomEvaluation.ToList();

            //為每個問題手動生成項次
            var viewModel = questions.Select((q, index) => new ConcussionScreeningViewModel
            {
                OrderNumber = index + 1,  // 自動遞增項次
                Question = q.SymptomItem
            }).ToList();

            return PartialView("_SymptomEvaluation", viewModel);
        }
        #endregion

        #region 腦震盪篩檢-防護員評估
        /// <summary>
        /// 第三部分 認知篩檢-定位
        /// </summary>
        /// <returns></returns>
        public ActionResult CognitiveScreening()
        {
            // 從資料庫中讀取問卷問題
            var questions = _db.CognitiveScreening.ToList();

            // 為每個問題手動生成項次並轉換成 ViewModel
            var viewModel = questions.Select((q, index) => new CognitiveScreeningViewModel
            {
                OrderNumber = index + 1,  // 自動遞增項次
                Question = q.Question     // 顯示問題
            }).ToList();

            return PartialView("_CognitiveScreening", viewModel);
        }

        /// <summary>
        /// 第三部分 認知篩檢-短期記憶
        /// </summary>
        /// <returns></returns>
        public ActionResult ImmediateMemory()
        {
            // 從資料庫中讀取問卷問題
            var questions = _db.ImmediateMemory.ToList();

            // 為每個問題手動生成項次並轉換成 ViewModel
            var viewModel = questions.Select((q, index) => new CognitiveScreeningViewModel
            {
                OrderNumber = index + 1,  // 自動遞增項次
                Question = q.Word    // 顯示問題
            }).ToList();

            return PartialView("_ImmediateMemory", viewModel);
        }

        /// <summary>
        /// 第三部分 認知篩檢-專注力
        /// </summary>
        /// <returns></returns>
        //public ActionResult Concentration()
        //{
        //    return PartialView("_Concentration", viewModel);
        //}

        /// <summary>
        /// 協調與平衡測驗
        /// </summary>
        /// <returns></returns>
        public ActionResult CoordinationAndBalanceExamination()
        {
            return PartialView("_CoordinationAndBalanceExamination");
        }

        /// <summary>
        /// 延遲記憶
        /// </summary>
        /// <returns></returns>
        public ActionResult DelayedRecall()
        {
            return PartialView("_DelayedRecall");
        }

        /// <summary>
        /// 認知篩檢分數總合
        /// </summary>
        /// <returns></returns>
        public ActionResult CognitiveScreeningTotalScore()
        {
            // 從資料庫中讀取認知篩檢的各項分數
            var scores = _db.CognitiveScreeningScores.ToList();

            // 建立 ViewModel 並計算各項的分數
            var viewModel = new CognitiveScreeningViewModel
            {
                OrientationScore = scores.FirstOrDefault(x => x.ItemScreening == "定位 (Orientation)")?.TotalScore ?? 0,
                ImmediateMemoryScore = scores.FirstOrDefault(x => x.ItemScreening == "短期記憶 (Immediate Memory)")?.TotalScore ?? 0,
                ConcentrationScore = scores.FirstOrDefault(x => x.ItemScreening == "專注力 (Concentration)")?.TotalScore ?? 0,
                DelayedRecallScore = scores.FirstOrDefault(x => x.ItemScreening == "延遲記憶 (Delayed Recall)")?.TotalScore ?? 0,
                TotalScore = scores.Sum(x => x.TotalScore) // 總分計算
            };


            return PartialView("_CognitiveScreeningTotalScore", viewModel);
        }
        
        // 根據項目名稱取得對應的最大分數
        private int GetMaxScore(string itemScreening)
        {
            switch (itemScreening)
            {
                case "定位 (Orientation)":
                    return 5;
                case "短期記憶 (Immediate Memory)":
                    return 30;
                case "專注力 (Concentration)":
                    return 4;
                case "延遲記憶 (Delayed Recall)":
                    return 10;
                default:
                    return 0; // 預設為 0
            }
        }

        #endregion

        #region 骨科篩檢
        public ActionResult OrthopaedicScreening()
        {
            // 從資料庫中讀取問卷問題
            var questions = _db.OrthopaedicScreening.ToList();

            // 將資料轉換為 ViewModel
            var viewModel = questions.Select((q, index) => new OrthopaedicScreeningViewModel
            {
                OrderNumber = index + 1,  // 自動遞增項次
                Instructions = q.Instructions,
                ObservationPoints = q.ObservationPoints
            }).ToList();

            return PartialView("_OrthopaedicScreening", viewModel);
        }
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