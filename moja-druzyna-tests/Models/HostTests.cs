using Microsoft.EntityFrameworkCore;
using moja_druzyna.Const;
using moja_druzyna.Data;
using moja_druzyna.Lib.Exceptions;
using moja_druzyna.Models;
using moja_druzyna_tests.TestLib;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace moja_druzyna_tests.Models
{
    public class HostTests : IDisposable
    {
        private static string dbName = "HostTestsDb";
        private static DbContextOptions<ApplicationDbContext> options =
            new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: dbName)
            .Options;
        private static TestObjectComparerFactory objectComparerFactory = new TestObjectComparerFactory();

        ApplicationDbContext dbContext = new ApplicationDbContext(options);

        public HostTests()
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

        [Fact]
        public void GetHost_ShouldReturnProperHost()
        {
            ObjectsComparer.Comparer<Host> comparer =
                objectComparerFactory.CreateShallowObjectComparer<Host>();
            IEnumerable<ObjectsComparer.Difference> differences;
            Host expected = new SampleDataset.SampleHosts().TeamAlphaHost1;

            Host actual = Host.GetHost(dbContext, expected.IdHost);

            Assert.True(comparer.Compare(expected, actual, out differences));
        }

        [Fact]
        public void GetHost_ShouldThrowArgumentNullExcpetion_WhenDbContextArgumentIsNull()
        {
            int hostId = new SampleDataset.SampleHosts().TeamAlphaHost1.IdHost;
            Type expected = new ArgumentNullException().GetType();

            Func<Host> actual = () => Host.GetHost(null, hostId);

            Assert.Throws(expected, actual);
        }

        [Fact]
        public void GetHost_ShouldThrowRecordNotFoundException_WhenHostWithGivenIdDoesntExist()
        {
            int invalidId = -1;
            Type expected = new RecordNotFoundException().GetType();

            Func<Host> actual = () => Host.GetHost(dbContext, invalidId);

            Assert.Throws(expected, actual);
        }

        [Fact]
        public void AddScout_ShouldAddAScoutToTheHostProperly()
        {
            Host host = Host.GetHost(dbContext, new SampleDataset.SampleHosts().TeamAlphaHost1.IdHost);
            string pesel = new SampleDataset.SampleScouts().TeamAlphaQuatermaster.PeselScout;

            host.AddScout(pesel);

            List<ScoutHost> scoutHosts = dbContext.ScoutHost.Where(sh => sh.HostIdHost == host.IdHost).ToList();
            Assert.NotEmpty(scoutHosts.Where(sh => sh.ScoutPeselScout == pesel));
            Assert.Equal(HostRoles.Scout, scoutHosts.First(sh => sh.ScoutPeselScout == pesel).Role);
        }

        [Fact]
        public void AddScout_ShouldThrowRecordNotFoundExcpetion_WhenScoutWithGivenPeselIsntInTheHostsTeam()
        {
            int hostId = new SampleDataset.SampleHosts().TeamAlphaHost1.IdHost;
            Type expected = new RecordNotFoundException().GetType();
            Host host = Host.GetHost(dbContext, hostId);

            Action actual = () => host.AddScout("invalid pesel");

            Assert.Throws(expected, actual);
        }

        [Fact]
        public void UpdateScoutRole_ShouldUpdateScoutsRoleInAHostCorrectly_WhenDifferentHostCaptainIsPicked()
        {
            int hostId = new SampleDataset.SampleHosts().TeamAlphaHost1.IdHost;
            Host host = Host.GetHost(dbContext, hostId);
            SampleDataset.SampleScouts sampleScouts = new();
            string host1CaptainPesel = sampleScouts.TeamAlphaHost1Captain.PeselScout;
            string host1ScoutPesel = sampleScouts.TeamAlphaHost1Scout.PeselScout;

            host.UpdateScoutRole(host1ScoutPesel, HostRoles.HostCaptain);

            ScoutHost host1CaptainSH = dbContext.ScoutHost.First(sh => sh.HostIdHost == hostId && sh.ScoutPeselScout == host1CaptainPesel);
            ScoutHost host1ScoutSH = dbContext.ScoutHost.First(sh => sh.HostIdHost == hostId && sh.ScoutPeselScout == host1ScoutPesel);
            Assert.Equal(HostRoles.Scout, host1CaptainSH.Role);
            Assert.Equal(HostRoles.HostCaptain, host1ScoutSH.Role);
        }

        [Fact]
        public void UpdateScoutRole_ShouldUpdateScoutsRoleInAHostCorrectly_WhenHostCaptainsRoleIsChangedToRegularScout()
        {
            int hostId = new SampleDataset.SampleHosts().TeamAlphaHost1.IdHost;
            Host host = Host.GetHost(dbContext, hostId);
            SampleDataset.SampleScouts sampleScouts = new();
            string host1CaptainPesel = sampleScouts.TeamAlphaHost1Captain.PeselScout;
            string host1ScoutPesel = sampleScouts.TeamAlphaHost1Scout.PeselScout;

            host.UpdateScoutRole(host1CaptainPesel, HostRoles.Scout);

            ScoutHost host1CaptainSH = dbContext.ScoutHost.First(sh => sh.HostIdHost == hostId && sh.ScoutPeselScout == host1CaptainPesel);
            ScoutHost host1ScoutSH = dbContext.ScoutHost.First(sh => sh.HostIdHost == hostId && sh.ScoutPeselScout == host1ScoutPesel);
            Assert.Equal(HostRoles.Scout, host1CaptainSH.Role);
            Assert.Equal(HostRoles.Scout, host1ScoutSH.Role);
        }

        [Fact]
        public void UpdateScoutRole_ShouldUpdateScoutsRoleInAHostCorrectly_WhenScoutCaptainIsReassigned()
        {
            int hostId = new SampleDataset.SampleHosts().TeamAlphaHost1.IdHost;
            Host host = Host.GetHost(dbContext, hostId);
            SampleDataset.SampleScouts sampleScouts = new();
            string host1CaptainPesel = sampleScouts.TeamAlphaHost1Captain.PeselScout;
            string host1ScoutPesel = sampleScouts.TeamAlphaHost1Scout.PeselScout;

            host.UpdateScoutRole(host1CaptainPesel, HostRoles.HostCaptain);

            ScoutHost host1CaptainSH = dbContext.ScoutHost.First(sh => sh.HostIdHost == hostId && sh.ScoutPeselScout == host1CaptainPesel);
            ScoutHost host1ScoutSH = dbContext.ScoutHost.First(sh => sh.HostIdHost == hostId && sh.ScoutPeselScout == host1ScoutPesel);
            Assert.Equal(HostRoles.HostCaptain, host1CaptainSH.Role);
            Assert.Equal(HostRoles.Scout, host1ScoutSH.Role);
        }

        [Fact]
        public void UpdateScoutRole_ShouldThrowUnauthorizedAccessException_WhenScoutWithGivenPeselIsntInTheHost()
        {
            int hostId = new SampleDataset.SampleHosts().TeamAlphaHost1.IdHost;
            Type expected = new UnauthorizedAccessException().GetType();
            Host host = Host.GetHost(dbContext, hostId);

            Action actual = () => host.UpdateScoutRole("invalid pesel", HostRoles.HostCaptain);

            Assert.Throws(expected, actual);
        }

        [Fact]
        public void UpdateScoutRole_ShouldArgumentException_WhenGivenRoleIsInvalid()
        {
            int hostId = new SampleDataset.SampleHosts().TeamAlphaHost1.IdHost;
            Type expected = new ArgumentException().GetType();
            Host host = Host.GetHost(dbContext, hostId);
            string pesel = new SampleDataset.SampleScouts().TeamAlphaHost1Captain.PeselScout;

            Action actual = () => host.UpdateScoutRole(pesel, "invalid role");

            Assert.Throws(expected, actual);
        }

        [Fact]
        public void RemoveScout_ShouldRemoveScoutFromAHostCorrectly()
        {
            int teamId = new SampleDataset.SampleTeams().TeamAlpha.IdTeam;
            int hostId = new SampleDataset.SampleHosts().TeamAlphaHost1.IdHost;
            Host host = Host.GetHost(dbContext, hostId);
            string host1CaptainPesel = new SampleDataset.SampleScouts().TeamAlphaHost1Captain.PeselScout;

            host.RemoveScout(host1CaptainPesel);

            Assert.Null(dbContext.ScoutHost.Find(host1CaptainPesel, hostId));
            Assert.Equal(TeamRoles.Scout, dbContext.ScoutTeam.Find(host1CaptainPesel, teamId).Role);
        }

        [Fact]
        public void RemoveScout_ShouldThrowUnauthorizedAccessException_WhenScoutWithGivenPeselIsntInTheHost()
        {
            int hostId = new SampleDataset.SampleHosts().TeamAlphaHost1.IdHost;
            Type expected = new UnauthorizedAccessException().GetType();
            Host host = Host.GetHost(dbContext, hostId);

            Action actual = () => host.RemoveScout("invalid pesel");

            Assert.Throws(expected, actual);
        }

    }
}
