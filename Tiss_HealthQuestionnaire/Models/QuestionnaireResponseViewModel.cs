using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Tiss_HealthQuestionnaire.Models
{
    public class QuestionnaireResponseViewModel
    {
        public int AthleteID { get; set; }
        public int? GenderID { get; set; }
        public string Specialty { get; set; }
        public string FillName { get; set; }

        public List<QuestionnaireAnswer> PastHealthAnswers { get; set; }
        public List<QuestionnaireAnswer> AllergicHistoryAnswers { get; set; }
        public List<QuestionnaireAnswer> FamilyHistoryAnswers { get; set; }
        public List<QuestionnaireAnswer> PastHistoryAnswers { get; set; }
        public List<QuestionnaireAnswer> SurgeryHistoryAnswers { get; set; }
        public List<QuestionnaireAnswer> PresentIllnessAnswers { get; set; }
        public List<QuestionnaireAnswer> PastDrugsAnswers { get; set; }
        public List<QuestionnaireAnswer> PastSupplementsAnswers { get; set; }
        public List<QuestionnaireAnswer> FemaleQuestionnaireAnswers { get; set; }
    }

    public class QuestionnaireAnswer
    {
        public int QuestionID { get; set; }
        public string Answer { get; set; }
    }
}