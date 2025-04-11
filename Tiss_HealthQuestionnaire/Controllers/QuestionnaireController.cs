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
    public class QuestionnaireController : BaseAuthController
    {
        protected override string[] AllowedRoles => new[] { "athlete" ,"admin"};

        private HealthQuestionnaireEntities _db = new HealthQuestionnaireEntities();

        #region 主頁
        public ActionResult Main()
        {
            try
            {
                var user = GetLoggedInUser();

                if (user == null)
                    return RedirectToAction("Login", "Account");

                SetUserViewBag(user);

                var viewModel = CreateQuestionnaireViewModel(user.GenderID);

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
                    IsYes = false,
                    Details = null
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
                IsUsed = false
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
                    IsUsed = false
                }).ToList();
        }
        #endregion

        #region 女性問卷
        private List<FemaleQuestionnaireViewModel> GetFemaleQuestionnaireViewModel(int genderID)
        {
            if (genderID != 2)
            {
                return new List<FemaleQuestionnaireViewModel>();
            }

            var questions = _db.FemaleQuestionnaire
                .Select(q => new FemaleQuestionnaireViewModel
                {
                    ID = q.ID,
                    QuestionZh = q.QuestionZh,
                    QuestionEn = q.QuestionEn
                }).ToList();

            return questions;
        }
        #endregion

        #region 過去傷害狀況(已復原)
        private List<PastInjuryStatusViewModel> GetPastInjuryItems()
        {
            return _db.PastInjuryStatus.Select(i => new QuestionnaireViewModel.PastInjuryStatusViewModel
            {
                Id = i.Id,
                PastInjuryPart = i.InjuryPart
            }).ToList();
        }

        private List<InjuryTypeViewModel> GetPastInjuryTypesList()
        {
            return _db.PastInjuryType.Join(_db.PastInjuryCategory,
                type => type.PastInjuryCategoryId,
                cat => cat.PastInjuryCategoryId,
                (type, cat) => new QuestionnaireViewModel.InjuryTypeViewModel
                {
                    InjuryName = type.InjuryName,
                    CategoryName = cat.CategoryName
                }).ToList();
        }

        private List<PastTreatmentMethodViewModel> GetPastTreatmentItems()
        {
            return _db.PastTreatmentMethod.Select(t => new PastTreatmentMethodViewModel
            {
                Id = t.Id,
                Method = t.Method
            }).ToList();
        }
        #endregion

        #region 目前傷害狀況
        private List<CurrentInjuryStatusViewModel> GetCurrentInjuryItems()
        {
            return _db.CurrentInjuryStatus.Select(i => new CurrentInjuryStatusViewModel
            {
                Id = i.Id,
                CurrentInjuryPart = i.InjuryPart
            }).ToList();
        }

        private List<InjuryTypeViewModel> GetCurrentInjuryTypesList()
        {
            return _db.CurrentInjuryType.Join(_db.CurrentInjuryCategory,
                type => type.CurrentInjuryCategoryId,
                cat => cat.CurrentInjuryCategoryId,
                (type, cat) => new InjuryTypeViewModel
                {
                    InjuryName = type.InjuryName,
                    CategoryName = cat.CategoryName
                }).ToList();
        }

        private List<CurrentTreatmentMethodViewModel> GetCurrentTreatmentItems()
        {
            return _db.CurrentTreatmentMethod.Select(t => new CurrentTreatmentMethodViewModel
            {
                Id = t.Id,
                Method = t.Method
            }).ToList();
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

        #region 腦震盪篩檢 - 選手自填
        private List<ConcussionScreeningViewModel> GetConcussionScreeningViewModel()
        {
            return _db.ConcussionScreening.Select(q => new ConcussionScreeningViewModel
            {
                Id = q.Id,
                Question = q.Question
            }).ToList();
        }
        #endregion

        #region 症狀自我評估 - 選手自填
        private List<SymptomEvaluationViewModel> GetSymptomEvaluationViewModel()
        {
            return _db.SymptomEvaluation.Select(q => new SymptomEvaluationViewModel
            {
                ID = q.ID,
                SymptomItem = q.SymptomItem
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
                ObservationPoints = item.ObservationPoints,
                ResultNormal = item.ResultNormal,
                ResultAbnormal = item.ResultAbnormal,
                Result = ""
            }).ToList();
        }
        #endregion

        #endregion

        #region 主頁優化方法
        private AthleteProfile GetLoggedInUser()
        {
            var userId = Session["UserID"] as int?;
            if (!userId.HasValue) return null;

            return _db.AthleteProfile.FirstOrDefault(a => a.UserID == userId.Value);
        }

        private void SetUserViewBag(AthleteProfile user)
        {
            ViewBag.Specialist = user.SportSpecialization;
            ViewBag.FillName = user.Name;
            ViewBag.AtheNum = user.AthleteNumber;
            ViewBag.GenderID = user.GenderID;
            ViewBag.ShowFemaleTab = (user.GenderID == 2);
        }

        private QuestionnaireViewModel CreateQuestionnaireViewModel(int genderID)
        {
            return new QuestionnaireViewModel
            {
                PastHealthItems = GetPastHealthItemViewModel(),
                AllergicHistoryItems = GetAllergicHistoryItemViewModels(),
                FamilyHistoryItems = GetFamilyHistoryItemsViewModels(),
                PastHistoryItems = GetPastHistoryViewModel(),
                PresentIllnessItems = GetPresentIllnessViewModel(),
                PastDrugsItems = GetPastDrugsViewModel(),
                TUE = "no",
                OtherDrug = "",
                PastSupplementsItems = GetPastSupplementsViewModel(),
                FemaleQuestionnaireItems = GetFemaleQuestionnaireViewModel(genderID),

                PastInjuryStatusAnswer = "yes",
                PastInjuryItems = GetPastInjuryItems(),
                PastInjuryTypes = GetPastInjuryTypesList(),
                PastTreatmentItems = GetPastTreatmentItems(),

                CurrentInjuryStatusAnswer = "yes",
                CurrentInjuryItems = GetCurrentInjuryItems(),
                CurrentInjuryTypes = GetCurrentInjuryTypesList(),
                CurrentTreatmentItems = GetCurrentTreatmentItems(),

                CardiovascularScreeningItems = GetCardiovascularScreeningViewModel(),
                ConcussionScreeningItems = GetConcussionScreeningViewModel(),
                SymptomEvaluationItems = GetSymptomEvaluationViewModel(),
                OrthopaedicScreeningItems = GetOrthopaedicScreeningViewModel(),

                //CognitiveScreeningItems = _db.CognitiveScreening.ToList(),
                //ImmediateMemoryItems = _db.ImmediateMemory.ToList(),
                //ConcentrationItems = _db.Concentration.ToList(),
                //CoordinationAndBalanceItems = _db.CoordinationAndBalanceExamination.ToList(),
                //DelayedRecallItems = _db.DelayedRecall.ToList(),
            };
        }
        #endregion

        #region 泛型共用方法-問卷題目
        private ActionResult LoadSimpleTable<T>(string viewName) where T : class
        {
            var data = _db.Set<T>().ToList();
            return View(viewName, data);
        }
        #endregion

        #region 問卷題目
        public ActionResult PastHealth() => LoadSimpleTable<PastHealth>("PastHealth");

        public ActionResult AllergicHistory() => LoadSimpleTable<AllergicHistory>("AllergicHistory");

        public ActionResult FamilyHistory() => LoadSimpleTable<FamilyHistory>("FamilyHistory");

        public ActionResult PastHistory() => LoadSimpleTable<PastHistory>("PastHistory");

        public ActionResult PresentIllness() => LoadSimpleTable<PresentIllness>("PresentIllness");

        public ActionResult PastSupplements() => LoadSimpleTable<PastSupplements>("PastSupplements");

        public ActionResult FemaleQuestionnaire() => LoadSimpleTable<FemaleQuestionnaire>("FemaleQuestionnaire");

        // 特例處理：藥物史過濾 ID != 13
        public ActionResult PastDrugs()
        {
            var pastDrugs = _db.PastDrugs.Where(drug => drug.ID != 13).ToList();
            return View("PastDrugs", pastDrugs);
        }

        private List<InjuryTypeViewModel> GetPastInjuryTypes()
        {
            return _db.PastInjuryType.Join(_db.PastInjuryCategory,
                type => type.PastInjuryCategoryId,
                category => category.PastInjuryCategoryId,
                (type, category) => new InjuryTypeViewModel
                {
                    CategoryName = category.CategoryName,
                    InjuryName = type.InjuryName
                }).ToList();
        }

        private List<InjuryTypeViewModel> GetCurrentInjuryTypes()
        {
            return _db.CurrentInjuryType.Join(_db.CurrentInjuryCategory,
                type => type.CurrentInjuryCategoryId,
                category => category.CurrentInjuryCategoryId,
                (type, category) => new InjuryTypeViewModel
                {
                    CategoryName = category.CategoryName,
                    InjuryName = type.InjuryName
                }).ToList();
        }

        // 特殊處理的問卷篩檢仍保留原邏輯（未抽象）
        public ActionResult CardiovascularScreening()
        {
            var questions = _db.CardiovascularScreening.ToList();

            var viewModel = questions.Select((q, index) => new CardiovascularScreeningViewModel
            {
                OrderNumber = index + 1,
                Question = q.Question
            }).ToList();

            return View("CardiovascularScreening", viewModel);
        }

        public ActionResult ConcussionScreening()
        {
            var questions = _db.ConcussionScreening.ToList();

            var viewModel = questions.Select((q, index) => new ConcussionScreening
            {
                Id = index + 1,
                Question = q.Question
            }).ToList();

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

            ViewBag.MedicationAnswer = Session["ConcussionScreeningMedicationAnswer"] as string;
            ViewBag.MedicationDetails = Session["ConcussionScreeningMedicationDetails"] as string;
            ViewBag.Notes = Session["ConcussionScreeningNotes"] as string;

            return View("ConcussionScreening", viewModel);
        }

        [HttpGet]
        public ActionResult SymptomEvaluation()
        {
            var questions = _db.SymptomEvaluation.ToList();

            var viewModel = questions.Select((q, index) => new ConcussionScreening
            {
                Id = index + 1,
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

        public ActionResult OrthopaedicScreening()
        {
            var questions = _db.OrthopaedicScreening.ToList();

            var viewModel = questions.Select((q, index) => new OrthopaedicScreeningViewModel
            {
                OrderNumber = index + 1,
                Instructions = q.Instructions,
                ObservationPoints = q.ObservationPoints
            }).ToList();

            return View("OrthopaedicScreening", viewModel);
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
                model.FemaleQuestionnaireItems = GetFemaleQuestionnaireViewModel(model.Gender);
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
                ProcessCognitiveScreening(model, form); //認知篩檢
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

        #region 醫療團隊評估
        private void ProcessCognitiveScreening(QuestionnaireViewModel model, FormCollection form)
        {
            model.OrientationScore = int.Parse(form["OrientationScore"] ?? "0");
            model.ImmediateMemoryScore = int.Parse(form["ImmediateMemoryScore"] ?? "0");
            model.CompletionTime = form["CompletionTime"] ?? "00:00";
            model.ConcentrationScore = int.Parse(form["ConcentrationScore"] ?? "0");
            model.CoordinationErrors = int.Parse(form["CoordinationError"] ?? "0");
            model.CoordinationAverageTime = float.Parse(form["CoordinationAvg"] ?? "0");
            model.CoordinationFastestTime = float.Parse(form["CoordinationFast"] ?? "0");
            model.DelayedRecallScore = int.Parse(form["DelayedRecallTotalScore"] ?? "0");
            model.CognitiveScreeningTotalScore = int.Parse(form["CognitiveScreeningTotalScores"] ?? "0");
        }
        #endregion 

        #region 醫療團隊-認知篩檢 (1~6)
        [HttpPost]
        public ActionResult PreviewFromMedical(FormCollection form)
        {
            var model = new QuestionnaireViewModel
            {
                OrientationScore = int.Parse(form["OrientationScore"] ?? "0"),
                ImmediateMemoryScore = int.Parse(form["ImmediateMemoryScore"] ?? "0"),
                CompletionTime = form["CompletionTime"] ?? "00:00",
                ConcentrationScore = int.Parse(form["ConcentrationScore"] ?? "0"),
                CoordinationErrors = int.Parse(form["CoordinationError"] ?? "0"),
                CoordinationAverageTime = float.Parse(form["CoordinationAverageTime"] ?? "0"),
                CoordinationFastestTime = float.Parse(form["CoordinationFastestTime"] ?? "0"),
                DelayedRecallScore = int.Parse(form["DelayedRecallTotalScore"] ?? "0"),
                CognitiveScreeningTotalScore = int.Parse(form["CognitiveScreeningTotalScores"] ?? "0")
            };

            return View("Preview", model);
        }
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
                    ProcessConcussionScreening(model, form);
                    ProcessSymptomEvaluation(model, form);

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
                        string entityName = validationResult.Entry.Entity.GetType().Name;

                        foreach (var error in validationResult.ValidationErrors)
                        {
                            string errorMessage = $"實體: {entityName}, 屬性: {error.PropertyName}, 錯誤: {error.ErrorMessage}";
                            errorMessages.Add(errorMessage);
                        }
                    }

                    string fullErrorMessage = string.Join(" | ", errorMessages);

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
                             item.IsUnknown ? "Unknown" : "Unknown";

                var familyHistory = new ResponseFamilyHistory
                {
                    QuestionnaireResponseID = responseId,
                    Disease = item.GeneralPartsZh,
                    Status = status
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

            _db.SaveChanges();
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
                    Disease = item.GeneralPartsZh,
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
                    Status = model.OtherPastHistory
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

            if (presentIllnessList.Any())
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
                .Where(item => item.IsUsed)
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
                    Used = model.TUE == "yes"
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
                .Where(item => item.IsUsed)
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
            if (model.Gender != 2 || model.FemaleQuestionnaireItems == null)
            {
                return;
            }

            var femaleResponses = model.FemaleQuestionnaireItems
                .Where(q => !string.IsNullOrEmpty(q.Answer))
                .Select(q => new ResponseFemaleQuestionnaire
                {
                    QuestionnaireResponseID = responseId,
                    Question = q.QuestionZh,
                    Answer = q.Answer
                }).ToList();

            _db.ResponseFemaleQuestionnaire.AddRange(femaleResponses);
            //if (model.Gender != 2 || model.FemaleQuestionnaireAnswers == null)
            //{
            //    return;
            //}

            //var femaleResponses = model.FemaleQuestionnaireItems
            //    .Where(q => model.FemaleQuestionnaireAnswers.ContainsKey(q.ID))
            //    .Select(q => new ResponseFemaleQuestionnaire
            //    {
            //        QuestionnaireResponseID = responseId,
            //        Question = q.QuestionZh,
            //        Answer = model.FemaleQuestionnaireAnswers[q.ID]
            //    }).ToList();

            //_db.ResponseFemaleQuestionnaire.AddRange(femaleResponses);
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
                    Recovered = true,
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
                    Answer = true,
                    QuestionText = item.ItemZh
                }).ToList();

            System.Diagnostics.Debug.WriteLine($"CardiovascularScreening 篩選後數量: {cardiovascularList.Count}");

            if (cardiovascularList.Any())
                _db.ResponseCardiovascularScreening.AddRange(cardiovascularList);
        }
        #endregion

        #region 儲存 Concussion Screening (腦震盪篩檢 - 選手自填)
        private void SaveConcussionScreening(QuestionnaireViewModel model, int responseId)
        {
            if (model.ConcussionScreeningItems == null || !model.ConcussionScreeningItems.Any()) return;

            foreach (var item in model.ConcussionScreeningItems)
            {
                var concussionScreening = new ResponseConcussionScreening
                {
                    QuestionnaireResponseID = responseId,
                    QuestionNumber = item.Id,
                    Question = item.Question,
                    Answer = item.Answer == "是"
                };
                _db.ResponseConcussionScreening.Add(concussionScreening);
            }

            _db.ResponseConcussionScreening.Add(new ResponseConcussionScreening
            {
                QuestionnaireResponseID = responseId,
                QuestionNumber = 0,
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
            if (model.SymptomEvaluationItems == null || !model.SymptomEvaluationItems.Any()) return;

            foreach (var item in model.SymptomEvaluationItems)
            {
                var symptomEvaluation = new ResponseSymptomEvaluation
                {
                    QuestionnaireResponseID = responseId,
                    Symptom = item.SymptomItem,
                    Severity = item.Score
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

        #region 跳轉到醫療防護團隊題目頁及身分驗證
        [HttpPost]
        public ActionResult RedirectToMedicalEvaluation(string userName, string password)
        {
            var trainer = _db.Test_AthleticTrainer.FirstOrDefault(at => at.ATName == userName && at.IsActive);

            if (trainer != null && VerifyPassword(password, trainer.ATNumber))
            {
                Session["TrainerAuthenticated"] = true;
                Session["TrainerUserName"] = userName;

                return RedirectToAction("ConcussionMedicalEvaluation", "MedicalEvaluation");
            }

            ViewBag.AuthError = "身份驗證失敗，請確認帳號與密碼。";

            var user = GetLoggedInUser();
            var model = CreateQuestionnaireViewModel(user?.GenderID ?? 1);
            SetUserViewBag(user);
            return View("Main", model);
        }

        private bool VerifyPassword(string inputPassword, string storedPassword)
        {
            return inputPassword == storedPassword;
        }
        #endregion
    }
}