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
            // 從 Session 中取得暫存的問卷答案
            var concussionData = Session["ConcussionScreeningData"] as QuestionnaireViewModel;
            var symptomData = Session["SymptomEvaluationData"] as QuestionnaireViewModel;

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

            ViewBag.ConcussionData = concussionData;
            ViewBag.SymptomData = symptomData;

            return View();
        }
        #endregion

        #region 第一部份問卷

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

        #region 開刀史
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
                OrderNumber = index + 1,  //自動遞增項次
                Question = q.Question
            }).ToList();

            return PartialView("CardiovascularScreening", viewModel);
        }
        #endregion

        #region 腦震盪篩檢-選手自填
        /// 第一部分 選手背景
        public ActionResult ConcussionScreening()
        {
            var questions = _db.ConcussionScreening.ToList();

            var viewModel = questions.Select((q, index) => new ConcussionScreeningViewModel
            {
                OrderNumber = index + 1,  //自動遞增項次
                Question = q.Question
            }).ToList();

            // 如果有暫存的數據，恢復填寫狀態
            var savedAnswers = Session["ConcussionScreeningAnswers"] as Dictionary<int, string>;
            if (savedAnswers != null)
            {
                foreach (var item in viewModel)
                {
                    if (savedAnswers.ContainsKey(item.OrderNumber))
                    {
                        item.Answer = savedAnswers[item.OrderNumber]; //恢復用戶填寫的答案
                    }
                }
            }

            return View("ConcussionScreening", viewModel);
        }

        /// 第二部分 症狀自我評估
        [HttpGet]
        public ActionResult SymptomEvaluation()
        {
            var questions = _db.SymptomEvaluation.ToList();

            var viewModel = questions.Select((q, index) => new ConcussionScreeningViewModel
            {
                OrderNumber = index + 1,  //自動遞增項次
                Question = q.SymptomItem
            }).ToList();

            // 如果有暫存的數據，恢復填寫狀態
            var savedAnswers = Session["SymptomEvaluationAnswers"] as Dictionary<int, string>;
            if (savedAnswers != null)
            {
                foreach (var item in viewModel)
                {
                    if (savedAnswers.ContainsKey(item.OrderNumber))
                    {
                        item.Answer = savedAnswers[item.OrderNumber]; //恢復用戶填寫的答案
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
                OrderNumber = index + 1,  //自動遞增項次
                Instructions = q.Instructions,
                ObservationPoints = q.ObservationPoints
            }).ToList();

            return View("OrthopaedicScreening", viewModel);
        }
        #endregion

        #region 腦震盪篩檢-防護員評估
        /// 認知篩檢-定位(1)
        public ActionResult CognitiveScreening()
        {
            try
            {
                string loggedInUserName = Session["UserName"] as string;
                //bool isAthleticTrainer = _db.Test_AthleticTrainer.Any(at => at.ATName == loggedInUserName && at.IsActive);
                bool isAthleticTrainer = _db.Test_AthleticTrainer.Where(at => at.ATName == loggedInUserName).Any(at => at.IsActive == true);

                var questions = _db.CognitiveScreening.ToList();

                var viewModel = questions.Select((q, index) => new CognitiveScreeningViewModel
                {
                    OrderNumber = index + 1,  // 自動遞增項次
                    Question = q.Question     // 顯示問題
                }).ToList();

                var savedAnswers = Session["OrientationAnswers"] as Dictionary<int, int>; //如果有暫存的數據，恢復填寫狀態
                if (savedAnswers != null)
                {
                    foreach (var item in viewModel)
                    {
                        if (savedAnswers.ContainsKey(item.OrderNumber))
                        {
                            item.OrientationScore = savedAnswers[item.OrderNumber];
                        }
                    }
                }

                return View("CognitiveScreening", viewModel);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        /// 認知篩檢-短期記憶(2)
        public ActionResult ImmediateMemory()
        {
            var questions = _db.ImmediateMemory.ToList();

            // 從 Session 中讀取已保存的數據
            var immediateMemoryAnswers = Session["ImmediateMemoryAnswers"] as Dictionary<string, int>;
            var completionTime = Session["ImmediateMemoryCompletionTime"] as string ?? "00:00";

            int totalScore = 0;// 計算短期記憶總分

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

        /// 認知篩檢-專注力(3)
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

        /// 協調與平衡測驗(4)
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

        /// 延遲記憶(5)
        public ActionResult DelayedRecall()
        {
            // 從資料庫中讀取問卷問題
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

            //// 將問題列表傳送到前端作為 ViewModel
            //var viewModel = questions.Select((q, index) => new DelayedRecallViewModel
            //{
            //    OrderNumber = index + 1,
            //    Word = q.Word

            //}).ToList();
            ViewBag.DelayedRecallStartTime = delayedRecallStartTime; //傳遞開始時間
            return View("DelayedRecall", viewModel);
            //return PartialView("_DelayedRecall", viewModel);
        }

        /// 認知篩檢分數總合(6)
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

            return View("CognitiveScreeningTotalScore", viewModel);
            //return PartialView("_CognitiveScreeningTotalScore", viewModel);
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

            if (trainer != null && VerifyPassword(password, trainer.ATNumber)) // 假設有密碼加密驗證方法 VerifyPassword
            {
                return Json(new { success = true });
            }

            return Json(new { success = false });
        }

        private bool VerifyPassword(string inputPassword, string storedPassword)
        {
            // 假設這是一個驗證密碼的邏輯，根據實際情況實作
            return inputPassword == storedPassword; // 實際應用中應進行加密驗證
        }

        #endregion

        #endregion

        #region 預覽頁
        public ActionResult Preview(QuestionnaireViewModel model)
        {
            return View("Preview", model); // 從表單收集的數據進行處理
        }

        [HttpPost]
        public ActionResult Preview(QuestionnaireViewModel model, FormCollection form)
        {
            try
            {
                //處理填寫者基本信息
                model.Specialist = form["specialist"];
                model.FillName = form["fillName"];
                model.AtheNum = form["atheNum"];
                model.Gender = int.Parse(form["gender"]);
                model.FillDate = DateTime.Parse(form["fillDate"]);

                //動態收集表單的所有數據到 FormData
                foreach (var key in form.AllKeys)
                {
                    //model.FormData[key] = form[key];
                    model.FormData[key] = string.IsNullOrEmpty(form[key]) ? "未回答" : form[key];
                }

                #region 過去健康檢查病史
                var pastHealthItems = _db.PastHealth.ToList();
                foreach (var item in pastHealthItems)
                {
                    var selectedValue = form[$"pastHealth_{item.ID}"];

                    if (selectedValue == "yes")
                    {
                        // 更新資料庫的 IsYes 和 IsNo 狀態
                        item.IsYes = true;
                        item.IsNo = false;

                        model.PastHealthHistory = "yes";
                        model.PastHealthDetails.Add(new PastHealthDetailViewModel
                        {
                            ItemId = item.ID,
                            ItemZh = item.ItemZh,
                            Item1 = string.IsNullOrEmpty(form[$"item1_{item.ID}"]) ? "未回答" : form[$"item1_{item.ID}"],
                            Item2 = string.IsNullOrEmpty(form[$"item2_{item.ID}"]) ? "未回答" : form[$"item2_{item.ID}"],
                            Item3 = string.IsNullOrEmpty(form[$"item3_{item.ID}"]) ? "未回答" : form[$"item3_{item.ID}"]
                        });
                    }
                    else if (selectedValue == "no")
                    {
                        // 更新資料庫的 IsYes 和 IsNo 狀態
                        item.IsYes = false;
                        item.IsNo = true;

                        model.PastHealthHistory = "no";
                        model.PastHealthDetails.Add(new PastHealthDetailViewModel
                        {
                            ItemId = item.ID,
                            ItemZh = "否",
                            Item1 = "無異常項目",
                            Item2 = "無異常項目",
                            Item3 = "無異常項目"
                        });
                    }
                    else
                    {
                        // 若未選擇「是」或「否」
                        item.IsYes = false;
                        item.IsNo = false;

                        model.PastHealthHistory = "未回答";
                        model.PastHealthDetails.Add(new PastHealthDetailViewModel
                        {
                            ItemId = item.ID,
                            ItemZh = "未回答",
                            Item1 = "未回答",
                            Item2 = "未回答",
                            Item3 = "未回答"
                        });
                    }

                    _db.Entry(item).State = EntityState.Modified; //將變更保存至資料庫
                }
                #endregion

                #region 過敏史
                var allergicItems = _db.AllergicHistory.ToList();
                foreach (var item in allergicItems)
                {
                    var selectedValue = form[$"allergy_{item.ID}"];

                    if (selectedValue == "yes")
                    {
                        item.IsYes = true;
                        item.IsNo = false;

                        string isAllergicKey = $"allergy_{item.ID}";
                        string descriptionKey = $"details_{item.ID}";

                        model.AllergicHistory = "yes";
                        model.AllergicHistoryDetails.Add(new AllergicHistoryDetailViewModel
                        {
                            ItemId = item.ID,
                            ItemZh = item.ItemZh,  //將過敏項目的中文名綁定
                            ItemEn = item.ItemEn,  //將過敏項目的英文名綁定
                            IsAllergic = string.IsNullOrEmpty(form[isAllergicKey]) ? "未回答" : form[isAllergicKey],
                            AllergyDescription = string.IsNullOrEmpty(form[descriptionKey]) ? "未回答" : form[descriptionKey]
                        });
                    }
                    else if (selectedValue == "no")
                    {
                        item.IsYes = false;
                        item.IsNo = true;

                        string isAllergicKey = $"allergy_{item.ID}";
                        string descriptionKey = $"details_{item.ID}";

                        model.AllergicHistory = "no";
                        model.AllergicHistoryDetails.Add(new AllergicHistoryDetailViewModel
                        {
                            ItemId = item.ID,
                            ItemZh = item.ItemZh,
                            //ItemEn = item.ItemEn,  //將過敏項目的英文名綁定
                            IsAllergic = "無異常項目",
                            AllergyDescription = "無異常項目",
                        });
                    }
                    else
                    {
                        // 若未選擇「是」或「否」
                        item.IsYes = false;
                        item.IsNo = false;

                        model.AllergicHistory = "未回答";
                        model.AllergicHistoryDetails.Add(new AllergicHistoryDetailViewModel
                        {
                            ItemId = item.ID,
                            ItemZh = item.ItemZh,
                            IsAllergic = "未回答",
                            AllergyDescription = "未回答"
                        });
                    }

                    _db.Entry(item).State = EntityState.Modified; //將變更保存至資料庫
                }
                #endregion

                #region 家族病史
                var familyHistoryItems = _db.FamilyHistory.ToList();
                foreach (var item in familyHistoryItems)
                {
                    var selectedValue = form[$"familyHistory_{item.ID}"];

                    var familyHistoryOption = "未回答"; //處理 "是"、"否"、"未知" 等選項

                    if (selectedValue == "yes")
                    {
                        item.IsYes = true;
                        item.IsNo = false;
                        item.IsUnknown = false;
                        familyHistoryOption = "yes";
                    }
                    else if (selectedValue == "no")
                    {
                        item.IsYes = false;
                        item.IsNo = true;
                        item.IsUnknown = false;
                        familyHistoryOption = "no";
                    }
                    else if (selectedValue == "unknown")
                    {
                        item.IsYes = false;
                        item.IsNo = false;
                        item.IsUnknown = true;
                        familyHistoryOption = "unknown";
                    }
                    else
                    {
                        item.IsYes = false;
                        item.IsNo = false;
                        item.IsUnknown = false;
                    }

                    // 判斷是否為「其他」項目並處理描述
                    string otherFamilyHistory = item.ID == 10 ? form["otherFamilyHistory"] ?? "未回答" : null;

                    // 將每個病史項目添加到模型的 FamilyHistoryDetails
                    model.FamilyHistoryDetails.Add(new FamilyHistoryViewModel
                    {
                        ItemId = item.ID,
                        GeneralPartsZh = item.GeneralPartsZh,
                        GeneralPartsEn = item.GeneralPartsEn,
                        FamilyHistoryOption = familyHistoryOption,
                        OtherFamilyHistory = otherFamilyHistory // 只有「其他」項目會有描述
                    });

                    // 將變更保存至資料庫
                    _db.Entry(item).State = EntityState.Modified;
                }
                #endregion

                #region 過去病史
                var pastHistoryItems = _db.PastHistory.ToList();
                foreach (var item in pastHistoryItems)
                {
                    var selectedValue = form[$"pastHistory_{item.ID}"];

                    var pastHistoryOption = "未回答";

                    if (selectedValue == "yes")
                    {
                        item.IsYes = true;
                        item.IsNo = false;
                        item.IsUnknown = false;
                        pastHistoryOption = "yes";
                    }
                    else if (selectedValue == "no")
                    {
                        item.IsYes = false;
                        item.IsNo = true;
                        item.IsUnknown = false;
                        pastHistoryOption = "no";
                    }
                    else if (selectedValue == "unknown")
                    {
                        item.IsYes = false;
                        item.IsNo = false;
                        item.IsUnknown = true;
                        pastHistoryOption = "unknown";
                    }
                    else
                    {
                        item.IsYes = false;
                        item.IsNo = false;
                        item.IsUnknown = false;
                    }

                    // 判斷是否為「其他」項目並處理描述
                    string otherPastHistory = item.ID == 10 ? form["PastHistoryOption"] ?? "未回答" : null;


                    model.PastHistoryDetails.Add(new PastHistoryDetailViewModel
                    {
                        ItemId = item.ID,
                        GeneralPartsZh = item.GeneralPartsZh,
                        GeneralPartsEn = item.GeneralPartsEn,
                        PastHistoryOption = pastHistoryOption,
                        OtherPastHistory = otherPastHistory,
                    });

                    _db.Entry(item).State = EntityState.Modified; //將變更保存至資料庫
                }
                #endregion

                #region 不使用-開刀史

                var surgeryItems = _db.SurgeryHistory.ToList();
                foreach (var item in surgeryItems)
                {
                    var selectedValue = form[$"surgeryHistory_{item.ID}"];

                    if (selectedValue == "yes")
                    {
                        item.IsYes = true;
                        item.IsNo = false;

                        string isSurgery_Key = $"surgery_{item.ID}";

                        model.SurgeryHistory = "yes";

                        model.SurgeryHistoryDetails.Add(new SurgeryHistoryDetailViewModel
                        {
                            ItemId = item.ID,
                            PartsOfBodyZh = item.PartsOfBodyZh,
                            PartsOfBodyEn = item.PartsOfBodyEn,
                            OperationOption = "是",
                        });
                    }
                    else if (selectedValue == "no")
                    {
                        item.IsYes = false;
                        item.IsNo = true;

                        string isSurgery_Key = $"surgery_{item.ID}";

                        model.SurgeryHistory = "no";

                        model.SurgeryHistoryDetails.Add(new SurgeryHistoryDetailViewModel
                        {
                            ItemId = item.ID,
                            PartsOfBodyZh = item.PartsOfBodyZh,
                            PartsOfBodyEn = item.PartsOfBodyEn,
                            OperationOption = "否",
                        });
                    }
                    else
                    {
                        item.IsYes = false;
                        item.IsNo = false;

                        model.SurgeryHistory = "未回答";

                        model.SurgeryHistoryDetails.Add(new SurgeryHistoryDetailViewModel
                        {
                            ItemId = item.ID,
                            PartsOfBodyZh = item.PartsOfBodyZh,
                            PartsOfBodyEn = item.PartsOfBodyEn,
                            OperationOption = "未回答"
                        });
                    }
                    _db.Entry(item).State = EntityState.Modified;
                }

                #endregion

                #region 現在病史
                var presentIllnessItems = _db.PresentIllness.ToList();
                foreach (var item in presentIllnessItems)
                {
                    var selectedValue = form[$"presentIllness_{item.ID}"];

                    if (selectedValue == "yes")
                    {
                        item.IsYes = true;
                        item.IsNo = false;

                        string isReceiving_Key = $"presentIllness_{item.ID}";

                        model.PresentIllness = "yes";

                        model.PresentIllnessDetails.Add(new PresentIllnessDetailViewModel
                        {
                            ItemId = item.ID,
                            PartsOfBodyZh = item.PartsOfBodyZh,
                            PartsOfBodyEn = item.PartsOfBodyEn,
                            ReceivingOtherTherapies = "是"
                        });
                    }
                    else if (selectedValue == "no")
                    {
                        item.IsYes = false;
                        item.IsNo = true;

                        string isReceiving_Key = $"presentIllness_{item.ID}";

                        model.PresentIllness = "no";

                        model.PresentIllnessDetails.Add(new PresentIllnessDetailViewModel
                        {
                            ItemId = item.ID,
                            PartsOfBodyZh = item.PartsOfBodyZh,
                            PartsOfBodyEn = item.PartsOfBodyEn,
                            ReceivingOtherTherapies = "否"
                        });
                    }
                    else
                    {
                        item.IsYes = false;
                        item.IsNo = true;

                        model.PresentIllness = "未回答";

                        model.PresentIllnessDetails.Add(new PresentIllnessDetailViewModel
                        {
                            ItemId = item.ID,
                            PartsOfBodyZh = item.PartsOfBodyZh,
                            PartsOfBodyEn = item.PartsOfBodyEn,
                            ReceivingOtherTherapies = "未回答"
                        });
                    }
                    _db.Entry(item).State = EntityState.Modified;
                }
                #endregion

                #region 藥物史
                model.PastDrugsDetails = new List<PastDrugsDetailViewModel>(); //初始化 PastDrugsDetails 列表
                var pastDrugsItems = _db.PastDrugs.ToList();
                string otherDrugsDescription = form["otherDrugsDetail"]; // 取得 "其他" 的描述
                bool isOtherChecked = form["usedDrugs_other"] == "on"; //檢查「其他」選項是否勾選

                // 如果「其他」選項勾選但未填寫描述
                if (isOtherChecked && string.IsNullOrEmpty(otherDrugsDescription))
                {
                    ModelState.AddModelError("otherDrugsDetail", "您勾選了「其他」選項，請填寫描述內容。");

                    //return View("Main", model); // 返回主頁並顯示錯誤訊息，待修正
                }

                foreach (var item in pastDrugsItems)
                {
                    string checkboxName = $"usedDrugs_{item.ID}";
                    bool isChecked = form[checkboxName] == "on";

                    // 如果勾選，則將此項目添加到模型的 PastDrugsDetails 列表中
                    if (isChecked || (item.ItemZh == "其他" && isOtherChecked))
                    {
                        model.PastDrugsDetails.Add(new PastDrugsDetailViewModel
                        {
                            ItemId = item.ID,
                            ItemZh = item.ItemZh,
                            ItemEn = item.ItemEn,
                            IsUsed = "yes",
                            OtherDrugs = item.ItemZh == "其他" ? otherDrugsDescription : null
                        });
                    }
                    //_db.Entry(item).State = EntityState.Modified;
                }
                model.TUE = form["TUE"] == "yes" ? "是" : "否";
                #endregion

                #region 營養品
                var supplementsItems = _db.PastSupplements.ToList();

                string otherSupplementDescription = form["otherSupplementDetail"]; // 取得 "其他" 的描述
                bool isOthersChecked = form["usedSupplement_other"] == "on"; //檢查「其他」選項是否勾選 如果「其他」選項勾選但未填寫描述

                if (isOthersChecked && string.IsNullOrEmpty(otherSupplementDescription))
                {
                    ModelState.AddModelError("otherSupplementDetail", "您勾選了「其他」選項，請填寫描述內容。");
                }

                foreach (var item in supplementsItems)
                {
                    string usedKey = $"usedSupplements_{item.ID}";
                    bool isChecked = form[usedKey] == "on";

                    if (isChecked || (item.ItemZh == "其他" && isOthersChecked))
                    {
                        model.PastSupplementsDetails.Add(new PastSupplementsDetailViewModel
                        {
                            ItemId = item.ID,
                            ItemZh = item.ItemZh,
                            ItemEn = item.ItemEn,
                            IsUsed = "yes",
                            OtherSupplements = item.ItemZh == "其他" ? otherSupplementDescription : null
                        });
                    }
                }
                #endregion

                #region 女性問卷
                if (model.Gender == 2) // 女性
                {
                    var femaleQuestionnaireItems = _db.FemaleQuestionnaire.ToList();
                    foreach (var item in femaleQuestionnaireItems)
                    {
                        string answerKey = $"femaleQuestion_{item.ID}";
                        string answerValue = form[answerKey]; // 收集回答值

                        // 為每個問題項目設定選項對應的中英文描述
                        var answerOptions = new Dictionary<string, string>
                {
                    { "10以下", "10歲 (含) 以下" },
                    { "11", "11歲" },
                    { "12", "12歲" },
                    { "13", "13歲" },
                    { "14", "14歲" },
                    { "15", "15歲" },
                    { "16以上", "16歲 (含) 以上" },
                    { "yes", "是 Yes" },
                    { "no", "否 No" },
                    { "noCycle", "目前無生理期" }
                };

                        model.FemaleQuestionnaireDetails.Add(new FemaleQuestionnaireDetailViewModel
                        {
                            ID = item.ID,
                            QuestionZh = item.QuestionZh,
                            QuestionEn = item.QuestionEn,
                            Answer = answerValue, // 將答案傳遞給 Answer 屬性
                            AnswerOptions = answerOptions // 設定中英文對照選項
                        });
                    }
                }
                #endregion

                #region 過去傷害狀況(已復原)
                // 初始化傷害部位列表
                model.PastInjuryDetails = new List<PastInjuryStatuSViewModel>();

                // 收集傷害部位數據
                var pastInjuryItems = _db.PastInjuryStatus.ToList();
                foreach (var item in pastInjuryItems)
                {
                    string leftPartKey = $"pastinjuryLeft_{item.Id}";
                    string rightPartKey = $"pastinjuryRight_{item.Id}";

                    // 判斷是否有受傷部位選擇
                    bool hasInjury = form[leftPartKey] != null || form[rightPartKey] != null;

                    if (hasInjury)
                    {
                        // 初始化傷勢類型列表
                        var injuryTypes = new List<string>();

                        // 收集傷勢類型數據
                        string muscleTendon = form["MuscleTendon"]; // 肌肉/肌腱類
                        string bone = form["Bone"]; // 骨類
                        string ligament = form["Ligament"]; // 韌帶類
                        string nerve = form["Nerve"]; // 神經類
                        string cartilageSynoviumBursa = form["CartilageSynoviumBursa"]; // 軟骨/滑膜/滑囊類
                        string epidermalTissue = form["EpidermalTissue"]; // 表皮組織
                        string bloodVessel = form["BloodVessel"]; // 血管類
                        string organLimb = form["OrganLimb"]; // 器官/四肢類

                        // 添加用戶選擇的傷勢類型到列表中
                        if (!string.IsNullOrEmpty(muscleTendon)) injuryTypes.Add(muscleTendon);
                        if (!string.IsNullOrEmpty(bone)) injuryTypes.Add(bone);
                        if (!string.IsNullOrEmpty(ligament)) injuryTypes.Add(ligament);
                        if (!string.IsNullOrEmpty(nerve)) injuryTypes.Add(nerve);
                        if (!string.IsNullOrEmpty(cartilageSynoviumBursa)) injuryTypes.Add(cartilageSynoviumBursa);
                        if (!string.IsNullOrEmpty(epidermalTissue)) injuryTypes.Add(epidermalTissue);
                        if (!string.IsNullOrEmpty(bloodVessel)) injuryTypes.Add(bloodVessel);
                        if (!string.IsNullOrEmpty(organLimb)) injuryTypes.Add(organLimb);

                        // 添加到模型
                        model.PastInjuryDetails.Add(new PastInjuryStatuSViewModel
                        {
                            PastInjuryPart = item.InjuryPart, // 受傷部位
                            LeftSide = form[leftPartKey] != null, // 是否左側受傷
                            RightSide = form[rightPartKey] != null, // 是否右側受傷
                            InjuryTypes = injuryTypes // 傷勢類型列表
                        });
                    }
                }

                #endregion

                #region 過去傷害狀況(已復原)-治療方式

                var PasttreatmentMethods = _db.PastTreatmentMethod.ToList();
                foreach (var method in PasttreatmentMethods)
                {
                    string methodKey = $"Pasttreatment_{method.Id}";
                    if (form[methodKey] != null)
                    {
                        model.PastTreatmentDetails.Add(new PastTreatmentMethoDViewModel
                        {
                            Method = method.Method
                        });
                    }
                }
                #endregion

                #region 目前傷害狀況
                var injuryItems = _db.InjuryStatus.ToList();
                foreach (var item in injuryItems)
                {
                    string leftPartKey = $"NowinjuryLeft_{item.Id}";
                    string rightPartKey = $"NowinjuryRight_{item.Id}";
                    bool hasInjury = form[leftPartKey] != null || form[rightPartKey] != null;

                    if (hasInjury)
                    {
                        // 初始化傷勢類型列表
                        var injuryTypes = new List<string>();

                        // 收集傷勢類型數據
                        string muscleTendon = form["NowmuscleTendon"]; // 肌肉/肌腱類
                        string bone = form["Nowbone"]; // 骨類
                        string ligament = form["Nowligament"]; // 韌帶類
                        string nerve = form["Nownerve"]; // 神經類
                        string cartilageSynoviumBursa = form["NowcartilageSynoviumBursa"]; // 軟骨/滑膜/滑囊類
                        string epidermalTissue = form["NowepidermalTissue"]; // 表皮組織
                        string bloodVessel = form["NowbloodVessel"]; // 血管類
                        string organLimb = form["NoworganLimb"]; // 器官/四肢類

                        // 添加用戶選擇的傷勢類型到列表中
                        if (!string.IsNullOrEmpty(muscleTendon)) injuryTypes.Add(muscleTendon);
                        if (!string.IsNullOrEmpty(bone)) injuryTypes.Add(bone);
                        if (!string.IsNullOrEmpty(ligament)) injuryTypes.Add(ligament);
                        if (!string.IsNullOrEmpty(nerve)) injuryTypes.Add(nerve);
                        if (!string.IsNullOrEmpty(cartilageSynoviumBursa)) injuryTypes.Add(cartilageSynoviumBursa);
                        if (!string.IsNullOrEmpty(epidermalTissue)) injuryTypes.Add(epidermalTissue);
                        if (!string.IsNullOrEmpty(bloodVessel)) injuryTypes.Add(bloodVessel);
                        if (!string.IsNullOrEmpty(organLimb)) injuryTypes.Add(organLimb);

                        // 添加到模型
                        model.NowInjuryDetails.Add(new InjuryStatuSViewModel
                        {
                            InjuryPart = item.InjuryPart, // 受傷部位
                            LeftSide = form[leftPartKey] != null, // 是否左側受傷
                            RightSide = form[rightPartKey] != null, // 是否右側受傷
                            InjuryTypes = injuryTypes // 傷勢類型列表
                        });
                    }
                }
                #endregion

                #region 目前治療方式
                var treatmentMethods = _db.TreatmentMethod.ToList();
                foreach (var method in treatmentMethods)
                {
                    string methodKey = $"Nowtreatment_{method.Id}";
                    if (form[methodKey] != null)
                    {
                        model.NowTreatmentDetails.Add(new TreatmentMethoDViewModel
                        {
                            Method = method.Method
                        });
                    }
                }
                #endregion

                #region 心血管篩檢
                var cardiovascularScreenings = _db.CardiovascularScreening.ToList();
                foreach (var item in cardiovascularScreenings)
                {
                    string answerKey = $"question_{item.Id}";
                    var answer = form[answerKey];

                    // 判斷選項並設定顯示文字
                    string displayAnswer = answer == "yes" ? "是 Yes" : "否 No";

                    model.CardiovascularScreeningDetails.Add(new CardiovascularScreeningDetailViewModel
                    {
                        OrderNumber = item.Id,   // 使用資料庫的項次
                        Question = item.Question,
                        Answer = displayAnswer // 將轉換後的答案傳遞給 Answer 屬性
                    });
                }
                #endregion

                #region 腦震盪篩檢-選手自填

                var concussionAnswers = Session["ConcussionScreeningAnswers"] as Dictionary<int, string>;
                if (concussionAnswers != null)
                {
                    var questions = _db.ConcussionScreening.ToList();

                    foreach (var q in questions)
                    {
                        if (concussionAnswers.ContainsKey(q.Id))
                        {
                            model.ConcussionScreeningDetails.Add(new ConcussionScreeningViewModel
                            {
                                OrderNumber = q.Id,
                                Question = q.Question,
                                Answer = concussionAnswers[q.Id]
                            });
                        }
                    }

                    // 取出藥物與備註
                    model.ConcussionScreeningMedicationAnswer = Session["ConcussionScreeningMedicationAnswer"] as string;
                    model.ConcussionScreeningMedicationDetails = Session["ConcussionScreeningMedicationDetails"] as string;
                    model.ConcussionScreeningNotes = Session["ConcussionScreeningNotes"] as string;
                }

                // 症狀自我評估
                var symptomAnswers = Session["SymptomEvaluationAnswers"] as Dictionary<int, int>;
                if (symptomAnswers != null)
                {
                    var symptomQuestions = _db.SymptomEvaluation.ToList();

                    foreach (var q in symptomQuestions)
                    {
                        if (symptomAnswers.ContainsKey(q.ID))
                        {
                            model.SymptomEvaluationDetails.Add(new SymptomEvaluationViewModel
                            {
                                OrderNumber = q.ID,
                                SymptomItem = q.SymptomItem,
                                Score = symptomAnswers[q.ID]
                            });
                        }
                    }
                }

                // 暂存到 Session（防止數據遺失）
                Session["PreviewData"] = model;

                #endregion

                #region 醫療團隊評估-認知篩檢-定位(1)
                //var CognitiveScreeningAnswer = Session["CognitiveScreeningAnswers"] as Dictionary<int, int>;
                var CognitiveScreeningAnswer = Session["CognitiveScreeningAnswers"] as Dictionary<int, int> ?? new Dictionary<int, int>();

                if (CognitiveScreeningAnswer != null)
                {
                    var orientationQuestions = _db.CognitiveScreening.ToList();

                    foreach (var q in orientationQuestions)
                    {
                        if (CognitiveScreeningAnswer.ContainsKey(q.ID))
                        {
                            model.CognitiveScreeningDetails.Add(new CognitiveScreeningViewModel
                            {
                                OrderNumber = q.ID,
                                Question = q.Question,
                                OrientationScore = CognitiveScreeningAnswer[q.ID]
                            });
                        }
                    }
                }
                // 計算定位總分
                model.CognitiveScreeningTotalScore = CognitiveScreeningAnswer.Values.Sum();

                Session["PreviewData"] = model;

                #endregion

                #region 醫療團隊評估-認知篩檢-短期記憶(2)
                var immediateMemoryAnswers = Session["ImmediateMemoryAnswers"] as Dictionary<string, int>;
                var immediateMemoryQuestions = _db.ImmediateMemory.ToList();

                foreach (var q in immediateMemoryQuestions)
                {
                    int firstScore = 0, secondScore = 0, thirdScore = 0;

                    // 使用 TryGetValue 檢查並獲取分數
                    immediateMemoryAnswers?.TryGetValue($"first_{q.ID}", out firstScore);
                    immediateMemoryAnswers?.TryGetValue($"second_{q.ID}", out secondScore);
                    immediateMemoryAnswers?.TryGetValue($"third_{q.ID}", out thirdScore);

                    model.ImmediateMemoryDetails.Add(new ImmediateMemoryViewModel
                    {
                        OrderNumber = q.ID,
                        Word = q.Word,
                        FirstTestScore = firstScore,
                        SecondTestScore = secondScore,
                        ThirdTestScore = thirdScore,
                    });
                }

                // 計算短期記憶總分
                if (immediateMemoryAnswers != null)
                {
                    model.ImmediateMemoryTotalScore = immediateMemoryAnswers.Values.Sum();
                }
                model.CompletionTime = Session["ImmediateMemoryCompletionTime"] as string ?? "00:00";

                #endregion

                #region 醫療團隊評估-認知篩檢-專注力(3)
                var concentrationAnswers = Session["ConcentrationScores"] as Dictionary<int, int>;
                var concentrationQuestions = _db.Concentration.ToList();

                foreach (var q in concentrationQuestions)
                {
                    int score = 0;

                    // 使用 TryGetValue 檢查並獲取分數
                    concentrationAnswers?.TryGetValue(q.Id, out score);

                    model.ConcentrationDetails.Add(new ConcentrationViewModel
                    {
                        OrderNumber = q.Id,
                        ListA = q.ListA,
                        ListB = q.ListB,
                        ListC = q.ListC,
                        Score = score
                    });
                }

                // 計算專注力總分
                if (concentrationAnswers != null)
                {
                    model.ConcentrationTotalScore = concentrationAnswers.Values.Sum();
                }
                #endregion

                #region 醫療團隊評估-認知篩檢-協調與平衡測驗(4)
                var coordinationData = Session["CoordinationAndBalanceData"] as CoordinationAndBalanceExaminationViewModel;
                if (coordinationData != null)
                {
                    model.CoordinationAndBalanceDetails.Add(coordinationData);

                    // 計算總錯誤次數、平均時間和最快時間
                    model.CoordinationAndBalanceTotalErrors = coordinationData.DoubleLegError
                                                            + coordinationData.TandemError
                                                            + coordinationData.SingleLegError;

                    model.CoordinationAndBalanceAverageTime = (coordinationData.FirstTime
                                                            + coordinationData.SecondTime
                                                            + coordinationData.ThirdTime) / 3;

                    model.CoordinationAndBalanceFastestTime = new[] { coordinationData.FirstTime,
                                              coordinationData.SecondTime,
                                              coordinationData.ThirdTime }
                                                                      .Where(t => t > 0)
                                                                      .DefaultIfEmpty(0)
                                                                      .Min();
                }
                #endregion

                #region 醫療團隊評估-認知篩檢-延遲記憶(5)
                var delayedrecallAnswers = Session["DelayedRecallAnswers"] as Dictionary<int, int>;

                var delayedrecallQuestions = _db.DelayedRecall.ToList();

                var delayedRecallStartTime = Session["DelayedRecallStartTime"] as string;
                model.DelayedRecallStartTime = delayedRecallStartTime; // 傳遞開始時間

                foreach (var q in delayedrecallQuestions) 
                {
                    int Score = 0;

                    delayedrecallAnswers?.TryGetValue(q.ID, out Score);

                    model.DelayedRecallDetails.Add(new DelayedRecallViewModel
                    { 
                        OrderNumber = q.ID,
                        Word = q.Word,
                        Score = Score,
                    });
                }

                model.DelayedRecallTotalScore = delayedrecallAnswers?.Values.Sum() ?? 0;
                #endregion

                #region 醫療團隊評估-認知篩檢-分數總合(6)

                var orientationScore = Session["OrientationScore"] as int? ?? 0;
                var immediateMemoryScore = Session["ImmediateMemoryScore"] as int? ?? 0;
                var concentrationScore = Session["ConcentrationScore"] as int? ?? 0;
                var delayedRecallScore = Session["DelayedRecallScore"] as int? ?? 0;

                // 計算總分
                model.CognitiveScreeningTotalScores = orientationScore + immediateMemoryScore + concentrationScore + delayedRecallScore;

                #endregion

                #region 骨科篩檢
                var orthopaedicScreenings = _db.OrthopaedicScreening.ToList();
                foreach (var item in orthopaedicScreenings)
                {
                    string resultKey = $"result_{item.Id}";
                    var result = form[resultKey];

                    // 判斷選項並設定顯示文字
                    string displayAnswer = result == "normal" ? "正常(Normal)" : "異常(Abnormal)";

                    model.OrthopaedicScreeningDetails.Add(new OrthopaedicScreeninGViewModel
                    {
                        OrderNumber = item.Id,
                        Instructions = item.Instructions,
                        ObservationPoints = item.ObservationPoints,
                        Result = displayAnswer
                    });
                }
                #endregion

                _db.SaveChanges(); // 儲存所有的變更

                return View("Preview", model);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "發生錯誤：" + ex.Message);
                return View("Main", model);
            }
        }
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

        #region 保存腦震盪篩檢-選手自填(選手背景)
        [HttpPost]
        public ActionResult SaveConcussionScreening(FormCollection form)
        {
            try
            {
                var answers = new Dictionary<int, string>(); //蒐集問卷答案

                foreach (var key in form.AllKeys)
                {
                    if (key.StartsWith("question_"))
                    {
                        int orderNumber = int.Parse(key.Substring("question_".Length));
                        string answer = form[key];
                        answers[orderNumber] = answer;
                    }
                }

                Session["ConcussionScreeningAnswers"] = answers; //保存到 Session

                //保存藥物與備註
                Session["ConcussionScreeningMedicationAnswer"] = form["medication"];
                Session["ConcussionScreeningMedicationDetails"] = form["medicationDetails"];
                Session["ConcussionScreeningNotes"] = form["notes"];

                return RedirectToAction("SymptomEvaluation"); //重定向到下一個問卷頁面
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion

        #region 保存腦震盪篩檢-選手症狀自我評估
        [HttpPost]
        public ActionResult SaveSymptomEvaluation(FormCollection form)
        {
            try
            {
                var answers = new Dictionary<int, int>(); //蒐集問卷答案

                foreach (var key in form.AllKeys)
                {
                    if (key.StartsWith("score_"))
                    {
                        int orderNumber = int.Parse(key.Substring("score_".Length));
                        int score = int.Parse(form[key]);
                        answers[orderNumber] = score;
                    }
                }

                Session["SymptomEvaluationAnswers"] = answers; //保存到 Session

                return RedirectToAction("Main");
            }
            catch (Exception ex)
            {
                throw ex;
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
                throw ex;
            }
        }
        #endregion

        #region 保存醫療團隊-認知篩檢-短期記憶(2)
        [HttpPost]
        public ActionResult SaveImmediateMemory(FormCollection form)
        {
            try
            {
                string action = form["action"]; //檢查是哪個按鈕觸發的
                if (action == "Previous")
                {
                    // 不保存數據，直接返回定位頁面
                    return RedirectToAction("CognitiveScreening");
                }

                var answers = new Dictionary<string, int>();

                foreach (var key in form.AllKeys)
                {
                    if (key.StartsWith("first_") || key.StartsWith("second_") || key.StartsWith("third_"))
                    {
                        string[] parts = key.Split('_');
                        string questionKey = $"{parts[0]}_{parts[1]}"; // 保存區分次數的鍵
                        int answer = int.Parse(form[key]);
                        answers[questionKey] = answer;
                    }
                }

                // 保存完成時間
                string completionTime = form["CompletionTime"];
                Session["ImmediateMemoryCompletionTime"] = completionTime;

                // 保存數據到 Session
                Session["ImmediateMemoryAnswers"] = answers;

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
                string action = form["action"]; //檢查是哪個按鈕觸發的
                if (action == "Previous")
                {
                    // 不保存數據，直接返回定位頁面
                    return RedirectToAction("CognitiveScreening");
                }

                var scores = new Dictionary<int, int>();

                foreach (var key in form.AllKeys)
                {
                    if (key.StartsWith("response_"))
                    {
                        int orderNumber = int.Parse(key.Split('_')[1]);
                        int score = int.Parse(form[key]);
                        scores[orderNumber] = score;
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