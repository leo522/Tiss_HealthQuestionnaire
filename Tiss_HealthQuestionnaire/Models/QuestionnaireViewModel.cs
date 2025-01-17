using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Tiss_HealthQuestionnaire.Models
{
    public class QuestionnaireViewModel
    {
        #region 基本信息
        public string Specialist { get; set; }         // 專項
        public string FillName { get; set; }           // 填表人
        public string AtheNum { get; set; }            // 選手編號
        public int Gender { get; set; }                // 性別（1 = 男, 2 = 女）
        public DateTime FillDate { get; set; }         // 填表日期
        #endregion

        #region 問卷數據
        public string PastHealthHistory { get; set; }  // 過去健康檢查病史
        public string AllergicHistory { get; set; }    // 過敏史
        public string FamilyHistory { get; set; }      // 家族病史
        public string PastHistory { get; set; }        // 過去病史
        public string SurgeryHistory { get; set; }     // 手術病史
        public string PresentIllness { get; set; }     // 現在病史
        public string PastSupplements { get; set; }    // 營養品
        public string FemaleQuestionnaire { get; set; } // 女性問卷（若有）

        public string PastInjuryRestored { get; set; } // 過去傷害狀況（已復原）
        public string NowInjuryRestored { get; set; }  // 目前傷害狀況
        public string CardiovascularScreening { get; set; } // 心血管篩檢
        public string ConcussionScreening { get; set; }     // 腦震盪篩檢-選手自填
        public string OrthopaedicScreening { get; set; }    // 骨科篩檢
        public string CognitiveScreening { get; set; }      // 腦震盪篩檢-醫療團隊評估-定位
        #endregion

        #region 中英文轉換
        public string ConvertYesNoToLocalizedString(string value)
        {
            if (value == "yes")
            {
                return "是 Yes";
            }
            else if (value == "no")
            {
                return "否 No";
            }
            else if (value == "unknown")
            {
                return "未知 Unknown";
            }
            else
            {
                return "未定義 Undefined"; // 處理潛在的未定義值
            }
        }
        #endregion

        #region 動態存放所有問卷數據
        public Dictionary<string, string> FormData { get; set; } = new Dictionary<string, string>();
        #endregion

        #region 過去健康檢查病史
        public List<PastHealthDetailViewModel> PastHealthDetails { get; set; } = new List<PastHealthDetailViewModel>();
        #endregion

        #region 過敏史
        public List<AllergicHistoryDetailViewModel> AllergicHistoryDetails { get; set; } = new List<AllergicHistoryDetailViewModel>();
        #endregion

        #region 家族病史
        public List<FamilyHistoryViewModel> FamilyHistoryDetails { get; set; } = new List<FamilyHistoryViewModel>();
        #endregion

        #region 過去病史
        public List<PastHistoryDetailViewModel> PastHistoryDetails { get; set; } = new List<PastHistoryDetailViewModel>();
        #endregion

        #region 手術病史
        public List<SurgeryHistoryDetailViewModel> SurgeryHistoryDetails { get; set; } = new List<SurgeryHistoryDetailViewModel>();
        #endregion

        #region 現在病史
        public List<PresentIllnessDetailViewModel> PresentIllnessDetails { get; set; } = new List<PresentIllnessDetailViewModel>();
        #endregion

        #region 藥物史
        public List<PastDrugsDetailViewModel> PastDrugsDetails { get; set; } = new List<PastDrugsDetailViewModel>();
        public string TUE { get; set; } //是否有申請治療用途豁免
        #endregion

        #region 營養品詳細
        public List<PastSupplementsDetailViewModel> PastSupplementsDetails { get; set; } = new List<PastSupplementsDetailViewModel>();
        #endregion

        #region 女性問卷
        public List<FemaleQuestionnaireDetailViewModel> FemaleQuestionnaireDetails { get; set; } = new List<FemaleQuestionnaireDetailViewModel>();
        #endregion

        #region 顯示過去傷害狀況(已復原)
        public List<PastInjuryStatuSViewModel> PastInjuryDetails { get; set; } = new List<PastInjuryStatuSViewModel>();
        #endregion

        #region 顯示過去傷害狀況(已復原)-傷勢類型
        public List<PastInjuryTypeViewModel> PastInjuryTypes { get; set; } = new List<PastInjuryTypeViewModel>();
        #endregion

        #region 顯示過去傷害狀況(已復原)-治療方法
        public List<PastTreatmentMethoDViewModel> PastTreatmentDetails { get; set; } = new List<PastTreatmentMethoDViewModel>();
        #endregion

        #region 目前傷害狀況
        public List<InjuryStatuSViewModel> NowInjuryDetails { get; set; } = new List<InjuryStatuSViewModel>();
        #endregion

        #region 目前傷害狀況-傷勢類型
        public List<InjuryStatuTypeViewModel> NowInjuryTypes { get; set; } = new List<InjuryStatuTypeViewModel>();
        #endregion

        #region 目前傷害狀況-治療方法
        public List<TreatmentMethoDViewModel> NowTreatmentDetails { get; set; } = new List<TreatmentMethoDViewModel>();
        #endregion

        #region 心血管篩檢
        public List<CardiovascularScreeningDetailViewModel> CardiovascularScreeningDetails { get; set; } = new List<CardiovascularScreeningDetailViewModel>();
        #endregion

        #region 腦震盪篩檢(選手自填)
        public List<ConcussionScreeningViewModel> ConcussionScreeningDetails { get; set; } = new List<ConcussionScreeningViewModel>();
        public string ConcussionScreeningMedicationAnswer { get; set; } //是否服用藥物
        public string ConcussionScreeningMedicationDetails { get; set; } //藥物細節
        public string ConcussionScreeningNotes { get; set; } //備註
        #endregion

        #region 症狀自我評估(選手自填)
        public List<SymptomEvaluationViewModel> SymptomEvaluationDetails { get; set; } = new List<SymptomEvaluationViewModel>();
        #endregion

        #region 醫療團隊-認知篩檢-定位(1)
        public List<CognitiveScreeningViewModel> CognitiveScreeningDetails { get; set; } = new List<CognitiveScreeningViewModel>();
        public int CognitiveScreeningTotalScore { get; set; } //定位總分
        #endregion

        #region 醫療團隊-認知篩檢-短期記憶(2)
        public List<ImmediateMemoryViewModel> ImmediateMemoryDetails { get; set; } = new List<ImmediateMemoryViewModel>();
        public int ImmediateMemoryTotalScore { get; set; } //短期記憶總分
        public string CompletionTime { get; set; } //短期記憶完成時間
        #endregion

        #region 醫療團隊-認知篩檢-專注力(3)
        public List<ConcentrationViewModel> ConcentrationDetails { get; set; } = new List<ConcentrationViewModel>();
        public int ConcentrationTotalScore { get; set; } // 總分
        #endregion

        #region 醫療團隊-認知篩檢-協調與平衡測驗(4)
        public List<CoordinationAndBalanceExaminationViewModel> CoordinationAndBalanceDetails { get; set; }
           = new List<CoordinationAndBalanceExaminationViewModel>();

        public int CoordinationAndBalanceTotalErrors { get; set; } = 0; // 總錯誤次數
        public float CoordinationAndBalanceAverageTime { get; set; } = 0; // 平均時間
        public float CoordinationAndBalanceFastestTime { get; set; } = 0; // 最快時間
        #endregion

        #region 醫療團隊-認知篩檢-延遲記憶(5)
        public List<DelayedRecallViewModel> DelayedRecallDetails { get; set; } = new List<DelayedRecallViewModel>();
        public int DelayedRecallTotalScore { get; set; } //延遲記憶總分
        public string DelayedRecallStartTime { get; set; } // 測驗開始時間
        #endregion

        #region 醫療團隊-認知篩檢-分數總合(6)
        public List<CognitiveScreeningTotalScoreViewModel> CognitiveScreeningTotalScoreDetails { get; set; } = new List<CognitiveScreeningTotalScoreViewModel>();
        public int CognitiveScreeningTotalScores { get; set; }
        #endregion

        #region 骨科篩檢
        public List<OrthopaedicScreeninGViewModel> OrthopaedicScreeningDetails { get; set; } = new List<OrthopaedicScreeninGViewModel>();
        #endregion
    }

    #region 過去健康檢查病史
    public class PastHealthDetailViewModel //
    {
        public int ItemId { get; set; }    // 項目的ID
        public string ItemZh { get; set; } // 中文
        public string ItemEn { get; set; }  // 英文
        public string Item1 { get; set; } = "未回答";  // 第一個輸入框的內容
        public string Item2 { get; set; } = "未回答";  // 第二個輸入框的內容
        public string Item3 { get; set; } = "未回答";  // 第三個輸入框的內容
    }
    #endregion

    #region 過敏史
    public class AllergicHistoryDetailViewModel
    {
        public int ItemId { get; set; }    // 項目的ID
        public string ItemZh { get; set; }
        public string ItemEn { get; set; }
        public string IsAllergic { get; set; }  // 是否過敏（"yes" 或 "no"）
        public string AllergyDescription { get; set; }  // 過敏詳情描述
    }
    #endregion

    #region 家族病史的每個項目
    public class FamilyHistoryViewModel
    {
        public int ItemId { get; set; }
        public string GeneralPartsZh { get; set; }
        public string GeneralPartsEn { get; set; }
        public string FamilyHistoryOption { get; set; }  // 可為 "yes", "no", 或 "unknown"
        public string OtherFamilyHistory { get; set; }  // 其他家族病史
        public bool? IsNo { get; set; }
        public bool? IsYes { get; set; }
        public bool? IsUnknown { get; set; }
    }
    #endregion

    #region 過去病史
    public class PastHistoryDetailViewModel
    {
        public int ItemId { get; set; } //項目的ID
        public string GeneralPartsZh { get; set; }
        public string GeneralPartsEn { get; set; }
        public string PastHistoryOption { get; set; } // "yes", "no", "unknown"
        public string OtherPastHistory { get; set; }
        public bool? IsNo { get; set; }
        public bool? IsYes { get; set; }
        public bool? IsUnknown { get; set; }
    }
    #endregion

    #region 開刀史
    public class SurgeryHistoryDetailViewModel
    {
        public int ItemId { get; set; }           // 手術項目ID
        public string PartsOfBodyZh { get; set; } // 手術部位 (中文)
        public string PartsOfBodyEn { get; set; } // 手術部位 (英文)
        public string OperationOption { get; set; } // 是否接受手術的選擇 ("yes", "no")
    }
    #endregion

    #region 現在病史
    public class PresentIllnessDetailViewModel
    {
        public int ItemId { get; set; }    // 項目的ID
        public string PartsOfBodyZh { get; set; }
        public string PartsOfBodyEn { get; set; }
        public string ReceivingOtherTherapies { get; set; }  // 是否接受其他治療("yes", "no")
    }
    #endregion

    #region 藥物史
    public class PastDrugsDetailViewModel
    {
        public int ItemId { get; set; }    // 藥物項目 ID
        public string ItemZh { get; set; } // 藥物項目中文名稱
        public string ItemEn { get; set; } // 藥物項目英文名稱
        public string IsUsed { get; set; } // 是否使用過（"yes" 或 "no"）
        public string OtherDrugs { get; set; } // 其他藥物使用情況描述
    }
    #endregion

    #region 營養品
    public class PastSupplementsDetailViewModel
    {
        public int ItemId { get; set; }    // 補充品項目 ID
        public string ItemZh { get; set; } // 補充品項目中文名稱
        public string ItemEn { get; set; } // 補充品項目英文名稱
        public string IsUsed { get; set; }   // 是否使用過（true 或 false）
        public string OtherSupplements { get; set; } // 其他補充品描述
    }
    #endregion

    #region 女性問卷
    public class FemaleQuestionnaireDetailViewModel
    {
        public int ID { get; set; } // 問題的唯一識別碼
        public string QuestionZh { get; set; } // 問題的中文描述
        public string QuestionEn { get; set; } // 問題的英文描述
        public string Answer { get; set; } // 單選題的答案代碼

        // 保存可選項目，鍵為選項代碼，值為選項描述（中英文）
        public Dictionary<string, string> AnswerOptions { get; set; } = new Dictionary<string, string>();

        // 顯示選擇的答案（中英文對照）
        public string DisplayAnswer
        {
            get
            {
                if (string.IsNullOrEmpty(Answer))
                {
                    return "未回答"; // 如果沒有回答，顯示預設
                }

                // 檢查 AnswerOptions 是否為 null 並確認包含 Answer 的鍵
                return AnswerOptions != null && AnswerOptions.ContainsKey(Answer) ? AnswerOptions[Answer] : Answer;
            }
        }
    }
    #endregion

    #region 顯示過去傷害狀況(已復原)
    public class PastInjuryStatuSViewModel
    {
        public string PastInjuryPart { get; set; } // 部位名稱
        public bool LeftSide { get; set; } // 左側是否受傷
        public bool RightSide { get; set; } // 右側是否受傷
        public List<string> InjuryTypes { get; set; } = new List<string>(); // 傷勢類型列表
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

    #region 心血管篩檢
    public class CardiovascularScreeningDetailViewModel
    {
        public int OrderNumber { get; set; } // 項次
        public string Question { get; set; }  // 問題描述
        public string Answer { get; set; }    // 答案
    }
    #endregion

    #region 腦震盪篩檢
    public class ConcussionScreeningViewModel
    {
        public int OrderNumber { get; set; }  // 問卷項次
        public string Question { get; set; }  // 問卷問題
        public string Answer { get; set; } // 答案 (yes/no)
    }
    #endregion

    #region 症狀自我評估-選手自填
    public class SymptomEvaluationViewModel
    {
        public int OrderNumber { get; set; }
        public string SymptomItem { get; set; }
        public int Score { get; set; }
    }
    #endregion

    #region 顯示目前傷害狀況
    public class InjuryStatuSViewModel
    {
        public string InjuryPart { get; set; } // 部位名稱
        public bool LeftSide { get; set; } // 左側是否受傷
        public bool RightSide { get; set; } // 右側是否受傷
        public List<string> InjuryTypes { get; set; } = new List<string>(); // 傷勢類型列表
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

    #region 醫療團隊-認知篩檢-定位(1)
    public class CognitiveScreeningViewModel
    {
        public int OrderNumber { get; set; }  // 項次
        public string Question { get; set; }  // 問題文字
        public int OrientationScore { get; set; } // 定位分數
        public int ImmediateMemoryScore { get; set; } // 短期記憶分數
        public int ConcentrationScore { get; set; } // 專注力分數
        public int DelayedRecallScore { get; set; } // 延遲記憶分數
        public int TotalScore { get; set; } // 總分
    }
    #endregion

    #region 醫療團隊-認知篩檢-短期記憶(2)
    public class ImmediateMemoryViewModel
    {
        public int OrderNumber { get; set; }  // 項次
        public string Word { get; set; }      // 顯示的詞彙
        public int FirstTestScore { get; set; } = 0;  // 第一次測驗分數
        public int SecondTestScore { get; set; } = 0; // 第二次測驗分數
        public int ThirdTestScore { get; set; } = 0;  // 第三次測驗分數
        public string CompletionTime { get; set; } = "00:00";
    }
    #endregion

    #region 醫療團隊-認知篩檢-專注力(3)
    public class ConcentrationViewModel
    {
        public int OrderNumber { get; set; } // 項次
        public string ListA { get; set; }    // 列表 A
        public string ListB { get; set; }    // 列表 B
        public string ListC { get; set; }    // 列表 C
        public int Score { get; set; } = 0;  // 分數 (0 或 1)
    }
    #endregion

    #region 醫療團隊-認知篩檢-協調與平衡測驗(4)
    public class CoordinationAndBalanceExaminationViewModel
    {
        public int OrderNumber { get; set; } //項次
        public string TestFoot { get; set; } //測試腳
        public string TestSurface { get; set; } //測試平面
        public string Footwear { get; set; } //腳穿著
        public int DoubleLegError { get; set; } = 0; //雙腳站立錯誤次數
        public int TandemError { get; set; } = 0; //腳跟前後站立錯誤次數
        public int SingleLegError { get; set; } = 0; //單腳站立錯誤次數
        public int TotalErrors { get; set; } = 0; //錯誤次數分數
        public float FirstTime { get; set; } = 0; //第一次測試時間
        public float SecondTime { get; set; } = 0; //第二次測試時間
        public float ThirdTime { get; set; } = 0; //第三次測試時間
        public float AverageTimes { get; set; } = 0; //平均時間
        public float FastestTimes { get; set; } = 0; //最快時間
    }
    #endregion

    #region 醫療團隊-認知篩檢-延遲記憶(5)
    public class DelayedRecallViewModel
    {
        public int OrderNumber { get; set; } //項次，用於顯示題目的順序
        public string Word { get; set; } //詞彙內容
        public int Score { get; set; } = 0; //測驗分數
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

    #region 骨科篩檢
    public class OrthopaedicScreeninGViewModel
    {
        public int OrderNumber { get; set; }
        public string Instructions { get; set; }
        public string ObservationPoints { get; set; }
        public string Result { get; set; }  //正常或異常
    }
    #endregion
}