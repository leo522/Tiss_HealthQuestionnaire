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
            return View("PastHealth", pastHealthItems);
            //return PartialView("_PastHealth", pastHealthItems);
        }
        #endregion

        #region 過敏史
        public ActionResult AllergicHistory()
        {
            var allergicHistoryItems = _db.AllergicHistory.ToList();
            return View("AllergicHistory", allergicHistoryItems);
            //return PartialView("_AllergicHistory", allergicHistoryItems);
        }
        #endregion

        #region 家族病史
        public ActionResult FamilyHistory()
        {
            var familyHistory = _db.FamilyHistory.ToList();
            return View("FamilyHistory", familyHistory);
            //return PartialView("_FamilyHistory", familyHistory);
        }
        #endregion

        #region 過去病史Past history
        public ActionResult PastHistory()
        {
            var pastHistory = _db.PastHistory.ToList();
            return View("PastHistory", pastHistory);
            //return PartialView("_PastHistory", pastHistory);
        }
        #endregion

        #region 開刀史
        public ActionResult SurgeryHistory()
        {
            var surgeryHistory = _db.SurgeryHistory.ToList();
            return View("SurgeryHistory", surgeryHistory);
            //return PartialView("_SurgeryHistory", surgeryHistory);
        }
        #endregion

        #region 現在病史
        public ActionResult PresentIllness()
        {
            var presentIllness = _db.PresentIllness.ToList();
            return View("PresentIllness", presentIllness);
            //return PartialView("_PresentIllness", presentIllness);
        }
        #endregion

        #region 藥物史
        public ActionResult PastDrugs()
        {
            var pastDrugs = _db.PastDrugs.Where(drug => drug.ID != 13).ToList();

            return View("PastDrugs", pastDrugs);
            //return PartialView("_PastDrugs", pastDrugs);
        }
        #endregion

        #region 營養品
        public ActionResult PastSupplements()
        {
            var pastSupplements = _db.PastSupplements.ToList();
            return View("PastSupplements", pastSupplements);
            //return PartialView("_PastSupplements", pastSupplements);
        }
        #endregion

        #region 女性問卷
        public ActionResult FemaleQuestionnaire()
        {
            var femaleQuestionnaire = _db.FemaleQuestionnaire.ToList();
            return View("FemaleQuestionnaire", femaleQuestionnaire);
            //return PartialView("_FemaleQuestionnaire", femaleQuestionnaire);
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
                //return PartialView("_PastInjury", PastInjuryList);
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
            //return PartialView("_PastInjuryType", PastInjuryType);
        }

        public ActionResult PastTreatmentMethod() //過去治療方式
        {
            var PastTreatmentMethod = _db.PastTreatmentMethod.ToList();
            return View("PastTreatmentMethod", PastTreatmentMethod);
            //return PartialView("_PastTreatmentMethod", PastTreatmentMethod);
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
                return View("NowInjuryRestored", injuryList);
                //return PartialView("_NowInjuryRestored", injuryList);
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
            //return PartialView("_NowInjuryType", InjuryType);
        }

        public ActionResult NowTreatmentMethod() //目前治療方式
        {
            var treatmentMethod = _db.TreatmentMethod.ToList();
            return View("NowTreatmentMethod", treatmentMethod);
            //return PartialView("_NowTreatmentMethod", treatmentMethod);
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

            return PartialView("CardiovascularScreening", viewModel);
            //return PartialView("_CardiovascularScreening", viewModel);
        }
        #endregion

        #region 腦震盪篩檢-選手自填
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

            return View("ConcussionScreening", viewModel);
            //return PartialView("_ConcussionScreening", viewModel);
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

            return View("SymptomEvaluation", viewModel);
            //return PartialView("_SymptomEvaluation", viewModel);
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

            return View("OrthopaedicScreening", viewModel);
            //return PartialView("_OrthopaedicScreening", viewModel);
        }
        #endregion

        #region 腦震盪篩檢-防護員評估
        /// <summary>
        /// 第三部分 認知篩檢-定位
        /// </summary>
        /// <returns></returns>

        public ActionResult CognitiveScreening()
        {
            string loggedInUserName = Session["UserName"] as string;
            bool isAthleticTrainer = _db.Test_AthleticTrainer.Any(at => at.ATName == loggedInUserName && at.IsActive);

            // 從資料庫中讀取問卷問題
            var questions = _db.CognitiveScreening.ToList();

            // 為每個問題手動生成項次並轉換成 ViewModel
            var viewModel = questions.Select((q, index) => new CognitiveScreeningViewModel
            {
                OrderNumber = index + 1,  // 自動遞增項次
                Question = q.Question     // 顯示問題
            }).ToList();

            return View("CognitiveScreening", viewModel);
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
            var viewModel = questions.Select((q, index) => new ImmediateMemoryViewModel
            {
                OrderNumber = index + 1,  // 自動遞增項次
                Word = q.Word,             // 顯示的詞彙
                CompletionTime = "00:00"   // 確保為合法的分:秒格式
            }).ToList();

            return View("ImmediateMemory", viewModel);
        }

        /// <summary>
        /// 第三部分 認知篩檢-專注力
        /// </summary>
        /// <returns></returns>
        public ActionResult Concentration()
        {
            // 模擬數據或從資料庫中獲取資料
            var viewModel = new List<ConcentrationViewModel>
            {
                new ConcentrationViewModel { OrderNumber = 1, ListA = "4-9-3", ListB = "5-2-6", ListC = "1-4-2" },
                new ConcentrationViewModel { OrderNumber = 2, ListA = "6-2-9", ListB = "4-1-5", ListC = "6-5-8" },
                new ConcentrationViewModel { OrderNumber = 3, ListA = "3-8-1-4", ListB = "1-7-9-5", ListC = "6-8-3-1" },
                new ConcentrationViewModel { OrderNumber = 4, ListA = "3-2-7-9", ListB = "4-9-6-8", ListC = "3-4-8-1" }
            };

            return View("Concentration", viewModel);
            //return PartialView("_Concentration", viewModel);
        }

        /// <summary>
        /// 協調與平衡測驗
        /// </summary>
        /// <returns></returns>
        public ActionResult CoordinationAndBalanceExamination()
        {
            var model = new CoordinationAndBalanceExamination
            {
                TestSurface = "", // 確保初始為空
                Footwear = ""     // 確保初始為空
            };
            return View(model);
            //return PartialView("_CoordinationAndBalanceExamination");
        }

        /// <summary>
        /// 延遲記憶
        /// </summary>
        /// <returns></returns>
        public ActionResult DelayedRecall()
        {
            // 從資料庫中讀取問卷問題
            var questions = _db.DelayedRecall.ToList();

            // 將問題列表傳送到前端作為 ViewModel
            var viewModel = questions.Select((q, index) => new DelayedRecallViewModel
            {
                OrderNumber = index + 1,
                Word = q.Word

            }).ToList();

            return View("DelayedRecall", viewModel);
            //return PartialView("_DelayedRecall", viewModel);
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

        #region 問卷填完預覽頁
        public ActionResult Preview(QuestionnaireViewModel model)
        {
            return View("Preview", model); // 從表單收集的數據進行處理
            //return View("_PreviewTotal", model); // 從表單收集的數據進行處理
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

                    // 將變更保存至資料庫
                    _db.Entry(item).State = EntityState.Modified;
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

                    // 將變更保存至資料庫
                    _db.Entry(item).State = EntityState.Modified;
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

                #region 腦震盪篩檢(選手自填)
                var concussionScreenings = _db.CognitiveScreening.ToList();
                foreach (var item in concussionScreenings)
                {
                    string answerKey = $"question_{item.ID}";
                    var answer = form[answerKey];

                    if (!string.IsNullOrEmpty(answer))
                    {
                        model.ConcussionScreeningDetails.Add(new ConcussionScreeningViewModel
                        {
                            OrderNumber = item.ID,
                            Question = item.Question,
                            Answer = answer
                        });
                    }
                }

                // 收集是否正在服用藥物的數據
                var medicationAnswer = form["medication"];
                if (!string.IsNullOrEmpty(medicationAnswer))
                {
                    model.MedicationAnswer = medicationAnswer;
                    model.MedicationDetails = form["medicationDetails"];
                }

                // 收集備註
                model.Notes = form["notes"];
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
    }
}