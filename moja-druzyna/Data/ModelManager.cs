using moja_druzyna.Const;
using moja_druzyna.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace moja_druzyna.Data
{
    public class ModelManager
    {
        private readonly ApplicationDbContext _dbContext;

        public ModelManager(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public bool ScoutPrimaryKeyIsAvailable(string pesel)
        {
            return _dbContext.Scouts.Where(_scout => _scout.PeselScout == pesel).Count() == 0;
        }

        public void CreateScoutCaptainWithTeam(Scout scoutCaptain)
        {
            if (!ScoutPrimaryKeyIsAvailable(scoutCaptain.PeselScout))
                throw new ArgumentException("CreateScoutAccount: scout with such primary key already exists");

            Team newTeam = new()
            {
                Name = string.Format("Drużyna - {0} {1}", scoutCaptain.Name, scoutCaptain.Surname)
            };

            ScoutTeam scoutTeam = new ScoutTeam()
            {
                Role = "captain",
                Scout = scoutCaptain,
                ScoutPeselScout = scoutCaptain.PeselScout,
                Team = newTeam
            };

            Parent parent = scoutCaptain.Parent;
            Address address = scoutCaptain.Adress;

            if (parent == null)
            {
                List<Scout> scouts = new List<Scout>();
                scouts.Add(scoutCaptain);

                parent = new Parent() { Pesel = scoutCaptain.PeselScout, Scouts = scouts };
            }

            if (address == null)
            {
                address = new Address() { Scout = scoutCaptain, ScoutPeselScout = scoutCaptain.PeselScout, Parent = parent, ParentPesel = parent.Pesel };
            }

            scoutCaptain.Adress = address;

            newTeam.ScoutTeam.Add(scoutTeam);
            scoutCaptain.ScoutTeam.Add(scoutTeam);

            _dbContext.Scouts.Add(scoutCaptain);
            _dbContext.Teams.Add(newTeam);
            _dbContext.ScoutTeam.Add(scoutTeam);
            _dbContext.Adresses.Add(address);
            _dbContext.Parents.Add(parent);
            _dbContext.SaveChanges();
        }

#warning temporary solution
        public void InitializeDbValues()
        {
            if (_dbContext.Achievements.Any() && _dbContext.Ranks.Any())
                return;

            List<string> achievementNames = new List<string>()
            {
                ScoutAbilities.Hygenist,
                ScoutAbilities.Paramedic,
                ScoutAbilities.Lifesaver,
                ScoutAbilities.Glimmer,
                ScoutAbilities.FireGuard,
                ScoutAbilities.FireplaceMaster,
                ScoutAbilities.DrillExpert,
                ScoutAbilities.DrillMaster,
                ScoutAbilities.Needle,
                ScoutAbilities.Tailor,
                ScoutAbilities.YoungSwimmer,
                ScoutAbilities.Swimmer,
                ScoutAbilities.ExcellentSwimmer,
                ScoutAbilities.Internaut,
                ScoutAbilities.FamilyHistorian,
                ScoutAbilities.European,
                ScoutAbilities.HealthLeader,
                ScoutAbilities.NatureFriend,
                ScoutAbilities.Photograph
            };

            List<string> rankNames = new List<string>()
            {
                ScoutRanks.Rank1,
                ScoutRanks.Rank2,
                ScoutRanks.Rank3,
                ScoutRanks.Rank4,
                ScoutRanks.Rank5,
                ScoutRanks.Rank6,
            };

            foreach (string name in achievementNames)
            {
                Achievement achievement = new Achievement() { Type = name };

                if (_dbContext.Achievements.FirstOrDefault(a => a.Type == achievement.Type) == null)
                {
                    _dbContext.Achievements.Add(achievement);
                    _dbContext.SaveChanges();
                }
            }

            foreach (string name in rankNames)
            {
                Rank rank = new Rank()
                {
                    Name = name,
                };

                if(_dbContext.Ranks.FirstOrDefault(r => r.Name == name) == null)
                {
                    _dbContext.Ranks.Add(rank);
                    _dbContext.SaveChanges();
                }
            }
        }
    }
}
