using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Tiss_HealthQuestionnaire.Models;

namespace Tiss_HealthQuestionnaire.ViewModels
{
    #region 防護員問卷填答資料
    public class AdminViewTrainerResponseViewModel
	{
        public int ResponseId { get; set; }
        public string TrainerID { get; set; }
        public string FillName { get; set; }
        public string Specialty { get; set; }
        public DateTime FillingDate { get; set; }
        public int? TotalScore { get; set; }

        public List<ResponseCognitiveScreening> CognitiveScreeningList { get; set; } //定位
        public List<ResponseImmediateMemory> ImmediateMemoryList { get; set; } //短期記憶
        public List<ResponseConcentration> ConcentrationList { get; set; } //專注力
        public List<ResponseCoordinationAndBalance> CoordinationAndBalanceList { get; set; } //協調與平衡測驗
        public List<ResponseDelayedRecall> DelayedRecallList { get; set; } //延遲記憶
        public List<ResponseOrthopaedicScreening> OrthopaedicScreeningList { get; set; } //骨科篩檢
    }
    #endregion
}