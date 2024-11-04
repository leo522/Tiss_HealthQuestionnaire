using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Tiss_HealthQuestionnaire.Models
{
    public class QuestionnaireViewModel
    {
        // 基本信息
        public string Specialist { get; set; }         // 專項
        public string FillName { get; set; }           // 填表人
        public string AtheNum { get; set; }            // 選手編號
        public int Gender { get; set; }                // 性別（1 = 男, 2 = 女）
        public DateTime FillDate { get; set; }         // 填表日期

        // 問卷數據
        public string PastHealthHistory { get; set; }  // 過去健康檢查病史
        public string AllergicHistory { get; set; }    // 過敏史
        public string FamilyHistory { get; set; }      // 家族病史
        public string PastHistory { get; set; }        // 過去病史
        public string SurgeryHistory { get; set; }     // 手術病史
        public string PresentIllness { get; set; }     // 現在病史
        public string PastDrugs { get; set; }          // 藥物史
        public string PastSupplements { get; set; }    // 營養品
        public string FemaleQuestionnaire { get; set; } // 女性問卷（若有）

        public string PastInjuryRestored { get; set; } // 過去傷害狀況（已復原）
        public string NowInjuryRestored { get; set; }  // 目前傷害狀況
        public string CardiovascularScreening { get; set; } // 心血管篩檢
        public string ConcussionScreening { get; set; }     // 腦震盪篩檢-選手自填
        public string OrthopaedicScreening { get; set; }    // 骨科篩檢
        public string CognitiveScreening { get; set; }      // 腦震盪篩檢-醫療團隊評估

        public string ConvertYesNoToLocalizedString(string value) //中英文轉換
        {
            return value == "yes" ? "是 Yes" : "否 No";
        }

        //動態存放所有問卷數據
        public Dictionary<string, string> FormData { get; set; } = new Dictionary<string, string>();

        //過去健康檢查病史的文字敘述
        public List<PastHealthDetailViewModel> PastHealthDetails { get; set; } = new List<PastHealthDetailViewModel>();

        //過敏史
        public List<AllergicHistoryDetailViewModel> AllergicHistoryDetails { get; set; } = new List<AllergicHistoryDetailViewModel>();

        //家族病史
        public List<FamilyHistoryViewModel> FamilyHistoryDetails { get; set; } = new List<FamilyHistoryViewModel>();

        //過去病史
        public List<PastHistoryDetailViewModel> PastHistoryDetails { get; set; } = new List<PastHistoryDetailViewModel>();

        //手術病史
        public List<SurgeryHistoryDetailViewModel> SurgeryHistoryDetails { get; set; } = new List<SurgeryHistoryDetailViewModel>();

        //現在病史
        public List<PresentIllnessDetailViewModel> PresentIllnessDetails { get; set; } = new List<PresentIllnessDetailViewModel>();

        //藥物史
        public List<PastDrugsDetailViewModel> PastDrugsDetails { get; set; } = new List<PastDrugsDetailViewModel>();

        //營養品詳細
        public List<PastSupplementsDetailViewModel> PastSupplementsDetails { get; set; } = new List<PastSupplementsDetailViewModel>();

        //女性問卷
        public List<FemaleQuestionnaireDetailViewModel> FemaleQuestionnaireDetails { get; set; } = new List<FemaleQuestionnaireDetailViewModel>();

        //顯示過去傷害狀況(已復原)
        public List<PastInjuryStatuSViewModel> PastInjuryDetails { get; set; } = new List<PastInjuryStatuSViewModel>();
        public List<PastTreatmentMethoDViewModel> PastTreatmentDetails { get; set; } = new List<PastTreatmentMethoDViewModel>();

        //目前傷害狀況
        public List<InjuryStatuSViewModel> NowInjuryDetails { get; set; } = new List<InjuryStatuSViewModel>();
        public List<TreatmentMethoDViewModel> NowTreatmentDetails { get; set; } = new List<TreatmentMethoDViewModel>();

        //心血管篩檢
        public List<CardiovascularScreeningDetailViewModel> CardiovascularScreeningDetails { get; set; } = new List<CardiovascularScreeningDetailViewModel>();

        //腦震盪篩檢(選手自填)
        public List<ConcussionScreeningViewModel> ConcussionScreeningDetails { get; set; } = new List<ConcussionScreeningViewModel>();
            public string MedicationAnswer { get; set; }
            public string MedicationDetails { get; set; }
            public string Notes { get; set; }


        //骨科篩檢
        public List<OrthopaedicScreeninGViewModel> OrthopaedicScreeningDetails { get; set; } = new List<OrthopaedicScreeninGViewModel>();
    }

    public class PastHealthDetailViewModel //過去健康檢查病史
    {
        public int ItemId { get; set; }    // 項目的ID
        public string ItemZh { get; set; }
        public string ItemEn { get; set; }
        public string Item1 { get; set; }  // 第一個輸入框的內容
        public string Item2 { get; set; }  // 第二個輸入框的內容
        public string Item3 { get; set; }  // 第三個輸入框的內容
    }

    public class AllergicHistoryDetailViewModel //過敏史
    {
        public int ItemId { get; set; }    // 項目的ID
        public string ItemZh { get; set; }
        public string ItemEn { get; set; }
        public string IsAllergic { get; set; }  // 是否過敏（"yes" 或 "no"）
        public string AllergyDescription { get; set; }  // 過敏詳情描述
    }

    public class FamilyHistoryViewModel //家族病史的每個項目
    {
        public int ItemId { get; set; }
        public string GeneralPartsZh { get; set; }
        public string GeneralPartsEn { get; set; }
        public string FamilyHistoryOption { get; set; } // 可為 "yes", "no", 或 "unknown"
        public string OtherFamilyHistory { get; set; }  // 其他家族病史
    }

    public class PastHistoryDetailViewModel //過去病史
    {
        public int ItemId { get; set; } // 項目的ID
        public string GeneralPartsZh { get; set; }
        public string GeneralPartsEn { get; set; }
        public string PastHistoryOption { get; set; } // "yes", "no", "unknown"
    }

    public class SurgeryHistoryDetailViewModel //手術病史
    {
        public int ItemId { get; set; }           // 手術項目ID
        public string PartsOfBodyZh { get; set; } // 手術部位 (中文)
        public string PartsOfBodyEn { get; set; } // 手術部位 (英文)
        public string OperationOption { get; set; } // 是否接受手術的選擇 ("yes", "no", "unknown")
    }

    public class PresentIllnessDetailViewModel //現在病史
    {
        public int ItemId { get; set; }    // 項目的ID
        public string PartsOfBodyZh { get; set; }
        public string PartsOfBodyEn { get; set; }
        public string ReceivingOtherTherapies { get; set; }  // 是否接受其他治療
    }

    public class PastDrugsDetailViewModel //藥物史
    {
        public int ItemId { get; set; }    // 藥物項目 ID
        public string ItemZh { get; set; } // 藥物項目中文名稱
        public string ItemEn { get; set; } // 藥物項目英文名稱
        public string IsUsed { get; set; } // 是否使用過（"yes" 或 "no"）
        public string OtherDrugs { get; set; } // 其他藥物使用情況描述
    }

    public class PastSupplementsDetailViewModel //營養品
    {
        public int ItemId { get; set; }    // 補充品項目 ID
        public string ItemZh { get; set; } // 補充品項目中文名稱
        public string ItemEn { get; set; } // 補充品項目英文名稱
        public bool IsUsed { get; set; }   // 是否使用過（true 或 false）
        public string OtherSupplements { get; set; } // 其他補充品描述
    }

    public class FemaleQuestionnaireDetailViewModel //女性問卷
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

    public class PastInjuryStatuSViewModel //顯示過去傷害狀況(已復原)
    {
        public string PastInjuryPart { get; set; }
        public bool PastIsSingleSelect { get; set; }
        public bool LeftSide { get; set; }
        public bool RightSide { get; set; }
    }

    public class PastTreatmentMethoDViewModel //顯示過去傷害狀況(治療方法)
    {
        public string Method { get; set; }
    }

    public class CardiovascularScreeningDetailViewModel //心血管篩檢
    {
        public int OrderNumber { get; set; } // 項次
        public string Question { get; set; }  // 問題描述
        public string Answer { get; set; }    // 答案
    }

    public class InjuryStatuSViewModel //顯示目前傷害狀況
    {
        public string InjuryPart { get; set; } // 部位
        public bool IsSingleSelect { get; set; } // 是否單選
        public bool LeftSide { get; set; } // 左邊
        public bool RightSide { get; set; } // 右邊
        public List<string> InjuryTypes { get; set; } = new List<string>(); // 傷勢類型
    }

    public class TreatmentMethoDViewModel //顯示目前傷害狀況(治療方法)
    {
        public string Method { get; set; }
    }

    public class OrthopaedicScreeninGViewModel //骨科篩檢
    {
        public int OrderNumber { get; set; }
        public string Instructions { get; set; }
        public string ObservationPoints { get; set; }
        public string Result { get; set; }  // 正常或異常
    }


}