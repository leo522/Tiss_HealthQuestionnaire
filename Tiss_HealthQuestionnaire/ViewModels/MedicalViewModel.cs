using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Tiss_HealthQuestionnaire.Models
{
    public class MedicalViewModel
    {
        #region 醫療團隊-認知篩檢-定位(1)
        public List<CognitiveScreening> CognitiveScreeningItems { get; set; }
        public int CognitiveScreeningTotalScore { get; set; }
        public int ID { get; set; }
        public string Question { get; set; }
        public int AnswerOption1 { get; set; }
        public int AnswerOption2 { get; set; }
        #endregion

        #region 醫療團隊-認知篩檢-短期記憶(2)
        public List<ImmediateMemoryViewModel> ImmediateMemoryItems { get; set; }
        public int ImmediateMemoryTotalScore { get; set; }
        public string CompletionTime { get; set; }
        public string CompletionTimeDisplay { get; set; }
        #endregion

        #region 醫療團隊-認知篩檢-專注力(3)
        public List<ConcentrationViewModel> ConcentrationItems { get; set; }
        public int ConcentrationTotalScore { get; set; }
        #endregion

        #region 醫療團隊-認知篩檢-協調與平衡測驗(4)
        public List<CoordinationAndBalanceExaminationViewModel> CoordinationAndBalanceItems { get; set; }
        public int CoordinationAndBalanceTotalErrors { get; set; } = 0;
        public float CoordinationAndBalanceAverageTime { get; set; } = 0;
        public float CoordinationAndBalanceFastestTime { get; set; } = 0;
        #endregion

        #region 醫療團隊-認知篩檢-延遲記憶(5)
        public List<DelayedRecallViewModel> DelayedRecallItems { get; set; }
        public int DelayedRecallTotalScore { get; set; }
        public string DelayedRecallStartTime { get; set; }
        public string testStartTimeDisplay { get; set; }
        #endregion

        #region 醫療團隊-認知篩檢-分數總合(6)
        public int CognitiveScreeningTotalScores { get; set; }
        #endregion

        #region 認知篩檢完成狀態
        public CognitiveStepStatusViewModel CognitiveStepStatus { get; set; }
        #endregion
    }

    #region 醫療團隊-認知篩檢-定位(1)
    public class CognitiveScreeningViewModel
    {
        public int ID { get; set; }
        public string Question { get; set; }
        public int OrientationScore { get; set; }
        public int ImmediateMemoryScore { get; set; }
        public int ConcentrationScore { get; set; }
        public int DelayedRecallScore { get; set; }
        public int TotalScore { get; set; }
    }
    #endregion

    #region 醫療團隊-認知篩檢-短期記憶(2)
    public class ImmediateMemoryViewModel
    {
        public int ID { get; set; }
        public string Word { get; set; }
        public int FirstTestScore { get; set; } = 0;
        public int SecondTestScore { get; set; } = 0;
        public int ThirdTestScore { get; set; } = 0;
        public string CompletionTime { get; set; } = "00:00";
    }
    #endregion

    #region 醫療團隊-認知篩檢-專注力(3)
    public class ConcentrationViewModel
    {
        public int ID { get; set; }
        public string ListA { get; set; }
        public string ListB { get; set; }
        public string ListC { get; set; }
        public int Score { get; set; } = 0;
    }
    #endregion

    #region 醫療團隊-認知篩檢-協調與平衡測驗(4)
    public class CoordinationAndBalanceExaminationViewModel
    {
        public int ID { get; set; }
        public string TestFoot { get; set; }
        public string TestSurface { get; set; }
        public string Footwear { get; set; }
        public int DoubleLegError { get; set; } = 0;
        public int TandemError { get; set; } = 0;
        public int SingleLegError { get; set; } = 0;
        public int TotalErrors { get; set; } = 0;
        public float FirstTime { get; set; } = 0;
        public float SecondTime { get; set; } = 0;
        public float ThirdTime { get; set; } = 0;
        public float AverageTimes { get; set; } = 0;
        public float FastestTimes { get; set; } = 0;
    }
    #endregion

    #region 醫療團隊-認知篩檢-延遲記憶(5)
    public class DelayedRecallViewModel
    {
        public int ID { get; set; }
        public string Word { get; set; }
        public int Score { get; set; } = 0;
    }
    #endregion

    #region 醫療團隊-認知篩檢-分數總合(6)
    public class CognitiveScreeningTotalScoreViewModel
    {
        public int OrientationScore { get; set; }
        public int ImmediateMemoryScore { get; set; }
        public int ConcentrationScore { get; set; }
        public int DelayedRecallScore { get; set; }
        public int TotalScore { get; set; }
    }
    #endregion
}