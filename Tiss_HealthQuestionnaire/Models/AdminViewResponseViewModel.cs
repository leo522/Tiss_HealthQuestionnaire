using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Tiss_HealthQuestionnaire.Models
{
	public class AdminViewResponseViewModel
	{
        // 主問卷基本資料
        public int ResponseId { get; set; }
        public string AthleteID { get; set; }
        public string FillName { get; set; }
        public string Specialty { get; set; }
        public int GenderID { get; set; }
        public string GenderDisplay => GenderID == 1 ? "男" : "女";
        public DateTime FillingDate { get; set; }


        public List<ResponsePastHealth> PastHealthList { get; set; }
        public List<ResponseAllergicHistory> AllergicHistoryList { get; set; }
        public List<ResponseFamilyHistory> FamilyHistoryList { get; set; }
        public List<ResponsePastHistory> PastHistoryList { get; set; }
        public List<ResponsePresentIllness> PresentIllnessList { get; set; }
        public List<ResponsePastDrugs> PastDrugsList { get; set; }
        public List<ResponsePastSupplements> PastSupplementsList { get; set; }
        public List<ResponseFemaleQuestionnaire> FemaleQuestionnaireList { get; set; }
        public List<ResponsePastInjuries> PastInjuryList { get; set; }
        public List<ResponseCurrentInjuries> CurrentInjuryList { get; set; }
        public List<ResponseCardiovascularScreening> CardiovascularList { get; set; }
        public List<ResponseConcussionScreening> ConcussionScreeningList { get; set; }
        public List<ResponseSymptomEvaluation> SymptomEvaluationList { get; set; }
        public List<ResponseOrthopaedicScreening> OrthopaedicScreeningList { get; set; }
    }

    public class AdminPastHealthItem
    {
        public string ItemName { get; set; }
        public bool HasAbnormalItems { get; set; }
        public string Details { get; set; }
    }

    public class AdminAllergicHistoryItem
    {
        public string AllergyType { get; set; }
        public bool IsYes { get; set; }
        public string Details { get; set; }
    }

    public class AdminFamilyHistoryItem
    {
        public string Disease { get; set; }
        public string Status { get; set; }
    }

    public class AdminSymptomEvaluationItem
    {
        public string Symptom { get; set; }
        public int Severity { get; set; }
    }
}