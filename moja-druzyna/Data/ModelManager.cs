using moja_druzyna.Models;
using System;
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

        public void CreateScoutAccount(int teamId, Scout scout)
        {
            if (!ScoutPrimaryKeyIsAvailable(scout.PeselScout))
                throw new ArgumentException("CreateScoutAccount: scout with such primary key already exists");

            Team team = _dbContext.Teams.Find(teamId);
            ScoutTeam newScoutTeam = new() { Role = "scout", Scout = scout, ScoutPeselScout = scout.PeselScout, Team = team, TeamIdTeam = team.IdTeam };

            _dbContext.Scouts.Add(scout);
            _dbContext.ScoutTeam.Add(newScoutTeam);
            _dbContext.SaveChanges();
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

            newTeam.ScoutTeam.Add(scoutTeam);
            scoutCaptain.ScoutTeam.Add(scoutTeam);

            _dbContext.Scouts.Add(scoutCaptain);
            _dbContext.Teams.Add(newTeam);
            _dbContext.ScoutTeam.Add(scoutTeam);
            _dbContext.SaveChanges();
        }

        public void EditScout(Scout newScout)
        {
            Scout oldScout = _dbContext.Scouts.Find(newScout.PeselScout);

            if (newScout.Name != null)
                oldScout.Name = newScout.Name;
            if (newScout.Surname != null)
                oldScout.Surname = newScout.Surname;
            if (newScout.SecondName != null)
                oldScout.SecondName = newScout.SecondName;
            if (newScout.MembershipNumber != null)
                oldScout.MembershipNumber = newScout.MembershipNumber;
            if (newScout.DateOfBirth != null)
                oldScout.DateOfBirth = newScout.DateOfBirth;
            if (newScout.Nationality != null)
                oldScout.Nationality = newScout.Nationality;
            oldScout.Ns = newScout.Ns;
            if (newScout.DateOfEntry != null)
                oldScout.DateOfEntry = newScout.DateOfEntry;
            if (newScout.DateOfLeaving != null)
                oldScout.DateOfLeaving = newScout.DateOfLeaving;
            if (newScout.ParentParentPesel != null)
                oldScout.ParentParentPesel = newScout.ParentParentPesel;
            if (newScout.Adress != null)
            {
                if (newScout.Adress.AddressKor != null)
                    oldScout.Adress.AddressKor = newScout.Adress.AddressKor;
                if (newScout.Adress.CityKor != null)
                    oldScout.Adress.CityKor = newScout.Adress.CityKor;
                if (newScout.Adress.CountryKor != null)
                    oldScout.Adress.CountryKor = newScout.Adress.CountryKor;
                if (newScout.Adress.NumberHouseKor != null)
                    oldScout.Adress.NumberHouseKor = newScout.Adress.NumberHouseKor;
                if (newScout.Adress.StreetKor != null)
                    oldScout.Adress.StreetKor = newScout.Adress.StreetKor;
                if (newScout.Adress.ZipKor != null)
                    oldScout.Adress.ZipKor = newScout.Adress.ZipKor;
                if (newScout.Adress.AddressZam != null)
                    oldScout.Adress.AddressZam = newScout.Adress.AddressZam;
                if (newScout.Adress.CityZam != null)
                    oldScout.Adress.CityZam = newScout.Adress.CityZam;
                if (newScout.Adress.CountryZam != null)
                    oldScout.Adress.CountryZam = newScout.Adress.CountryZam;
                if (newScout.Adress.NumberHouseZam != null)
                    oldScout.Adress.NumberHouseZam = newScout.Adress.NumberHouseZam;
                if (newScout.Adress.StreetZam != null)
                    oldScout.Adress.StreetZam = newScout.Adress.StreetZam;
                if (newScout.Adress.ZipZam != null)
                    oldScout.Adress.ZipZam = newScout.Adress.ZipZam;
            }


            _dbContext.Scouts.Update(oldScout);
            _dbContext.SaveChanges();
        }
    }
}
