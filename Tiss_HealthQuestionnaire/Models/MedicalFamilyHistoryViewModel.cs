using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Tiss_HealthQuestionnaire.Models
{
    public class MedicalFamilyHistoryViewModel
    {
        public string Question { get; set; }  // 問題文字
        public List<string> Symptoms { get; set; }  // 相關症狀列表
    }
}