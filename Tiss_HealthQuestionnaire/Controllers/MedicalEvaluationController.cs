using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Tiss_HealthQuestionnaire.Models;

namespace Tiss_HealthQuestionnaire.Controllers
{
    public class MedicalEvaluationController : Controller
    {
        private HealthQuestionnaireEntities _db = new HealthQuestionnaireEntities();

        #region 醫療團隊評估主頁
        public ActionResult ConcussionMedicalEvaluation()
        {
            try
            {
                if (Session["TrainerAuthenticated"] == null || !(bool)Session["TrainerAuthenticated"])
                {
                    return RedirectToAction("Main", "Questionnaire");
                }

                var model = new MedicalViewModel
                {
                    CognitiveStepStatus = new CognitiveStepStatusViewModel
                    {
                        Step1Completed = Session["OrientationScore"] != null,
                        Step2Completed = Session["ImmediateMemoryScore"] != null,
                        Step3Completed = Session["ConcentrationScore"] != null,
                        Step4Completed = Session["CoordinationError"] != null,
                        Step5Completed = Session["DelayedRecallTotalScore"] != null,
                        Step6Completed = Session["CognitiveScreeningTotalScores"] != null
                    }
                };

                return View(model);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region 醫療團隊評估-腦震盪篩檢-認知篩檢-定位(1)
        public ActionResult CognitiveScreening()
        {
            try
            {
                var questions = _db.CognitiveScreening.ToList();

                var model = new MedicalViewModel
                {
                    CognitiveScreeningItems = questions
                };

                Session["CognitiveScreeningItems"] = questions;

                return View("CognitiveScreening", model);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpPost]
        public ActionResult CognitiveScreening(FormCollection form)
        {
            try
            {
                var questions = Session["CognitiveScreeningItems"] as List<CognitiveScreening>;
                int totalScore = 0;

                foreach (var item in questions)
                {
                    string key = $"question_{item.ID}";
                    int score = form[key] == "1" ? 1 : 0;
                    totalScore += score;
                }

                Session["OrientationScore"] = totalScore;

                return RedirectToAction("ImmediateMemory");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region 醫療團隊評估-腦震盪篩檢-認知篩檢-短期記憶(2)
        public ActionResult ImmediateMemory()
        {
            try
            {
                var items = _db.ImmediateMemory.ToList();

                var model = new MedicalViewModel
                {
                    ImmediateMemoryItems = items,
                    ImmediateMemoryTotalScore = Convert.ToInt32(Session["ImmediateMemoryScore"] ?? 0),
                    CompletionTime = Session["CompletionTime"]?.ToString() ?? "00:00"
                };

                return View("ImmediateMemory", model);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpPost]
        public ActionResult ImmediateMemory(FormCollection form)
        {
            try
            {
                int total = 0;
                var items = _db.ImmediateMemory.ToList();

                foreach (var item in items)
                {
                    int f = int.Parse(form[$"first_{item.ID}"] ?? "0");
                    int s = int.Parse(form[$"second_{item.ID}"] ?? "0");
                    int t = int.Parse(form[$"third_{item.ID}"] ?? "0");

                    total += f + s + t;

                    item.FirstTest = f;
                    item.SecondTest = s;
                    item.ThirdTest = t;
                }

                Session["ImmediateMemoryScore"] = total;
                Session["CompletionTime"] = form["CompletionTime"] ?? "00:00";

                return RedirectToAction("Concentration");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region 醫療團隊評估-腦震盪篩檢-認知篩檢-專注力(3)
        public ActionResult Concentration()
        {
            try
            {
                var items = _db.Concentration.ToList();
                var model = new MedicalViewModel
                {
                    ConcentrationItems = items
                };

                Session["ConcentrationItems"] = items;
                return View("Concentration", model);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpPost]
        public ActionResult Concentration(FormCollection form)
        {
            try
            {
                var questions = Session["ConcentrationItems"] as List<Concentration>;
                int total = 0;

                foreach (var item in questions)
                {
                    string key = $"response_{item.Id}";
                    int score = form[key] == "1" ? 1 : 0;
                    total += score;
                }

                Session["ConcentrationScore"] = total;
                return RedirectToAction("CoordinationAndBalanceExamination");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region 醫療團隊評估-腦震盪篩檢-認知篩檢-協調與平衡測驗(4)
        public ActionResult CoordinationAndBalanceExamination()
        {
            try
            {
                var model = new MedicalViewModel
                {
                    CoordinationAndBalanceItems = new List<CoordinationAndBalanceExaminationViewModel>
                    {
                        new CoordinationAndBalanceExaminationViewModel()
                    }
                };

                return View("CoordinationAndBalanceExamination", model);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpPost]
        public ActionResult CoordinationAndBalanceExamination(FormCollection form)
        {
            try
            {
                var errors = new CoordinationAndBalanceExaminationViewModel
                {
                    TestFoot = form["TestFoot"],
                    TestSurface = form["TestSurface"],
                    Footwear = form["Footwear"],
                    DoubleLegError = int.Parse(form["DoubleLegError"] ?? "0"),
                    TandemError = int.Parse(form["TandemError"] ?? "0"),
                    SingleLegError = int.Parse(form["SingleLegError"] ?? "0"),
                    FirstTime = float.Parse(form["FirstTime"] ?? "0"),
                    SecondTime = float.Parse(form["SecondTime"] ?? "0"),
                    ThirdTime = float.Parse(form["ThirdTime"] ?? "0")
                };

                errors.TotalErrors = errors.DoubleLegError + errors.TandemError + errors.SingleLegError;
                errors.AverageTimes = (errors.FirstTime + errors.SecondTime + errors.ThirdTime) / 3;
                errors.FastestTimes = Math.Min(errors.FirstTime, Math.Min(errors.SecondTime, errors.ThirdTime));

                Session["CoordinationError"] = errors.TotalErrors;
                Session["CoordinationAvg"] = errors.AverageTimes;
                Session["CoordinationFast"] = errors.FastestTimes;

                return RedirectToAction("DelayedRecall");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region 醫療團隊評估-腦震盪篩檢-認知篩檢-延遲記憶(5)
        public ActionResult DelayedRecall()
        {
            try
            {
                var model = new MedicalViewModel
                {
                    DelayedRecallItems = _db.DelayedRecall.ToList(),
                    DelayedRecallStartTime = Session["DelayedRecallStartTime"]?.ToString() ?? "00:00"
                };

                return View("DelayedRecall", model);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpPost]
        public ActionResult DelayedRecall(FormCollection form)
        {
            try
            {
                var items = _db.DelayedRecall.ToList();
                int total = 0;

                foreach (var item in items)
                {
                    int score = int.Parse(form[$"score_{item.ID}"] ?? "0");
                    total += score;
                }

                Session["DelayedRecallTotalScore"] = total;
                Session["DelayedRecallStartTime"] = form["testStartTime"] ?? "00:00";

                return RedirectToAction("CognitiveScreeningTotalScore");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region 醫療團隊評估-腦震盪篩檢-認知篩檢-認知篩檢分數總合(6)
        public ActionResult CognitiveScreeningTotalScore()
        {
            try
            {
                int orientation = Convert.ToInt32(Session["OrientationScore"] ?? 0);
                int memory = Convert.ToInt32(Session["ImmediateMemoryScore"] ?? 0);
                int concentration = Convert.ToInt32(Session["ConcentrationScore"] ?? 0);
                int delayed = Convert.ToInt32(Session["DelayedRecallTotalScore"] ?? 0);
                int coordinationError = Convert.ToInt32(Session["CoordinationError"] ?? 0);
                int coordinationScore = Math.Max(0, 30 - coordinationError);

                var model = new MedicalViewModel
                {
                    CognitiveScreeningTotalScore = orientation,
                    ImmediateMemoryTotalScore = memory,
                    ConcentrationTotalScore = concentration,
                    DelayedRecallTotalScore = delayed,
                    CoordinationAndBalanceTotalErrors = coordinationError,
                    CognitiveScreeningTotalScores = orientation + memory + concentration + coordinationScore + delayed
                };

                return View("CognitiveScreeningTotalScore", model);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region 醫療團隊-認知篩檢 (1~6)-用不到?!
        //private void ProcessCognitiveScreening(MedicalViewModel model, FormCollection form)
        //{
        //    try
        //    {
        //        // 定位 (Orientation)
        //        var orientationItems = _db.CognitiveScreening.ToList();
        //        model.CognitiveScreeningItems = new List<CognitiveScreening>();
        //        model.CognitiveScreeningTotalScore = 0;

        //        foreach (var item in orientationItems)
        //        {
        //            string answerKey = $"question_{item.ID}";
        //            var answer = form[answerKey];
        //            int score = answer == "1" ? 1 : 0;

        //            model.CognitiveScreeningItems.Add(new CognitiveScreening
        //            {
        //                ID = item.ID,
        //                Question = item.Question,
        //                AnswerOption1 = score,
        //                AnswerOption2 = 0
        //            });
        //            model.CognitiveScreeningTotalScore += score;
        //        }

        //        // 短期記憶 (Immediate Memory)
        //        var immediateMemoryItems = _db.ImmediateMemory.ToList();
        //        model.ImmediateMemoryItems = new List<ImmediateMemory>();
        //        model.ImmediateMemoryTotalScore = 0;

        //        foreach (var item in immediateMemoryItems)
        //        {
        //            int first = int.Parse(form[$"first_{item.ID}"] ?? "0");
        //            int second = int.Parse(form[$"second_{item.ID}"] ?? "0");
        //            int third = int.Parse(form[$"third_{item.ID}"] ?? "0");

        //            model.ImmediateMemoryItems.Add(new ImmediateMemory
        //            {
        //                ID = item.ID,
        //                Word = item.Word,
        //                FirstTest0 = first,
        //                SecondTest0 = second,
        //                ThirdTest1 = third
        //            });
        //            model.ImmediateMemoryTotalScore += first + second + third;
        //        }
        //        model.CompletionTime = form["CompletionTime"] ?? "00:00";

        //        // 專注力 (Concentration)
        //        var concentrationItems = _db.Concentration.ToList();
        //        model.ConcentrationItems = new List<Concentration>();
        //        model.ConcentrationTotalScore = 0;

        //        foreach (var item in concentrationItems)
        //        {
        //            string answerKey = $"response_{item.Id}";
        //            var answer = form[answerKey];
        //            int score = answer == "1" ? 1 : 0;

        //            model.ConcentrationItems.Add(new Concentration
        //            {
        //                OrderNumber = item.Id,
        //                ListA = item.ListA,
        //                ListB = item.ListB,
        //                ListC = item.ListC
        //            });
        //            model.ConcentrationTotalScore += score;
        //        }

        //        // 協調與平衡測驗 (Coordination and Balance)
        //        var coordination = new CoordinationAndBalanceExaminationViewModel
        //        {
        //            TestFoot = form["TestFoot"],
        //            TestSurface = form["TestSurface"],
        //            Footwear = form["Footwear"],
        //            DoubleLegError = int.Parse(form["DoubleLegError"] ?? "0"),
        //            TandemError = int.Parse(form["TandemError"] ?? "0"),
        //            SingleLegError = int.Parse(form["SingleLegError"] ?? "0"),
        //            FirstTime = float.Parse(form["FirstTime"] ?? "0"),
        //            SecondTime = float.Parse(form["SecondTime"] ?? "0"),
        //            ThirdTime = float.Parse(form["ThirdTime"] ?? "0")
        //        };

        //        coordination.TotalErrors = coordination.DoubleLegError + coordination.TandemError + coordination.SingleLegError;
        //        coordination.AverageTimes = (coordination.FirstTime + coordination.SecondTime + coordination.ThirdTime) / 3;
        //        coordination.FastestTimes = Math.Min(coordination.FirstTime, Math.Min(coordination.SecondTime, coordination.ThirdTime));

        //        model.CoordinationAndBalanceItems = new List<CoordinationAndBalanceExaminationViewModel> { coordination };
        //        model.CoordinationAndBalanceTotalErrors = coordination.TotalErrors;
        //        model.CoordinationAndBalanceAverageTime = coordination.AverageTimes;
        //        model.CoordinationAndBalanceFastestTime = coordination.FastestTimes;


        //        // 延遲記憶 (Delayed Recall)
        //        var delayedRecallItems = _db.DelayedRecall.ToList();
        //        model.DelayedRecallItems = new List<DelayedRecall>();
        //        model.DelayedRecallTotalScore = 0;

        //        foreach (var item in delayedRecallItems)
        //        {
        //            int score = int.Parse(form[$"score_{item.ID}"] ?? "0");

        //            model.DelayedRecallItems.Add(new DelayedRecall
        //            {
        //                ID = item.ID,
        //                Word = item.Word,
        //                Score0 = score
        //            });
        //            model.DelayedRecallTotalScore += score;
        //        }
        //        model.DelayedRecallStartTime = form["testStartTime"] ?? "00:00";

        //        // 總分計算
        //        model.CognitiveScreeningTotalScores =
        //            model.CognitiveScreeningTotalScore +
        //            model.ImmediateMemoryTotalScore +
        //            model.ConcentrationTotalScore +
        //            model.DelayedRecallTotalScore;
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new ApplicationException("處理認知篩檢資料時發生錯誤。", ex);
        //    }
        //}
        #endregion
    }
}