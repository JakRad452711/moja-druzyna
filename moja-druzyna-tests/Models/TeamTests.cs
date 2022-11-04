using Microsoft.EntityFrameworkCore;
using moja_druzyna.Const;
using moja_druzyna.Data;
using moja_druzyna.Lib.Exceptions;
using moja_druzyna.Lib.Order;
using moja_druzyna.Models;
using moja_druzyna_tests.TestLib;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace moja_druzyna_tests.Models
{
    public class TeamTests : IDisposable
    {
        private static string dbName = "TeamTestsDb";
        private static DbContextOptions<ApplicationDbContext> options =
            new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: dbName)
            .Options;
        private static TestObjectComparerFactory objectComparerFactory = new TestObjectComparerFactory();

        ApplicationDbContext dbContext = new ApplicationDbContext(options);

        public TeamTests()
        {
            dbContext.ScoutRanks.RemoveRange(dbContext.ScoutRanks);
            dbContext.ScoutAchievements.RemoveRange(dbContext.ScoutAchievements);
            dbContext.ScoutHost.RemoveRange(dbContext.ScoutHost);
            dbContext.ScoutTeam.RemoveRange(dbContext.ScoutTeam);
            dbContext.Ranks.RemoveRange(dbContext.Ranks);
            dbContext.Achievements.RemoveRange(dbContext.Achievements);
            dbContext.Scouts.RemoveRange(dbContext.Scouts);
            dbContext.Hosts.RemoveRange(dbContext.Hosts);
            dbContext.Teams.RemoveRange(dbContext.Teams);
            dbContext.SaveChanges();
            SampleDataset sampleDataset = new SampleDataset();
            dbContext.Teams.AddRange(new SampleDataset.SampleTeams().Teams);
            dbContext.Hosts.Add(new SampleDataset.SampleHosts().TeamAlphaHost1);
            dbContext.Hosts.Add(new SampleDataset.SampleHosts().TeamAlphaHost2);
            dbContext.Scouts.AddRange(new SampleDataset.SampleScouts().Scouts);
            dbContext.Achievements.AddRange(new SampleDataset.SampleAchievements().Achievements);
            dbContext.Ranks.AddRange(new SampleDataset.SampleRanks().Ranks);
            dbContext.ScoutTeam.AddRange(sampleDataset.ScoutTeams);
            dbContext.ScoutHost.AddRange(sampleDataset.ScoutHosts);
            dbContext.ScoutAchievements.AddRange(sampleDataset.ScoutAchievements);
            dbContext.ScoutRanks.AddRange(sampleDataset.ScoutRanks);
            dbContext.SaveChanges();
        }

        public void Dispose()
        {
            dbContext.ScoutRanks.RemoveRange(dbContext.ScoutRanks);
            dbContext.ScoutAchievements.RemoveRange(dbContext.ScoutAchievements);
            dbContext.ScoutHost.RemoveRange(dbContext.ScoutHost);
            dbContext.ScoutTeam.RemoveRange(dbContext.ScoutTeam);
            dbContext.Ranks.RemoveRange(dbContext.Ranks);
            dbContext.Achievements.RemoveRange(dbContext.Achievements);
            dbContext.Scouts.RemoveRange(dbContext.Scouts);
            dbContext.Hosts.RemoveRange(dbContext.Hosts);
            dbContext.Teams.RemoveRange(dbContext.Teams);
            dbContext.Adresses.RemoveRange(dbContext.Adresses);
            dbContext.OrderInfos.RemoveRange(dbContext.OrderInfos);
            dbContext.Orders.RemoveRange(dbContext.Orders);
            dbContext.SaveChanges();
        }

        public static IEnumerable<object[]> GetTeams()
        {
            foreach (Team team in new SampleDataset.SampleTeams().Teams)
                yield return new object[] { team };
        }

        public static IEnumerable<object[]> GetScouts()
        {
            foreach (Scout scout in new SampleDataset.SampleScouts().Scouts)
                yield return new object[] { scout };
        }

        [Theory]
        [MemberData(nameof(GetTeams))]
        public void GetTeam_ShouldReturnProperTeam(Team team)
        {
            ObjectsComparer.Comparer<Team> comparer = 
                objectComparerFactory.CreateShallowObjectComparer<Team>();
            IEnumerable<ObjectsComparer.Difference> differences;
            Team expected = team;

            Team actual = Team.GetTeam(dbContext, team.IdTeam);

            Assert.True(comparer.Compare(expected, actual, out differences));
        }

        [Fact]
        public void GetTeam_ShouldThrowArgumentNullExcpetion_WhenDbContextArgumentIsNull()
        {
            int teamId = new SampleDataset.SampleTeams().TeamAlpha.IdTeam;
            Type expected = new ArgumentNullException().GetType();

            Func<Team> actual = () => Team.GetTeam(null, teamId);

            Assert.Throws(expected, actual);
        }

        [Fact]
        public void GetTeam_ShouldThrowRecordNotFoundException_WhenTeamWithGivenIdDoesntExist()
        {
            int invalidId = -1;
            Type expected = new RecordNotFoundException().GetType();

            Func<Team> actual = () => Team.GetTeam(dbContext, invalidId);

            Assert.Throws(expected, actual);
        }

        [Fact]
        public void ScoutHasOneOfRoles_ShouldReturnTrue_WhenScoutHasOneOfRoles()
        {
            Team team = Team.GetTeam(dbContext, new SampleDataset.SampleTeams().TeamAlpha.IdTeam);
            string pesel = new SampleDataset.SampleScouts().TeamAlphaCaptain.PeselScout;
            List<string> listOfRolesOtherThanCaptainRole = TeamRoles.TeamRolesList.Where(tr => tr != TeamRoles.Captain).ToList();

            bool scoutIsCaptain = team.ScoutHasOneOfRoles(pesel, new() { TeamRoles.Captain });
            bool scoutHasAnyOtherRoleThanCaptain = team.ScoutHasOneOfRoles(pesel, listOfRolesOtherThanCaptainRole);
            bool scoutIsCaptainOrQuatermasterOrScout = team.ScoutHasOneOfRoles(pesel, new() { TeamRoles.Scout, TeamRoles.Captain, TeamRoles.Quatermaster });
            bool scoutHasNoRole = team.ScoutHasOneOfRoles(pesel, new());

            Assert.True(scoutIsCaptain);
            Assert.False(scoutHasAnyOtherRoleThanCaptain);
            Assert.True(scoutIsCaptainOrQuatermasterOrScout);
            Assert.False(scoutHasNoRole);
        }

        [Fact]
        public void ScoutHasOneOfRoles_ShouldReturnFalse_WhenScoutIsFromOtherTeam()
        {
            Team team = Team.GetTeam(dbContext, new SampleDataset.SampleTeams().TeamAlpha.IdTeam);
            string pesel = new SampleDataset.SampleScouts().TeamBetaCaptain.PeselScout;
            List<string> listOfRolesOtherThanCaptainRole = TeamRoles.TeamRolesList.Where(tr => tr != TeamRoles.Captain).ToList();

            bool scoutIsCaptain = team.ScoutHasOneOfRoles(pesel, new() { TeamRoles.Captain });
            bool scoutHasAnyOtherRoleThanCaptain = team.ScoutHasOneOfRoles(pesel, listOfRolesOtherThanCaptainRole);
            bool scoutIsCaptainOrQuatermasterOrScout = team.ScoutHasOneOfRoles(pesel, new() { TeamRoles.Scout, TeamRoles.Captain, TeamRoles.Quatermaster });
            bool scoutHasNoRole = team.ScoutHasOneOfRoles(pesel, new());

            Assert.False(scoutIsCaptain);
            Assert.False(scoutHasAnyOtherRoleThanCaptain);
            Assert.False(scoutIsCaptainOrQuatermasterOrScout);
            Assert.False(scoutHasNoRole);
        }

        [Fact]
        public void CreateScout_ShouldCreateANewScoutInTheTeam()
        {
            ObjectsComparer.Comparer<Scout> comparer = objectComparerFactory.CreateShallowObjectComparer<Scout>();
            ObjectsComparer.Comparer<Address> addressComparer = objectComparerFactory.CreateShallowObjectComparer<Address>();
            ObjectsComparer.Comparer<ScoutTeam> scoutTeamComparer = objectComparerFactory.CreateShallowObjectComparer<ScoutTeam>();
            IEnumerable<ObjectsComparer.Difference> differences;
            int teamId = new SampleDataset.SampleTeams().TeamAlpha.IdTeam;
            Team team = Team.GetTeam(dbContext, teamId);
            Address adress = new()
            {
                ScoutPeselScout = "00520135879",
                ParentPesel = "00520135879",
                CountryZam = "Ruia",
                CityZam = "Oyran",
                StreetZam = "Soian",
                NumberHouseZam = "5"
            };
            Scout created = new()
            {
                PeselScout = "00520135879",
                Name = "Michael",
                Surname = "Goodbrand",
                Adress = adress,
                MembershipNumber = "pl-1203"
            };
            Address expectedAdress = new()
            {
                ScoutPeselScout = "00520135879",
                ParentPesel = "00520135879",
                CountryZam = "Ruia",
                CityZam = "Oyran",
                StreetZam = "Soian",
                NumberHouseZam = "5"
            };
            Scout expected = new()
            {
                PeselScout = "00520135879",
                Name = "Michael",
                Surname = "Goodbrand",
                Adress = adress,
                MembershipNumber = "pl-1203"
            };
            ScoutTeam expectedScoutTeam = new ScoutTeam()
            {
                ScoutPeselScout = "00520135879",
                TeamIdTeam = teamId,
                Role = TeamRoles.Scout
            };

            team.CreateScout(created);
            
            Scout actual = Scout.GetScout(dbContext, created.PeselScout);
            ScoutTeam actualScoutTeam = actual.ScoutTeam.First();
            Assert.True(comparer.Compare(expected, actual, out differences));
            Assert.True(addressComparer.Compare(expectedAdress, actual.Adress, out differences));
            Assert.True(scoutTeamComparer.Compare(expectedScoutTeam, actualScoutTeam, out differences));
        }

        [Theory]
        [MemberData(nameof(GetScouts))]
        public void CreateScout_ShouldThrowArgumentException_WhenScoutWithGivenPeselAlreadyExists(Scout scout)
        {
            Team team = Team.GetTeam(dbContext, new SampleDataset.SampleTeams().TeamAlpha.IdTeam);
            Scout created = new Scout()
            {
                PeselScout = scout.PeselScout,
                Name = "not null",
                Surname = "not null"
            };
            Type expected = new ArgumentException().GetType();

            Action actual = () => team.CreateScout(created);

            Assert.Throws(expected, actual);
        }

        [Fact]
        public void CreateHost_ShouldCreateANewHostInTheTeam()
        {
            ObjectsComparer.Comparer<Host> comparer = objectComparerFactory.CreateShallowObjectComparer<Host>();
            IEnumerable<ObjectsComparer.Difference> differences;
            int teamId = new SampleDataset.SampleTeams().TeamAlpha.IdTeam;
            string pesel = new SampleDataset.SampleScouts().TeamAlphaRegularScout.PeselScout;
            Team team = Team.GetTeam(dbContext, teamId);
            Host created = new()
            {
                Name = "Squirrels",
                TeamIdTeam = teamId
            };
            Host expected = new()
            {
                Name = "Squirrels",
                TeamIdTeam = teamId
            };

            team.CreateHost(created, pesel);
            
            Host actual = dbContext.Hosts.First(h => h.Name == "Squirrels");
            ScoutHost actualScoutHost = dbContext.ScoutHost.First(sh => sh.HostIdHost == actual.IdHost);
            expected.IdHost = actual.IdHost;
            Assert.True(comparer.Compare(expected, actual, out differences));
            Assert.Equal(expected.TeamIdTeam, actual.TeamIdTeam);
            Assert.Equal(expected.IdHost, actualScoutHost.HostIdHost);
            Assert.Equal(pesel, actualScoutHost.ScoutPeselScout);
            Assert.Equal(HostRoles.HostCaptain, actualScoutHost.Role);
        }

        [Fact]
        public void CreateHost_ShouldThrowArgumentException_WhenScoutWithGivenPeselHasASpecialRoleInTheTeam()
        {
            string scoutQuatermasterPesel = new SampleDataset.SampleScouts().TeamAlphaQuatermaster.PeselScout;
            Type expected = new ArgumentException().GetType();
            Team team = Team.GetTeam(dbContext, new SampleDataset.SampleTeams().TeamAlpha.IdTeam);

            Action actual = () => team.CreateHost(new Host() { Name = "not null" }, scoutQuatermasterPesel);

            Assert.Throws(expected, actual);
        }

        [Fact]
        public void Appoint_ShouldChangeScoutsRoleInTeamProperly()
        {
            int teamId = new SampleDataset.SampleTeams().TeamAlpha.IdTeam;
            Team team = Team.GetTeam(dbContext, teamId);
            SampleDataset.SampleScouts sampleScouts = new SampleDataset.SampleScouts();
            string quatermasterPesel = sampleScouts.TeamAlphaQuatermaster.PeselScout;
            string regularScoutPesel = sampleScouts.TeamAlphaRegularScout.PeselScout;
            string hostCaptainPesel = sampleScouts.TeamAlphaHost1Captain.PeselScout;
            Appointment appointScoutAToQuatermaster = new()
            {
                ScoutPesel = quatermasterPesel,
                Role = TeamRoles.ViceCaptain
            };
            Appointment appointScoutAToChronicler = new()
            {
                ScoutPesel = quatermasterPesel,
                Role = TeamRoles.Chronicler
            };
            Appointment appointScoutBToViceCaptain = new()
            {
                ScoutPesel = regularScoutPesel,
                Role = TeamRoles.ViceCaptain
            };
            Appointment appointScoutCToQuatermaster = new()
            {
                ScoutPesel = hostCaptainPesel,
                Role = TeamRoles.Quatermaster
            };

            team.Appoint(appointScoutAToQuatermaster);
            team.Appoint(appointScoutAToChronicler);
            team.Appoint(appointScoutBToViceCaptain);
            team.Appoint(appointScoutCToQuatermaster);

            ScoutTeam quatermasterST = dbContext.ScoutTeam.First(st => st.TeamIdTeam == teamId && st.ScoutPeselScout == quatermasterPesel);
            ScoutTeam regularScoutST = dbContext.ScoutTeam.First(st => st.TeamIdTeam == teamId && st.ScoutPeselScout == regularScoutPesel);
            ScoutTeam hostCaptainST = dbContext.ScoutTeam.First(st => st.TeamIdTeam == teamId && st.ScoutPeselScout == hostCaptainPesel);
            int hostId = team.GetScoutsHost(hostCaptainPesel).IdHost;
            ScoutHost hostCaptainSH = dbContext.ScoutHost.First(sh => sh.HostIdHost == hostId && sh.ScoutPeselScout == hostCaptainPesel);
            Assert.Equal(TeamRoles.Chronicler, quatermasterST.Role);
            Assert.Equal(TeamRoles.ViceCaptain, regularScoutST.Role);
            Assert.Equal(TeamRoles.Quatermaster, hostCaptainST.Role);
            Assert.Equal(HostRoles.Scout, hostCaptainSH.Role);
            Assert.Empty(dbContext.ScoutHost.Where(sh => sh.HostIdHost == hostId && sh.Role == HostRoles.HostCaptain));
        }

#warning add also checking if method will fail for captain and scouts not from the team!
        [Fact]
        public void Appoint_ShouldRemoveHostCaptainFromHisCurrentHost_WhenAppointingHostCaptainToHostCaptainOfANewHost()
        {
            int teamId = new SampleDataset.SampleTeams().TeamAlpha.IdTeam;
            Team team = Team.GetTeam(dbContext, teamId);
            SampleDataset.SampleScouts sampleScouts = new SampleDataset.SampleScouts();
            string host1CaptainPesel = sampleScouts.TeamAlphaHost1Captain.PeselScout;
            int newHostId = new SampleDataset.SampleHosts().TeamAlphaHost2.IdHost;
            Appointment appointHostCaptainToHostCaptainOfAnotherHost = new()
            {
                ScoutPesel = host1CaptainPesel,
                Role = TeamRoles.HostCaptain,
                Host = newHostId.ToString()
            };
            
            team.Appoint(appointHostCaptainToHostCaptainOfAnotherHost);

            Host newHost = Host.GetHost(dbContext, newHostId);
            Host oldHost = Host.GetHost(dbContext, new SampleDataset.SampleHosts().TeamAlphaHost1.IdHost);
            Assert.Null(oldHost.GetCaptain());
            Assert.Null(oldHost.GetScoutRole(host1CaptainPesel));
            Assert.Equal(host1CaptainPesel, newHost.GetCaptain().PeselScout);
            Assert.Equal(TeamRoles.HostCaptain, team.GetScoutRole(host1CaptainPesel));
            Assert.Single(newHost.ScoutHost.Where(sh => sh.Role == HostRoles.HostCaptain));
        }

        [Fact]
        public void Appoint_ShouldChangeNothing_WhenAppointingHostCaptainToHostCaptainOfHisOwnHost()
        {
            int teamId = new SampleDataset.SampleTeams().TeamAlpha.IdTeam;
            Team team = Team.GetTeam(dbContext, teamId);
            SampleDataset.SampleScouts sampleScouts = new SampleDataset.SampleScouts();
            string host1CaptainPesel = sampleScouts.TeamAlphaHost1Captain.PeselScout;
            int hostId = new SampleDataset.SampleHosts().TeamAlphaHost1.IdHost;
            Appointment reappointHostCaptainToHostCaptainOfHisOwnHost = new()
            {
                ScoutPesel = host1CaptainPesel,
                Role = TeamRoles.HostCaptain,
                Host = hostId.ToString()
            };

            team.Appoint(reappointHostCaptainToHostCaptainOfHisOwnHost);

            Host host = Host.GetHost(dbContext, hostId);
            Assert.Equal(host1CaptainPesel, host.GetCaptain().PeselScout);
            Assert.Equal(TeamRoles.HostCaptain, team.GetScoutRole(host1CaptainPesel));
            Assert.Single(host.ScoutHost.Where(sh => sh.Role == HostRoles.HostCaptain));
        }

        [Fact]
        public void Appoint_ShouldChangeHostCaptainToANewOne_WhenAppointingRegularScoutToHostCaptainRole()
        {
            int teamId = new SampleDataset.SampleTeams().TeamAlpha.IdTeam;
            Team team = Team.GetTeam(dbContext, teamId);
            SampleDataset.SampleScouts sampleScouts = new SampleDataset.SampleScouts();
            string host1ScoutPesel = sampleScouts.TeamAlphaHost1Captain.PeselScout;
            int hostId = new SampleDataset.SampleHosts().TeamAlphaHost1.IdHost;
            Appointment apointScoutAsAHostCaptain = new()
            {
                ScoutPesel = host1ScoutPesel,
                Role = TeamRoles.HostCaptain,
                Host = hostId.ToString()
            };

            team.Appoint(apointScoutAsAHostCaptain);

            Host host = Host.GetHost(dbContext, hostId);
            Assert.Equal(host1ScoutPesel, host.GetCaptain().PeselScout);
            Assert.Equal(TeamRoles.HostCaptain, team.GetScoutRole(host1ScoutPesel));
            Assert.Single(host.ScoutHost.Where(sh => sh.Role == HostRoles.HostCaptain));
        }

        [Fact]
        public void Appoint_ShouldThrowUnauthorizedAccessException_WhenTeamCaptainIsAppointedOrSomeoneIsAppointedAsATeamCaptain()
        {
            int teamId = new SampleDataset.SampleTeams().TeamAlpha.IdTeam;
            Team team = Team.GetTeam(dbContext, teamId);
            Type expected = new UnauthorizedAccessException().GetType();
            SampleDataset.SampleScouts sampleScouts = new SampleDataset.SampleScouts();
            string teamCaptainPesel = sampleScouts.TeamAlphaCaptain.PeselScout;
            string scoutPesel = sampleScouts.TeamAlphaRegularScout.PeselScout;
            Appointment appointTeamCaptain = new()
            {
                ScoutPesel = teamCaptainPesel,
                Role = TeamRoles.Quatermaster
            };
            Appointment appointSomeoneAsATeamCaptain = new()
            {
                ScoutPesel = scoutPesel,
                Role = TeamRoles.Captain
            };

            Action actual1 = () => team.Appoint(appointTeamCaptain);
            Action actual2 = () => team.Appoint(appointSomeoneAsATeamCaptain);

            Assert.Throws(expected, actual1);
            Assert.Throws(expected, actual2);
        }

        [Fact]
        public void Appoint_ShouldThrowUnauthorizedAccessException_WhenScoutNotFromTheTeamIsAppointed()
        {
            int teamId = new SampleDataset.SampleTeams().TeamAlpha.IdTeam;
            Team team = Team.GetTeam(dbContext, teamId);
            Type expected = new UnauthorizedAccessException().GetType();
            SampleDataset.SampleScouts sampleScouts = new SampleDataset.SampleScouts();
            string teamBetaQuatermasterPesel = sampleScouts.TeamBetaQuatermaster.PeselScout;
            Appointment invalidAppointment = new()
            {
                ScoutPesel = teamBetaQuatermasterPesel,
                Role = TeamRoles.Quatermaster
            };

            Action actual = () => team.Appoint(invalidAppointment);

            Assert.Throws(expected, actual);
        }

        [Fact]
        public void Layoff_ShouldChangeScoutsRoleInTeamProperly()
        {
            int teamId = new SampleDataset.SampleTeams().TeamAlpha.IdTeam;
            Team team = Team.GetTeam(dbContext, teamId);
            int hostId = new SampleDataset.SampleHosts().TeamAlphaHost1.IdHost;
            SampleDataset.SampleScouts sampleScouts = new SampleDataset.SampleScouts();
            string quatermasterPesel = sampleScouts.TeamAlphaQuatermaster.PeselScout;
            string host1CaptainPesel = sampleScouts.TeamAlphaHost1Captain.PeselScout;
            Layoff layoffQuatermaster = new()
            {
                ScoutPesel = quatermasterPesel,
                Role = TeamRoles.Quatermaster
            };
            Layoff layoffHost1Captain = new()
            {
                ScoutPesel = host1CaptainPesel,
                Role = TeamRoles.HostCaptain,
                Host = hostId.ToString()
            };

            team.Layoff(layoffQuatermaster);
            team.Layoff(layoffHost1Captain);

            Host host = Host.GetHost(dbContext, hostId);
            Assert.Equal(TeamRoles.Scout, team.GetScoutRole(quatermasterPesel));
            Assert.Equal(TeamRoles.Scout, team.GetScoutRole(host1CaptainPesel));
            Assert.Equal(HostRoles.Scout, host.GetScoutRole(host1CaptainPesel));
            Assert.Empty(host.ScoutHost.Where(sh => sh.Role == HostRoles.HostCaptain));
        }

        [Fact]
        public void Layoff_ShouldThrowUnauthorizedAccessException_WhenTeamCaptainIsLayedOff()
        {
            int teamId = new SampleDataset.SampleTeams().TeamAlpha.IdTeam;
            Team team = Team.GetTeam(dbContext, teamId);
            Type expected = new UnauthorizedAccessException().GetType();
            SampleDataset.SampleScouts sampleScouts = new SampleDataset.SampleScouts();
            string teamCaptainPesel = sampleScouts.TeamAlphaCaptain.PeselScout;
            Layoff layoffCaptain = new()
            {
                ScoutPesel = teamCaptainPesel,
                Role = TeamRoles.Quatermaster
            };

            Action actual = () => team.Layoff(layoffCaptain);

            Assert.Throws(expected, actual);
        }

        [Fact]
        public void Layoff_ShouldThrowLayoffRoleMismatchExcpetion_WhenLayoffRoleDoesntMatchScoutsRoleInTheTeam()
        {
            int teamId = new SampleDataset.SampleTeams().TeamAlpha.IdTeam;
            Team team = Team.GetTeam(dbContext, teamId);
            Type expected = new LayoffRoleMismatchException().GetType();
            SampleDataset.SampleScouts sampleScouts = new SampleDataset.SampleScouts();
            string quatermasterPesel = sampleScouts.TeamAlphaQuatermaster.PeselScout;
            Layoff invalidLayoff = new()
            {
                ScoutPesel = quatermasterPesel,
                Role = TeamRoles.ViceCaptain
            };

            Action actual = () => team.Layoff(invalidLayoff);

            Assert.Throws(expected, actual);
        }

        [Fact]
        public void Layoff_ShouldThrowUnauthorizedAccessException_WhenScoutNotFromTheTeamIsLayoffed()
        {
            int teamId = new SampleDataset.SampleTeams().TeamAlpha.IdTeam;
            Team team = Team.GetTeam(dbContext, teamId);
            Type expected = new UnauthorizedAccessException().GetType();
            SampleDataset.SampleScouts sampleScouts = new SampleDataset.SampleScouts();
            string quatermasterFromTeamBetaPesel = sampleScouts.TeamBetaQuatermaster.PeselScout;
            Layoff invalidLayoff = new()
            {
                ScoutPesel = quatermasterFromTeamBetaPesel,
                Role = TeamRoles.Quatermaster
            };

            string role = team.GetScoutRole(quatermasterFromTeamBetaPesel);

            Action actual = () => team.Layoff(invalidLayoff);

            Assert.Throws(expected, actual);
        }

        [Fact]
        public void CloseATrial_ShouldAddARankForAScout_WhenTrialTypeIsRankAndTheScoutDidntHaveTheRank()
        {
            int teamId = new SampleDataset.SampleTeams().TeamAlpha.IdTeam;
            Team team = Team.GetTeam(dbContext, teamId);
            SampleDataset.SampleScouts sampleScouts = new SampleDataset.SampleScouts();
            string quatermasterPesel = sampleScouts.TeamAlphaQuatermaster.PeselScout;
            TrialClosing rankTrialClosing = new()
            {
                ScoutPesel = quatermasterPesel,
                TrialType = TrialTypes.Rank,
                Rank = ScoutRanks.Rank2
            };
            List<ScoutRank> before = dbContext.ScoutRanks.Where(sr => sr.ScoutPeselScout == quatermasterPesel).ToList();
            Assert.Single(before.Where(sr => sr.IsCurrent));

            team.CloseATrial(rankTrialClosing);

            List<ScoutRank> after = dbContext.ScoutRanks.Where(sr => sr.ScoutPeselScout == quatermasterPesel).ToList();
            Assert.Single(after.Where(sr => sr.IsCurrent));
            Assert.Equal(before.Count(), after.Count() - 1);
            Assert.NotEmpty(after.Where(sr => sr.RankName == ScoutRanks.Rank2 && sr.IsCurrent));
        }

        [Fact]
        public void CloseATrial_ShouldSetRankAsCurrent_WhenTrialTypeIsRankAndTheScoutHadTheRank()
        {
            int teamId = new SampleDataset.SampleTeams().TeamAlpha.IdTeam;
            Team team = Team.GetTeam(dbContext, teamId);
            SampleDataset.SampleScouts sampleScouts = new SampleDataset.SampleScouts();
            string quatermasterPesel = sampleScouts.TeamAlphaQuatermaster.PeselScout;
            TrialClosing rank1 = new()
            {
                ScoutPesel = quatermasterPesel,
                TrialType = TrialTypes.Rank,
                Rank = ScoutRanks.Rank1
            };
            TrialClosing rank2 = new()
            {
                ScoutPesel = quatermasterPesel,
                TrialType = TrialTypes.Rank,
                Rank = ScoutRanks.Rank2
            };
            team.CloseATrial(rank1);
            team.CloseATrial(rank2);
            List<ScoutRank> before = dbContext.ScoutRanks.Where(sr => sr.ScoutPeselScout == quatermasterPesel).ToList();
            Assert.Single(before.Where(sr => sr.IsCurrent));
            Assert.Equal(ScoutRanks.Rank2, before.First(sr => sr.IsCurrent).RankName);

            team.CloseATrial(rank1);

            List<ScoutRank> after = dbContext.ScoutRanks.Where(sr => sr.ScoutPeselScout == quatermasterPesel).ToList();
            Assert.Single(after.Where(sr => sr.IsCurrent));
            Assert.Equal(before.Count(), after.Count());
            Assert.NotEmpty(after.Where(sr => sr.RankName == ScoutRanks.Rank1 && sr.IsCurrent));
        }

        [Fact]
        public void CloseATrial_ShouldAddAnAbility_WhenTrialTypeIsAbilityAndScoutDoesntHaveTheGivenAbility()
        {
            int teamId = new SampleDataset.SampleTeams().TeamAlpha.IdTeam;
            Team team = Team.GetTeam(dbContext, teamId);
            SampleDataset.SampleScouts sampleScouts = new SampleDataset.SampleScouts();
            string quatermasterPesel = sampleScouts.TeamAlphaQuatermaster.PeselScout;
            int abilityId = new SampleDataset.SampleAchievements().FireGuard.IdAchievement;
            TrialClosing abilityTrialClosing = new()
            {
                ScoutPesel = quatermasterPesel,
                TrialType = TrialTypes.Ability,
                Ability = abilityId.ToString()
            };
            List<ScoutAchievement> before = dbContext.ScoutAchievements.Where(sa => sa.ScoutPeselScout == quatermasterPesel).ToList();
            Assert.Empty(before.Where(sa => sa.AchievementIdAchievement == abilityId));

            team.CloseATrial(abilityTrialClosing);

            List<ScoutAchievement> after = dbContext.ScoutAchievements.Where(sr => sr.ScoutPeselScout == quatermasterPesel).ToList();
            Assert.Equal(before.Count(), after.Count() - 1);
            Assert.NotEmpty(after.Where(sr => sr.AchievementIdAchievement == abilityId));
        }

        [Fact]
        public void CloseATrial_ShouldThrowUnauthorizedAccessException_WhenScoutNotFromTheTeamHasHisTrialClosed()
        {
            int teamId = new SampleDataset.SampleTeams().TeamAlpha.IdTeam;
            Team team = Team.GetTeam(dbContext, teamId);
            Type expected = new UnauthorizedAccessException().GetType();
            SampleDataset.SampleScouts sampleScouts = new SampleDataset.SampleScouts();
            string quatermasterFromTeamBetaPesel = sampleScouts.TeamBetaQuatermaster.PeselScout;
            TrialClosing invalidTrialClosing = new()
            {
                ScoutPesel = quatermasterFromTeamBetaPesel,
                TrialType = TrialTypes.Rank,
                Rank = ScoutRanks.Rank2
            };

            Action actual = () => team.CloseATrial(invalidTrialClosing);

            Assert.Throws(expected, actual);
        }

        [Fact]
        public void CloseATrial_ShouldThrowArgumentException_WhenTrialTypeIsSetToInvalidValue()
        {
            int teamId = new SampleDataset.SampleTeams().TeamAlpha.IdTeam;
            Team team = Team.GetTeam(dbContext, teamId);
            Type expected = new ArgumentException().GetType();
            SampleDataset.SampleScouts sampleScouts = new SampleDataset.SampleScouts();
            string quatermasterPesel = sampleScouts.TeamAlphaQuatermaster.PeselScout;
            int abilityId = new SampleDataset.SampleAchievements().FireGuard.IdAchievement;
            TrialClosing invalidTrialClosing = new()
            {
                ScoutPesel = quatermasterPesel,
                TrialType = "invalid trial type",
                Ability = abilityId.ToString()
            };

            Action actual = () => team.CloseATrial(invalidTrialClosing);

            Assert.Throws(expected, actual);
        }

        [Fact]
        public void RemoveScout_ShouldRemoveScoutFromTheTeamAndAllDataRelatedToTheScoutAndTheTeam()
        {
            int teamId = new SampleDataset.SampleTeams().TeamAlpha.IdTeam;
            Team team = Team.GetTeam(dbContext, teamId);
            SampleDataset.SampleScouts sampleScouts = new SampleDataset.SampleScouts();
            string pesel = sampleScouts.TeamAlphaHost1Captain.PeselScout;

            team.RemoveScout(pesel);

            List<ScoutHost> scoutHosts = dbContext.ScoutHost
                .Where(sh => sh.ScoutPeselScout == pesel && sh.HostIdHost == new SampleDataset.SampleHosts().TeamAlphaHost1.IdHost)
                .ToList();
            ScoutTeam scoutTeam = dbContext.ScoutTeam.FirstOrDefault(st => st.ScoutPeselScout == pesel && st.TeamIdTeam == teamId);
            Assert.Empty(scoutHosts);
            Assert.Null(scoutTeam);
        }

        [Fact]
        public void RemoveScout_ShouldThrowUnauthorizedAccessException_WhenPeselOfScoutNotFromTheTeamIsPassedAsArgument()
        {
            int teamId = new SampleDataset.SampleTeams().TeamAlpha.IdTeam;
            Team team = Team.GetTeam(dbContext, teamId);
            Type expected = new UnauthorizedAccessException().GetType();
            SampleDataset.SampleScouts sampleScouts = new SampleDataset.SampleScouts();

            Action actual1 = () => team.RemoveScout("invalid pesel");
            Action actual2 = () => team.RemoveScout(sampleScouts.TeamBetaQuatermaster.PeselScout);

            Assert.Throws(expected, actual1);
            Assert.Throws(expected, actual2);
        }

        [Fact]
        public void RemoveHost_ShouldRemoveAHostAndAllScoutHostEntriesFromATeam()
        {
            int teamId = new SampleDataset.SampleTeams().TeamAlpha.IdTeam;
            int hostId = new SampleDataset.SampleHosts().TeamAlphaHost1.IdHost;
            Team team = Team.GetTeam(dbContext, teamId);
            SampleDataset.SampleScouts sampleScouts = new SampleDataset.SampleScouts();
            string hostCaptainPesel = sampleScouts.TeamAlphaHost1Captain.PeselScout;

            team.RemoveHost(hostId);

            List<ScoutHost> scoutHosts = dbContext.ScoutHost.Where(sh => sh.HostIdHost == hostId).ToList();
            ScoutTeam hostCaptainsST = dbContext.ScoutTeam.FirstOrDefault(st => st.ScoutPeselScout == hostCaptainPesel && st.TeamIdTeam == teamId);
            Assert.Empty(scoutHosts);
            Assert.Null(dbContext.Hosts.Find(hostId));
            Assert.Equal(TeamRoles.Scout, hostCaptainsST.Role);
        }

        [Fact]
        public void RemoveHost_ShouldThrowUnauthorizedAccessException_WhenIdOfHostNotFromTheTeamIsPassedAsAnArgument()
        {
            int teamId = new SampleDataset.SampleTeams().TeamAlpha.IdTeam;
            int invalidHostId = -1;
            Team team = Team.GetTeam(dbContext, teamId);
            Type expected = new UnauthorizedAccessException().GetType();
            SampleDataset.SampleScouts sampleScouts = new SampleDataset.SampleScouts();

            Action actual = () => team.RemoveHost(invalidHostId);

            Assert.Throws(expected, actual);
        }

    }
}
