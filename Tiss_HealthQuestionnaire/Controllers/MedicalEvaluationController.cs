using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Tiss_HealthQuestionnaire.Models;

namespace Tiss_HealthQuestionnaire.Controllers
{
    public class MedicalEvaluationController : BaseAuthController
    {
        protected override string[] AllowedRoles => new[] { "trainer"};

        private HealthQuestionnaireEntities _db = new HealthQuestionnaireEntities();

        #region 醫療團隊評估主頁
        public ActionResult ConcussionMedicalEvaluation()
        {
            try
            {
                if (Session["UserRole"] == null || Session["UserRole"].ToString() != "trainer")
                {
                    return RedirectToAction("Login", "Account", new { role = "trainer" });
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
                    item.AnswerOption1 = score;
                    totalScore += score;
                }

                Session["OrientationScore"] = totalScore;
                Session["CognitiveScreeningItems"] = questions;

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

                var viewModels = items.Select((x, i) => new ImmediateMemoryViewModel
                {
                    ID = i + 1,
                    Word = x.Word,
                    FirstTestScore = x.FirstTest,
                    SecondTestScore = x.SecondTest,
                    ThirdTestScore = x.ThirdTest
                }).ToList();

                var model = new MedicalViewModel
                {
                    ImmediateMemoryItems = viewModels,
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

                var viewModels = items.Select((item, i) =>
                {
                    int f = int.Parse(form[$"first_{item.ID}"] ?? "0");
                    int s = int.Parse(form[$"second_{item.ID}"] ?? "0");
                    int t = int.Parse(form[$"third_{item.ID}"] ?? "0");

                    total += f + s + t;

                    return new ImmediateMemoryViewModel
                    {
                        ID = item.ID,
                        Word = item.Word,
                        FirstTestScore = f,
                        SecondTestScore = s,
                        ThirdTestScore = t
                    };
                }).ToList();

                Session["ImmediateMemoryScore"] = total;
                Session["CompletionTime"] = form["CompletionTime"] ?? "00:00";
                Session["ImmediateMemoryItems"] = viewModels;

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

                var viewModels = items.Select((x, i) => new ConcentrationViewModel
                {
                    ID = i + 1,
                    ListA = x.ListA,
                    ListB = x.ListB,
                    ListC = x.ListC,
                    Score = 0 // 預設為 0
                }).ToList();

                var model = new MedicalViewModel
                {
                    ConcentrationItems = viewModels
                };

                Session["ConcentrationItems"] = viewModels;
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
                var questions = Session["ConcentrationItems"] as List<ConcentrationViewModel>;
                int total = 0;

                foreach (var item in questions)
                {
                    string key = $"response_{item.ID}";
                    int score = form[key] == "1" ? 1 : 0;
                    item.Score = score;
                    total += score;
                }

                Session["ConcentrationScore"] = total;
                Session["ConcentrationItems"] = questions;

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
                Session["CoordinationItems"] = new List<CoordinationAndBalanceExaminationViewModel> { errors };

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
                var items = _db.DelayedRecall.ToList();

                var viewModels = items.Select((x, i) => new DelayedRecallViewModel
                {
                    ID = i + 1,
                    Word = x.Word,
                    Score = x.Score1
                }).ToList();

                var model = new MedicalViewModel
                {
                    DelayedRecallItems = viewModels,
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

                var viewModels = items.Select((x, i) =>
                {
                    int score = int.Parse(form[$"score_{x.ID}"] ?? "0");
                    total += score;
                    return new DelayedRecallViewModel
                    {
                        ID = i + 1,
                        Word = x.Word,
                        Score = score
                    };
                }).ToList();

                Session["DelayedRecallTotalScore"] = total;
                Session["DelayedRecallStartTime"] = form["testStartTime"] ?? "00:00";
                Session["DelayedRecallViewModels"] = viewModels;

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

        #region 醫療團隊預覽頁
        public ActionResult TrainerPreview()
        {
            try
            {
                var model = new MedicalViewModel
                {
                    // 原本的分數
                    CognitiveScreeningTotalScore = Convert.ToInt32(Session["OrientationScore"] ?? 0),
                    ImmediateMemoryTotalScore = Convert.ToInt32(Session["ImmediateMemoryScore"] ?? 0),
                    CompletionTime = Session["CompletionTime"]?.ToString() ?? "00:00",
                    CompletionTimeDisplay = FormatTime(Session["CompletionTime"]?.ToString()),
                    ConcentrationTotalScore = Convert.ToInt32(Session["ConcentrationScore"] ?? 0),
                    CoordinationAndBalanceTotalErrors = Convert.ToInt32(Session["CoordinationError"] ?? 0),
                    CoordinationAndBalanceAverageTime = float.Parse(Session["CoordinationAvg"]?.ToString() ?? "0"),
                    CoordinationAndBalanceFastestTime = float.Parse(Session["CoordinationFast"]?.ToString() ?? "0"),
                    DelayedRecallTotalScore = Convert.ToInt32(Session["DelayedRecallTotalScore"] ?? 0),
                    DelayedRecallStartTime = Session["DelayedRecallStartTime"]?.ToString() ?? "00:00",

                    // 項目內容
                    CognitiveScreeningItems = Session["CognitiveScreeningItems"] as List<CognitiveScreening>,
                    ImmediateMemoryItems = Session["ImmediateMemoryItems"] as List<ImmediateMemoryViewModel>,
                    ConcentrationItems = Session["ConcentrationItems"] as List<ConcentrationViewModel>,
                    DelayedRecallItems = Session["DelayedRecallViewModels"] as List<DelayedRecallViewModel>,

                    // 總分
                    CognitiveScreeningTotalScores =
                        Convert.ToInt32(Session["OrientationScore"] ?? 0) +
                        Convert.ToInt32(Session["ImmediateMemoryScore"] ?? 0) +
                        Convert.ToInt32(Session["ConcentrationScore"] ?? 0) +
                        Math.Max(0, 30 - Convert.ToInt32(Session["CoordinationError"] ?? 0)) +
                        Convert.ToInt32(Session["DelayedRecallTotalScore"] ?? 0)
                };

                return View("TrainerPreview", model);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region 顯示時間上午下午格式共用方法
        private string FormatTime(string timeStr)
        {
            if (string.IsNullOrEmpty(timeStr))
                return "未填寫";

            if (DateTime.TryParseExact(timeStr, "HH:mm", null, System.Globalization.DateTimeStyles.None, out DateTime dt))
            {
                string ampm = dt.Hour < 12 ? "上午" : "下午";
                return $"{ampm} {dt:hh:mm}";
            }
            return "格式錯誤";
        }
        #endregion
    }
}