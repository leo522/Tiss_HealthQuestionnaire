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
            var concussionAnswers = Session["ConcussionScreeningAnswers"] as Dictionary<int, string>;
            var symptomAnswers = Session["SymptomEvaluationAnswers"] as Dictionary<int, int>;

            string loggedInUserName = Session["UserName"] as string; //從 Session 獲取使用者的名稱
            
            if (string.IsNullOrEmpty(loggedInUserName))
            {
                return RedirectToAction("Login", "Account"); //如果 Session 中沒有登入資訊，重導向到登入頁面
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

            ViewBag.ConcussionComplete = concussionAnswers != null && concussionAnswers.Count > 0;
            ViewBag.SymptomComplete = symptomAnswers != null && symptomAnswers.Count > 0;

            return View();
        }
        #endregion

        #region 第一部份問卷

        #region 過去健康檢查病史
        public ActionResult PastHealth()
        {
            var pastHealthItems = _db.PastHealth.ToList();

            ViewBag.IsComplete = pastHealthItems.All(item => !string.IsNullOrEmpty(item.ItemZh));

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

        #region 開刀史-不使用
        public ActionResult SurgeryHistory()
        {
            var surgeryHistory = _db.SurgeryHistory.ToList();
            return View("SurgeryHistory", surgeryHistory);
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

                return View("PastInjuryRestored", PastInjuryList);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public ActionResult PastInjuryType() //過去傷勢類型
        {
            var PastInjuryType = _db.PastInjuryType.ToList();
            return View("PastInjuryType", PastInjuryType);
        }

        public ActionResult PastTreatmentMethod() //過去治療方式
        {
            var PastTreatmentMethod = _db.PastTreatmentMethod.ToList();
            return View("PastTreatmentMethod", PastTreatmentMethod);
        }
        #endregion

        #region 目前傷害狀況
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

        public ActionResult NowInjuryType() //目前傷勢類型
        {
            var InjuryType = _db.InjuryType.ToList();
            return View("NowInjuryType", InjuryType);
        }

        public ActionResult NowTreatmentMethod() //目前治療方式
        {
            var treatmentMethod = _db.TreatmentMethod.ToList();
            return View("NowTreatmentMethod", treatmentMethod);
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
            var questions = _db.CardiovascularScreening.ToList();

            var viewModel = questions.Select((q, index) => new CardiovascularScreeningViewModel
            {
                OrderNumber = index + 1, //自動遞增項次
                Question = q.Question
            }).ToList();

            return PartialView("CardiovascularScreening", viewModel);
        }
        #endregion

        #region 腦震盪篩檢-選手自填-選手背景(1) 
        public ActionResult ConcussionScreening()
        {
            var questions = _db.ConcussionScreening.ToList();

            var viewModel = questions.Select((q, index) => new ConcussionScreeningViewModel
            {
                OrderNumber = index + 1, //自動遞增項次
                Question = q.Question
            }).ToList();

            // 從 Session 恢復數據
            if (Session["ConcussionScreeningAnswers"] is Dictionary<int, string> savedAnswers)
            {
                foreach (var item in viewModel)
                {
                    if (savedAnswers.ContainsKey(item.OrderNumber))
                    {
                        item.Answer = savedAnswers[item.OrderNumber];
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

            var viewModel = questions.Select((q, index) => new ConcussionScreeningViewModel
            {
                OrderNumber = index + 1, //自動遞增項次
                Question = q.SymptomItem
            }).ToList();

            // 從 Session 恢復數據
            if (Session["SymptomEvaluationAnswers"] is Dictionary<int, int> savedAnswers)
            {
                foreach (var item in viewModel)
                {
                    if (savedAnswers.ContainsKey(item.OrderNumber))
                    {
                        item.OrderNumber = savedAnswers[item.OrderNumber];
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

                var savedAnswers = Session["CognitiveScreeningAnswers"] as Dictionary<int, int> ?? new Dictionary<int, int>();

                var viewModel = questions.Select((q, index) => new CognitiveScreeningViewModel
                {
                    OrderNumber = index + 1,
                    Question = q.Question,
                    OrientationScore = savedAnswers.ContainsKey(index + 1) ? savedAnswers[index + 1] : 0
                }).ToList();

                return View("CognitiveScreening", viewModel);
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

            int totalScore = 0; //計算短期記憶總分

            // 為每個問題生成 ViewModel
            var viewModel = questions.Select(q =>
            {
                int firstScore = 0, secondScore = 0, thirdScore = 0;

                // 使用 TryGetValue 獲取已保存的分數
                immediateMemoryAnswers?.TryGetValue($"first_{q.ID}", out firstScore);
                immediateMemoryAnswers?.TryGetValue($"second_{q.ID}", out secondScore);
                immediateMemoryAnswers?.TryGetValue($"third_{q.ID}", out thirdScore);

                totalScore += firstScore + secondScore + thirdScore;

                return new ImmediateMemoryViewModel
                {
                    OrderNumber = q.ID,
                    Word = q.Word,
                    FirstTestScore = firstScore,
                    SecondTestScore = secondScore,
                    ThirdTestScore = thirdScore,
                    CompletionTime = completionTime
                };
            }).ToList();

            Session["ImmediateMemoryTotalScore"] = totalScore; //保存短期記憶總分到 Session

            return View("ImmediateMemory", viewModel);
        }
        #endregion

        #region 腦震盪篩檢-防護員評估-認知篩檢-專注力(3)
        public ActionResult Concentration()
        {
            // 從資料庫或其他來源加載數據
            var viewModel = new List<ConcentrationViewModel>
            {
                new ConcentrationViewModel { OrderNumber = 1, ListA = "4-9-3", ListB = "5-2-6", ListC = "1-4-2" },
                new ConcentrationViewModel { OrderNumber = 2, ListA = "6-2-9", ListB = "4-1-5", ListC = "6-5-8" },
                new ConcentrationViewModel { OrderNumber = 3, ListA = "3-8-1-4", ListB = "1-7-9-5", ListC = "6-8-3-1" },
                new ConcentrationViewModel { OrderNumber = 4, ListA = "3-2-7-9", ListB = "4-9-6-8", ListC = "3-4-8-1" }
            };

            // 從 Session 中恢復數據
            var savedScores = Session["ConcentrationScores"] as Dictionary<int, int>;
            if (savedScores != null)
            {
                foreach (var item in viewModel)
                {
                    if (savedScores.ContainsKey(item.OrderNumber))
                    {
                        item.Score = savedScores[item.OrderNumber];
                    }
                }
            }
            return View("Concentration", viewModel);
        }
        #endregion

        #region 腦震盪篩檢-防護員評估-認知篩檢-協調與平衡測驗(4)
        public ActionResult CoordinationAndBalanceExamination()
        {
            // 初始化模型
            var model = new CoordinationAndBalanceExaminationViewModel
            {
                TestFoot = "",
                TestSurface = "",
                Footwear = "",
                DoubleLegError = 0,
                TandemError = 0,
                SingleLegError = 0,
                FirstTime = 0,
                SecondTime = 0,
                ThirdTime = 0
            };

            // 從 Session 中恢復數據（若有）
            if (Session["CoordinationAndBalanceData"] is CoordinationAndBalanceExaminationViewModel savedData)
            {
                model = savedData;
            }

            return View(model);
        }
        #endregion

        #region 腦震盪篩檢-防護員評估-認知篩檢-延遲記憶(5)
        public ActionResult DelayedRecall()
        {
            var questions = _db.DelayedRecall.ToList();

            var delayedrecallAnswers = Session["DelayedRecallAnswers"] as Dictionary<int, int>;

            var delayedRecallStartTime = Session["DelayedRecallStartTime"] as string;

            var viewModel = questions.Select(q =>
            {
                int Scores = 0;

                delayedrecallAnswers?.TryGetValue(q.ID, out Scores);

                return new DelayedRecallViewModel
                {
                    OrderNumber = q.ID,
                    Word = q.Word,
                    Score = Scores,
                };
            }).ToList();

            ViewBag.DelayedRecallStartTime = delayedRecallStartTime; //傳遞開始時間
            return View("DelayedRecall", viewModel);
        }
        #endregion

        #region 腦震盪篩檢-防護員評估-認知篩檢-認知篩檢分數總合(6)
        public ActionResult CognitiveScreeningTotalScore()
        {
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

            return View("CognitiveScreeningTotalScore", viewModel);
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

        #endregion

        #region 預覽頁
        public ActionResult Preview(QuestionnaireViewModel model)
        {
            return View("Preview", model); //顯示預覽頁
        }

        [HttpPost]
        public ActionResult Preview(QuestionnaireViewModel model, FormCollection form)
        {
            try
            {
                ProcessBasicInfo(model, form); //基本資料

                ProcessPastHealth(model, form); //過去健康檢查病史

                ProcessAllergicHistory(model, form); //過敏史

                ProcessFamilyHistory(model, form); //家族病史

                ProcessPastHistory(model, form); //過去病史

                ProcessSurgeryHistory(model, form); //處理手術史

                ProcessPresentIllness(model, form); //現在病史

                ProcessPastDrugs(model, form); //藥物史

                ProcessPastSupplements(model, form); //營養品

                ProcessFemaleQuestionnaire(model, form); //處理女性問卷

                ProcessPastInjuryStatus(model, form); //過去傷害狀況(已復原)

                ProcessPastTreatmentMethod(model, form); //過去傷害狀況(已復原)-治療方式

                ProcessCurrentInjuryStatus(model, form); //目前傷害狀況

                ProcessCurrentTreatmentMethod(model, form); //目前傷害狀況-治療方式

                ProcessCardiovascularScreening(model, form); //心血管篩檢

                ProcessConcussionScreening(model, form); //腦震盪篩檢-選手自填(選手背景)-(1)

                ProcessSymptomEvaluation(model, form); //症狀自我評估-選手-(2)

                ProcessCognitiveScreening(model, form); //認知篩檢-定位

                ProcessOrthopaedicScreening(model, form); //骨科篩檢

                _db.SaveChanges(); // 儲存所有變更

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
                model.FormData[key] = string.IsNullOrEmpty(form[key]) ? "未回答" : form[key];
            }
        }
        #endregion

        #region 過去健康檢查病史
        private void ProcessPastHealth(QuestionnaireViewModel model, FormCollection form)
        {
            var pastHealthItems = _db.PastHealth.ToList();
            foreach (var item in pastHealthItems)
            {
                var selectedValue = form[$"pastHealth_{item.ID}"];
                var detail = new PastHealthDetailViewModel
                {
                    ItemId = item.ID,
                    ItemZh = item.ItemZh,
                    Item1 = form[$"item1_{item.ID}"] ?? "未回答",
                    Item2 = form[$"item2_{item.ID}"] ?? "未回答",
                    Item3 = form[$"item3_{item.ID}"] ?? "未回答"
                };

                if (selectedValue == "yes")
                {
                    item.IsYes = true;
                    item.IsNo = false;
                    model.PastHealthHistory = "yes";
                }
                else if (selectedValue == "no")
                {
                    item.IsYes = false;
                    item.IsNo = true;
                    model.PastHealthHistory = "no";
                }
                else
                {
                    item.IsYes = false;
                    item.IsNo = false;
                    model.PastHealthHistory = "未回答";
                }

                model.PastHealthDetails.Add(detail);
            }
        }
        #endregion

        #region 過敏史
        private void ProcessAllergicHistory(QuestionnaireViewModel model, FormCollection form)
        {
            var allergicItems = _db.AllergicHistory.ToList();
            foreach (var item in allergicItems)
            {
                var selectedValue = form[$"allergy_{item.ID}"];
                var detail = new AllergicHistoryDetailViewModel
                {
                    ItemId = item.ID,
                    ItemZh = item.ItemZh,
                    ItemEn = item.ItemEn,
                    AllergyDescription = form[$"details_{item.ID}"] ?? "未回答"
                };

                if (selectedValue == "yes")
                {
                    item.IsYes = true;
                    item.IsNo = false;
                    model.AllergicHistory = "yes";
                }
                else if (selectedValue == "no")
                {
                    item.IsYes = false;
                    item.IsNo = true;
                    model.AllergicHistory = "no";
                }
                else
                {
                    item.IsYes = false;
                    item.IsNo = false;
                    model.AllergicHistory = "未回答";
                }

                model.AllergicHistoryDetails.Add(detail);
            }
        }
        #endregion

        #region 家族病史
        private void ProcessFamilyHistory(QuestionnaireViewModel model, FormCollection form)
        {
            var familyHistoryItems = _db.FamilyHistory.ToList();
            foreach (var item in familyHistoryItems)
            {
                var selectedValue = form[$"familyHistory_{item.ID}"];
                var detail = new FamilyHistoryViewModel
                {
                    ItemId = item.ID,
                    GeneralPartsZh = item.GeneralPartsZh,
                    GeneralPartsEn = item.GeneralPartsEn,
                    FamilyHistoryOption = selectedValue ?? "未回答",
                    OtherFamilyHistory = item.ID == 10 ? form["otherFamilyHistory"] ?? "未回答" : null
                };

                model.FamilyHistoryDetails.Add(detail);
            }
        }
        #endregion

        #region 過去病史
        private void ProcessPastHistory(QuestionnaireViewModel model, FormCollection form)
        {
            var pastHistoryItems = _db.PastHistory.ToList();
            foreach (var item in pastHistoryItems)
            {
                var selectedValue = form[$"pastHistory_{item.ID}"];
                var detail = new PastHistoryDetailViewModel
                {
                    ItemId = item.ID,
                    GeneralPartsZh = item.GeneralPartsZh,
                    GeneralPartsEn = item.GeneralPartsEn,
                    PastHistoryOption = selectedValue ?? "未回答",
                    OtherPastHistory = item.ID == 20 ? form["otherPastHistory"] ?? "未回答" : null
                };

                model.PastHistoryDetails.Add(detail);
            }
        }
        #endregion

        #region 手術史
        private void ProcessSurgeryHistory(QuestionnaireViewModel model, FormCollection form)
        {
            var surgeryItems = _db.SurgeryHistory.ToList();
            foreach (var item in surgeryItems)
            {
                var selectedValue = form[$"surgeryHistory_{item.ID}"];
                var detail = new SurgeryHistoryDetailViewModel
                {
                    ItemId = item.ID,
                    PartsOfBodyZh = item.PartsOfBodyZh,
                    PartsOfBodyEn = item.PartsOfBodyEn,
                    OperationOption = selectedValue ?? "未回答"
                };

                model.SurgeryHistoryDetails.Add(detail);
            }
        }
        #endregion

        #region 現在病史
        private void ProcessPresentIllness(QuestionnaireViewModel model, FormCollection form)
        {
            var presentIllnessItems = _db.PresentIllness.ToList();
            foreach (var item in presentIllnessItems)
            {
                var selectedValue = form[$"presentIllness_{item.ID}"];
                var detail = new PresentIllnessDetailViewModel
                {
                    ItemId = item.ID,
                    PartsOfBodyZh = item.PartsOfBodyZh,
                    PartsOfBodyEn = item.PartsOfBodyEn,
                    ReceivingOtherTherapies = selectedValue ?? "未回答"
                };

                model.PresentIllnessDetails.Add(detail);
            }
        }
        #endregion

        #region 藥物史
        private void ProcessPastDrugs(QuestionnaireViewModel model, FormCollection form)
        {
            var pastDrugsItems = _db.PastDrugs.ToList();
            foreach (var item in pastDrugsItems)
            {
                if (form[$"usedDrugs_{item.ID}"] == "on")
                {
                    model.PastDrugsDetails.Add(new PastDrugsDetailViewModel
                    {
                        ItemId = item.ID,
                        ItemZh = item.ItemZh,
                        ItemEn = item.ItemEn,
                        IsUsed = "yes"
                    });
                }
            }

            model.TUE = form["TUE"] ?? "unknown";
        }
        #endregion

        #region 營養品
        private void ProcessPastSupplements(QuestionnaireViewModel model, FormCollection form)
        {
            // 從資料庫中獲取所有營養品項目
            var supplementsItems = _db.PastSupplements.ToList();

            // 初始化模型中的 PastSupplementsDetails 列表
            model.PastSupplementsDetails = new List<PastSupplementsDetailViewModel>();

            // 獲取 "其他" 的描述內容
            string otherSupplementDescription = form["otherSupplementDetail"];
            // 檢查「其他」選項是否勾選
            bool isOthersChecked = form["usedSupplement_other"] == "on";

            // 如果「其他」選項勾選但未填寫描述，添加錯誤提示
            if (isOthersChecked && string.IsNullOrEmpty(otherSupplementDescription))
            {
                ModelState.AddModelError("otherSupplementDetail", "您勾選了「其他」選項，請填寫描述內容。");
            }

            // 遍歷所有營養品項目
            foreach (var item in supplementsItems)
            {
                // 根據項目 ID 構建表單鍵名，檢查是否選中
                string usedKey = $"usedSupplements_{item.ID}";
                bool isChecked = form[usedKey] == "on";

                // 如果選中或是「其他」項目且已勾選
                if (isChecked || (item.ItemZh == "其他" && isOthersChecked))
                {
                    // 添加到模型的 PastSupplementsDetails 列表
                    model.PastSupplementsDetails.Add(new PastSupplementsDetailViewModel
                    {
                        ItemId = item.ID,
                        ItemZh = item.ItemZh, // 項目中文名稱
                        ItemEn = item.ItemEn, // 項目英文名稱
                        IsUsed = "yes", // 標記為已使用
                        OtherSupplements = item.ItemZh == "其他" ? otherSupplementDescription : null // 如果是「其他」，保存描述
                    });
                }
            }
        }
        #endregion

        #region 女性問卷
        private void ProcessFemaleQuestionnaire(QuestionnaireViewModel model, FormCollection form)
        {
            if (model.Gender == 2)
            {
                var femaleQuestionnaireItems = _db.FemaleQuestionnaire.ToList();
                foreach (var item in femaleQuestionnaireItems)
                {
                    var detail = new FemaleQuestionnaireDetailViewModel
                    {
                        ID = item.ID,
                        QuestionZh = item.QuestionZh,
                        QuestionEn = item.QuestionEn,
                        Answer = form[$"femaleQuestion_{item.ID}"] ?? "未回答",
                        AnswerOptions = new Dictionary<string, string>
                        {
                            { "yes", "是 Yes" },
                            { "no", "否 No" },
                            { "unknown", "未知 Unknown" }
                        }
                    };

                    model.FemaleQuestionnaireDetails.Add(detail);
                }
            }
        }
        #endregion

        #region 過去傷害狀況-已復原
        private void ProcessPastInjuryStatus(QuestionnaireViewModel model, FormCollection form)
        {
            var pastInjuryItems = _db.PastInjuryStatus.ToList(); //收集傷害部位數據

            model.PastInjuryDetails = new List<PastInjuryStatuSViewModel>(); //初始化傷害部位列表

            foreach (var item in pastInjuryItems)
            {
                string leftPartKey = $"pastinjuryLeft_{item.Id}";
                string rightPartKey = $"pastinjuryRight_{item.Id}";
                bool hasInjury = form[leftPartKey] != null || form[rightPartKey] != null; //判斷是否有受傷部位選擇

                if (hasInjury)
                {
                    var injuryTypes = new List<string>
                    {
                        form[$"MuscleTendon"],          // 肌肉/肌腱
                        form[$"Bone"],                 // 骨骼
                        form[$"Ligament"],             // 韌帶
                        form[$"Nerve"],                // 神經
                        form[$"CartilageSynoviumBursa"], // 軟骨/滑膜/滑囊
                        form[$"EpidermalTissue"],      // 表皮組織
                        form[$"BloodVessel"],          // 血管
                        form[$"OrganLimb"]             // 器官/四肢
                    }.Where(x => !string.IsNullOrEmpty(x)).ToList();

                    model.PastInjuryDetails.Add(new PastInjuryStatuSViewModel
                    {
                        PastInjuryPart = item.InjuryPart,
                        LeftSide = !string.IsNullOrEmpty(form[leftPartKey]),
                        RightSide = !string.IsNullOrEmpty(form[rightPartKey]),
                        InjuryTypes = injuryTypes
                    });
                }
            }
        }
        #endregion

        #region 過去傷害狀況-治療方式
        private void ProcessPastTreatmentMethod(QuestionnaireViewModel model, FormCollection form)
        {
            var pastTreatmentMethods = _db.PastTreatmentMethod.ToList();
            foreach (var method in pastTreatmentMethods)
            {
                string methodKey = $"Pasttreatment_{method.Id}";
                if (!string.IsNullOrEmpty(form[methodKey]))
                {
                    model.PastTreatmentDetails.Add(new PastTreatmentMethoDViewModel
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

            model.NowInjuryDetails = new List<InjuryStatuSViewModel>(); //初始化傷害部位詳細資料

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
                    model.NowInjuryDetails.Add(new InjuryStatuSViewModel
                    {
                        InjuryPart = item.InjuryPart,            // 傷害部位名稱
                        LeftSide = !string.IsNullOrEmpty(form[leftPartKey]), // 左側是否受傷
                        RightSide = !string.IsNullOrEmpty(form[rightPartKey]), // 右側是否受傷
                        InjuryTypes = injuryTypes              // 綁定傷勢類型
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
                    model.NowTreatmentDetails.Add(new TreatmentMethoDViewModel
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
            var screenings = _db.CardiovascularScreening.ToList();
            foreach (var item in screenings)
            {
                string answerKey = $"question_{item.Id}";
                var answer = form[answerKey];
                model.CardiovascularScreeningDetails.Add(new CardiovascularScreeningDetailViewModel
                {
                    OrderNumber = item.Id,
                    Question = item.Question,
                    Answer = answer == "yes" ? "是 Yes" : "否 No"
                });
            }
        }
        #endregion

        #region 腦震盪篩檢-選手自填-(1)
        private void ProcessConcussionScreening(QuestionnaireViewModel model, FormCollection form)
        {
            var screenings = _db.ConcussionScreening.ToList();
            foreach (var item in screenings)
            {
                string answerKey = $"concussion_question_{item.Id}";
                model.ConcussionScreeningDetails.Add(new ConcussionScreeningViewModel
                {
                    OrderNumber = item.Id,
                    Question = item.Question,
                    Answer = form[answerKey]
                });
            }

            model.ConcussionScreeningMedicationAnswer = form["medication"] ?? "無";
            model.ConcussionScreeningMedicationDetails = form["medicationDetails"] ?? "無";
            model.ConcussionScreeningNotes = form["notes"] ?? "無";
        }
        #endregion

        #region 症狀自我評估-選手自填-(2)
        private void ProcessSymptomEvaluation(QuestionnaireViewModel model, FormCollection form)
        {
            foreach (var key in form.AllKeys.Where(k => k.StartsWith("score_")))
            {
                int questionId = int.Parse(key.Replace("score_", ""));
                int score = !string.IsNullOrEmpty(form[key]) ? int.Parse(form[key]) : 0;

                var symptomItem = _db.SymptomEvaluation.FirstOrDefault(q => q.ID == questionId);
                if (symptomItem != null)
                {
                    model.SymptomEvaluationDetails.Add(new SymptomEvaluationViewModel
                    {
                        OrderNumber = questionId,
                        SymptomItem = symptomItem.SymptomItem,
                        Score = score
                    });
                }
            }
        }
        #endregion

        private void ProcessCognitiveScreening(QuestionnaireViewModel model, FormCollection form)
        {
            // Example for cognitive screening
            // Similar to the sections above, you can structure the logic to calculate and process values.
        }

        #region 骨科篩檢
        private void ProcessOrthopaedicScreening(QuestionnaireViewModel model, FormCollection form)
        {
            var screenings = _db.OrthopaedicScreening.ToList();
            foreach (var item in screenings)
            {
                string resultKey = $"result_{item.Id}";
                var result = form[resultKey];
                model.OrthopaedicScreeningDetails.Add(new OrthopaedicScreeninGViewModel
                {
                    OrderNumber = item.Id,
                    Instructions = item.Instructions,
                    ObservationPoints = item.ObservationPoints,
                    Result = result == "normal" ? "正常(Normal)" : "異常(Abnormal)"
                });
            }
        }
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
        [HttpPost]
        public ActionResult SaveHealth(QuestionnaireViewModel model)
        {
            try
            {
                // 保存問卷主表
                var response = new QuestionnaireResponses
                {
                    Specialist = model.Specialist,
                    FillName = model.FillName,
                    AtheNum = model.AtheNum,
                    Gender = model.Gender,
                    FillDate = model.FillDate,
                    SubmissionDate = DateTime.Now
                };
                _db.QuestionnaireResponses.Add(response);
                _db.SaveChanges();

                // 儲存過去健康檢查病史
                var pastHealth = new ResponsePastHealth
                {
                    QuestionnaireResponseId = response.Id,
                    HasAbnormality = model.PastHealthHistory == "yes"
                };
                _db.ResponsePastHealth.Add(pastHealth);
                _db.SaveChanges();

                foreach (var detail in model.PastHealthDetails)
                {
                    var pastHealthDetail = new ResponsePastHealthDetails
                    {
                        PastHealthId = pastHealth.Id,
                        Item1 = detail.Item1,
                        Item2 = detail.Item2,
                        Item3 = detail.Item3
                    };
                    _db.ResponsePastHealthDetails.Add(pastHealthDetail);
                }

                // 儲存過敏史
                var allergicHistory = new ResponseAllergicHistory
                {
                    QuestionnaireResponseId = response.Id,
                    IsAllergic = model.AllergicHistory == "yes"
                };
                _db.ResponseAllergicHistory.Add(allergicHistory);
                _db.SaveChanges();

                foreach (var detail in model.AllergicHistoryDetails)
                {
                    var allergicHistoryDetail = new ResponseAllergicHistoryDetails
                    {
                        AllergicHistoryId = allergicHistory.Id,
                        ItemZh = detail.ItemZh,
                        IsAllergic = detail.IsAllergic == "yes",
                        AllergyDescription = detail.AllergyDescription
                    };
                    _db.ResponseAllergicHistoryDetails.Add(allergicHistoryDetail);
                }

                // 儲存家族病史
                foreach (var detail in model.FamilyHistoryDetails)
                {
                    var familyHistory = new ResponseFamilyHistory
                    {
                        QuestionnaireResponseId = response.Id,
                        GeneralPartsZh = detail.GeneralPartsZh,
                        GeneralPartsEn = detail.GeneralPartsEn,
                        FamilyHistoryOption = detail.FamilyHistoryOption,
                        OtherFamilyHistory = detail.OtherFamilyHistory
                    };
                    _db.ResponseFamilyHistory.Add(familyHistory);
                }

                // 儲存過去病史
                foreach (var detail in model.PastHistoryDetails)
                {
                    var pastHistory = new ResponsePastHistory
                    {
                        QuestionnaireResponseId = response.Id,
                        GeneralPartsZh = detail.GeneralPartsZh,
                        GeneralPartsEn = detail.GeneralPartsEn,
                        PastHistoryOption = detail.PastHistoryOption,
                        OtherPastHistory = detail.OtherPastHistory
                    };
                    _db.ResponsePastHistory.Add(pastHistory);
                }

                // 儲存現在病史
                foreach (var detail in model.PresentIllnessDetails)
                {
                    var presentIllness = new ResponsePresentIllness
                    {
                        QuestionnaireResponseId = response.Id,
                        PartsOfBodyZh = detail.PartsOfBodyZh,
                        ReceivingOtherTherapies = detail.ReceivingOtherTherapies == "yes"
                    };
                    _db.ResponsePresentIllness.Add(presentIllness);
                }

                // 儲存藥物史
                foreach (var detail in model.PastDrugsDetails)
                {
                    var pastDrugs = new ResponsePastDrugs
                    {
                        QuestionnaireResponseId = response.Id,
                        MedicationName = detail.ItemZh,
                        IsUsed = detail.IsUsed == "yes",
                        OtherDrugs = detail.OtherDrugs,
                        TUE = model.TUE == "yes"
                    };
                    _db.ResponsePastDrugs.Add(pastDrugs);
                }

                // 儲存營養品
                foreach (var detail in model.PastSupplementsDetails)
                {
                    var supplement = new ResponseSupplements
                    {
                        QuestionnaireResponseId = response.Id,
                        ItemZh = detail.ItemZh,
                        IsUsed = detail.IsUsed == "yes",
                        OtherSupplements = detail.OtherSupplements
                    };
                    _db.ResponseSupplements.Add(supplement);
                }

                // 儲存女性問卷
                if (model.Gender == 2)
                {
                    foreach (var detail in model.FemaleQuestionnaireDetails)
                    {
                        var femaleQuestionnaire = new ResponseFemaleQuestionnaire
                        {
                            QuestionnaireResponseId = response.Id,
                            QuestionZh = detail.QuestionZh,
                            QuestionEn = detail.QuestionEn,
                            DisplayAnswer = detail.Answer
                        };
                        _db.ResponseFemaleQuestionnaire.Add(femaleQuestionnaire);
                    }
                }

                // 儲存傷害狀況
                foreach (var injury in model.NowInjuryDetails)
                {
                    var responseInjury = new ResponseInjury
                    {
                        QuestionnaireResponseId = response.Id,
                        InjuryPart = injury.InjuryPart,
                        LeftSide = injury.LeftSide,
                        RightSide = injury.RightSide,
                        InjuryTypes = string.Join(",", injury.InjuryTypes),
                        TreatmentMethods = string.Join(",", model.NowTreatmentDetails.Select(t => t.Method)),
                        IsCurrent = true
                    };
                    _db.ResponseInjury.Add(responseInjury);
                }

                foreach (var injury in model.PastInjuryDetails)
                {
                    var responseInjury = new ResponseInjury
                    {
                        QuestionnaireResponseId = response.Id,
                        InjuryPart = injury.PastInjuryPart,
                        LeftSide = injury.LeftSide,
                        RightSide = injury.RightSide,
                        InjuryTypes = string.Join(",", injury.InjuryTypes),
                        TreatmentMethods = string.Join(",", model.PastTreatmentDetails.Select(t => t.Method)),
                        IsCurrent = false
                    };
                    _db.ResponseInjury.Add(responseInjury);
                }

                // 儲存心血管篩檢
                foreach (var detail in model.CardiovascularScreeningDetails)
                {
                    var cardiovascularScreening = new ResponseCardiovascularScreening
                    {
                        QuestionnaireResponseId = response.Id,
                        OrderNumber = detail.OrderNumber,
                        Question = detail.Question,
                        Answer = detail.Answer
                    };
                    _db.ResponseCardiovascularScreening.Add(cardiovascularScreening);
                }

                // 儲存腦震盪篩檢-選手
                foreach (var detail in model.ConcussionScreeningDetails)
                {
                    var concussionScreening = new ResponseConcussionScreening
                    {
                        QuestionnaireResponseId = response.Id,
                        OrderNumber = detail.OrderNumber,
                        Question = detail.Question,
                        Answer = detail.Answer
                    };
                    _db.ResponseConcussionScreening.Add(concussionScreening);
                }

                // 儲存症狀自我評估-選手
                foreach (var detail in model.SymptomEvaluationDetails)
                {
                    var symptomEvaluation = new ResponseSymptomEvaluation
                    {
                        QuestionnaireResponseId = response.Id,
                        OrderNumber = detail.OrderNumber,
                        Symptom = detail.SymptomItem,
                        Score = detail.Score
                    };
                    _db.ResponseSymptomEvaluation.Add(symptomEvaluation);
                }

                // 儲存骨科篩檢
                foreach (var detail in model.OrthopaedicScreeningDetails)
                {
                    var orthopaedicScreening = new ResponseOrthopaedicScreening
                    {
                        QuestionnaireResponseId = response.Id,
                        OrderNumber = detail.OrderNumber,
                        ObservationPoints = detail.ObservationPoints,
                        Result = detail.Result
                    };
                    _db.ResponseOrthopaedicScreening.Add(orthopaedicScreening);
                }

                #region 認知篩檢 - 定位
                foreach (var detail in model.CognitiveScreeningDetails)
                {
                    var orientation = new ResponseOrientation
                    {
                        QuestionnaireResponseId = response.Id,
                        OrderNumber = detail.OrderNumber,
                        Question = detail.Question,
                        OrientationScore = detail.OrientationScore
                    };
                    _db.ResponseOrientation.Add(orientation);
                }
                #endregion

                #region 認知篩檢 - 短期記憶
                foreach (var detail in model.ImmediateMemoryDetails)
                {
                    var immediateMemory = new ResponseImmediateMemory
                    {
                        QuestionnaireResponseId = response.Id,
                        Word = detail.Word,
                        FirstTestScore = detail.FirstTestScore,
                        SecondTestScore = detail.SecondTestScore,
                        ThirdTestScore = detail.ThirdTestScore
                    };
                    _db.ResponseImmediateMemory.Add(immediateMemory);
                }
                #endregion

                #region 認知篩檢 - 專注力
                foreach (var detail in model.ConcentrationDetails)
                {
                    var concentration = new ResponseConcentration
                    {
                        QuestionnaireResponseId = response.Id,
                        ListA = detail.ListA,
                        ListB = detail.ListB,
                        ListC = detail.ListC,
                        Response = detail.Score == 1 ? "Y" : "N",
                        Score = detail.Score
                    };
                    _db.ResponseConcentration.Add(concentration);
                }
                #endregion

                #region 認知篩檢 - 協調與平衡測驗
                foreach (var detail in model.CoordinationAndBalanceDetails)
                {
                    var coordinationAndBalance = new ResponseCoordinationAndBalance
                    {
                        QuestionnaireResponseId = response.Id,
                        TestFoot = detail.TestFoot,
                        TestSurface = detail.TestSurface,
                        Footwear = detail.Footwear,
                        DoubleLegError = detail.DoubleLegError,
                        TandemError = detail.TandemError,
                        SingleLegError = detail.SingleLegError,
                        FirstTime = detail.FirstTime,
                        SecondTime = detail.SecondTime,
                        ThirdTime = detail.ThirdTime,
                        TotalErrors = detail.TotalErrors,
                        AverageTime = detail.AverageTimes,
                        FastestTime = detail.FastestTimes
                    };
                    _db.ResponseCoordinationAndBalance.Add(coordinationAndBalance);
                }
                #endregion

                #region 認知篩檢 - 延遲記憶
                foreach (var detail in model.DelayedRecallDetails)
                {
                    var delayedRecall = new ResponseDelayedRecall
                    {
                        QuestionnaireResponseId = response.Id,
                        OrderNumber = detail.OrderNumber,
                        Word = detail.Word,
                        Score = detail.Score
                    };
                    _db.ResponseDelayedRecall.Add(delayedRecall);
                }
                #endregion

                #region 認知篩檢 - 分數總和
                var totalScores = new ResponseTotalScores
                {
                    QuestionnaireResponseId = response.Id,
                    OrientationScore = model.CognitiveScreeningTotalScore,
                    ImmediateMemoryScore = model.ImmediateMemoryTotalScore,
                    ConcentrationScore = model.ConcentrationTotalScore,
                    DelayedRecallScore = model.DelayedRecallTotalScore,
                };
                _db.ResponseTotalScores.Add(totalScores);
                #endregion

                _db.SaveChanges(); // 儲存所有變更

                TempData["SuccessMessage"] = "問卷資料已成功送出！";
                return RedirectToAction("Preview");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "發生錯誤：" + ex.Message;
                return RedirectToAction("Preview", model);
            }
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

        #region 保存醫療團隊-認知篩檢-定位(1)
        [HttpPost]
        public ActionResult SaveCognitiveScreening(FormCollection form)
        {
            try
            {
                var answers = new Dictionary<int, int>();

                foreach (var key in form.AllKeys)
                {
                    if (key.StartsWith("question_"))
                    {
                        int orderNumber = int.Parse(key.Substring("question_".Length));

                        int answer = int.Parse(form[key]);

                        answers[orderNumber] = answer;
                    }
                }

                Session["CognitiveScreeningAnswers"] = answers;

                return RedirectToAction("ImmediateMemory");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"保存數據時出錯：{ex.Message}");
                return View("CognitiveScreening");
            }
        }
        #endregion

        #region 保存醫療團隊-認知篩檢-短期記憶(2)
        [HttpPost]
        public ActionResult SaveImmediateMemory(FormCollection form)
        {
            try
            {
                var answers = new Dictionary<string, int>();
                foreach (var key in form.AllKeys)
                {
                    if (key.StartsWith("first_") || key.StartsWith("second_") || key.StartsWith("third_"))
                    {
                        answers[key] = int.Parse(form[key]);
                    }
                }

                string completionTime = form["CompletionTime"]; //保存完成時間
                Session["ImmediateMemoryCompletionTime"] = completionTime;

                Session["ImmediateMemoryAnswers"] = answers; //保存數據到 Session

                return RedirectToAction("Concentration");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region 保存醫療團隊-認知篩檢-專注力(3)
        [HttpPost]
        public ActionResult SaveConcentration(FormCollection form)
        {
            try
            {
                //string action = form["action"]; //檢查是哪個按鈕觸發的
                //if (action == "Previous")
                //{
                //    // 不保存數據，直接返回定位頁面
                //    return RedirectToAction("CognitiveScreening");
                //}

                //var scores = new Dictionary<int, int>();

                //foreach (var key in form.AllKeys)
                //{
                //    if (key.StartsWith("response_"))
                //    {
                //        int orderNumber = int.Parse(key.Split('_')[1]);
                //        int score = int.Parse(form[key]);
                //        scores[orderNumber] = score;
                //    }
                //}

                var scores = new Dictionary<int, int>();
                foreach (var key in form.AllKeys)
                {
                    if (key.StartsWith("response_"))
                    {
                        int orderNumber = int.Parse(key.Split('_')[1]);
                        scores[orderNumber] = int.Parse(form[key]);
                    }
                }
                Session["ConcentrationScores"] = scores;

                return RedirectToAction("CoordinationAndBalanceExamination");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "保存專注力測驗數據時出錯：" + ex.Message);
                return View("Concentration");
            }
        }
        #endregion

        #region 保存醫療團隊-認知篩檢-協調與平衡測驗(4)
        [HttpPost]
        public ActionResult SaveCoordinationAndBalanceExamination(FormCollection form)
        {
            try
            {
                string action = form["action"]; // 檢查是哪個按鈕觸發的
                if (action == "Previous")
                {
                    // 不保存數據，直接返回專注力頁面
                    return RedirectToAction("Concentration");
                }

                // 初始化協調與平衡測驗數據
                var model = new CoordinationAndBalanceExaminationViewModel
                {
                    TestFoot = form["TestFoot"],
                    TestSurface = form["TestSurface"],
                    Footwear = form["Footwear"],
                    DoubleLegError = int.TryParse(form["DoubleLegError"], out var dle) ? dle : 0,
                    TandemError = int.TryParse(form["TandemError"], out var te) ? te : 0,
                    SingleLegError = int.TryParse(form["SingleLegError"], out var sle) ? sle : 0,
                    FirstTime = float.TryParse(form["FirstTime"], out var ft) ? ft : 0,
                    SecondTime = float.TryParse(form["SecondTime"], out var st) ? st : 0,
                    ThirdTime = float.TryParse(form["ThirdTime"], out var tt) ? tt : 0
                };

                // 計算總錯誤次數、平均時間和最快時間
                var totalErrors = model.DoubleLegError + model.TandemError + model.SingleLegError;
                var averageTime = (model.FirstTime + model.SecondTime + model.ThirdTime) / 3;
                var fastestTime = new[] { model.FirstTime, model.SecondTime, model.ThirdTime }
                    .Where(t => t > 0)
                    .DefaultIfEmpty(0)
                    .Min();

                // 保存至 Session
                Session["CoordinationAndBalanceData"] = model;
                Session["CoordinationAndBalanceTotalErrors"] = totalErrors;
                Session["CoordinationAndBalanceAverageTime"] = averageTime;
                Session["CoordinationAndBalanceFastestTime"] = fastestTime;

                return RedirectToAction("DelayedRecall"); // 跳轉到下一頁
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"保存數據時發生錯誤：{ex.Message}");
                return View("CoordinationAndBalanceExamination");
            }
        }
        #endregion

        #region 保存醫療團隊-認知篩檢-延遲記憶(5)
        [HttpPost]
        public ActionResult SaveDelayedRecall(FormCollection form)
        {
            try
            {
                string action = form["action"];

                if (action == "Previous")
                {
                    return RedirectToAction("CoordinationAndBalanceExamination"); //返回上一頁
                }

                var scores = new Dictionary<int, int>(); //收集分數

                foreach (var key in form.AllKeys)
                {
                    if (key.StartsWith("score_"))
                    {
                        int orderNumber = int.Parse(key.Split('_')[1]);
                        int score = int.TryParse(form[key], out var value) ? value : 0;
                        scores[orderNumber] = score;
                    }
                }

                // 保存測驗開始時間
                string startTime = form["testStartTime"];
                Session["DelayedRecallStartTime"] = startTime;

                int totalScore = scores.Values.Sum(); //保存計算總分
                Session["DelayedRecallAnswers"] = scores;
                Session["DelayedRecallTotalScore"] = totalScore;

                return RedirectToAction("CognitiveScreeningTotalScore");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"保存數據時發生錯誤：{ex.Message}");
                return View("DelayedRecall");
            }
        }
        #endregion

        #region 保存醫療團隊-認知篩檢-分數總合(6)
        [HttpPost]
        public ActionResult SaveCSTotalScore(FormCollection form)
        {
            try
            {
                // 收集各項分數
                int orientationScore = int.TryParse(form["OrientationScore"], out var oScore) ? oScore : 0;
                int immediateMemoryScore = int.TryParse(form["ImmediateMemoryScore"], out var imScore) ? imScore : 0;
                int concentrationScore = int.TryParse(form["ConcentrationScore"], out var cScore) ? cScore : 0;
                int delayedRecallScore = int.TryParse(form["DelayedRecallScore"], out var drScore) ? drScore : 0;

                // 計算總分
                int totalScore = orientationScore + immediateMemoryScore + concentrationScore + delayedRecallScore;

                // 保存到資料庫
                var totalScoreRecord = new CognitiveScreeningTotalScoreViewModel
                {
                    OrientationScore = orientationScore,
                    ImmediateMemoryScore = immediateMemoryScore,
                    ConcentrationScore = concentrationScore,
                    DelayedRecallScore = delayedRecallScore,
                    TotalScore = totalScore,
                    //RecordedDate = DateTime.Now // 假設需要保存記錄時間
                };

                Session["CognitiveScreeningTotalScores"] = totalScore;  //更新 Session

                return RedirectToAction("Main");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "保存分數總合時發生錯誤：" + ex.Message);
                return View("CognitiveScreeningTotalScore");
            }
        }
        #endregion
    }
}