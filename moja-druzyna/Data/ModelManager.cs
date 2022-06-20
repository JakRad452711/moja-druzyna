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

        public bool ScoutIsInTheTeam(string pesel, int teamId)
        {
            return _dbContext.ScoutTeam.Where(scoutTeam => scoutTeam.ScoutPeselScout == pesel && scoutTeam.TeamIdTeam == teamId).Count() > 0;
        }

        public string GetScoutPesel(string id)
        {
            return _dbContext.Scouts.Where(scout => scout.IdentityId == id).First().PeselScout;
        }

        public void CreateScoutAccount(int teamId, Scout scout)
        {
            if (!ScoutPrimaryKeyIsAvailable(scout.PeselScout))
                throw new ArgumentException("CreateScoutAccount: scout with such primary key already exists");

            Team team = _dbContext.Teams.Find(teamId);
            ScoutTeam newScoutTeam = new() { Role = "scout", Scout = scout, ScoutPeselScout = scout.PeselScout, Team = team, TeamIdTeam = team.IdTeam };
            Parent parent = scout.Parent;
            Address address = scout.Adress;

            if(parent == null)
            {
                List<Scout> scouts = new List<Scout>();
                scouts.Add(scout);

                parent = new Parent() { Pesel = scout.PeselScout, Scouts = scouts };
            }

            if (address == null)
            {
                address = new Address() { Scout = scout, ScoutPeselScout = scout.PeselScout, Parent = parent, ParentPesel = parent.Pesel };
            }
            

            scout.Adress = address;

            _dbContext.Scouts.Add(scout);
            _dbContext.ScoutTeam.Add(newScoutTeam);
            _dbContext.Adresses.Add(address);
            _dbContext.Parents.Add(parent);
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

            if (newScout.Adress != null)
            {
                Address address = _dbContext.Adresses.Where(_address => _address.ScoutPeselScout == newScout.PeselScout).First();

                if (newScout.Adress.AddressKor != null)
                    address.AddressKor = newScout.Adress.AddressKor;
                if (newScout.Adress.CityKor != null)
                    address.CityKor = newScout.Adress.CityKor;
                if (newScout.Adress.CountryKor != null)
                    address.CountryKor = newScout.Adress.CountryKor;
                if (newScout.Adress.NumberHouseKor != null)
                    address.NumberHouseKor = newScout.Adress.NumberHouseKor;
                if (newScout.Adress.StreetKor != null)
                    address.StreetKor = newScout.Adress.StreetKor;
                if (newScout.Adress.ZipKor != null)
                    address.ZipKor = newScout.Adress.ZipKor;
                if (newScout.Adress.AddressZam != null)
                    address.AddressZam = newScout.Adress.AddressZam;
                if (newScout.Adress.CityZam != null)
                    address.CityZam = newScout.Adress.CityZam;
                if (newScout.Adress.CountryZam != null)
                    address.CountryZam = newScout.Adress.CountryZam;
                if (newScout.Adress.NumberHouseZam != null)
                    address.NumberHouseZam = newScout.Adress.NumberHouseZam;
                if (newScout.Adress.StreetZam != null)
                    address.StreetZam = newScout.Adress.StreetZam;
                if (newScout.Adress.ZipZam != null)
                    address.ZipZam = newScout.Adress.ZipZam;

                _dbContext.Adresses.Update(address);
            }

            if (newScout.Parent != null)
            {
                Parent parent = _dbContext.Parents.Where(_parent => _parent.Pesel == newScout.ParentParentPesel).First();

                if (newScout.Parent.Adresses != null)
                    parent.Adresses = newScout.Parent.Adresses;
                if (newScout.Parent.Scouts != null)
                    parent.Scouts = newScout.Parent.Scouts;
                if (newScout.Parent.Name != null)
                    parent.Name = newScout.Parent.Name;
                if (newScout.Parent.Surname != null)
                    parent.Name = newScout.Parent.Surname;

                _dbContext.Parents.Update(parent);
            }

            _dbContext.Scouts.Update(oldScout);
            _dbContext.SaveChanges();
        }

        public List<Scout> GetScoutsFromATeam(int teamId)
        {
            List<string> peselsOfScoutsThatAreInTheTeam= _dbContext.ScoutTeam
                .Where(scoutTeam => scoutTeam.TeamIdTeam == teamId)
                .Select(scoutTeam => scoutTeam.ScoutPeselScout)
                .ToList();

            return _dbContext.Scouts.Where(scout => peselsOfScoutsThatAreInTheTeam.Contains(scout.PeselScout)).ToList();
        }

        public List<Host> GetListOfHostsFromATeam(int teamId)
        {
            List<Host> hostsInTheTeam = _dbContext.Hosts
                .Where(host => host.TeamIdTeam == teamId)
                .ToList();

            return hostsInTheTeam;
        }

        public List<Scout> GetScoutsFromAHost(int hostId)
        {
            List<string> peselsOfScoutsInTheHost = _dbContext.ScoutHost
                .Where(scoutHost => scoutHost.HostIdHost == hostId)
                .Select(scoutHost => scoutHost.ScoutPeselScout)
                .ToList();
            List<Scout> scoutsFromTheHost = _dbContext.Scouts
                .Where(scout => peselsOfScoutsInTheHost
                .Contains(scout.PeselScout))
                .ToList();

            return scoutsFromTheHost;
        }

        public List<Scout> GetScoutsFromATeamThatAreNotInAnyHostFromTheTeam(int teamId)
        {
            List<Host> hostsInTheTeam = _dbContext.Hosts
                .Where(host => host.TeamIdTeam == teamId)
                .ToList();
            List<int> hostIdsInTheTeam = hostsInTheTeam.Select(host => host.IdHost).ToList();

            List<Scout> scoutsInTheTeam = _dbContext.ScoutTeam
                .Where(scoutTeam => scoutTeam.TeamIdTeam == teamId && scoutTeam.Role == "scout")
                .Select(_scoutTeam => _scoutTeam.Scout)
                .ToList();
            List<string> scoutPeselsInTheTeam = scoutsInTheTeam.Select(scout => scout.PeselScout)
                .ToList();

            List<string> peselsOfScoutsThatAreInAHostInTheTeam = _dbContext.ScoutHost
                .Where(scoutHost => scoutPeselsInTheTeam.Contains(scoutHost.ScoutPeselScout) && hostIdsInTheTeam.Contains(scoutHost.HostIdHost))
                .Select(scoutHost => scoutHost.ScoutPeselScout)
                .ToList();

            List<Scout> scoutsThatAreNotInAnyHostFromTheTeam = scoutsInTheTeam
                .Where(scout => !peselsOfScoutsThatAreInAHostInTheTeam.Contains(scout.PeselScout))
                .ToList();

            return scoutsThatAreNotInAnyHostFromTheTeam;
        }
    }
}
