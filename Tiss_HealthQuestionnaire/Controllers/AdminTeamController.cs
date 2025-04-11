using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Tiss_HealthQuestionnaire.Models;
using Tiss_HealthQuestionnaire.ViewModels;

namespace Tiss_HealthQuestionnaire.Controllers
{
    public class AdminTeamController : BaseAuthController
    {
        protected override string[] AllowedRoles => new[] { "admin" };

        private HealthQuestionnaireEntities _db = new HealthQuestionnaireEntities();

        #region 隊伍清單總覽
        public ActionResult TeamList()
        {
            var teams = _db.Team.Select(t => new TeamViewModel
            {
                TeamID = t.TeamID,
                TeamName = t.TeamName,
                SportType = t.SportType,
                CreatedDate = t.CreatedDate
            }).ToList();

            return View(teams);
        }
        #endregion

        #region 新增隊伍
        public ActionResult CreateTeam()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateTeam(TeamViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var team = new Team
            {
                TeamName = model.TeamName,
                SportType = model.SportType,
                CreatedDate = DateTime.Now
            };
            _db.Team.Add(team);
            _db.SaveChanges();

            return RedirectToAction("TeamList");
        }
        #endregion

        #region 編輯隊伍
        public ActionResult EditTeam(int id)
        {
            var team = _db.Team.Find(id);
            if (team == null) return HttpNotFound();

            var model = new TeamViewModel
            {
                TeamID = team.TeamID,
                TeamName = team.TeamName,
                SportType = team.SportType
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditTeam(TeamViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var team = _db.Team.Find(model.TeamID);
            if (team == null) return HttpNotFound();

            team.TeamName = model.TeamName;
            team.SportType = model.SportType;
            _db.SaveChanges();

            return RedirectToAction("TeamList");
        }
        #endregion

        #region 刪除隊伍
        [HttpPost]
        public ActionResult DeleteTeam(int id)
        {
            var team = _db.Team.Find(id);
            if (team == null) return HttpNotFound();

            _db.Team.Remove(team);
            _db.SaveChanges();

            return RedirectToAction("TeamList");
        }
        #endregion

        #region 指派選手與防護員至隊伍
        public ActionResult AssignMembers(int id)
        {
            var team = _db.Team.Find(id);
            if (team == null) return HttpNotFound();

            var model = new AssignTeamMembersViewModel
            {
                TeamID = id,
                TeamName = team.TeamName,
                AllAthletes = _db.AthleteProfile.ToList(),
                AllTrainers = _db.TrainerProfile.ToList(),
                SelectedAthleteIDs = _db.AthleteTeam.Where(at => at.TeamID == id).Select(at => at.AthleteID).ToList(),
                SelectedTrainerIDs = _db.TrainerTeam.Where(tt => tt.TeamID == id).Select(tt => tt.TrainerID).ToList()
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AssignMembers(AssignTeamMembersViewModel model)
        {
            var team = _db.Team.Find(model.TeamID);
            if (team == null) return HttpNotFound();

            var oldAthletes = _db.AthleteTeam.Where(at => at.TeamID == model.TeamID);
            var oldTrainers = _db.TrainerTeam.Where(tt => tt.TeamID == model.TeamID);

            _db.AthleteTeam.RemoveRange(oldAthletes);
            _db.TrainerTeam.RemoveRange(oldTrainers);

            if (model.SelectedAthleteIDs != null)
            {
                foreach (var aid in model.SelectedAthleteIDs)
                {
                    _db.AthleteTeam.Add(new AthleteTeam { AthleteID = aid, TeamID = model.TeamID });
                }
            }

            if (model.SelectedTrainerIDs != null)
            {
                foreach (var tid in model.SelectedTrainerIDs)
                {
                    _db.TrainerTeam.Add(new TrainerTeam { TrainerID = tid, TeamID = model.TeamID });
                }
            }

            _db.SaveChanges();
            return RedirectToAction("TeamList");
        }
        #endregion
    }
}