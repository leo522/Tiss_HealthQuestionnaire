using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Tiss_HealthQuestionnaire.Models;

namespace Tiss_HealthQuestionnaire.Controllers
{
    public class PreExerciseHealthController : Controller
    {
        private HealthQuestionnaireEntities _db = new HealthQuestionnaireEntities();

        #region 過去傷害狀況(已復原)
        public ActionResult PastInjury()
        { 
            return View();
        }
        #endregion

        #region 目前傷害狀況
        public ActionResult CurrentInjury() 
        {
            return View();
        }
        #endregion

        #region 醫療史與家族史
        public ActionResult MedicalFamilyHistory()
        {
            return View();
        }
        #endregion

        #region 心血管篩檢
        public ActionResult Cardiovascular()
        {
            return View();
        }
        #endregion

        #region 腦震盪篩檢
        public ActionResult Concussion()
        {
            return View();
        }
        #endregion

        #region 腦震盪篩檢-症狀自我評估
        public ActionResult SymptomEvaluation()
        {
            return View();
        }
        #endregion

        #region 腦震盪篩檢-認知篩檢-定位
        public ActionResult CognitiveOrientation()
        { 
            return View();
        }
        #endregion

        #region 腦震盪篩檢-認知篩檢-短期記憶
        public ActionResult ImmediateMemory()
        {
            return View();
        }
        #endregion

        #region 腦震盪篩檢-認知篩檢-專注力
        public ActionResult Concentration()
        {
            return View();
        }
        #endregion

        #region 腦震盪篩檢-認知篩檢-協調與平衡測驗
        public ActionResult CoordinationAndBalance()
        {
            return View();
        }
        #endregion

        #region 腦震盪篩檢-認知篩檢-延遲記憶
        public ActionResult DelayedRecall()
        { 
            return View();
        }
        #endregion

        #region 腦震盪篩檢-認知篩檢-分數總和
        public ActionResult CognitiveScorce()
        { 
            return View();
        }
        #endregion

        #region MyRegion

        #endregion

        #region 骨科篩檢
        public ActionResult Orthopaedic()
        {
            return View();
        }
        #endregion
    }
}