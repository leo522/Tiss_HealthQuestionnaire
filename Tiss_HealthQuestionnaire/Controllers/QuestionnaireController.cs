using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Validation;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Tiss_HealthQuestionnaire.Models;
using static Tiss_HealthQuestionnaire.Models.QuestionnaireViewModel;

namespace Tiss_HealthQuestionnaire.Controllers
{
    public class QuestionnaireController : Controller
    {
        private HealthQuestionnaireEntities _db = new HealthQuestionnaireEntities(); //資料庫

        #region 主頁
        public ActionResult Main()
        {
            try
            {
                string loggedInUserName = Session["UserName"] as string; //確認登入狀態
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

                //過去傷害相關資料
                var pastInjuryItems = GetPastInjuryItems(); //過去傷害部位
                var pastInjuryTypesList = GetPastInjuryTypesList(); //過去傷勢類型
                var pastTreatmentItems = GetPastTreatmentItems(); //過去治療方式

                //取得目前傷害相關資料
                var currentInjuryItems = GetCurrentInjuryItems();
                var currentInjuryTypesList = GetCurrentInjuryTypesList();
                var currentTreatmentItems = GetCurrentTreatmentItems();

                //var supplements = _db.PastSupplements.ToList(); // 這行確保變數存在

                // 獲取女性問卷 (僅限女性)
                var femaleQuestionnaire = GetFemaleQuestionnaireViewModel(user.GenderID);
                // 整合問卷資料
                var viewModel = new QuestionnaireViewModel
                {
                    PastHealthItems = GetPastHealthItemViewModel(), //過去健康檢查史
                    AllergicHistoryItems = GetAllergicHistoryItemViewModels(), //過敏史
                    FamilyHistoryItems = GetFamilyHistoryItemsViewModels(), //家族病史
                    PastHistoryItems = GetPastHistoryViewModel(), //過去病史
                    PresentIllnessItems = GetPresentIllnessViewModel(), //現在病史
                    PastDrugsItems = GetPastDrugsViewModel(), //藥物史
                    TUE = "no", //藥物史
                    OtherDrug = "", //藥物史
                    PastSupplementsItems = GetPastSupplementsViewModel(), //營養品
                    FemaleQuestionnaireItems = femaleQuestionnaire, //女性問卷
                    FemaleQuestionnaireAnswers = new Dictionary<int, string>(), //女性問卷
                    PastInjuryStatusAnswer = "yes",  // 確保前端顯示
                    PastInjuryItems = pastInjuryItems ?? new List<QuestionnaireViewModel.PastInjuryStatusViewModel>(),
                    PastInjuryTypes = pastInjuryTypesList ?? new List<InjuryTypeViewModel>(),
                    PastTreatmentItems = pastTreatmentItems ?? new List<PastTreatmentMethodViewModel>(),
                    CurrentInjuryStatusAnswer = "yes", //目前傷害
                    CurrentInjuryItems = currentInjuryItems ?? new List<CurrentInjuryStatusViewModel>(),
                    CurrentInjuryTypes = currentInjuryTypesList ?? new List<InjuryTypeViewModel>(),
                    CurrentTreatmentItems = currentTreatmentItems ?? new List<CurrentTreatmentMethodViewModel>(),
                    CardiovascularScreeningItems = GetCardiovascularScreeningViewModel(), //心血管篩檢
                    ConcussionScreeningItems = _db.ConcussionScreening.ToList(),
                    SymptomEvaluationItems = _db.SymptomEvaluation.ToList(),
                    OrthopaedicScreeningItems = GetOrthopaedicScreeningViewModel(),
                    CognitiveScreeningItems = _db.CognitiveScreening.ToList(),
                    ImmediateMemoryItems = _db.ImmediateMemory.ToList(),
                    ConcentrationItems = _db.Concentration.ToList(),
                    CoordinationAndBalanceItems = _db.CoordinationAndBalanceExamination.ToList(),
                    DelayedRecallItems = _db.DelayedRecall.ToList(),
                };

                return View(viewModel);
            }
            catch (Exception ex)
            {
                return View("Error", new HandleErrorInfo(ex, "Error500", "Error"));
            }
        }
        #endregion

        #region 主頁獨立方法

        #region 過去健康檢查病史
        private List<PastHealthItemViewModel> GetPastHealthItemViewModel()
        {
            return _db.PastHealth.Select(item => new PastHealthItemViewModel
            {
                ID = item.ID,
                ItemZh = item.ItemZh,
                ItemEn = item.ItemEn,
                IsYes = false,
                Details = ""
            }).ToList();
        }
        #endregion

        #region 過敏史
        private List<AllergicHistoryItemViewModel> GetAllergicHistoryItemViewModels()
        {
            return _db.AllergicHistory
                .Select(item => new AllergicHistoryItemViewModel
                {
                    ID = item.ID,
                    ItemZh = item.ItemZh,
                    ItemEn = item.ItemEn,
                    IsYes = false, // 預設值
                    Details = null // 預設值
                }).ToList();
        }
        #endregion

        #region 家族病史
        private List<FamilyHistoryViewModel> GetFamilyHistoryItemsViewModels()
        {
            return _db.FamilyHistory.Select(item => new FamilyHistoryViewModel
            {
                ID = item.ID,
                GeneralPartsZh = item.GeneralPartsZh,
                IsYes = false,
                IsNo = false,
                IsUnknown = true
            }).ToList();
        }
        #endregion

        #region 過去病史
        private List<PastHistoryViewModel> GetPastHistoryViewModel()
        {
            return _db.PastHistory.Select(item => new PastHistoryViewModel
            {
                ID = item.ID,
                GeneralPartsZh = item.GeneralPartsZh,
                IsYes = false,
                IsNo = false,
                IsUnknown = true
            }).ToList();
        }
        #endregion

        #region 現在病史
        private List<PresentIllnessViewModel> GetPresentIllnessViewModel()
        {
            return _db.PresentIllness.Select(item => new PresentIllnessViewModel
            {
                ID = item.ID,
                PartsOfBodyZh = item.PartsOfBodyZh
            }).ToList();
        }
        #endregion

        #region 藥物史 
        private List<PastDrugsViewModel> GetPastDrugsViewModel()
        {
            return _db.PastDrugs.Select(item => new PastDrugsViewModel
            {
                ID = item.ID,
                ItemZh = item.ItemZh,
                ItemEn = item.ItemEn,
                IsUsed = false // 預設未勾選
            }).ToList();
        }
        #endregion

        #region 營養品
        private List<PastSupplementsViewModel> GetPastSupplementsViewModel()
        {
            return _db.PastSupplements
                .Select(s => new PastSupplementsViewModel
                {
                    ID = s.ID,
                    ItemZh = s.ItemZh,
                    ItemEn = s.ItemEn,
                    IsUsed = false // 預設未使用
                }).ToList();
        }
        #endregion

        #region 女性問卷
        private List<FemaleQuestionnaireViewModel> GetFemaleQuestionnaireViewModel(int genderID)
        {
            if (genderID != 2)
            {
                return new List<FemaleQuestionnaireViewModel>(); // 只有女性才有問卷，非女性回傳空清單
            }

            var questions = _db.FemaleQuestionnaire
                .Select(q => new
                {
                    q.ID,
                    q.QuestionZh,
                    q.QuestionEn
                })
                .ToList();

            var result = questions.Select(q => new FemaleQuestionnaireViewModel
            {
                ID = q.ID,
                QuestionZh = q.QuestionZh,
                QuestionEn = q.QuestionEn,
                AnswerOptions = q.ID == 1
                    ? new Dictionary<string, string>
                    {
                { "10", "10 歲(含)以下" },
                { "11", "11 歲" },
                { "12", "12 歲" },
                { "13", "13 歲" },
                { "14", "14 歲" },
                { "15", "15 歲" },
                { "16", "16 歲(含)以上" }
                    }
                    : new Dictionary<string, string>
                    {
                { "yes", "是" },
                { "no", "否" },
                { "none", "目前無生理週期" }
                    }
            }).ToList();

            return result;
        }
        #endregion

        #region 心血管篩檢
        private List<CardiovascularScreeningItemViewModel> GetCardiovascularScreeningViewModel()
        {
            return _db.CardiovascularScreening.Select(item => new CardiovascularScreeningItemViewModel
            {
                ID = item.Id,
                Question = item.Question,
                IsUsed = false
            }).ToList();
        }
        #endregion

        #region 骨科篩檢
        private List<OrthopaedicScreeningItmeViewModel> GetOrthopaedicScreeningViewModel()
        {
            return _db.OrthopaedicScreening.Select(item => new OrthopaedicScreeningItmeViewModel
            {
                ID = item.Id,
                Instructions = item.Instructions,
                ObservationPoints =item.ObservationPoints,
                ResultNormal = item.ResultNormal,
                ResultAbnormal = item.ResultAbnormal,
                Result = ""
            }).ToList();
        }
        #endregion

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
        private List<QuestionnaireViewModel.PastInjuryStatusViewModel> GetPastInjuryItems()
        {
            var injuries = _db.PastInjuryStatus
            .Select(injury => new QuestionnaireViewModel.PastInjuryStatusViewModel
            {
                Id = injury.Id,
                PastInjuryPart = injury.InjuryPart
            }).ToList();

            return injuries;
        }
        #endregion

        #region 過去傷勢類型-(2)
        private List<QuestionnaireViewModel.InjuryTypeViewModel> GetPastInjuryTypesList()
        {
            var injuryTypes = _db.PastInjuryType
            .Join(_db.PastInjuryCategory,
                type => type.PastInjuryCategoryId,
                category => category.PastInjuryCategoryId,
                (type, category) => new QuestionnaireViewModel.InjuryTypeViewModel
                {
                    CategoryName = category.CategoryName,
                    InjuryName = type.InjuryName
                }).ToList();

            return injuryTypes;
        }
        #endregion

        #region 過去治療方式-(3)
        private List<QuestionnaireViewModel.PastTreatmentMethodViewModel> GetPastTreatmentItems()
        {
            var treatments = _db.PastTreatmentMethod
            .Select(t => new QuestionnaireViewModel.PastTreatmentMethodViewModel
            {
                Id = t.Id,
                Method = t.Method
            }).ToList();

            return treatments;
        }
        #endregion

        #region 目前傷害狀況-(1)
        private List<QuestionnaireViewModel.CurrentInjuryStatusViewModel> GetCurrentInjuryItems()
        {
            var currentInjury = _db.CurrentInjuryStatus.Select(injury => new CurrentInjuryStatusViewModel
            {
                Id = injury.Id,
                CurrentInjuryPart = injury.InjuryPart
            }).ToList();

            return currentInjury;
        }
        #endregion

        #region 目前傷勢類型-(2)
        private List<QuestionnaireViewModel.InjuryTypeViewModel> GetCurrentInjuryTypesList()
        {
            var currentInjurtTypes = _db.CurrentInjuryType.Join(_db.CurrentInjuryCategory, type => type.CurrentInjuryCategoryId, category => category.CurrentInjuryCategoryId, (type, category) => new InjuryTypeViewModel
            {
                CategoryName = category.CategoryName,
                InjuryName = type.InjuryName
            }).ToList();

            return currentInjurtTypes;
        }
        #endregion

        #region 目前治療方式-(3)
        private List<QuestionnaireViewModel.CurrentTreatmentMethodViewModel> GetCurrentTreatmentItems()
        {
            var CurrentTreaments = _db.CurrentTreatmentMethod.Select(t => new CurrentTreatmentMethodViewModel
            {
                Id = t.Id,
                Method = t.Method
            }).ToList();

            return CurrentTreaments;
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

        #endregion

        #region 醫療團隊評估主頁
        public ActionResult ConcussionMedicalEvaluation()
        {
            if (Session["TrainerAuthenticated"] == null || !(bool)Session["TrainerAuthenticated"])
            {
                return RedirectToAction("Main"); // 如果沒驗證，回到主頁
            }

            return View();
        }
        #endregion

        #region 醫療團隊評估-腦震盪篩檢-認知篩檢-定位(1)
        public ActionResult CognitiveScreening()
        {
            try
            {
                var questions = _db.CognitiveScreening.ToList(); //直接取得 List<CognitiveScreening>

                return PartialView("CognitiveScreening", questions); //確保回傳的是 List<CognitiveScreening>
            }
            catch (Exception ex)
            {
                return View("Error", new HandleErrorInfo(ex, "Questionnaire", "CognitiveScreening"));
            }
        }
        #endregion

        #region 醫療團隊評估-腦震盪篩檢-認知篩檢-短期記憶(2)
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

        #region 醫療團隊評估-腦震盪篩檢-認知篩檢-專注力(3)
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

        #region 醫療團隊評估-腦震盪篩檢-認知篩檢-協調與平衡測驗(4)
        public ActionResult CoordinationAndBalanceExamination()
        {
            var model = _db.CoordinationAndBalanceExamination.FirstOrDefault();

            if (Session["CoordinationAndBalanceData"] is CoordinationAndBalanceExamination savedData)
            {
                model = savedData; //若有 session 資料，覆蓋原本的
            }

            if (model == null)
            {
                model = new CoordinationAndBalanceExamination
                {
                    TestFoot = "",
                    TestSurface = "",
                    Footwear = "",
                };
            }

            return PartialView("CoordinationAndBalanceExamination", model);
        }
        #endregion

        #region 醫療團隊評估-腦震盪篩檢-認知篩檢-延遲記憶(5)
        public ActionResult DelayedRecall()
        {
            var questions = _db.DelayedRecall.ToList();
            var delayedRecallAnswers = Session["DelayedRecallAnswers"] as Dictionary<int, int> ?? new Dictionary<int, int>();
            var delayedRecallStartTime = Session["DelayedRecallStartTime"] as string ?? DateTime.Now.ToString("HH:mm"); //預設為當前時間

            int totalScore = 0;

            var viewModel = questions.Select(q =>
            {
                int score = 0;
                delayedRecallAnswers.TryGetValue(q.ID, out score);
                totalScore += score; //計算總分

                return new DelayedRecall
                {
                    ID = q.ID,
                    Word = q.Word,
                    Score0 = score //確保這個欄位名稱正確
                };
            }).ToList();

            //存回 Session，避免前端重新計算
            Session["DelayedRecallTotalScore"] = totalScore;
            Session["DelayedRecallStartTime"] = delayedRecallStartTime;

            ViewBag.DelayedRecallStartTime = delayedRecallStartTime;

            return PartialView("DelayedRecall", viewModel);
        }
        #endregion

        #region 醫療團隊評估-腦震盪篩檢-認知篩檢-認知篩檢分數總合(6)
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

        #region 處理過去傷勢(已復原)-跳轉頁
        //[HttpPost]
        //public ActionResult pastInjuryStatusNextStep(QuestionnaireViewModel model, string pastInjuryStatus)
        //{
        //    Session["PastInjuryStatus"] = model.PastInjuryStatusAnswer; // 這裡存的是 "no" 或 "yes"
        //    if (pastInjuryStatus == "no")
        //    {
        //        return RedirectToAction("NowInjuryStatus"); // 進入目前傷害問卷
        //    }

        //    return RedirectToAction("PastInjuryType"); //否則進入傷勢類型頁面
        //}

        ////過去傷勢類型
        //[HttpPost]
        //public ActionResult pastInjuryTypesNextStep(QuestionnaireViewModel model)
        //{
        //    if (model.PastInjuryTypesItems == null) //確保不為 `null`
        //    {
        //        model.PastInjuryTypesItems = new List<PastInjuryType>();
        //    }

        //    Session["SelectedInjuryTypes"] = model.PastInjuryTypesItems; //存入 Session

        //    return RedirectToAction("PastTreatmentMethod");
        //}

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
                ProcessPastInjuryStatus(model, form);        // 處理過去傷害狀況(已復原)
                ProcessPastInjuryParts(model, form);         // 處理過去傷勢部位
                ProcessPastTreatmentMethod(model, form);     // 處理過去治療方式
                ProcessCurrentInjuryStatus(model, form);     // 目前傷害狀況
                ProcessCurrentInjuryParts(model, form);         // 目前傷勢部位
                ProcessCurrentTreatmentMethod(model, form);  // 目前治療方式
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
                return RedirectToAction("Error404", "Error");
            }
        }
        #endregion

        #region 預覽頁分區處理方法

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
            model.PastHealthResponses = new Dictionary<string, string>();

            model.PastHealthItems = _db.PastHealth.ToList().Select(item => new PastHealthItemViewModel
            {
                ID = item.ID,
                ItemZh = item.ItemZh,
                ItemEn = item.ItemEn,
                IsYes = form[$"pastHealth_{item.ID}"] == "yes",
                Details = form[$"PastHealthResponses[{item.ID}]"] != null ? form[$"PastHealthResponses[{item.ID}]"].Trim() : ""
            }).ToList();

            foreach (var item in model.PastHealthItems)
            {
                if (item.IsYes)
                {
                    model.PastHealthResponses[item.ID.ToString()] = item.Details;
                }
            }

            model.PastHealthHistory = model.PastHealthItems.Any(i => i.IsYes) ? "yes" : "no";
        }
        #endregion

        #region 過敏史
        private void ProcessAllergicHistory(QuestionnaireViewModel model, FormCollection form)
        {
            model.AllergicHistoryItems = _db.AllergicHistory.ToList().Select(item => new AllergicHistoryItemViewModel
            {
                ID = item.ID,
                ItemZh = item.ItemZh,
                ItemEn = item.ItemEn,
                IsYes = form[$"allergy_{item.ID}"] == "yes",
                Details = form[$"details_{item.ID}"] != null
                      ? form[$"details_{item.ID}"].Trim()
                      : null
            }).ToList();
            model.AllergicHistory = model.AllergicHistoryItems.Any(i => i.IsYes) ? "yes" : "no";
        }
        #endregion

        #region 家族病史
        private void ProcessFamilyHistory(QuestionnaireViewModel model, FormCollection form)
        {
            model.FamilyHistoryItems = new List<FamilyHistoryViewModel>();

            var familyItems = _db.FamilyHistory.ToList();
            int index = 0;

            foreach (var item in familyItems)
            {
                string option = form[$"FamilyHistoryItems[{index}].FamilyHistoryOption"]?.Trim().ToLower() ?? "unknown";

                var newItem = new FamilyHistoryViewModel
                {
                    ID = item.ID,
                    GeneralPartsZh = item.GeneralPartsZh,
                    FamilyHistoryOption = option
                };

                newItem.IsYes = (option == "yes");
                newItem.IsNo = (option == "no");
                newItem.IsUnknown = (option == "unknown");

                model.FamilyHistoryItems.Add(newItem);

                index++;
            }

            model.OtherFamilyHistory = form["OtherFamilyHistory"]?.Trim() ?? "";
        }
        #endregion

        #region 過去病史
        private void ProcessPastHistory(QuestionnaireViewModel model, FormCollection form)
        {
            model.PastHistoryItems = new List<PastHistoryViewModel>();

            int i = 0;
            while (form[$"PastHistoryItems[{i}].ID"] != null)
            {
                int id = int.Parse(form[$"PastHistoryItems[{i}].ID"]);
                string generalPartsZh = form[$"PastHistoryItems[{i}].GeneralPartsZh"];
                string option = form[$"PastHistoryItems[{i}].PastHistoryOption"]?.Trim().ToLower() ?? "unknown";

                var newItem = new PastHistoryViewModel
                {
                    ID = id,
                    GeneralPartsZh = generalPartsZh,
                    PastHistoryOption = option
                };

                if (option == "yes")
                {
                    newItem.IsYes = true;
                    newItem.IsNo = false;
                    newItem.IsUnknown = false;
                }
                else if (option == "no")
                {
                    newItem.IsYes = false;
                    newItem.IsNo = true;
                    newItem.IsUnknown = false;
                }
                else
                {
                    newItem.IsYes = false;
                    newItem.IsNo = false;
                    newItem.IsUnknown = true;
                }

                model.PastHistoryItems.Add(newItem);

                Console.WriteLine($"✅ [DEBUG] PastHistory Added: ID={id}, Disease={generalPartsZh}, Option={option}");

                i++;
            }

            model.OtherPastHistory = form["OtherPastHistory"]?.Trim() ?? "";
        }
        #endregion

        #region 現在病史
        private void ProcessPresentIllness(QuestionnaireViewModel model, FormCollection form)
        {
            if (model.PresentIllnessItems == null)
            {
                model.PresentIllnessItems = new List<PresentIllnessViewModel>();
            }
            else
            {
                model.PresentIllnessItems.Clear();
            }

            foreach (var illness in _db.PresentIllness.ToList())
            {
                string therapyValue = form[$"PresentIllnessItems[{illness.ID}].ReceivingTherapy"];

                if (!string.IsNullOrEmpty(therapyValue))
                {
                    model.PresentIllnessItems.Add(new PresentIllnessViewModel
                    {
                        ID = illness.ID,
                        PartsOfBodyZh = illness.PartsOfBodyZh,
                        ReceivingTherapy = therapyValue // 記錄用戶選擇的值
                    });
                }
            }
        }
        #endregion

        #region 藥物史
        private void ProcessPastDrugs(QuestionnaireViewModel model, FormCollection form)
        {
            model.PastDrugsItems = new List<PastDrugsViewModel>();

            int index = 0;
            while (form[$"PastDrugsItems[{index}].ID"] != null)
            {
                string isUsedValue = form[$"PastDrugsItems[{index}].IsUsed"];
                bool isUsed = isUsedValue != null && isUsedValue == "true";

                var drug = new PastDrugsViewModel
                {
                    ID = int.Parse(form[$"PastDrugsItems[{index}].ID"]),
                    ItemZh = form[$"PastDrugsItems[{index}].ItemZh"],
                    IsUsed = isUsed
                };

                model.PastDrugsItems.Add(drug);

                index++;
            }

            //TUE (治療用途豁免)
            model.TUE = form["TUE"] ?? "no";  // **確保 `TUE` 不為 null**
        }
        #endregion

        #region 營養品
        private void ProcessPastSupplements(QuestionnaireViewModel model, FormCollection form)
        {
            model.PastSupplementsItems = new List<PastSupplementsViewModel>();

            int index = 0;
            while (form[$"PastSupplementsItems[{index}].ID"] != null)
            {
                string isUsedValue = form[$"PastSupplementsItems[{index}].IsUsed"];
                bool isUsed = form.AllKeys.Contains($"PastSupplementsItems[{index}].IsUsed") &&
                              isUsedValue == "true";

                var supplement = new PastSupplementsViewModel
                {
                    ID = int.Parse(form[$"PastSupplementsItems[{index}].ID"] ?? "0"),
                    ItemZh = form[$"PastSupplementsItems[{index}].ItemZh"] ?? "",
                    IsUsed = isUsed
                };

                model.PastSupplementsItems.Add(supplement);

                index++;
            }

            model.OtherSupplements = form["OtherSupplements"]?.Trim() ?? "";
        }
        #endregion

        #region 女性問卷
        private void ProcessFemaleQuestionnaire(QuestionnaireViewModel model, FormCollection form)
        {
            if (model.FemaleQuestionnaireItems == null || !model.FemaleQuestionnaireItems.Any())
            {
                return; // 無資料不處理
            }

            if (model.FemaleQuestionnaireAnswers == null)
            {
                model.FemaleQuestionnaireAnswers = new Dictionary<int, string>();
            }
            else
            {
                model.FemaleQuestionnaireAnswers.Clear(); // 清除舊資料
            }

            foreach (var item in model.FemaleQuestionnaireItems)
            {
                string answerKey = $"FemaleQuestionnaireAnswers[{item.ID}]";
                string answer = form[answerKey];

                model.FemaleQuestionnaireAnswers[item.ID] = string.IsNullOrEmpty(answer) ? "未回答" : answer;
            }
        }
        #endregion

        #region 過去傷害狀況-已復原
        private void ProcessPastInjuryStatus(QuestionnaireViewModel model, FormCollection form)
        {
            model.PastInjuryStatusAnswer = form["pastInjuryStatus"];

            var selectedInjuryTypes = form.GetValues("SelectedInjuryTypes");
            if (selectedInjuryTypes != null && selectedInjuryTypes.Any())
            {
                model.PastInjuryTypes = selectedInjuryTypes
                    .Select(type => new InjuryTypeViewModel { InjuryName = type.Trim() })
                    .ToList();
            }
        }
        #endregion

        #region 過去傷害狀況-傷勢部位
        private void ProcessPastInjuryParts(QuestionnaireViewModel model, FormCollection form)
        {
            model.PastInjuryItems = new List<QuestionnaireViewModel.PastInjuryStatusViewModel>(); // **確保初始化**

            var selectedLeftInjuries = form.GetValues("PastInjuryLeft")?.Select(int.Parse).ToList() ?? new List<int>();
            var selectedRightInjuries = form.GetValues("PastInjuryRight")?.Select(int.Parse).ToList() ?? new List<int>();

            foreach (var injury in _db.PastInjuryStatus)
            {
                var isLeft = selectedLeftInjuries.Contains(injury.Id);
                var isRight = selectedRightInjuries.Contains(injury.Id);

                if (isLeft || isRight)
                {
                    model.PastInjuryItems.Add(new QuestionnaireViewModel.PastInjuryStatusViewModel
                    {
                        Id = injury.Id,
                        PastInjuryPart = injury.InjuryPart,
                        LeftSide = isLeft,
                        RightSide = isRight
                    });
                }
            }
        }
        #endregion

        #region 過去傷害狀況-治療方式
        private void ProcessPastTreatmentMethod(QuestionnaireViewModel model, FormCollection form)
        {
            var selectedTreatmentMethods = form.GetValues("SelectedTreatmentMethods");
            if (selectedTreatmentMethods != null && selectedTreatmentMethods.Any())
            {
                model.PastTreatmentItems = selectedTreatmentMethods
                    .Select(method => new QuestionnaireViewModel.PastTreatmentMethodViewModel { Method = method.Trim() })
                    .ToList();
            }
        }
        #endregion

        #region 目前傷害狀況
        private void ProcessCurrentInjuryStatus(QuestionnaireViewModel model, FormCollection form)
        {
            model.CurrentInjuryStatusAnswer = form["currentInjuryStatus"];

            var selectedInjuryTypes = form.GetValues("SelectedInjuryTypes");

            if (selectedInjuryTypes != null && selectedInjuryTypes.Any())
            {
                model.CurrentInjuryTypes = selectedInjuryTypes
                     .Select(type => new InjuryTypeViewModel { InjuryName = type.Trim() }).ToList();
            }
        }
        #endregion

        #region 目前傷害狀況-傷勢部位
        private void ProcessCurrentInjuryParts(QuestionnaireViewModel model, FormCollection form)
        {
            model.CurrentInjuryItems = new List<QuestionnaireViewModel.CurrentInjuryStatusViewModel>(); // **確保初始化**

            var selectedLeftInjuries = form.GetValues("CurrentInjuryLeft")?.Select(int.Parse).ToList() ?? new List<int>();
            var selectedRightInjuries = form.GetValues("CurrentInjuryRight")?.Select(int.Parse).ToList() ?? new List<int>();

            foreach (var injury in _db.CurrentInjuryStatus)
            {
                var isLeft = selectedLeftInjuries.Contains(injury.Id);
                var isRight = selectedRightInjuries.Contains(injury.Id);

                if (isLeft || isRight)
                {
                    model.CurrentInjuryItems.Add(new QuestionnaireViewModel.CurrentInjuryStatusViewModel
                    {
                        Id = injury.Id,
                        CurrentInjuryPart = injury.InjuryPart,
                        LeftSide = isLeft,
                        RightSide = isRight
                    });
                }
            }
        }
        #endregion

        #region 目前傷害狀況-治療方式
        private void ProcessCurrentTreatmentMethod(QuestionnaireViewModel model, FormCollection form)
        {
            var selectedTreatmentMethods = form.GetValues("SelectedTreatmentMethods");

            if (selectedTreatmentMethods != null && selectedTreatmentMethods.Any())
            {
                model.CurrentTreatmentItems = selectedTreatmentMethods
                    .Select(method => new CurrentTreatmentMethodViewModel { Method = method.Trim() }).ToList();
            }
        }
        #endregion

        #region 心血管篩檢
        private void ProcessCardiovascularScreening(QuestionnaireViewModel model, FormCollection form)
        {
            model.CardiovascularScreeningItems = new List<CardiovascularScreeningItemViewModel>();

            int index = 0;
            while (form[$"CardiovascularScreeningItems[{index}].ID"] != null)
            {
                string isUsedValue = form[$"CardiovascularScreeningItems[{index}].IsUsed"];
                bool isUsed = form.AllKeys.Contains($"CardiovascularScreeningItems[{index}].IsUsed") && isUsedValue == "true";

                int id = int.Parse(form[$"CardiovascularScreeningItems[{index}].ID"] ?? "0");
                // 重新查詢題目內容
                var question = _db.CardiovascularScreening.Where(c => c.Id == id).Select(c => c.Question).FirstOrDefault();
                var cardiovascular = new CardiovascularScreeningItemViewModel
                {
                    ID = id,
                    Question = question,
                    IsUsed = isUsed
                };

                model.CardiovascularScreeningItems.Add(cardiovascular);

                index++;
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
                string answer = form[answerKey] ?? "no"; // 預設 "no"

                model.ConcussionScreeningAnswers[item.Id] = answer == "yes" ? "是" : "否"; // 轉換成中文
            }

            // 服用藥物 & 備註
            model.ConcussionScreeningMedicationAnswer = form["Medication"] == "yes" ? "是" : "否";
            model.ConcussionScreeningMedicationDetails = form["MedicationDetails"]?.Trim() ?? "無";
            model.ConcussionScreeningNotes = form["Notes"]?.Trim() ?? "無";
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

                if (form.AllKeys.Contains(scoreKey) && !string.IsNullOrEmpty(form[scoreKey]))
                {
                    int.TryParse(form[scoreKey], out score); // 轉換為 int
                }

                // **手動限制分數範圍**
                if (score < 0) score = 0;
                if (score > 6) score = 6;

                model.SymptomEvaluationAnswers[item.ID] = score;
            }
        }
        #endregion

        #region 骨科篩檢
        private void ProcessOrthopaedicScreening(QuestionnaireViewModel model, FormCollection form)
        {
            model.OrthopaedicScreeningItems = new List<OrthopaedicScreeningItmeViewModel>(); // 確保初始化

            int index = 0;

            while (form[$"OrthopaedicScreeningItems[{index}].ID"] != null)
            {
                //string isUsedValue = form[$"OrthopaedicScreeningItems[{index}].IsUsed"];
                //bool isUsed = isUsedValue != null && isUsedValue == "true";
                string resultValue = form[$"OrthopaedicScreeningItems[{index}].Result"];

                var orthopaedicScreening = new OrthopaedicScreeningItmeViewModel
                {
                    ID = int.Parse(form[$"OrthopaedicScreeningItems[{index}].ID"]),
                    Instructions = form[$"OrthopaedicScreeningItems[{index}].Instructions"],
                    ObservationPoints = form[$"OrthopaedicScreeningItems[{index}].ObservationPoints"],
                    ResultNormal = "正常",
                    ResultAbnormal = "異常",
                    Result = resultValue
                };

                model.OrthopaedicScreeningItems.Add(orthopaedicScreening);

                System.Diagnostics.Debug.WriteLine($"OrthopaedicScreeningItems 篩選後數量:{model.OrthopaedicScreeningItems.Count}");

                index++;
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

        #region 問卷存檔
        [HttpPost]
        public ActionResult Submit(QuestionnaireViewModel model, FormCollection form)
        {
            using (var transaction = _db.Database.BeginTransaction())
            {
                try
                {
                    var newResponse = new QuestionnaireResponse
                    {
                        AthleteID = model.AtheNum,
                        GenderID = model.Gender,
                        FillingDate = model.FillDate,
                        Specialty = model.Specialist,
                        FillName = model.FillName
                    };

                    _db.QuestionnaireResponse.Add(newResponse);
                    _db.SaveChanges();

                    int responseId = newResponse.ID;

                    SavePastHealth(model, responseId);
                    SaveAllergicHistory(model, responseId);
                    SaveFamilyHistory(model, responseId);
                    SavePastHistory(model, responseId);
                    SavePresentIllness(model, responseId);
                    SavePastDrugs(model, responseId);
                    SavePastSupplements(model, responseId);
                    SaveFemaleQuestionnaire(model, responseId);
                    SavePastInjury(model, responseId);
                    SaveCurrentInjury(model, responseId);
                    SaveCardiovascularScreening(model, responseId);
                    SaveConcussionScreening(model, responseId);
                    SaveSymptomEvaluation(model, responseId);
                    SaveOrthopaedicScreening(model, responseId);

                    _db.SaveChanges();
                    transaction.Commit();

                    return RedirectToAction("Success", "Questionnaire");
                }
                catch (DbEntityValidationException dbEx)
                {
                    transaction.Rollback();

                    var errorMessages = new List<string>();

                    foreach (var validationResult in dbEx.EntityValidationErrors)
                    {
                        string entityName = validationResult.Entry.Entity.GetType().Name; // 取得實體名稱

                        foreach (var error in validationResult.ValidationErrors)
                        {
                            string errorMessage = $"實體: {entityName}, 屬性: {error.PropertyName}, 錯誤: {error.ErrorMessage}";
                            errorMessages.Add(errorMessage);
                        }
                    }

                    string fullErrorMessage = string.Join(" | ", errorMessages);

                    // **將錯誤記錄到日誌**
                    System.Diagnostics.Debug.WriteLine("資料驗證錯誤: " + fullErrorMessage);
                    ModelState.AddModelError("", "發生錯誤：" + fullErrorMessage);

                    return RedirectToAction("WebError", "Error");
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    ModelState.AddModelError("", "儲存失敗：" + ex.Message);
                    return RedirectToAction("WebError", "Error");
                }
            }
        }
        #endregion

        #region 各問卷子表的儲存

        #region 儲存 PastHealth (過去健康檢查)
        private void SavePastHealth(QuestionnaireViewModel model, int responseId)
        {
            var responses = model.PastHealthItems.Select(item => new ResponsePastHealth
            {
                QuestionnaireResponseID = responseId,
                HasAbnormalItems = item.IsYes,
                Details = item.IsYes ? (string.IsNullOrEmpty(item.Details) ? "未填寫" : item.Details) : null
            }).ToList();

            _db.ResponsePastHealth.AddRange(responses);
        }
        #endregion

        #region  儲存 AllergicHistory (過敏史)
        private void SaveAllergicHistory(QuestionnaireViewModel model, int responseId)
        {
            var allergicHistories = model.AllergicHistoryItems.Select(item => new ResponseAllergicHistory
            {
                QuestionnaireResponseID = responseId,
                AllergyType = item.ItemZh,
                IsYes = item.IsYes,
                IsNo = !item.IsYes,
                Details = item.IsYes ? (string.IsNullOrEmpty(item.Details) ? "未填寫" : item.Details) : null
            })
        .ToList();

            _db.ResponseAllergicHistory.AddRange(allergicHistories);
        }
        #endregion

        #region 儲存 FamilyHistory (家族病史)
        private void SaveFamilyHistory(QuestionnaireViewModel model, int responseId)
        {
            if (model.FamilyHistoryItems == null || model.FamilyHistoryItems.Count == 0) return;

            foreach (var item in model.FamilyHistoryItems)
            {
                var status = item.IsYes ? "Yes" :
                             item.IsNo ? "No" :
                             item.IsUnknown ? "Unknown" : "Unknown"; // 明確設置狀態

                var familyHistory = new ResponseFamilyHistory
                {
                    QuestionnaireResponseID = responseId,
                    Disease = item.GeneralPartsZh,
                    Status = status // 設定正確狀態
                };

                _db.ResponseFamilyHistory.Add(familyHistory);
            }

            if (!string.IsNullOrEmpty(model.OtherFamilyHistory))
            {
                _db.ResponseFamilyHistory.Add(new ResponseFamilyHistory
                {
                    QuestionnaireResponseID = responseId,
                    Disease = "其他",
                    Status = model.OtherFamilyHistory
                });
            }

            _db.SaveChanges(); // 確保數據存入
        }
        #endregion

        #region 儲存 PastHistory (過去病史)
        private void SavePastHistory(QuestionnaireViewModel model, int responseId)
        {
            if (model.PastHistoryItems == null || model.PastHistoryItems.Count == 0) return;

            foreach (var item in model.PastHistoryItems)
            {
                var status = item.IsYes ? "Yes" :
                             item.IsNo ? "No" :
                             item.IsUnknown ? "Unknown" : "Unknown";

                var pastHistory = new ResponsePastHistory
                {
                    QuestionnaireResponseID = responseId,
                    Disease = item.GeneralPartsZh, // 存入疾病名稱
                    Status = status
                };

                _db.ResponsePastHistory.Add(pastHistory);
            }

            if (!string.IsNullOrEmpty(model.OtherPastHistory))
            {
                _db.ResponsePastHistory.Add(new ResponsePastHistory
                {
                    QuestionnaireResponseID = responseId,
                    Disease = "其他",
                    Status = model.OtherPastHistory // 存入使用者輸入的其他疾病
                });
            }
            _db.SaveChanges();
        }
        #endregion

        #region 儲存 PresentIllness (現在病史)
        private void SavePresentIllness(QuestionnaireViewModel model, int responseId)
        {
            if (model.PresentIllnessItems == null || model.PresentIllnessItems.Count == 0)
            {
                return;
            }

            var presentIllnessList = model.PresentIllnessItems
                .Where(item => item.ReceivingTherapy == "yes" || item.ReceivingTherapy == "no")
                .Select(item => new ResponsePresentIllness
                {
                    QuestionnaireResponseID = responseId,
                    BodyPart = item.PartsOfBodyZh,
                    ReceivingTherapy = item.ReceivingTherapy == "yes"
                }).ToList();

            if (presentIllnessList.Any())  // **確保至少有 1 筆資料才存入**
            {
                _db.ResponsePresentIllness.AddRange(presentIllnessList);
            }
        }
        #endregion

        #region 儲存 PastDrugs (藥物史)
        private void SavePastDrugs(QuestionnaireViewModel model, int responseId)
        {
            if (model.PastDrugsItems == null || model.PastDrugsItems.Count == 0)
                return;

            var pastDrugsList = model.PastDrugsItems
                .Where(item => item.IsUsed)  //確保只存入選 "是" 的項目
                .Select(item => new ResponsePastDrugs
                {
                    QuestionnaireResponseID = responseId,
                    DrugName = item.ItemZh,
                    Used = true
                }).ToList();

            if (pastDrugsList.Any())
                _db.ResponsePastDrugs.AddRange(pastDrugsList);

            if (!string.IsNullOrEmpty(model.TUE))
            {
                _db.ResponsePastDrugs.Add(new ResponsePastDrugs
                {
                    QuestionnaireResponseID = responseId,
                    DrugName = "TUE",
                    Used = model.TUE == "yes"  //轉換為 bool
                });
            }

            _db.SaveChanges();
        }
        #endregion

        #region 儲存 PastSupplements (營養品)
        private void SavePastSupplements(QuestionnaireViewModel model, int responseId)
        {
            if (model.PastSupplementsItems == null || model.PastSupplementsItems.Count == 0)
                return;

            var supplementsList = model.PastSupplementsItems
                .Where(item => item.IsUsed) // **確保只存 "是" 的項目**
                .Select(item => new ResponsePastSupplements
                {
                    QuestionnaireResponseID = responseId,
                    SupplementName = item.ItemZh,
                    Used = true
                }).ToList();

            if (supplementsList.Any())
                _db.ResponsePastSupplements.AddRange(supplementsList);

            if (!string.IsNullOrEmpty(model.OtherSupplements))
            {
                _db.ResponsePastSupplements.Add(new ResponsePastSupplements
                {
                    QuestionnaireResponseID = responseId,
                    SupplementName = "其他",
                    Used = true
                });
            }

            if (supplementsList.Any() || !string.IsNullOrEmpty(model.OtherSupplements))
                _db.SaveChanges();
        }
        #endregion

        #region 儲存 FemaleQuestionnaire (女性問卷)
        private void SaveFemaleQuestionnaire(QuestionnaireViewModel model, int responseId)
        {
            if (model.Gender != 2 || model.FemaleQuestionnaireAnswers == null)
            {
                return; // 只有女性才存
            }

            var femaleResponses = model.FemaleQuestionnaireItems
                .Where(q => model.FemaleQuestionnaireAnswers.ContainsKey(q.ID)) // 確保有答案
                .Select(q => new ResponseFemaleQuestionnaire
                {
                    QuestionnaireResponseID = responseId,
                    Question = q.QuestionZh, // 存入問題
                    Answer = model.FemaleQuestionnaireAnswers[q.ID] // 存入答案
                }).ToList();

            _db.ResponseFemaleQuestionnaire.AddRange(femaleResponses);
        }
        #endregion

        #region 儲存 PastInjury (過去傷害狀況 - 已復原)
        private void SavePastInjury(QuestionnaireViewModel model, int responseId)
        {
            if (model.PastInjuryItems == null || !model.PastInjuryItems.Any())
            {
                return;
            }

            foreach (var item in model.PastInjuryItems)
            {
                var injuryType = model.PastInjuryTypes != null && model.PastInjuryTypes.Any()
                    ? string.Join(", ", model.PastInjuryTypes.Select(x => x.InjuryName))
                    : "未填寫";

                var treatment = model.PastTreatmentItems != null && model.PastTreatmentItems.Any()
                    ? string.Join(", ", model.PastTreatmentItems.Select(x => x.Method))
                    : "未填寫";

                string side = item.LeftSide ? (item.RightSide ? "Both" : "Left") : item.RightSide ? "Right" : "Unknown";

                var pastInjury = new ResponsePastInjuries
                {
                    QuestionnaireResponseID = responseId,
                    BodyPart = item.PastInjuryPart,
                    Side = side,
                    InjuryType = injuryType,
                    Recovered = true, // 過去傷害已恢復
                    Treatment = treatment
                };

                _db.ResponsePastInjuries.Add(pastInjury);
            }
        }
        #endregion

        #region 儲存 CurrentInjury (目前傷害狀況)
        private void SaveCurrentInjury(QuestionnaireViewModel model, int responseId)
        {
            if (model.CurrentInjuryItems == null || !model.CurrentInjuryItems.Any())
            {
                return;
            }

            foreach (var item in model.CurrentInjuryItems)
            {
                var injuryType = model.CurrentInjuryTypes != null && model.CurrentInjuryTypes.Any()
                    ? string.Join(", ", model.CurrentInjuryTypes.Select(x => x.InjuryName))
                    : "未填寫";

                var treatment = model.CurrentTreatmentItems != null && model.CurrentTreatmentItems.Any()
                    ? string.Join(", ", model.CurrentTreatmentItems.Select(x => x.Method))
                    : "未填寫";

                string side = item.LeftSide ? (item.RightSide ? "Both" : "Left") : item.RightSide ? "Right" : "Unknown";

                var currentInjury = new ResponseCurrentInjuries
                {
                    QuestionnaireResponseID = responseId,
                    BodyPart = item.CurrentInjuryPart,
                    Side = side,
                    InjuryType = injuryType,
                    Treatment = treatment
                };

                _db.ResponseCurrentInjuries.Add(currentInjury);
            }
        }
        #endregion

        #region 儲存 Cardiovascular Screening (心血管篩檢)
        private void SaveCardiovascularScreening(QuestionnaireViewModel model, int responseId)
        {
            if (model.CardiovascularScreeningItems == null || model.CardiovascularScreeningItems.Count == 0) return;

            var cardiovascularList = model.CardiovascularScreeningItems
                .Where(item => item.IsUsed)
                .Select(item => new ResponseCardiovascularScreening
            {
                QuestionnaireResponseID = responseId,
                QuestionNumber = item.ID,
                Answer = true
            }).ToList();

            System.Diagnostics.Debug.WriteLine($"CardiovascularScreening 篩選後數量: {cardiovascularList.Count}");

            if (cardiovascularList.Any())
                _db.ResponseCardiovascularScreening.AddRange(cardiovascularList);
        }
        #endregion

        #region 儲存 Concussion Screening (腦震盪篩檢 - 選手自填)
        private void SaveConcussionScreening(QuestionnaireViewModel model, int responseId)
        {
            if (model.ConcussionScreeningItems == null || !model.ConcussionScreeningItems.Any())
            {
                return;
            }

            foreach (var item in model.ConcussionScreeningItems)
            {
                bool answer = model.ConcussionScreeningAnswers.ContainsKey(item.Id) &&
                              model.ConcussionScreeningAnswers[item.Id] == "是";

                var concussionScreening = new ResponseConcussionScreening
                {
                    QuestionnaireResponseID = responseId,
                    QuestionNumber = item.Id,
                    Question = item.Question,
                    Answer = answer
                };

                _db.ResponseConcussionScreening.Add(concussionScreening);
            }

            // 儲存藥物與備註
            _db.ResponseConcussionScreening.Add(new ResponseConcussionScreening
            {
                QuestionnaireResponseID = responseId,
                QuestionNumber = 0, // 表示這是額外資訊
                Question = "是否服用藥物",
                Answer = model.ConcussionScreeningMedicationAnswer == "是"
            });

            _db.ResponseConcussionScreening.Add(new ResponseConcussionScreening
            {
                QuestionnaireResponseID = responseId,
                QuestionNumber = 0,
                Question = "服用藥物細節",
                Answer = !string.IsNullOrEmpty(model.ConcussionScreeningMedicationDetails)
            });

            _db.ResponseConcussionScreening.Add(new ResponseConcussionScreening
            {
                QuestionnaireResponseID = responseId,
                QuestionNumber = 0,
                Question = "備註",
                Answer = !string.IsNullOrEmpty(model.ConcussionScreeningNotes)
            });
        }
        #endregion

        #region 儲存 Symptom Evaluation (症狀評估 0-6分)
        private void SaveSymptomEvaluation(QuestionnaireViewModel model, int responseId)
        {
            if (model.SymptomEvaluationItems == null || !model.SymptomEvaluationItems.Any())
            {
                return;
            }

            foreach (var item in model.SymptomEvaluationItems)
            {
                int score = model.SymptomEvaluationAnswers.ContainsKey(item.ID)
                    ? model.SymptomEvaluationAnswers[item.ID]
                    : 0;

                // **手動確保分數範圍**
                if (score < 0) score = 0;
                if (score > 6) score = 6;

                var symptomEvaluation = new ResponseSymptomEvaluation
                {
                    QuestionnaireResponseID = responseId,
                    Symptom = item.SymptomItem,
                    Severity = score
                };

                _db.ResponseSymptomEvaluation.Add(symptomEvaluation);
            }
        }
        #endregion

        #region 儲存 Orthopaedic Screening (骨科篩檢)
        private void SaveOrthopaedicScreening(QuestionnaireViewModel model, int responseId)
        {
            if (model.OrthopaedicScreeningItems == null || model.OrthopaedicScreeningItems.Count == 0) return;

            var orthopaedicScreeningList = model.OrthopaedicScreeningItems
                .Select(item => new ResponseOrthopaedicScreening
                {
                    QuestionnaireResponseID = responseId,
                    TestNumber = item.ID,
                    TestName = item.Instructions,
                    Observation = item.ObservationPoints,
                    Result = item.Result == "normal" ? "Normal" : "Abnormal"
                }).ToList();

            System.Diagnostics.Debug.WriteLine($"OrthopaedicScreening 篩選後數量: {orthopaedicScreeningList.Count}");

            _db.ResponseOrthopaedicScreening.AddRange(orthopaedicScreeningList);
        }
        #endregion

        #endregion

        #region 問卷存檔成功畫面 
        public ActionResult Success()
        {
            return View();
        }
        #endregion
    }
}