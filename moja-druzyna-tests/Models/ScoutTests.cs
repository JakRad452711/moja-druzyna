using Microsoft.EntityFrameworkCore;
using moja_druzyna.Data;
using moja_druzyna.Lib.Exceptions;
using moja_druzyna.Models;
using moja_druzyna_tests.TestLib;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using static moja_druzyna.Models.Scout;

namespace moja_druzyna_tests.Models
{
    public class ScoutTests : IDisposable
    {
        private static string dbName = "ScoutTestsDb";
        private static DbContextOptions<ApplicationDbContext> options =
            new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: dbName)
            .Options;
        private static TestObjectComparerFactory objectComparerFactory = new TestObjectComparerFactory();

        ApplicationDbContext dbContext = new ApplicationDbContext(options);

        public ScoutTests()
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

        public static IEnumerable<object[]> GetScouts()
        {
            foreach (Scout scout in new SampleDataset.SampleScouts().Scouts)
                yield return new object[] { scout };
        }

        [Theory]
        [MemberData(nameof(GetScouts))]
        public void GetScout_ShouldReturnProperScout(Scout scout)
        {
            ObjectsComparer.Comparer<Scout> comparer = objectComparerFactory.CreateShallowObjectComparer<Scout>();
            IEnumerable<ObjectsComparer.Difference> differences;
            Scout expected = scout;

            Scout actual = GetScout(dbContext, scout.PeselScout);

            Assert.NotNull(actual);
            Assert.NotEqual(expected, actual);
            Assert.True(comparer.Compare(actual, expected, out differences));
        }

        [Fact]
        public void GetScout_ShouldThrowArgumentIsNullException_WhenDbContextArgumentIsNull()
        {
            Scout scout = new SampleDataset.SampleScouts().TeamAlphaCaptain;
            Type expected = new ArgumentNullException().GetType();

            Func<Scout> actual = () => GetScout(null, scout.PeselScout);

            Assert.Throws(expected, actual);
        }

        [Fact]
        public void GetScout_ShouldThrowRecordNotFoundException_WhenScoutWithGivenPeselDoesntExist()
        {
            Scout scout = new SampleDataset.SampleScouts().TeamAlphaCaptain;
            Type expected = new RecordNotFoundException().GetType();

            Func<Scout> actual = () => GetScout(dbContext, "invalid pesel");

            Assert.Throws(expected, actual);
        }

        [Theory]
        [MemberData(nameof(GetScouts))]
        public void GetScoutById_ShouldReturnProperScout(Scout scout)
        {
            ObjectsComparer.Comparer<Scout> comparer = objectComparerFactory.CreateShallowObjectComparer<Scout>();
            IEnumerable<ObjectsComparer.Difference> differences;
            Scout expected = scout;
            string scoutId = dbContext.Scouts.Find(scout.PeselScout).IdentityId;
            expected.IdentityId = scoutId;

            Scout actual = GetScoutById(dbContext, scoutId);
            
            Assert.NotNull(actual);
            Assert.NotEqual(expected, actual);
            Assert.True(comparer.Compare(actual, expected, out differences));
        }

        [Fact]
        public void GetScoutById_ShouldThrowArgumentIsNullException_WhenDbContextArgumentIsNull()
        {
            Scout scout = new SampleDataset.SampleScouts().TeamAlphaCaptain;
            Type expected = new ArgumentNullException().GetType();

            Func<Scout> actual = () => GetScoutById(null, scout.IdentityId);

            Assert.Throws(expected, actual);
        }

        [Fact]
        public void GetScoutById_ShouldThrowRecordNotFoundException_WhenScoutWithGivenPeselDoesntExist()
        {
            Scout scout = new SampleDataset.SampleScouts().TeamAlphaCaptain;
            Type expected = new RecordNotFoundException().GetType();

            Func<Scout> actual = () => GetScoutById(dbContext, "invalid id");

            Assert.Throws(expected, actual);
        }

        [Theory]
        [MemberData(nameof(GetScouts))]
        public void Edit_ShouldEditScoutFieldsProperly(Scout scout)
        {
            ObjectsComparer.Comparer<Scout> comparer = objectComparerFactory.CreateShallowObjectComparer<Scout>();
            IEnumerable<ObjectsComparer.Difference> differences;
            Scout actual = GetScout(dbContext, scout.PeselScout);
            Scout expected = scout;
            expected.Name = "Johas";
            expected.Ns = true;
            expected.Adress = new()
            {
                ScoutPeselScout = expected.PeselScout,
                ParentPesel = expected.PeselScout,
                CountryZam = "Ruia",
                CityZam = "Aihc",
                StreetZam = "Dystopian",
                NumberHouseZam = "5"
            };
            actual.Name = "Johas";
            actual.Ns = true;
            actual.Adress = new()
            {
                ScoutPeselScout = actual.PeselScout,
                ParentPesel = actual.PeselScout,
                CountryZam = "Ruia",
                CityZam = "Aihc",
                StreetZam = "Dystopian",
                NumberHouseZam = "5"
            };

            actual.Edit(actual);

            actual = GetScout(dbContext, actual.PeselScout);
            Assert.NotEqual(expected, actual);
            Assert.True(comparer.Compare(expected, actual, out differences));
        }

        [Fact]
        public void Edit_ShouldThrowMissingDbContextInstanceException_WhenDbContextIsNotInitialized()
        {
            Scout scout = new SampleDataset.SampleScouts().TeamAlphaCaptain;
            Type expected = new MissingDbContextInstanceException().GetType();

            Action actual = () => scout.Edit(scout);

            Assert.Throws(expected, actual);
        }

        [Theory]
        [MemberData(nameof(GetScouts))]
        public void GetScoutAchievements_ShouldReturnScoutsListOfAchievements(Scout scout)
        {
            List<ScoutAchievement> expected = new SampleDataset().ScoutAchievements
                .OrderBy(sa => sa.AchievementIdAchievement)
                .Where(sa => sa.ScoutPeselScout == scout.PeselScout)
                .ToList();
            ObjectsComparer.Comparer<List<ScoutAchievement>> comparer = 
                objectComparerFactory.CreateShallowObjectComparer<List<ScoutAchievement>>();
            IEnumerable<ObjectsComparer.Difference> differences;

            List<ScoutAchievement> actual = GetScout(dbContext, scout.PeselScout)
                .GetScoutAchievements()
                .OrderBy(sa => sa.AchievementIdAchievement)
                .ToList();
            actual.ForEach(sa => sa.Achievement = null);
            actual.ForEach(sa => sa.Scout = null);

            Assert.True(comparer.Compare(expected, actual, out differences));
        }

        [Theory]
        [InlineData("00410185153", "3")]
        [InlineData("00450167696", "1")]
        [InlineData("00460133113", null)]
        public void GetRank_ShouldReturnScoutsRank(string pesel, string rank)
        {
            string expected = rank;

            Rank actual = GetScout(dbContext, pesel).GetRank();

            Assert.Equal(expected, actual?.Name);
        }
    }
}
