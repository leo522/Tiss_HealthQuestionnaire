using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.Metadata.Edm;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Tiss_HealthQuestionnaire.Models;

namespace Tiss_HealthQuestionnaire.Controllers
{
    public class QuestionnaireController : Controller
    {
        private HealthQuestionnaireEntities _db = new HealthQuestionnaireEntities(); //資料庫

        #region 主頁
        public ActionResult Main()
        {
            // 確認登入狀態
            string loggedInUserName = Session["UserName"] as string;
            if (string.IsNullOrEmpty(loggedInUserName))
            {
                return RedirectToAction("Login", "Account");
            }

            var user = _db.AthleteUser.FirstOrDefault(u => u.Name == loggedInUserName);
            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }

            // 設置 ViewBag 值
            ViewBag.Specialist = user.SportSpecialization;
            ViewBag.FillName = user.Name;
            ViewBag.AtheNum = user.AthleteNumber;
            ViewBag.GenderID = user.GenderID;
            ViewBag.ShowFemaleTab = (user.GenderID == 2);

            // 整合問卷資料
            var viewModel = new QuestionnaireViewModel
            {
                PastHealthItems = _db.PastHealth.ToList(), //過去健康檢查病史
                AllergicHistoryItems = _db.AllergicHistory.ToList(), //過敏史
                FamilyHistoryItems = _db.FamilyHistory.ToList(), //家族病史
                PastHistoryItems = _db.PastHistory.ToList(), //過去病史
                PresentIllnessItems = _db.PresentIllness.ToList(), //現在病史
                PastDrugsItems = _db.PastDrugs.ToList(),
                TUE = "no",  // 預設值
                OtherDrug = "", // 預設為空
                PastSupplementsItems = _db.PastSupplements.ToList(), //營養品
                FemaleQuestionnaireItems = user.GenderID == 2 ? _db.FemaleQuestionnaire.ToList() : null, //女性問卷
                PastInjuryItems = _db.PastInjuryStatus.ToList(), //過去傷害(已復原)
                PastInjuryTypesItems = _db.PastInjuryType.ToList(), //過去傷害狀況(已復原)-傷勢類型
                //PastInjuryTypesByCategory = pastInjuryTypesByCategory,
                PastTreatmentItems = _db.PastTreatmentMethod.ToList(), //過去傷害狀況(已復原)-治療方法
                NowInjuryItems = _db.InjuryStatus.ToList(), //目前傷害狀況
                NowInjuryTypesItems = _db.InjuryType.ToList(), //目前傷害狀況-傷勢類型
                //NowInjuryTypesByCategory = pastInjuryTypesByCategory, //目前傷害狀況-傷勢類型
                NowTreatmentItems = _db.TreatmentMethod.ToList(), //目前傷害狀況-治療方法
                CardiovascularScreeningItems = _db.CardiovascularScreening.ToList(), //心血管篩檢
                ConcussionScreeningItems = _db.ConcussionScreening.ToList(), //腦震盪篩檢-選手自填-(1)
                SymptomEvaluationItems = _db.SymptomEvaluation.ToList(), //症狀自我評估-選手自填(2)
                OrthopaedicScreeningItems = _db.OrthopaedicScreening.ToList(), //骨科篩檢
                CognitiveScreeningItems = _db.CognitiveScreening.ToList(), //防護員填寫-認知篩檢-定位(1)
                ImmediateMemoryItems = _db.ImmediateMemory.ToList(), //防護員填寫-認知篩檢-短期記憶(2)
                ConcentrationItems = _db.Concentration.ToList(), //防護員填寫-認知篩檢-專注力(3)
                CoordinationAndBalanceItems = _db.CoordinationAndBalanceExamination.ToList(), //防護員填寫-認知篩檢-協調與平衡測驗(4)
                DelayedRecallItems = _db.DelayedRecall.ToList(), //防護員填寫-認知篩檢-延遲記憶(5)
            };

            return View(viewModel);
        }
        #endregion

        #region 問卷題目

        #region 過去健康檢查病史
        public ActionResult PastHealth()
        {
            var pastHealthItems = _db.PastHealth.ToList();
            return View("PastHealth", pastHealthItems);
        }
        #endregion

        #region 過敏史
        public ActionResult AllergicHistory()
        {
            var allergicHistoryItems = _db.AllergicHistory.ToList();
            return View("AllergicHistory", allergicHistoryItems);
        }
        #endregion

        #region 家族病史
        public ActionResult FamilyHistory()
        {
            var familyHistory = _db.FamilyHistory.ToList();
            return View("FamilyHistory", familyHistory);
        }
        #endregion

        #region 過去病史
        public ActionResult PastHistory()
        {
            var pastHistory = _db.PastHistory.ToList();
            return View("PastHistory", pastHistory);
        }
        #endregion

        #region 現在病史
        public ActionResult PresentIllness()
        {
            var presentIllness = _db.PresentIllness.ToList();
            return View("PresentIllness", presentIllness);
        }
        #endregion

        #region 藥物史
        public ActionResult PastDrugs()
        {
            var pastDrugs = _db.PastDrugs.Where(drug => drug.ID != 13).ToList();

            return View("PastDrugs", pastDrugs);
        }
        #endregion

        #region 營養品
        public ActionResult PastSupplements()
        {
            var pastSupplements = _db.PastSupplements.ToList();
            return View("PastSupplements", pastSupplements);
        }
        #endregion

        #region 女性問卷
        public ActionResult FemaleQuestionnaire()
        {
            var femaleQuestionnaire = _db.FemaleQuestionnaire.ToList();
            return View("FemaleQuestionnaire", femaleQuestionnaire);
        }
        #endregion

        #region 過去傷害狀況 (已復原)-(1)
        public ActionResult PastInjuryRestored() //過去有的症狀或疼痛部位
        {
            try
            {
                var pastInjury = _db.PastInjuryStatus.ToList();

                var PastInjuryList = pastInjury.Select(injury => new PastInjuryStatusViewModel
                {
                    Id = injury.Id,
                    PastInjuryPart = injury.InjuryPart,
                    PastIsSingleSelect = (injury.InjuryPart == "頭部/臉" || injury.InjuryPart == "頸椎" ||
                                      injury.InjuryPart == "胸椎" || injury.InjuryPart == "腰椎" ||
                                      injury.InjuryPart == "胸骨/肋骨" || injury.InjuryPart == "腹部" ||
                                      injury.InjuryPart == "骨盆/薦椎")
                }).ToList();

                return View("PastInjuryRestored", PastInjuryList);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region 過去傷勢類型-(2)
        [HttpGet]
        public JsonResult PastInjuryType()
        {
            try
            {
                var pastInjuryTypes = _db.PastInjuryType.ToList(); // 取得資料表內容

                var injuryCategories = new Dictionary<string, string>
                {
                    { "MuscleTendon", "肌肉/肌腱" },
                    { "Bone", "骨類" },
                    { "Ligament", "韌帶類" },
                    { "Nerve", "神經類" },
                    { "CartilageSynoviumBursa", "軟骨/滑膜/滑囊類" },
                    { "EpidermalTissue", "表皮組織" },
                    { "BloodVessel", "血管類" },
                    { "OrganLimb", "器官/四肢類" }
                };

                var pastInjuryTypesByCategory = new Dictionary<string, List<string>>();

                foreach (var category in injuryCategories.Keys)
                {
                    var values = pastInjuryTypes
                        .Select(i => i.GetType().GetProperty(category)?.GetValue(i, null)?.ToString()).Where(v => !string.IsNullOrEmpty(v)).Distinct().ToList();

                    pastInjuryTypesByCategory[injuryCategories[category]] = values;
                }

                return Json(pastInjuryTypesByCategory, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { error = "發生錯誤：" + ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }
        #endregion

        #region 過去治療方式-(3)
        public ActionResult PastTreatmentMethod()
        {
            var PastTreatmentMethod = _db.PastTreatmentMethod.ToList();
            return View("PastTreatmentMethod", PastTreatmentMethod);
        }
        #endregion

        #region 目前傷害狀況-(1)
        public ActionResult NowInjuryRestored() //目前有的症狀或疼痛部位
        {
            try
            {
                var nowInjury = _db.InjuryStatus.ToList();

                var injuryList = nowInjury.Select(injury => new InjuryStatusViewModel
                {
                    Id = injury.Id,
                    InjuryPart = injury.InjuryPart,
                    IsSingleSelect = (injury.InjuryPart == "頭部/臉" || injury.InjuryPart == "頸椎" ||
                                      injury.InjuryPart == "胸椎" || injury.InjuryPart == "腰椎" ||
                                      injury.InjuryPart == "胸骨/肋骨" || injury.InjuryPart == "腹部" ||
                                      injury.InjuryPart == "骨盆/薦椎")
                }).ToList();

                return View("NowInjuryRestored", injuryList);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region 目前傷勢類型-(2)
        [HttpGet]
        public JsonResult NowInjuryType()
        {
            try
            {
                var NowInjuryTypes = _db.InjuryType.ToList(); // 取得資料表內容

                var NowinjuryCategories = new Dictionary<string, string>
                {
                    { "MuscleTendon", "肌肉/肌腱" },
                    { "Bone", "骨類" },
                    { "Ligament", "韌帶類" },
                    { "Nerve", "神經類" },
                    { "CartilageSynoviumBursa", "軟骨/滑膜/滑囊類" },
                    { "EpidermalTissue", "表皮組織" },
                    { "BloodVessel", "血管類" },
                    { "OrganLimb", "器官/四肢類" }
                };

                var NowInjuryTypesByCategory = new Dictionary<string, List<string>>();

                foreach (var category in NowinjuryCategories.Keys)
                {
                    var values = NowInjuryTypes
                        .Select(i => i.GetType().GetProperty(category)?.GetValue(i, null)?.ToString()).Where(v => !string.IsNullOrEmpty(v)).Distinct().ToList();

                    NowInjuryTypesByCategory[NowinjuryCategories[category]] = values;
                }

                return Json(NowInjuryTypesByCategory, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { error = "發生錯誤：" + ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }
        #endregion

        #region 目前治療方式-(3)
        public ActionResult NowTreatmentMethod()
        {
            var treatmentMethod = _db.TreatmentMethod.ToList();
            return View("NowTreatmentMethod", treatmentMethod);
        }
        #endregion

        #region 心血管篩檢
        public ActionResult CardiovascularScreening()
        {
            var questions = _db.CardiovascularScreening.ToList();

            var viewModel = questions.Select((q, index) => new CardiovascularScreeningViewModel
            {
                OrderNumber = index + 1, //自動遞增項次
                Question = q.Question
            }).ToList();

            return View("CardiovascularScreening", viewModel);
        }
        #endregion

        #region 腦震盪篩檢-選手自填-選手背景(1) 
        public ActionResult ConcussionScreening()
        {
            var questions = _db.ConcussionScreening.ToList();

            var viewModel = questions.Select((q, index) => new ConcussionScreening
            {
                Id = index + 1, //自動遞增項次
                Question = q.Question
            }).ToList();

            // 從 Session 恢復數據
            if (Session["ConcussionScreeningAnswers"] is Dictionary<int, string> savedAnswers)
            {
                foreach (var item in viewModel)
                {
                    if (savedAnswers.ContainsKey(item.Id))
                    {
                        item.Question = savedAnswers[item.Id];
                    }
                }
            }

            // 恢復藥物及備註
            ViewBag.MedicationAnswer = Session["ConcussionScreeningMedicationAnswer"] as string;
            ViewBag.MedicationDetails = Session["ConcussionScreeningMedicationDetails"] as string;
            ViewBag.Notes = Session["ConcussionScreeningNotes"] as string;

            return View("ConcussionScreening", viewModel);
        }
        #endregion

        #region 腦震盪篩檢-選手自填-症狀自我評估(2)
        [HttpGet]
        public ActionResult SymptomEvaluation()
        {
            var questions = _db.SymptomEvaluation.ToList();

            var viewModel = questions.Select((q, index) => new ConcussionScreening
            {
                Id = index + 1, //自動遞增項次
                Question = q.SymptomItem
            }).ToList();

            if (Session["SymptomEvaluationAnswers"] is Dictionary<int, int> savedAnswers)
            {
                foreach (var item in viewModel)
                {
                    if (savedAnswers.ContainsKey(item.Id))
                    {
                        item.Id = savedAnswers[item.Id];
                    }
                }
            }

            return View("SymptomEvaluation", viewModel);
        }

        #endregion

        #region 骨科篩檢
        public ActionResult OrthopaedicScreening()
        {
            var questions = _db.OrthopaedicScreening.ToList();

            var viewModel = questions.Select((q, index) => new OrthopaedicScreeningViewModel
            {
                OrderNumber = index + 1, //自動遞增項次
                Instructions = q.Instructions,
                ObservationPoints = q.ObservationPoints
            }).ToList();

            return View("OrthopaedicScreening", viewModel);
        }
        #endregion

        #region 腦震盪篩檢-防護員評估-認知篩檢-定位(1)
        public ActionResult CognitiveScreening()
        {
            try
            {
                var questions = _db.CognitiveScreening.ToList();

                // 從 Session 取出先前回答，若沒有就給空的 Dictionary
                var savedAnswers = Session["CognitiveScreeningAnswers"] as Dictionary<int, int>
                                   ?? new Dictionary<int, int>();

                var viewModel = questions.Select((q, index) => new CognitiveScreening
                {
                    ID = index + 1,
                    Question = q.Question,
                    AnswerOption1 = savedAnswers.ContainsKey(index + 1)
                                       ? savedAnswers[index + 1]
                                       : 0
                }).ToList();

                return PartialView("CognitiveScreening", viewModel);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region 腦震盪篩檢-防護員評估-認知篩檢-短期記憶(2)
        public ActionResult ImmediateMemory()
        {
            var questions = _db.ImmediateMemory.ToList();

            // 從 Session 中讀取已保存的數據
            var immediateMemoryAnswers = Session["ImmediateMemoryAnswers"] as Dictionary<string, int>;
            var completionTime = Session["ImmediateMemoryCompletionTime"] as string ?? "00:00";

            int totalScore = 0;

            var viewModel = questions.Select(q =>
            {
                int firstScore = 0, secondScore = 0, thirdScore = 0;

                immediateMemoryAnswers?.TryGetValue($"first_{q.ID}", out firstScore);
                immediateMemoryAnswers?.TryGetValue($"second_{q.ID}", out secondScore);
                immediateMemoryAnswers?.TryGetValue($"third_{q.ID}", out thirdScore);

                totalScore += (firstScore + secondScore + thirdScore);

                return new ImmediateMemory
                {
                    ID = q.ID,
                    Word = q.Word,
                    FirstTest0 = firstScore,
                    SecondTest0 = secondScore,
                    ThirdTest1 = thirdScore,
                    //CompletionTime = completionTime
                };
            }).ToList();

            Session["ImmediateMemoryTotalScore"] = totalScore;

            return PartialView("ImmediateMemory", viewModel);
        }
        #endregion

        #region 腦震盪篩檢-防護員評估-認知篩檢-專注力(3)
        public ActionResult Concentration()
        {
            // 從資料庫取得 (範例直接硬寫四筆)
            var viewModel = new List<Concentration>
            {
                new Concentration { OrderNumber = 1, ListA = "4-9-3",  ListB = "5-2-6",  ListC = "1-4-2" },
                new Concentration { OrderNumber = 2, ListA = "6-2-9",  ListB = "4-1-5",  ListC = "6-5-8" },
                new Concentration { OrderNumber = 3, ListA = "3-8-1-4",ListB = "1-7-9-5",ListC = "6-8-3-1" },
                new Concentration { OrderNumber = 4, ListA = "3-2-7-9",ListB = "4-9-6-8",ListC = "3-4-8-1" }
            };

            // 從 Session 恢復先前填寫(若有)
            var savedScores = Session["ConcentrationScores"] as Dictionary<int, int>;
            if (savedScores != null)
            {
                foreach (var item in viewModel)
                {
                    if (savedScores.ContainsKey(item.OrderNumber))
                    {
                        item.OrderNumber = savedScores[item.OrderNumber];
                    }
                }
            }

            return PartialView("Concentration", viewModel);
        }
        #endregion

        #region 腦震盪篩檢-防護員評估-認知篩檢-協調與平衡測驗(4)
        public ActionResult CoordinationAndBalanceExamination()
        {
            var model = new CoordinationAndBalanceExamination
            {
                TestFoot = "",
                TestSurface = "",
                Footwear = "",
                //DoubleLegError = 0,
                //TandemError = 0,
                //SingleLegError = 0,
                //FirstTime = 0,
                //SecondTime = 0,
                //ThirdTime = 0
            };

            if (Session["CoordinationAndBalanceData"] is CoordinationAndBalanceExamination savedData)
            {
                model = savedData;
            }

            return PartialView("CoordinationAndBalanceExamination", model);
        }
        #endregion

        #region 腦震盪篩檢-防護員評估-認知篩檢-延遲記憶(5)
        public ActionResult DelayedRecall()
        {
            var questions = _db.DelayedRecall.ToList();

            var delayedRecallAnswers = Session["DelayedRecallAnswers"] as Dictionary<int, int>;
            var delayedRecallStartTime = Session["DelayedRecallStartTime"] as string;

            var viewModel = questions.Select(q =>
            {
                int Scores = 0;
                delayedRecallAnswers?.TryGetValue(q.ID, out Scores);

                return new DelayedRecall
                {
                    ID = q.ID,
                    Word = q.Word,
                    Score0 = Scores
                };
            }).ToList();

            ViewBag.DelayedRecallStartTime = delayedRecallStartTime;

            return PartialView("DelayedRecall", viewModel);
        }
        #endregion

        #region 腦震盪篩檢-防護員評估-認知篩檢-認知篩檢分數總合(6)
        public ActionResult CognitiveScreeningTotalScore()
        {
            var scores = _db.CognitiveScreeningScores.ToList();

            var viewModel = new CognitiveScreening
            {
                //Orientation = scores.FirstOrDefault(x => x.ItemScreening == "定位 (Orientation)")?.TotalScore ?? 0,
                //ImmediateMemory = scores.FirstOrDefault(x => x.ItemScreening == "短期記憶 (Immediate Memory)")?.TotalScore ?? 0,
                //ConcentrationScore = scores.FirstOrDefault(x => x.ItemScreening == "專注力 (Concentration)")?.TotalScore ?? 0,
                //DelayedRecallScore = scores.FirstOrDefault(x => x.ItemScreening == "延遲記憶 (Delayed Recall)")?.TotalScore ?? 0,
                //TotalScore = scores.Sum(x => x.TotalScore)
            };

            return PartialView("CognitiveScreeningTotalScore", viewModel);
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

        #region 防護員AT驗證身分-測試

        // 驗證防護員身份
        [HttpPost]
        public JsonResult ValidateAthleticTrainer(string userName, string password)
        {
            // 驗證防護員帳號和密碼
            var trainer = _db.Test_AthleticTrainer.FirstOrDefault(at => at.ATName == userName && at.IsActive);

            if (trainer != null && VerifyPassword(password, trainer.ATNumber)) //假設有密碼加密驗證方法VerifyPassword
            {
                Session["TrainerAuthenticated"] = true; //將身份驗證成功標記保存到 Session
                Session["TrainerUserName"] = userName;

                return Json(new { success = true });
            }

            return Json(new { success = false });
        }

        private bool VerifyPassword(string inputPassword, string storedPassword)
        {
            return inputPassword == storedPassword; //實際應用中應進行加密驗證
        }

        #endregion

        #region 開刀史-不使用
        //public ActionResult SurgeryHistory()
        //{
        //    var surgeryHistory = _db.SurgeryHistory.ToList();
        //    return View("SurgeryHistory", surgeryHistory);
        //}
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

        #endregion

        #region 處理過去傷勢(已復原)-跳轉頁
        [HttpPost]
        public ActionResult pastInjuryStatusNextStep(QuestionnaireViewModel model, string pastInjuryStatus)
        {
            Session["PastInjuryStatus"] = model.PastInjuryStatusAnswer; // 這裡存的是 "no" 或 "yes"
            if (pastInjuryStatus == "no")
            {
                return RedirectToAction("NowInjuryStatus"); // 進入目前傷害問卷
            }

            return RedirectToAction("PastInjuryType"); //否則進入傷勢類型頁面
        }

        //過去傷勢類型
        [HttpPost]
        public ActionResult pastInjuryTypesNextStep(QuestionnaireViewModel model)
        {
            if (model.PastInjuryTypesItems == null) //確保不為 `null`
            {
                model.PastInjuryTypesItems = new List<PastInjuryType>();
            }

            Session["SelectedInjuryTypes"] = model.PastInjuryTypesItems; //存入 Session

            return RedirectToAction("PastTreatmentMethod");
        }

        //過去傷勢治療方法
        [HttpPost]
        public ActionResult pastInjuryTreatmentNextStep(QuestionnaireViewModel model, List<string> SelectedTreatmentMethods, string OtherTreatment)
        {
            if (SelectedTreatmentMethods == null) // 確保不為 null
            {
                SelectedTreatmentMethods = new List<string>();
            }

            Session["SelectedTreatmentMethods"] = SelectedTreatmentMethods ?? new List<string>();
            Session["OtherTreatment"] = OtherTreatment ?? "";

            return RedirectToAction("NowInjuryRestored");  //跳到「目前傷害」的問卷
        }
        #endregion

        #region 處理目前過去傷勢(已復原)-跳轉頁
        [HttpPost]
        public ActionResult NowInjuryNextStep(QuestionnaireViewModel model, string NowInjuryStatus)
        {
            Session["NowInjuryStatus"] = NowInjuryStatus; //存入 Session

            if (NowInjuryStatus == "no")
            {
                return RedirectToAction("CardiovascularScreening"); //直接跳到下一個問卷
            }

            return RedirectToAction("NowInjuryType"); //否則進入傷勢類型頁面
        }

        // 目前傷勢類型
        [HttpPost]
        public ActionResult NowInjuryTypeNextStep(List<string> SelectedNowInjuryTypes)
        {
            if (SelectedNowInjuryTypes == null)
            {
                SelectedNowInjuryTypes = new List<string>();
            }

            Session["SelectedNowInjuryTypes"] = SelectedNowInjuryTypes; //存入 Session

            return RedirectToAction("NowTreatmentMethod");
        }

        // 目前治療方法
        [HttpPost]
        public ActionResult NowTreatmentNextStep(List<string> SelectedNowTreatmentMethods, string OtherTreatment)
        {
            if (SelectedNowTreatmentMethods == null)
            {
                SelectedNowTreatmentMethods = new List<string>();
            }

            Session["SelectedNowTreatmentMethods"] = SelectedNowTreatmentMethods; // 存入 Session
            Session["NowOtherTreatment"] = OtherTreatment;

            return RedirectToAction("CardiovascularScreening");  // 進入下一個問卷
        }
        #endregion

        #region 預覽頁
        public ActionResult Preview(QuestionnaireViewModel model)
        {
            return View("Preview", model);
        }

        [HttpPost]
        public ActionResult Preview(QuestionnaireViewModel model, FormCollection form)
        {
            try
            {
                ProcessBasicInfo(model, form);               // 基本資料
                ProcessPastHealth(model, form);              // 過去健康檢查病史
                ProcessAllergicHistory(model, form);         // 過敏史
                ProcessFamilyHistory(model, form);           // 家族病史
                ProcessPastHistory(model, form);             // 過去病史
                ProcessPresentIllness(model, form);          // 現在病史
                ProcessPastDrugs(model, form);               // 藥物史
                ProcessPastSupplements(model, form);         // 營養品
                ProcessFemaleQuestionnaire(model, form);     // 女性問卷
                ProcessPastInjuryStatus(model, form);        // 過去傷害狀況 (已復原)
                //ProcessCurrentInjuryStatus(model, form);     // 目前傷害狀況
                ProcessCardiovascularScreening(model, form); // 心血管篩檢
                ProcessConcussionScreening(model, form);     //腦震盪篩檢-選手自填(1)
                ProcessSymptomEvaluation(model, form);     //症狀自我評估-選手自填(2)
                //ProcessCognitiveScreening(model, form);      // 認知篩檢 (包含短期記憶、專注力等)
                ProcessOrthopaedicScreening(model, form);    // 骨科篩檢

                return View("Preview", model); // 返回預覽頁
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "發生錯誤：" + ex.Message);
                return View("Main", model);
            }
        }
        #endregion

        #region 問卷分區處理方法

        #region 基本資料
        private void ProcessBasicInfo(QuestionnaireViewModel model, FormCollection form)
        {
            model.Specialist = form["specialist"];
            model.FillName = form["fillName"];
            model.AtheNum = form["atheNum"];
            model.Gender = int.Parse(form["gender"]);
            model.FillDate = DateTime.Parse(form["fillDate"]);

            foreach (var key in form.AllKeys)
            {
                model.FormData[key] = form[key] ?? "";
            }
        }
        #endregion

        #region 過去健康檢查病史
        private void ProcessPastHealth(QuestionnaireViewModel model, FormCollection form)
        {
            model.PastHealthItems = _db.PastHealth.ToList(); // 讀取題目

            bool hasYes = false; // 判斷是否有勾選 "是"

            model.PastHealthResponses = new Dictionary<string, string>();

            foreach (var item in model.PastHealthItems)
            {
                var selectedValue = form[$"pastHealth_{item.ID}"];
                item.IsYes = selectedValue == "yes";
                item.IsNo = selectedValue == "no";

                if (item.IsYes)
                {
                    hasYes = true; // 只要有一個是 "yes"，PastHealthHistory 就是 "yes"

                    // 取得詳細填寫內容
                    var details = new List<string>();
                    if (!string.IsNullOrEmpty(form[$"item1_{item.ID}"])) details.Add(form[$"item1_{item.ID}"]);
                    if (!string.IsNullOrEmpty(form[$"item2_{item.ID}"])) details.Add(form[$"item2_{item.ID}"]);
                    if (!string.IsNullOrEmpty(form[$"item3_{item.ID}"])) details.Add(form[$"item3_{item.ID}"]);

                    // 如果有填寫細節，就存入 Dictionary
                    if (details.Any())
                    {
                        model.PastHealthResponses[item.ID.ToString()] = string.Join(", ", details);
                    }
                }
            }

            model.PastHealthHistory = hasYes ? "yes" : "no"; // 設定總狀態
        }
        #endregion

        #region 過敏史
        private void ProcessAllergicHistory(QuestionnaireViewModel model, FormCollection form)
        {
            model.AllergicHistoryItems = new List<AllergicHistory>();

            var allergicItems = _db.AllergicHistory.ToList(); // 只讀取題目
            foreach (var item in allergicItems)
            {
                var selectedValue = form[$"allergy_{item.ID}"];
                var allergyDescription = form[$"details_{item.ID}"] ?? ""; // 取得過敏描述

                if (selectedValue == "yes") // 只存有選擇 "是" 的項目
                {
                    model.AllergicHistoryItems.Add(new AllergicHistory
                    {
                        ID = item.ID,
                        ItemZh = item.ItemZh,
                        ItemEn = item.ItemEn,
                        IsYes = true,
                        IsNo = false,
                    });

                    // 存入過敏描述
                    if (!string.IsNullOrEmpty(allergyDescription))
                    {
                        if (model.AllergicHistoryResponses == null)
                        {
                            model.AllergicHistoryResponses = new Dictionary<string, string>();
                        }
                        model.AllergicHistoryResponses[item.ID.ToString()] = allergyDescription;
                    }
                }
            }

            // 確定過敏史的總狀態
            model.AllergicHistory = model.AllergicHistoryItems.Any() ? "yes" : "no";
        }
        #endregion

        #region 家族病史
        private void ProcessFamilyHistory(QuestionnaireViewModel model, FormCollection form)
        {
            model.FamilyHistoryItems = _db.FamilyHistory.ToList();

            foreach (var item in model.FamilyHistoryItems)
            {
                var selectedValue = form[$"familyHistory_{item.ID}"];

                item.IsYes = selectedValue == "yes";
                item.IsNo = selectedValue == "no";
                item.IsUnknown = selectedValue == "unknown";

                // 特別處理 "其他" 選項
                if (item.ID == 10) // "其他" 的 ID 為 10
                {
                    model.OtherFamilyHistory = form["otherFamilyHistory"]?.Trim() ?? ""; // 存到獨立的屬性
                }
            }
        }
        #endregion

        #region 過去病史
        private void ProcessPastHistory(QuestionnaireViewModel model, FormCollection form)
        {
            model.PastHistoryItems = _db.PastHistory.ToList();

            foreach (var item in model.PastHistoryItems)
            {
                var selectedValue = form[$"pastHistory_{item.ID}"];

                item.IsYes = selectedValue == "yes";
                item.IsNo = selectedValue == "no";
                item.IsUnknown = selectedValue == "unknown";

                // 處理 "其他" 選項 (假設 "其他" 的 ID 是 20)
                if (item.ID == 20)
                {
                    model.OtherPastHistory = form["otherPastHistory"]?.Trim() ?? "";
                }
            }
        }
        #endregion

        #region 現在病史
        private void ProcessPresentIllness(QuestionnaireViewModel model, FormCollection form)
        {
            model.PresentIllnessItems = _db.PresentIllness.ToList();

            foreach (var item in model.PresentIllnessItems)
            {
                var selectedValue = form[$"presentIllness_{item.ID}"];

                item.IsYes = selectedValue == "yes";
                item.IsNo = selectedValue == "no";
            }
        }
        #endregion

        #region 藥物史
        private void ProcessPastDrugs(QuestionnaireViewModel model, FormCollection form)
        {
            var pastDrugsItems = _db.PastDrugs.ToList();
            model.PastDrugsItems = new List<PastDrugs>();

            foreach (var item in pastDrugsItems)
            {
                bool isUsed = form[$"drug_{item.ID}"] == "on";
                if (isUsed)
                {
                    var drug = new PastDrugs
                    {
                        ID = item.ID,
                        ItemZh = item.ItemZh,
                        ItemEn = item.ItemEn,
                        IsUsed = true
                    };

                    model.PastDrugsItems.Add(drug);
                }
            }

            if (!string.IsNullOrEmpty(form["OtherDrug"]))
            {
                model.OtherDrug = form["OtherDrug"];
            }

            model.TUE = form["TUE"] ?? "未填寫";  // 如果沒填寫則顯示 "未填寫"
        }
        #endregion

        #region 營養品
        private void ProcessPastSupplements(QuestionnaireViewModel model, FormCollection form)
        {
            model.PastSupplementsItems = new List<PastSupplements>();

            var supplementsItems = _db.PastSupplements.ToList();

            foreach (var item in supplementsItems)
            {
                
                bool isUsed = form[$"supplement_{item.ID}"] == "on"; //表單名稱抓取方式 (supplement_@item.ID)

                if (isUsed)
                {
                    var supplement = new PastSupplements
                    {
                        ID = item.ID,
                        ItemZh = item.ItemZh,
                        ItemEn = item.ItemEn,
                        IsUsed = true
                    };

                    model.PastSupplementsItems.Add(supplement);
                }
            }
            if (!string.IsNullOrEmpty(form["OtherSupplements"]))
            {
                model.OtherSupplements = form["OtherSupplements"];
            }
        }
        #endregion

        #region 女性問卷
        private void ProcessFemaleQuestionnaire(QuestionnaireViewModel model, FormCollection form)
        {
            if (model.Gender == 2) // 只有女性才處理
            {
                model.FemaleQuestionnaireItems = _db.FemaleQuestionnaire.ToList(); // 取得所有女性問卷題目
                model.FemaleQuestionnaireAnswers = new Dictionary<int, string>(); // 確保初始化

                foreach (var item in model.FemaleQuestionnaireItems)
                {
                    string answerKey = $"femaleQ_{item.ID}"; // 與 HTML name 對應
                    string answer = form[answerKey] ?? "未回答"; // 取得使用者填寫的答案

                    // 第一題 (第一次生理期時間) 需要處理選項
                    if (item.ID == 1)
                    {
                        answer = form["femaleQ_1"] ?? "未填寫"; // 正確取得選擇的值
                    }

                    model.FemaleQuestionnaireAnswers[item.ID] = answer; // 存入 Dictionary
                }
            }
        }
        #endregion

        #region 過去傷害狀況-已復原
        private void ProcessPastInjuryStatus(QuestionnaireViewModel model, FormCollection form)
        {
            var pastInjuryItems = _db.PastInjuryStatus.ToList(); // 取得所有傷害部位
            model.PastInjuryItems = new List<PastInjuryStatus>(); // 初始化傷害部位列表

            foreach (var item in pastInjuryItems)
            {
                string partKey = $"pastinjury_{item.Id}"; // 傷害部位的 Checkbox 名稱
                bool isInjured = form[partKey] == "on"; // 是否勾選受傷部位

                if (isInjured)
                {
                    model.PastInjuryItems.Add(new PastInjuryStatus
                    {
                        InjuryPart = item.InjuryPart
                    });
                }
            }
        }
        #endregion

        #region 過去傷害狀況-治療方式
        private void ProcessPastTreatmentMethod(QuestionnaireViewModel model, FormCollection form)
        {
            var pastTreatmentMethods = _db.PastTreatmentMethod.ToList();
            model.PastTreatmentItems = new List<PastTreatmentMethod>();

            foreach (var method in pastTreatmentMethods)
            {
                string methodKey = $"Pasttreatment_{method.Id}";

                if (form[methodKey] == "on")
                {
                    model.PastTreatmentItems.Add(new PastTreatmentMethod
                    {
                        Method = method.Method
                    });
                }
            }
        }
        #endregion

        #region 目前傷害狀況
        private void ProcessCurrentInjuryStatus(QuestionnaireViewModel model, FormCollection form)
        {
            var injuryItems = _db.InjuryStatus.ToList(); //從資料庫獲取目前的傷害部位數據

            model.NowInjuryItems = new List<InjuryStatus>(); //初始化傷害部位詳細資料

            foreach (var item in injuryItems)
            {
                string leftPartKey = $"NowinjuryLeft_{item.Id}";
                string rightPartKey = $"NowinjuryRight_{item.Id}";

                bool hasInjury = form[leftPartKey] != null || form[rightPartKey] != null; //判斷是否選擇了左側或右側的傷害

                if (hasInjury)
                {
                    // 收集表單中固定鍵名的傷勢類型
                    var injuryTypes = new List<string>
                    {
                        form["NowmuscleTendon"],          // 肌肉/肌腱
                        form["Nowbone"],                 // 骨骼
                        form["Nowligament"],             // 韌帶
                        form["Nownerve"],                // 神經
                        form["NowcartilageSynoviumBursa"], // 軟骨/滑膜/滑囊
                        form["NowepidermalTissue"],      // 表皮組織
                        form["NowbloodVessel"],          // 血管
                        form["NoworganLimb"]             // 器官/四肢
                    }.Where(x => !string.IsNullOrEmpty(x)).ToList();

                    // 將詳細信息添加到 ViewModel
                    model.NowInjuryItems.Add(new InjuryStatus
                    {
                        InjuryPart = item.InjuryPart,            // 傷害部位名稱
                        //LeftSide = !string.IsNullOrEmpty(form[leftPartKey]), // 左側是否受傷
                        //RightSide = !string.IsNullOrEmpty(form[rightPartKey]), // 右側是否受傷
                        //InjuryTypes = injuryTypes              // 綁定傷勢類型
                    });
                }
            }
        }
        #endregion

        #region 目前傷害狀況-治療方式
        private void ProcessCurrentTreatmentMethod(QuestionnaireViewModel model, FormCollection form)
        {
            var treatmentMethods = _db.TreatmentMethod.ToList();
            foreach (var method in treatmentMethods)
            {
                string methodKey = $"Nowtreatment_{method.Id}";
                if (!string.IsNullOrEmpty(form[methodKey]))
                {
                    model.NowTreatmentItems.Add(new TreatmentMethod
                    {
                        Method = method.Method
                    });
                }
            }
        }
        #endregion

        #region 心血管篩檢
        private void ProcessCardiovascularScreening(QuestionnaireViewModel model, FormCollection form)
        {
            var screenings = _db.CardiovascularScreening.ToList();  // 取得題目
            model.CardiovascularScreeningItems = new List<CardiovascularScreening>(); // 確保初始化

            foreach (var item in screenings)
            {
                string answerKey = $"question_{item.Id}";  // 與 HTML input name 對應
                string answer = form[answerKey];  // 取得使用者選擇的值 ("yes" / "no")

                bool? response = null; // 預設為 null，避免 CS0029 錯誤

                if (answer == "yes") response = true;  // "yes" → true
                else if (answer == "no") response = false;  // "no" → false

                model.CardiovascularScreeningItems.Add(new CardiovascularScreening
                {
                    Id = item.Id,
                    Question = item.Question,
                    Response = response  // 對應資料表的 `bit` (bool?)
                });
            }
        }
        #endregion

        #region 腦震盪篩檢-選手自填-(1)
        private void ProcessConcussionScreening(QuestionnaireViewModel model, FormCollection form)
        {
            model.ConcussionScreeningItems = _db.ConcussionScreening.ToList(); // 取得所有題目
            model.ConcussionScreeningAnswers = new Dictionary<int, string>(); // 存使用者回答

            foreach (var item in model.ConcussionScreeningItems)
            {
                string answerKey = $"ConcussionQuestion_{item.Id}"; // 表單名稱對應
                var answer = form[answerKey] ?? "no"; // 預設 "no"

                model.ConcussionScreeningAnswers[item.Id] = answer == "yes" ? "是" : "否"; // 轉換成中文
            }

            // 服用藥物 & 備註
            model.ConcussionScreeningMedicationAnswer = form["Medication"] == "yes" ? "是" : "否";
            model.ConcussionScreeningMedicationDetails = form["MedicationDetails"] ?? "無";
            model.ConcussionScreeningNotes = form["Notes"] ?? "無";
        }
        #endregion

        #region 症狀自我評估-選手自填-(2)
        private void ProcessSymptomEvaluation(QuestionnaireViewModel model, FormCollection form)
        {
            model.SymptomEvaluationItems = _db.SymptomEvaluation.ToList(); // 取得所有題目
            model.SymptomEvaluationAnswers = new Dictionary<int, int>(); // 存使用者選擇的分數

            foreach (var item in model.SymptomEvaluationItems)
            {
                string scoreKey = $"SymptomScore_{item.ID}"; // 表單名稱對應
                int score = 0; // 預設分數為 0

                if (!string.IsNullOrEmpty(form[scoreKey]))
                {
                    int.TryParse(form[scoreKey], out score); // 轉換為 int
                }

                model.SymptomEvaluationAnswers[item.ID] = score; // 存入 Dictionary
            }
        }
        #endregion

        #region 骨科篩檢
        private void ProcessOrthopaedicScreening(QuestionnaireViewModel model, FormCollection form)
        {
            model.OrthopaedicScreeningItems = new List<OrthopaedicScreening>(); // 確保初始化

            var screenings = _db.OrthopaedicScreening.ToList(); // 取得所有骨科篩檢題目
            foreach (var item in screenings)
            {
                string resultKey = $"Result_{item.Id}"; // 表單對應的 key
                string answer = form[resultKey]; // 使用者選擇的值 ("normal" / "abnormal")

                bool? response = null; // 預設為 null 避免 CS0029 錯誤
                if (answer == "normal") response = true;  // "正常" → `true`
                else if (answer == "abnormal") response = false;  // "異常" → `false`

                model.OrthopaedicScreeningItems.Add(new OrthopaedicScreening
                {
                    Id = item.Id,
                    Instructions = item.Instructions,
                    ObservationPoints = item.ObservationPoints,
                    ResultNormal = item.ResultNormal, // 正常選項
                    ResultAbnormal = item.ResultAbnormal, // 異常選項
                    Response = response  // `bool?` 來對應 `bit` 型別
                });
            }
        }
        #endregion

        #region 醫療團隊-認知篩檢 (1~6)
        //private void ProcessCognitiveScreening(QuestionnaireViewModel model, FormCollection form)
        //{
        //    // 定位 (Orientation) (1)
        //    var orientationItems = _db.CognitiveScreening.ToList();
        //    foreach (var item in orientationItems)
        //    {
        //        string answerKey = $"question_{item.ID}";
        //        var answer = form[answerKey];
        //        int score = answer == "1" ? 1 : 0;

        //        model.CognitiveScreeningDetails.Add(new CognitiveScreeningViewModel
        //        {
        //            OrderNumber = item.ID,
        //            Question = item.Question,
        //            OrientationScore = score
        //        });
        //    }
        //    model.CognitiveScreeningTotalScore = model.CognitiveScreeningDetails.Sum(x => x.OrientationScore);

        //    // 短期記憶 (Immediate Memory) (2)
        //    var immediateMemoryItems = _db.ImmediateMemory.ToList();
        //    foreach (var item in immediateMemoryItems)
        //    {
        //        model.ImmediateMemoryDetails.Add(new ImmediateMemoryViewModel
        //        {
        //            OrderNumber = item.ID,
        //            Word = item.Word,
        //            FirstTestScore = int.Parse(form[$"first_{item.ID}"] ?? "0"),
        //            SecondTestScore = int.Parse(form[$"second_{item.ID}"] ?? "0"),
        //            ThirdTestScore = int.Parse(form[$"third_{item.ID}"] ?? "0")
        //        });
        //    }
        //    model.ImmediateMemoryTotalScore = model.ImmediateMemoryDetails.Sum(x => x.FirstTestScore + x.SecondTestScore + x.ThirdTestScore);
        //    model.CompletionTime = form["CompletionTime"] ?? "00:00";

        //    // 專注力 (Concentration) (3)
        //    var concentrationItems = _db.Concentration.ToList();
        //    foreach (var item in concentrationItems)
        //    {
        //        string answerKey = $"response_{item.Id}";
        //        var answer = form[answerKey];
        //        int score = answer == "1" ? 1 : 0;

        //        model.ConcentrationDetails.Add(new ConcentrationViewModel
        //        {
        //            OrderNumber = item.Id,
        //            ListA = item.ListA,
        //            ListB = item.ListB,
        //            ListC = item.ListC,
        //            Score = score
        //        });
        //    }
        //    model.ConcentrationTotalScore = model.ConcentrationDetails.Sum(x => x.Score);

        //    // 協調與平衡測驗 (Coordination and Balance Examination) (4)
        //    var coordinationItem = new CoordinationAndBalanceExaminationViewModel
        //    {
        //        TestFoot = form["TestFoot"],
        //        TestSurface = form["TestSurface"],
        //        Footwear = form["Footwear"],
        //        DoubleLegError = int.Parse(form["DoubleLegError"] ?? "0"),
        //        TandemError = int.Parse(form["TandemError"] ?? "0"),
        //        SingleLegError = int.Parse(form["SingleLegError"] ?? "0"),
        //        FirstTime = float.Parse(form["FirstTime"] ?? "0"),
        //        SecondTime = float.Parse(form["SecondTime"] ?? "0"),
        //        ThirdTime = float.Parse(form["ThirdTime"] ?? "0")
        //    };
        //    coordinationItem.TotalErrors = coordinationItem.DoubleLegError + coordinationItem.TandemError + coordinationItem.SingleLegError;
        //    coordinationItem.AverageTimes = (coordinationItem.FirstTime + coordinationItem.SecondTime + coordinationItem.ThirdTime) /3;
        //    coordinationItem.FastestTimes = Math.Min(coordinationItem.FirstTime, Math.Min(coordinationItem.SecondTime, coordinationItem.ThirdTime));
        //    model.CoordinationAndBalanceDetails.Add(coordinationItem);
        //    model.CoordinationAndBalanceTotalErrors = coordinationItem.TotalErrors;
        //    model.CoordinationAndBalanceAverageTime = coordinationItem.AverageTimes;
        //    model.CoordinationAndBalanceFastestTime = coordinationItem.FastestTimes;

        //    // 延遲記憶 (Delayed Recall) (5)
        //    var delayedRecallItems = _db.DelayedRecall.ToList();
        //    foreach (var item in delayedRecallItems)
        //    {
        //        string scoreKey = $"score_{item.ID}";
        //        int score = int.Parse(form[scoreKey] ?? "0");

        //        model.DelayedRecallDetails.Add(new DelayedRecallViewModel
        //        {
        //            OrderNumber = item.ID,
        //            Word = item.Word,
        //            Score = score
        //        });
        //    }
        //    model.DelayedRecallTotalScore = model.DelayedRecallDetails.Sum(x => x.Score);
        //    model.DelayedRecallStartTime = form["testStartTime"] ?? "00:00";

        //    // 分數總合 (6)
        //    model.CognitiveScreeningTotalScoreDetails.Add(new CognitiveScreeningTotalScoreViewModel
        //    {
        //        OrientationScore = model.CognitiveScreeningTotalScore,
        //        ImmediateMemoryScore = model.ImmediateMemoryTotalScore,
        //        ConcentrationScore = model.ConcentrationTotalScore,
        //        DelayedRecallScore = model.DelayedRecallTotalScore,
        //        TotalScore = model.CognitiveScreeningTotalScore +
        //         model.ImmediateMemoryTotalScore +
        //         model.ConcentrationTotalScore +
        //         model.DelayedRecallTotalScore
        //    });
        //    model.CognitiveScreeningTotalScores = model.CognitiveScreeningTotalScoreDetails.Sum(x => x.TotalScore);
        //}

        #endregion

        #endregion

        #region session暫存
        [HttpPost]
        public JsonResult SaveTabData(string tabName, string tabData)
        {
            Session[tabName] = tabData;
            return Json(new { success = true });
        }

        #endregion

        #region 問卷存檔
        //[HttpPost]
        //public ActionResult SaveHealth(QuestionnaireViewModel model)
        //{
        //    try
        //    {
        //        // 保存問卷主表
        //        var response = new QuestionnaireResponses
        //        {
        //            Specialist = model.Specialist,
        //            FillName = model.FillName,
        //            AtheNum = model.AtheNum,
        //            Gender = model.Gender,
        //            FillDate = model.FillDate,
        //            SubmissionDate = DateTime.Now
        //        };
        //        _db.QuestionnaireResponses.Add(response);
        //        _db.SaveChanges();

        //        // 儲存過去健康檢查病史
        //        var pastHealth = new ResponsePastHealth
        //        {
        //            QuestionnaireResponseId = response.Id,
        //            HasAbnormality = model.PastHealthHistory == "yes"
        //        };
        //        _db.ResponsePastHealth.Add(pastHealth);
        //        _db.SaveChanges();

        //        foreach (var detail in model.PastHealthDetails)
        //        {
        //            var pastHealthDetail = new ResponsePastHealthDetails
        //            {
        //                PastHealthId = pastHealth.Id,
        //                Item1 = detail.Item1,
        //                Item2 = detail.Item2,
        //                Item3 = detail.Item3
        //            };
        //            _db.ResponsePastHealthDetails.Add(pastHealthDetail);
        //        }

        //        // 儲存過敏史
        //        var allergicHistory = new ResponseAllergicHistory
        //        {
        //            QuestionnaireResponseId = response.Id,
        //            IsAllergic = model.AllergicHistory == "yes"
        //        };
        //        _db.ResponseAllergicHistory.Add(allergicHistory);
        //        _db.SaveChanges();

        //        foreach (var detail in model.AllergicHistoryDetails)
        //        {
        //            var allergicHistoryDetail = new ResponseAllergicHistoryDetails
        //            {
        //                AllergicHistoryId = allergicHistory.Id,
        //                ItemZh = detail.ItemZh,
        //                IsAllergic = detail.IsAllergic == "yes",
        //                AllergyDescription = detail.AllergyDescription
        //            };
        //            _db.ResponseAllergicHistoryDetails.Add(allergicHistoryDetail);
        //        }

        //        // 儲存家族病史
        //        foreach (var detail in model.FamilyHistoryDetails)
        //        {
        //            var familyHistory = new ResponseFamilyHistory
        //            {
        //                QuestionnaireResponseId = response.Id,
        //                GeneralPartsZh = detail.GeneralPartsZh,
        //                GeneralPartsEn = detail.GeneralPartsEn,
        //                FamilyHistoryOption = detail.FamilyHistoryOption,
        //                OtherFamilyHistory = detail.OtherFamilyHistory
        //            };
        //            _db.ResponseFamilyHistory.Add(familyHistory);
        //        }

        //        // 儲存過去病史
        //        foreach (var detail in model.PastHistoryDetails)
        //        {
        //            var pastHistory = new ResponsePastHistory
        //            {
        //                QuestionnaireResponseId = response.Id,
        //                GeneralPartsZh = detail.GeneralPartsZh,
        //                GeneralPartsEn = detail.GeneralPartsEn,
        //                PastHistoryOption = detail.PastHistoryOption,
        //                OtherPastHistory = detail.OtherPastHistory
        //            };
        //            _db.ResponsePastHistory.Add(pastHistory);
        //        }

        //        // 儲存現在病史
        //        foreach (var detail in model.PresentIllnessDetails)
        //        {
        //            var presentIllness = new ResponsePresentIllness
        //            {
        //                QuestionnaireResponseId = response.Id,
        //                PartsOfBodyZh = detail.PartsOfBodyZh,
        //                ReceivingOtherTherapies = detail.ReceivingOtherTherapies == "yes"
        //            };
        //            _db.ResponsePresentIllness.Add(presentIllness);
        //        }

        //        // 儲存藥物史
        //        foreach (var detail in model.PastDrugsDetails)
        //        {
        //            var pastDrugs = new ResponsePastDrugs
        //            {
        //                QuestionnaireResponseId = response.Id,
        //                MedicationName = detail.ItemZh,
        //                IsUsed = detail.IsUsed == "yes",
        //                OtherDrugs = detail.OtherDrugs,
        //                TUE = model.TUE == "yes"
        //            };
        //            _db.ResponsePastDrugs.Add(pastDrugs);
        //        }

        //        // 儲存營養品
        //        foreach (var detail in model.PastSupplementsDetails)
        //        {
        //            var supplement = new ResponseSupplements
        //            {
        //                QuestionnaireResponseId = response.Id,
        //                ItemZh = detail.ItemZh,
        //                IsUsed = detail.IsUsed == "yes",
        //                OtherSupplements = detail.OtherSupplements
        //            };
        //            _db.ResponseSupplements.Add(supplement);
        //        }

        //        // 儲存女性問卷
        //        if (model.Gender == 2)
        //        {
        //            foreach (var detail in model.FemaleQuestionnaireDetails)
        //            {
        //                var femaleQuestionnaire = new ResponseFemaleQuestionnaire
        //                {
        //                    QuestionnaireResponseId = response.Id,
        //                    QuestionZh = detail.QuestionZh,
        //                    QuestionEn = detail.QuestionEn,
        //                    DisplayAnswer = detail.Answer
        //                };
        //                _db.ResponseFemaleQuestionnaire.Add(femaleQuestionnaire);
        //            }
        //        }

        //        // 儲存傷害狀況
        //        foreach (var injury in model.NowInjuryDetails)
        //        {
        //            var responseInjury = new ResponseInjury
        //            {
        //                QuestionnaireResponseId = response.Id,
        //                InjuryPart = injury.InjuryPart,
        //                LeftSide = injury.LeftSide,
        //                RightSide = injury.RightSide,
        //                InjuryTypes = string.Join(",", injury.InjuryTypes),
        //                TreatmentMethods = string.Join(",", model.NowTreatmentDetails.Select(t => t.Method)),
        //                IsCurrent = true
        //            };
        //            _db.ResponseInjury.Add(responseInjury);
        //        }

        //        foreach (var injury in model.PastInjuryDetails)
        //        {
        //            var responseInjury = new ResponseInjury
        //            {
        //                QuestionnaireResponseId = response.Id,
        //                InjuryPart = injury.PastInjuryPart,
        //                LeftSide = injury.LeftSide,
        //                RightSide = injury.RightSide,
        //                InjuryTypes = string.Join(",", injury.InjuryTypes),
        //                TreatmentMethods = string.Join(",", model.PastTreatmentDetails.Select(t => t.Method)),
        //                IsCurrent = false
        //            };
        //            _db.ResponseInjury.Add(responseInjury);
        //        }

        //        // 儲存心血管篩檢
        //        foreach (var detail in model.CardiovascularScreeningDetails)
        //        {
        //            var cardiovascularScreening = new ResponseCardiovascularScreening
        //            {
        //                QuestionnaireResponseId = response.Id,
        //                OrderNumber = detail.OrderNumber,
        //                Question = detail.Question,
        //                Answer = detail.Answer
        //            };
        //            _db.ResponseCardiovascularScreening.Add(cardiovascularScreening);
        //        }

        //        // 儲存腦震盪篩檢-選手
        //        foreach (var detail in model.ConcussionScreeningDetails)
        //        {
        //            var concussionScreening = new ResponseConcussionScreening
        //            {
        //                QuestionnaireResponseId = response.Id,
        //                OrderNumber = detail.OrderNumber,
        //                Question = detail.Question,
        //                Answer = detail.Answer
        //            };
        //            _db.ResponseConcussionScreening.Add(concussionScreening);
        //        }

        //        // 儲存症狀自我評估-選手
        //        foreach (var detail in model.SymptomEvaluationDetails)
        //        {
        //            var symptomEvaluation = new ResponseSymptomEvaluation
        //            {
        //                QuestionnaireResponseId = response.Id,
        //                OrderNumber = detail.OrderNumber,
        //                Symptom = detail.SymptomItem,
        //                Score = detail.Score
        //            };
        //            _db.ResponseSymptomEvaluation.Add(symptomEvaluation);
        //        }

        //        // 儲存骨科篩檢
        //        foreach (var detail in model.OrthopaedicScreeningDetails)
        //        {
        //            var orthopaedicScreening = new ResponseOrthopaedicScreening
        //            {
        //                QuestionnaireResponseId = response.Id,
        //                OrderNumber = detail.OrderNumber,
        //                ObservationPoints = detail.ObservationPoints,
        //                Result = detail.Result
        //            };
        //            _db.ResponseOrthopaedicScreening.Add(orthopaedicScreening);
        //        }

        //        #region 認知篩檢 - 定位
        //        foreach (var detail in model.CognitiveScreeningDetails)
        //        {
        //            var orientation = new ResponseOrientation
        //            {
        //                QuestionnaireResponseId = response.Id,
        //                OrderNumber = detail.OrderNumber,
        //                Question = detail.Question,
        //                OrientationScore = detail.OrientationScore
        //            };
        //            _db.ResponseOrientation.Add(orientation);
        //        }
        //        #endregion

        //        #region 認知篩檢 - 短期記憶
        //        foreach (var detail in model.ImmediateMemoryDetails)
        //        {
        //            var immediateMemory = new ResponseImmediateMemory
        //            {
        //                QuestionnaireResponseId = response.Id,
        //                Word = detail.Word,
        //                FirstTestScore = detail.FirstTestScore,
        //                SecondTestScore = detail.SecondTestScore,
        //                ThirdTestScore = detail.ThirdTestScore
        //            };
        //            _db.ResponseImmediateMemory.Add(immediateMemory);
        //        }
        //        #endregion

        //        #region 認知篩檢 - 專注力
        //        foreach (var detail in model.ConcentrationDetails)
        //        {
        //            var concentration = new ResponseConcentration
        //            {
        //                QuestionnaireResponseId = response.Id,
        //                ListA = detail.ListA,
        //                ListB = detail.ListB,
        //                ListC = detail.ListC,
        //                Response = detail.Score == 1 ? "Y" : "N",
        //                Score = detail.Score
        //            };
        //            _db.ResponseConcentration.Add(concentration);
        //        }
        //        #endregion

        //        #region 認知篩檢 - 協調與平衡測驗
        //        foreach (var detail in model.CoordinationAndBalanceDetails)
        //        {
        //            var coordinationAndBalance = new ResponseCoordinationAndBalance
        //            {
        //                QuestionnaireResponseId = response.Id,
        //                TestFoot = detail.TestFoot,
        //                TestSurface = detail.TestSurface,
        //                Footwear = detail.Footwear,
        //                DoubleLegError = detail.DoubleLegError,
        //                TandemError = detail.TandemError,
        //                SingleLegError = detail.SingleLegError,
        //                FirstTime = detail.FirstTime,
        //                SecondTime = detail.SecondTime,
        //                ThirdTime = detail.ThirdTime,
        //                TotalErrors = detail.TotalErrors,
        //                AverageTime = detail.AverageTimes,
        //                FastestTime = detail.FastestTimes
        //            };
        //            _db.ResponseCoordinationAndBalance.Add(coordinationAndBalance);
        //        }
        //        #endregion

        //        #region 認知篩檢 - 延遲記憶
        //        foreach (var detail in model.DelayedRecallDetails)
        //        {
        //            var delayedRecall = new ResponseDelayedRecall
        //            {
        //                QuestionnaireResponseId = response.Id,
        //                OrderNumber = detail.OrderNumber,
        //                Word = detail.Word,
        //                Score = detail.Score
        //            };
        //            _db.ResponseDelayedRecall.Add(delayedRecall);
        //        }
        //        #endregion

        //        #region 認知篩檢 - 分數總和
        //        var totalScores = new ResponseTotalScores
        //        {
        //            QuestionnaireResponseId = response.Id,
        //            OrientationScore = model.CognitiveScreeningTotalScore,
        //            ImmediateMemoryScore = model.ImmediateMemoryTotalScore,
        //            ConcentrationScore = model.ConcentrationTotalScore,
        //            DelayedRecallScore = model.DelayedRecallTotalScore,
        //        };
        //        _db.ResponseTotalScores.Add(totalScores);
        //        #endregion

        //        _db.SaveChanges(); // 儲存所有變更

        //        TempData["SuccessMessage"] = "問卷資料已成功送出！";
        //        return RedirectToAction("Preview");
        //    }
        //    catch (Exception ex)
        //    {
        //        TempData["ErrorMessage"] = "發生錯誤：" + ex.Message;
        //        return RedirectToAction("Preview", model);
        //    }
        //}
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