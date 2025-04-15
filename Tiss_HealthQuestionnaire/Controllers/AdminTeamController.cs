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
            var teams = (from t in _db.Team
                         join st in _db.SportTypeCategory on t.SportTypeID equals st.SportTypeID into gj
                         from st in gj.DefaultIfEmpty()
                         select new TeamViewModel
                         {
                             TeamID = t.TeamID,
                             TeamName = t.TeamName,
                             SportType = st != null ? st.SportTypeName : "未分類",
                             CreatedDate = t.CreatedDate
                         }).ToList();

            return View(teams);
        }
        #endregion

        #region 新增隊伍
        public ActionResult CreateTeam()
        {
            ViewBag.SportTypeList = new SelectList(_db.SportTypeCategory.ToList(), "SportTypeID", "SportTypeName");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateTeam(TeamViewModel model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.SportTypeList = new SelectList(_db.SportTypeCategory.ToList(), "SportTypeID", "SportTypeName", model.SportTypeID);
                return View(model);
            }

            var team = new Team
            {
                TeamName = model.TeamName,
                SportTypeID = model.SportTypeID,
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
                SportTypeID = team.SportTypeID
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
            team.SportTypeID = model.SportTypeID;
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
            try
            {
                var team = _db.Team.Find(id);
                if (team == null) return HttpNotFound();

                var model = new AssignTeamMembersViewModel
                {
                    TeamID = id,
                    TeamName = team.TeamName,
                    AllAthletes = _db.AthleteProfile.Select(a => new AthleteOption
                    {
                        AthleteID = a.AthleteID,
                        AthleteName = a.Name
                    }).ToList(),

                    AllTrainers = _db.TrainerProfile.Select(t => new TrainerOption
                    {
                        TrainerID = t.TrainerID,
                        TrainerName = t.ATName
                    }).ToList(),

                    SelectedAthleteIDs = _db.AthleteTeam.Where(at => at.TeamID == id).Select(at => at.AthleteID).ToList(),
                    SelectedTrainerIDs = _db.TrainerTeam.Where(tt => tt.TeamID == id).Select(tt => tt.TrainerID).ToList()
                };
                return View(model);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AssignMembers(AssignTeamMembersViewModel model)
        {
            try
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
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region 防護員對應的選手清單與隊伍資訊
        public ActionResult TrainerTeamDetails()
        {
            var trainerTeams = _db.TrainerProfile.Select(tp => new TrainerTeamInfoViewModel
            {
                TrainerID = tp.TrainerID,
                TrainerName = tp.ATName,
                Teams = (from tt in _db.TrainerTeam
                         where tt.TrainerID == tp.TrainerID
                         join team in _db.Team on tt.TeamID equals team.TeamID
                         select new TeamAthletesViewModel
                         {
                             TeamID = team.TeamID,
                             TeamName = team.TeamName,
                             AthleteNames = (from at in _db.AthleteTeam
                                             join ap in _db.AthleteProfile on at.AthleteID equals ap.AthleteID
                                             where at.TeamID == team.TeamID
                                             select ap.Name).ToList()
                         }).ToList()
            }).ToList();

            return View(trainerTeams);
        }
        #endregion
    }
}