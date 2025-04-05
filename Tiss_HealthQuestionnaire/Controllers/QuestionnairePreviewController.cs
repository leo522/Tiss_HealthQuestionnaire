using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using static Tiss_HealthQuestionnaire.Models.QuestionnaireViewModel;
using Tiss_HealthQuestionnaire.Models;

namespace Tiss_HealthQuestionnaire.Controllers
{
    public class QuestionnairePreviewController : Controller
    {
        private HealthQuestionnaireEntities _db = new HealthQuestionnaireEntities();

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
                ProcessBasicInfo(model, form);
                ProcessPastHealth(model, form);
                ProcessAllergicHistory(model, form);
                ProcessFamilyHistory(model, form);
                ProcessPastHistory(model, form);
                ProcessPresentIllness(model, form);
                ProcessPastDrugs(model, form);
                ProcessPastSupplements(model, form);
                ProcessFemaleQuestionnaire(model, form);
                ProcessPastInjuryStatus(model, form);
                ProcessPastInjuryParts(model, form);
                ProcessPastTreatmentMethod(model, form);
                ProcessCurrentInjuryStatus(model, form);
                ProcessCurrentInjuryParts(model, form);
                ProcessCurrentTreatmentMethod(model, form);
                ProcessCardiovascularScreening(model, form);
                ProcessConcussionScreening(model, form);
                ProcessSymptomEvaluation(model, form);
                //ProcessCognitiveScreening(model, form); //認知篩檢
                ProcessOrthopaedicScreening(model, form);

                return View("Preview", model);
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
                        ReceivingTherapy = therapyValue
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

            model.TUE = form["TUE"] ?? "no";
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
                return;
            }

            foreach (var item in model.FemaleQuestionnaireItems)
            {
                string key = $"FemaleQuestionnaireAnswers[{item.ID}]";
                string answer = form[key];

                if (string.IsNullOrEmpty(answer))
                {
                    item.Answer = "未回答";
                }
                else
                {
                    switch (answer)
                    {
                        case "yes":
                            item.Answer = "是";
                            break;
                        case "no":
                            item.Answer = "否";
                            break;
                        case "none":
                            item.Answer = "目前無生理週期";
                            break;
                        default:
                            item.Answer = answer;
                            break;
                    }
                }
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
            model.PastInjuryItems = new List<QuestionnaireViewModel.PastInjuryStatusViewModel>();

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

            var selectedInjuryTypes = form.GetValues("SelectedCurrentInjuryTypes");

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
            model.CurrentInjuryItems = new List<QuestionnaireViewModel.CurrentInjuryStatusViewModel>();

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
            var selectedTreatmentMethods = form.GetValues("SelectedCurrentTreatmentMethods");

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
            model.ConcussionScreeningItems = new List<ConcussionScreeningViewModel>();

            int index = 0;
            while (form[$"ConcussionScreeningItems[{index}].Id"] != null)
            {
                int id = int.Parse(form[$"ConcussionScreeningItems[{index}].Id"]);
                string question = _db.ConcussionScreening.Where(c => c.Id == id).Select(c => c.Question).FirstOrDefault();
                string answerKey = $"ConcussionQuestion_{id}";
                string rawAnswer = form[answerKey] ?? "no";
                string answer = rawAnswer == "yes" ? "是" : "否";

                model.ConcussionScreeningItems.Add(new ConcussionScreeningViewModel
                {
                    Id = id,
                    Question = question,
                    Answer = answer
                });

                index++;
            }

            model.ConcussionScreeningMedicationAnswer = form["Medication"] == "yes" ? "是" : "否";
            model.ConcussionScreeningMedicationDetails = form["MedicationDetails"]?.Trim() ?? "無";
            model.ConcussionScreeningNotes = form["Notes"]?.Trim() ?? "無";
        }
        #endregion

        #region 症狀自我評估-選手自填-(2)
        private void ProcessSymptomEvaluation(QuestionnaireViewModel model, FormCollection form)
        {
            model.SymptomEvaluationItems = new List<SymptomEvaluationViewModel>();

            int index = 0;
            while (form[$"SymptomEvaluationItems[{index}].ID"] != null)
            {
                int id = int.Parse(form[$"SymptomEvaluationItems[{index}].ID"]);
                string symptom = _db.SymptomEvaluation.Where(s => s.ID == id).Select(s => s.SymptomItem).FirstOrDefault();

                string scoreKey = $"SymptomScore_{id}";
                int score = 0;
                if (form.AllKeys.Contains(scoreKey) && !string.IsNullOrEmpty(form[scoreKey]))
                {
                    int.TryParse(form[scoreKey], out score);
                }

                if (score < 0) score = 0;
                if (score > 6) score = 6;

                model.SymptomEvaluationItems.Add(new SymptomEvaluationViewModel
                {
                    ID = id,
                    SymptomItem = symptom,
                    Score = score
                });

                index++;
            }
        }
        #endregion

        #region 骨科篩檢
        private void ProcessOrthopaedicScreening(QuestionnaireViewModel model, FormCollection form)
        {
            model.OrthopaedicScreeningItems = new List<OrthopaedicScreeningItmeViewModel>();

            int index = 0;

            while (form[$"OrthopaedicScreeningItems[{index}].ID"] != null)
            {
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
    }
}