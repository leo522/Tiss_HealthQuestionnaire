using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Tiss_HealthQuestionnaire.Models
{
    public class QuestionnaireViewModel
    {
        #region 基本信息
        public string Specialist { get; set; }
        public string FillName { get; set; }
        public string AtheNum { get; set; }
        public int Gender { get; set; }
        public DateTime FillDate { get; set; }
        #endregion

        #region 問卷數據
        public string PastHealthHistory { get; set; }
        public string AllergicHistory { get; set; }
        public string FamilyHistory { get; set; }
        public string PastHistory { get; set; }
        public string SurgeryHistory { get; set; }
        public string PresentIllness { get; set; }
        public string PastSupplements { get; set; }
        public string FemaleQuestionnaire { get; set; }
        public string PastInjuryRestored { get; set; }
        public string NowInjuryRestored { get; set; }
        public string CardiovascularScreening { get; set; }
        public string ConcussionScreening { get; set; }
        public string OrthopaedicScreening { get; set; }
        public string CognitiveScreening { get; set; }
        #endregion

        #region 中英文轉換
        public static string ConvertYesNoToLocalizedString(string value)
        {
            switch (value)
            {
                case "yes":
                    return "是 Yes";
                case "no":
                    return "否 No";
                case "unknown":
                    return "未知 Unknown";
                default:
                    return "未定義 Undefined";
            }
        }
        #endregion

        #region 動態存放所有問卷數據
        public Dictionary<string, string> FormData { get; set; } = new Dictionary<string, string>();
        #endregion

        #region 各問卷的回應
        public Dictionary<string, string> PastHealthResponses { get; set; } = new Dictionary<string, string>();
        public Dictionary<string, string> AllergicHistoryResponses { get; set; } = new Dictionary<string, string>();
        public Dictionary<string, string> FamilyHistoryResponses { get; set; } = new Dictionary<string, string>();
        public Dictionary<string, string> PastHistoryResponses { get; set; } = new Dictionary<string, string>();
        public Dictionary<string, string> PresentIllnessResponses { get; set; } = new Dictionary<string, string>();
        public Dictionary<string, string> PastDrugsResponses { get; set; } = new Dictionary<string, string>();
        public Dictionary<string, string> PastSupplementsResponses { get; set; } = new Dictionary<string, string>();
        public Dictionary<string, string> FemaleQuestionnaireResponses { get; set; } = new Dictionary<string, string>();
        public Dictionary<string, string> PastInjuryResponses { get; set; } = new Dictionary<string, string>();
        public Dictionary<string, string> PastInjuryTypeResponses { get; set; } = new Dictionary<string, string>();
        public Dictionary<string, string> NowInjuryResponses { get; set; } = new Dictionary<string, string>();
        public Dictionary<string, string> NowInjuryTypeResponses { get; set; } = new Dictionary<string, string>();
        public Dictionary<string, string> CardiovascularScreeningResponses { get; set; } = new Dictionary<string, string>();
        public Dictionary<string, string> ConcussionScreeningResponses { get; set; } = new Dictionary<string, string>();
        public Dictionary<string, string> SymptomEvaluationResponses { get; set; } = new Dictionary<string, string>();
        public Dictionary<string, string> OrthopaedicScreeningResponses { get; set; } = new Dictionary<string, string>();
        public Dictionary<string, string> CognitiveScreeningResponses { get; set; } = new Dictionary<string, string>();
        public Dictionary<string, string> ImmediateMemoryResponses { get; set; } = new Dictionary<string, string>();
        public Dictionary<string, string> ConcentrationResponses { get; set; } = new Dictionary<string, string>();
        public Dictionary<string, string> CoordinationResponses { get; set; } = new Dictionary<string, string>();
        public Dictionary<string, string> DelayedRecallResponses { get; set; } = new Dictionary<string, string>();
        #endregion

        #region 過去健康檢查病史
        public List<PastHealthItemViewModel> PastHealthItems { get; set; }
        #endregion

        #region 過敏史
        public List<AllergicHistoryItemViewModel> AllergicHistoryItems { get; set; }
        #endregion

        #region 家族病史
        public List<FamilyHistoryViewModel> FamilyHistoryItems { get; set; }
        public string OtherFamilyHistory { get; set; }
        #endregion

        #region 過去病史
        public List<PastHistoryViewModel> PastHistoryItems { get; set; }
        public string OtherPastHistory { get; set; }
        #endregion

        #region 現在病史
        public List<PresentIllnessViewModel> PresentIllnessItems { get; set; }
        #endregion

        #region 藥物史
        public List<PastDrugsViewModel> PastDrugsItems { get; set; }
        public string TUE { get; set; }
        public string OtherDrug { get; set; }
        #endregion

        #region 營養品
        public List<PastSupplementsViewModel> PastSupplementsItems { get; set; }
        public string OtherSupplements { get; set; }
        #endregion

        #region 女性問卷
        public List<FemaleQuestionnaireViewModel> FemaleQuestionnaireItems { get; set; } = new List<FemaleQuestionnaireViewModel>();
        #endregion

        #region 傷害類型共用
        public class InjuryTypeViewModel
        {
            public string CategoryName { get; set; }
            public string InjuryName { get; set; }
        }
        #endregion

        #region 過去傷害狀況(已復原)
        public string PastInjuryStatusAnswer { get; set; } = "yes";

        public List<PastInjuryStatusViewModel> PastInjuryItems { get; set; } = new List<PastInjuryStatusViewModel>();

        public List<InjuryTypeViewModel> PastInjuryTypes { get; set; } = new List<InjuryTypeViewModel>();

        public List<PastTreatmentMethodViewModel> PastTreatmentItems { get; set; } = new List<PastTreatmentMethodViewModel>();

        public class PastInjuryStatusViewModel
        {
            public int Id { get; set; }
            public string PastInjuryPart { get; set; }
            public bool LeftSide { get; set; }
            public bool RightSide { get; set; }
        }

        public class PastTreatmentMethodViewModel
        {
            public int Id { get; set; }
            public string Method { get; set; }
        }
        #endregion

        #region 目前傷害
        public string CurrentInjuryStatusAnswer { get; set; } = "no";
        public List<CurrentInjuryStatusViewModel> CurrentInjuryItems { get; set; } = new List<CurrentInjuryStatusViewModel>();
        public List<InjuryTypeViewModel> CurrentInjuryTypes { get; set; } = new List<InjuryTypeViewModel>();
        public List<CurrentTreatmentMethodViewModel> CurrentTreatmentItems { get; set; } = new List<CurrentTreatmentMethodViewModel>();

        public class CurrentInjuryStatusViewModel
        {
            public int Id { get; set; }
            public string CurrentInjuryPart { get; set; }
            public bool LeftSide { get; set; }
            public bool RightSide { get; set; }
        }

        public class CurrentTreatmentMethodViewModel
        {
            public int Id { get; set; }
            public string Method { get; set; }
        }
        #endregion

        #region 心血管篩檢
        public List<CardiovascularScreeningItemViewModel> CardiovascularScreeningItems { get; set; }
        #endregion

        #region 腦震盪篩檢-選手自填-(1)
        public List<ConcussionScreeningViewModel> ConcussionScreeningItems { get; set; }
        public string ConcussionScreeningMedicationAnswer { get; set; }
        public string ConcussionScreeningMedicationDetails { get; set; }
        public string ConcussionScreeningNotes { get; set; }
        #endregion

        #region 症狀自我評估-選手自填(2)
        public List<SymptomEvaluationViewModel> SymptomEvaluationItems { get; set; }
        #endregion

        #region 骨科篩檢
        public List<OrthopaedicScreeningItmeViewModel> OrthopaedicScreeningItems { get; set; }
        #endregion

        #region 醫療團隊評估
        public int OrientationScore { get; set; }
        public int ImmediateMemoryScore { get; set; }
        public string CompletionTime { get; set; }
        public int ConcentrationScore { get; set; }
        public int CoordinationErrors { get; set; }
        public float CoordinationAverageTime { get; set; }
        public float CoordinationFastestTime { get; set; }
        public int DelayedRecallScore { get; set; }
        public int CognitiveScreeningTotalScore { get; set; }
        #endregion
    }

    #region 過去健康檢查病史
    public class PastHealthItemViewModel
    {
        public int ID { get; set; }
        public string ItemZh { get; set; }
        public string ItemEn { get; set; }
        public bool IsYes { get; set; }
        public string Details { get; set; }
    }
    #endregion

    #region 過敏史
    public class AllergicHistoryItemViewModel
    {
        public int ID { get; set; }
        public string ItemZh { get; set; }
        public string ItemEn { get; set; }
        public bool IsYes { get; set; }
        public string Details { get; set; }
    }
    #endregion

    #region 家族病史
    public class FamilyHistoryViewModel
    {
        public int ID { get; set; }
        public string GeneralPartsZh { get; set; }

        public bool IsYes { get; set; }
        public bool IsNo { get; set; }
        public bool IsUnknown { get; set; }

        public string FamilyHistoryOption
        {
            get
            {
                if (IsYes) return "yes";
                if (IsNo) return "no";
                return "unknown";
            }
            set
            {
                IsYes = (value == "yes");
                IsNo = (value == "no");
                IsUnknown = (value == "unknown");
            }
        }
    }
    #endregion

    #region 過去病史
    public class PastHistoryViewModel
    {
        public int ID { get; set; }
        public string GeneralPartsZh { get; set; }
        public bool IsYes { get; set; }
        public bool IsNo { get; set; }
        public bool IsUnknown { get; set; }

        public string PastHistoryOption
        {
            get
            {
                if (IsYes) return "yes";
                if (IsNo) return "no";
                return "unknown";
            }
            set
            {
                IsYes = (value == "yes");
                IsNo = (value == "no");
                IsUnknown = (value == "unknown");
            }
        }
    }
    #endregion

    #region 現在病史
    public class PresentIllnessViewModel
    {
        public int ID { get; set; }
        public string PartsOfBodyZh { get; set; }
        public string ReceivingTherapy { get; set; }
    }
    #endregion

    #region 藥物史
    public class PastDrugsViewModel
    {
        public int ID { get; set; }
        public string ItemZh { get; set; }
        public string ItemEn { get; set; }
        public bool IsUsed { get; set; }
    }
    #endregion

    #region 營養品
    public class PastSupplementsViewModel
    {
        public int ID { get; set; }
        public string ItemZh { get; set; }
        public string ItemEn { get; set; }
        public bool IsUsed { get; set; }
    }
    #endregion

    #region 女性問卷
    public class FemaleQuestionnaireViewModel
    {
        public int ID { get; set; }
        public string QuestionZh { get; set; }
        public string QuestionEn { get; set; }
        public string Answer { get; set; }
    }
    #endregion

    #region 顯示過去傷害狀況(已復原)
    public class PastInjuryStatuSViewModel
    {
        public string PastInjuryPart { get; set; }
        public bool LeftSide { get; set; }
        public bool RightSide { get; set; }
        public List<string> InjuryTypes { get; set; } = new List<string>();
    }
    #endregion

    #region 顯示過去傷害狀況(傷勢類型)
    public class PastInjuryTypeViewModel
    {
        public string Type { get; set; }
    }
    #endregion

    #region 顯示過去傷害狀況(治療方法)
    public class PastTreatmentMethoDViewModel
    {
        public string Method { get; set; }
    }
    #endregion

    #region 顯示目前傷害狀況
    public class InjuryStatuSViewModel
    {
        public string InjuryPart { get; set; }
        public bool LeftSide { get; set; }
        public bool RightSide { get; set; }
        public List<string> InjuryTypes { get; set; } = new List<string>();
    }
    #endregion

    #region 顯示目前傷害狀況(傷勢類型)
    public class InjuryStatuTypeViewModel
    {
        public string Type { get; set; }
    }
    #endregion

    #region 顯示目前傷害狀況(治療方法)
    public class TreatmentMethoDViewModel
    {
        public string Method { get; set; }
    }
    #endregion

    #region 心血管篩檢
    public class CardiovascularScreeningItemViewModel
    {
        public int ID { get; set; }
        public string Question { get; set; }
        public bool IsUsed { get; set; }
    }
    #endregion

    #region 腦震盪篩檢-選手自填(1)
    public class ConcussionScreeningViewModel
    {
        public int Id { get; set; }
        public string Question { get; set; }
        public string Answer { get; set; }
    }
    #endregion

    #region 症狀自我評估-選手自填(2)
    public class SymptomEvaluationViewModel
    {
        public int ID { get; set; }
        public string SymptomItem { get; set; }
        public int Score { get; set; }
    }
    #endregion

    #region 骨科篩檢
    public class OrthopaedicScreeningItmeViewModel
    {
        public int ID { get; set; }
        public string Instructions { get; set; }
        public string ObservationPoints { get; set; }
        public string ResultNormal { get; set; }
        public string ResultAbnormal { get; set; }
        public string Result { get; set; }
    }
    #endregion
}