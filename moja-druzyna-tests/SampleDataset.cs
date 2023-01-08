using Microsoft.AspNetCore.Identity;
using moja_druzyna.Const;
using moja_druzyna.Models;
using System.Collections.Generic;

namespace moja_druzyna_tests
{
    public class SampleDataset
    {
        SampleScouts sampleScouts = new SampleScouts();
        SampleTeams sampleTeams = new SampleTeams();
        SampleAchievements sampleAchievements = new SampleAchievements();
        SampleRanks sampleRanks = new SampleRanks();

        public SampleDataset()
        {
            ScoutTeams = new()
            {
                new ScoutTeam()
                {
                    ScoutPeselScout = sampleScouts.TeamAlphaCaptain.PeselScout,
                    TeamIdTeam = sampleTeams.TeamAlpha.IdTeam,
                    Role = TeamRoles.Captain
                },
                new ScoutTeam()
                {
                    ScoutPeselScout = sampleScouts.TeamAlphaViceCaptain.PeselScout,
                    TeamIdTeam = sampleTeams.TeamAlpha.IdTeam,
                    Role = TeamRoles.ViceCaptain
                },
                new ScoutTeam()
                {
                    ScoutPeselScout = sampleScouts.TeamAlphaHost1Captain.PeselScout,
                    TeamIdTeam = sampleTeams.TeamAlpha.IdTeam,
                    Role = TeamRoles.HostCaptain
                },
                new ScoutTeam()
                {
                    ScoutPeselScout = sampleScouts.TeamAlphaHost1Scout.PeselScout,
                    TeamIdTeam = sampleTeams.TeamAlpha.IdTeam,
                    Role = TeamRoles.Scout
                },
                new ScoutTeam()
                {
                    ScoutPeselScout = sampleScouts.TeamAlphaHost2Captain.PeselScout,
                    TeamIdTeam = sampleTeams.TeamAlpha.IdTeam,
                    Role = TeamRoles.HostCaptain
                },
                new ScoutTeam()
                {
                    ScoutPeselScout = sampleScouts.TeamAlphaQuatermaster.PeselScout,
                    TeamIdTeam = sampleTeams.TeamAlpha.IdTeam,
                    Role = TeamRoles.Quatermaster
                },
                new ScoutTeam()
                {
                    ScoutPeselScout = sampleScouts.TeamAlphaRegularScout.PeselScout,
                    TeamIdTeam = sampleTeams.TeamAlpha.IdTeam,
                    Role = TeamRoles.Scout
                },
                new ScoutTeam()
                {
                    ScoutPeselScout = sampleScouts.TeamBetaCaptain.PeselScout,
                    TeamIdTeam = sampleTeams.TeamBeta.IdTeam,
                    Role = TeamRoles.Captain
                },
                new ScoutTeam()
                {
                    ScoutPeselScout = sampleScouts.TeamBetaQuatermaster.PeselScout,
                    TeamIdTeam = sampleTeams.TeamBeta.IdTeam,
                    Role = TeamRoles.Quatermaster
                },
                new ScoutTeam()
                {
                    ScoutPeselScout = sampleScouts.TeamBetaRegularScout.PeselScout,
                    TeamIdTeam = sampleTeams.TeamBeta.IdTeam,
                    Role = TeamRoles.Scout
                }
            };

            ScoutHosts = new()
            {
                new ScoutHost()
                {
                    HostIdHost = new SampleHosts().TeamAlphaHost1.IdHost,
                    ScoutPeselScout = sampleScouts.TeamAlphaHost1Captain.PeselScout,
                    Role = HostRoles.HostCaptain
                },
                new ScoutHost()
                {
                    HostIdHost = new SampleHosts().TeamAlphaHost1.IdHost,
                    ScoutPeselScout = sampleScouts.TeamAlphaHost1Scout.PeselScout,
                    Role = HostRoles.Scout
                },
                new ScoutHost()
                {
                    HostIdHost = new SampleHosts().TeamAlphaHost2.IdHost,
                    ScoutPeselScout = sampleScouts.TeamAlphaHost2Captain.PeselScout,
                    Role = HostRoles.HostCaptain
                },
            };

            ScoutAchievements = new()
            {
                new()
                {
                    ScoutPeselScout = sampleScouts.TeamAlphaCaptain.PeselScout,
                    AchievementIdAchievement = sampleAchievements.Glimmer.IdAchievement
                },
                new()
                {
                    ScoutPeselScout = sampleScouts.TeamAlphaCaptain.PeselScout,
                    AchievementIdAchievement = sampleAchievements.European.IdAchievement
                },
                new()
                {
                    ScoutPeselScout = sampleScouts.TeamAlphaCaptain.PeselScout,
                    AchievementIdAchievement = sampleAchievements.Lifesaver.IdAchievement
                },
                new()
                {
                    ScoutPeselScout = sampleScouts.TeamBetaQuatermaster.PeselScout,
                    AchievementIdAchievement = sampleAchievements.Glimmer.IdAchievement
                }
            };

            ScoutRanks = new()
            {
                new()
                {
                    RankName = sampleRanks.Rank2.Name,
                    ScoutPeselScout = sampleScouts.TeamAlphaCaptain.PeselScout,
                    IsCurrent = false
                },
                new()
                {
                    RankName = sampleRanks.Rank3.Name,
                    ScoutPeselScout = sampleScouts.TeamAlphaCaptain.PeselScout,
                    IsCurrent = true
                },
                new()
                {
                    RankName = sampleRanks.Rank1.Name,
                    ScoutPeselScout = sampleScouts.TeamAlphaQuatermaster.PeselScout,
                    IsCurrent = true
                }
            };
        }

        public class SampleTeams
        {
            public SampleTeams()
            {
                Teams = new() { TeamAlpha, TeamBeta };
            }

            public readonly Team TeamAlpha = new Team() { IdTeam = 1 };
            public readonly Team TeamBeta = new Team() { IdTeam = 2 };

            public readonly List<Team> Teams = null;
        }

        public class SampleScouts
        {
            public SampleScouts()
            {
                TeamAlphaScouts = new()
                {
                    TeamAlphaCaptain, TeamAlphaViceCaptain, TeamAlphaHost1Captain, TeamAlphaHost1Scout, TeamAlphaHost2Captain, TeamAlphaQuatermaster, TeamAlphaRegularScout
                };
                TeamBetaScouts = new() 
                { 
                    TeamBetaCaptain, TeamBetaQuatermaster, TeamBetaRegularScout 
                };
                Scouts = new()
                {
                    TeamAlphaCaptain, TeamAlphaViceCaptain, TeamAlphaHost1Captain, TeamAlphaHost1Scout, TeamAlphaHost2Captain, TeamAlphaQuatermaster, TeamAlphaRegularScout, TeamBetaCaptain, TeamBetaQuatermaster, TeamBetaRegularScout
                };
            }

            public readonly Scout TeamAlphaCaptain = new Scout()
            {
                Name = "Jack",
                Surname = "Smith",
                MembershipNumber = "tss-212",
                Nationality = "Terra",
                PeselScout = "00410185153",
                IdentityId = "00410185153"
            };
            public readonly Scout TeamAlphaHost1Captain = new Scout()
            {
                Name = "Preston",
                Surname = "Melendez",
                MembershipNumber = "tss-209",
                Nationality = "Terra",
                PeselScout = "00420126478",
                IdentityId = "00420126478"
            };
            public readonly Scout TeamAlphaHost1Scout = new Scout()
            {
                Name = "Hubert",
                Surname = "Simmons",
                MembershipNumber = "tss-107",
                Nationality = "Terra",
                PeselScout = "00430163317",
                IdentityId = "00430163317"
            };
            public readonly Scout TeamAlphaHost2Captain = new Scout()
            {
                Name = "Hubert",
                Surname = "Simmons",
                MembershipNumber = "tss-107",
                Nationality = "Terra",
                PeselScout = "00510192392",
                IdentityId = "00510192392"
            };
            public readonly Scout TeamAlphaViceCaptain = new Scout()
            {
                Name = "Finbar",
                Surname = "Chambers",
                MembershipNumber = "tss-235",
                Nationality = "Terra",
                PeselScout = "00440144313",
                IdentityId = "00440144313"
            };
            public readonly Scout TeamAlphaQuatermaster = new Scout()
            {
                Name = "Keanan",
                Surname = "Correa",
                MembershipNumber = "tss-295",
                Nationality = "Terra",
                PeselScout = "00450167696",
                IdentityId = "00450167696"
            };
            public readonly Scout TeamAlphaRegularScout = new Scout()
            {
                Name = "Wilson",
                Surname = "Byrne",
                MembershipNumber = "tss-027",
                Nationality = "Terra",
                PeselScout = "00460133113"
            };
            public readonly List<Scout> TeamAlphaScouts = null;

            public readonly Scout TeamBetaCaptain = new Scout()
            {
                Name = "Max",
                Surname = "Webber",
                MembershipNumber = "tzm-289",
                Nationality = "Ruia",
                PeselScout = "00470145751",
                IdentityId = "00470145751"
            };
            public readonly Scout TeamBetaQuatermaster = new Scout()
            {
                Name = "Raihan",
                Surname = "Bradshaw",
                MembershipNumber = "tzm-294",
                Nationality = "Ruia",
                PeselScout = "00480155373",
                IdentityId = "00480155373"
            };
            public readonly Scout TeamBetaRegularScout = new Scout()
            {
                Name = "Raihan",
                Surname = "Bradshaw",
                MembershipNumber = "tzm-251",
                Nationality = "Ruia",
                PeselScout = "00490184754",
                IdentityId = "00490184754"
            };
            public readonly List<Scout> TeamBetaScouts = null;
            public readonly List<Scout> Scouts = null;
        }

        public class SampleHosts
        {
            public readonly Host TeamAlphaHost1 = new Host() { IdHost = 1, TeamIdTeam = new SampleTeams().TeamAlpha.IdTeam, Name = "host 1" };
            public readonly Host TeamAlphaHost2 = new Host() { IdHost = 2, TeamIdTeam = new SampleTeams().TeamAlpha.IdTeam, Name = "host 2" };
        }

        public class SampleAchievements
        {
            public SampleAchievements()
            {
                Achievements = new()
                {
                    Hygenist,
                    Paramedic,
                    Lifesaver,
                    Glimmer,
                    FireGuard,
                    FireplaceMaster,
                    DrillExpert,
                    DrillMaster,
                    Needle,
                    Tailor,
                    YoungSwimmer,
                    Swimmer,
                    ExcellentSwimmer,
                    Internaut,
                    FamilyHistorian,
                    European,
                    HealthLeader,
                    NatureFriend,
                    Photograph
                };
            }

            public readonly List<string> AchievementNames = new List<string>()
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

            public readonly Achievement Hygenist = new()
            {
                IdAchievement = 1,
                Type = ScoutAbilities.Hygenist
            };
            public readonly Achievement Paramedic = new()
            {
                IdAchievement = 2,
                Type = ScoutAbilities.Paramedic
            };
            public readonly Achievement Lifesaver = new()
            {
                IdAchievement = 3,
                Type = ScoutAbilities.Lifesaver
            };
            public readonly Achievement Glimmer = new()
            {
                IdAchievement = 4,
                Type = ScoutAbilities.Glimmer
            };
            public readonly Achievement FireGuard = new()
            {
                IdAchievement = 5,
                Type = ScoutAbilities.FireGuard
            };
            public readonly Achievement FireplaceMaster = new()
            {
                IdAchievement = 6,
                Type = ScoutAbilities.FireplaceMaster
            };
            public readonly Achievement DrillExpert = new()
            {
                IdAchievement = 7,
                Type = ScoutAbilities.DrillExpert
            };
            public readonly Achievement DrillMaster = new()
            {
                IdAchievement = 8,
                Type = ScoutAbilities.DrillMaster
            };
            public readonly Achievement Needle = new()
            {
                IdAchievement = 9,
                Type = ScoutAbilities.Needle
            };
            public readonly Achievement Tailor = new()
            {
                IdAchievement = 10,
                Type = ScoutAbilities.Tailor
            };
            public readonly Achievement YoungSwimmer = new()
            {
                IdAchievement = 11,
                Type = ScoutAbilities.YoungSwimmer
            };
            public readonly Achievement Swimmer = new()
            {
                IdAchievement = 12,
                Type = ScoutAbilities.Swimmer
            };
            public readonly Achievement ExcellentSwimmer = new()
            {
                IdAchievement = 13,
                Type = ScoutAbilities.ExcellentSwimmer
            };
            public readonly Achievement Internaut = new()
            {
                IdAchievement = 14,
                Type = ScoutAbilities.Internaut
            };
            public readonly Achievement FamilyHistorian = new()
            {
                IdAchievement = 15,
                Type = ScoutAbilities.FamilyHistorian
            };
            public readonly Achievement European = new()
            {
                IdAchievement = 16,
                Type = ScoutAbilities.European
            };
            public readonly Achievement HealthLeader = new()
            {
                IdAchievement = 17,
                Type = ScoutAbilities.HealthLeader
            };
            public readonly Achievement NatureFriend = new()
            {
                IdAchievement = 18,
                Type = ScoutAbilities.NatureFriend
            };
            public readonly Achievement Photograph = new()
            {
                IdAchievement = 19,
                Type = ScoutAbilities.Photograph
            };

            public readonly List<Achievement> Achievements = null;
        }

        public class SampleRanks
        {
            public SampleRanks()
            {
                Ranks = new()
                {
                    Rank1,
                    Rank2,
                    Rank3,
                    Rank4,
                    Rank5,
                    Rank6
                };
            }
            
            public readonly List<string> RankNames = new List<string>()
            {
                moja_druzyna.Const.ScoutRanks.Rank1,
                moja_druzyna.Const.ScoutRanks.Rank2,
                moja_druzyna.Const.ScoutRanks.Rank3,
                moja_druzyna.Const.ScoutRanks.Rank4,
                moja_druzyna.Const.ScoutRanks.Rank5,
                moja_druzyna.Const.ScoutRanks.Rank6,
            };

            public readonly Rank Rank1 = new()
            {
                Name = moja_druzyna.Const.ScoutRanks.Rank1
            };
            public readonly Rank Rank2 = new()
            {
                Name = moja_druzyna.Const.ScoutRanks.Rank2
            };
            public readonly Rank Rank3 = new()
            {
                Name = moja_druzyna.Const.ScoutRanks.Rank3
            };
            public readonly Rank Rank4 = new()
            {
                Name = moja_druzyna.Const.ScoutRanks.Rank4
            };
            public readonly Rank Rank5 = new()
            {
                Name = moja_druzyna.Const.ScoutRanks.Rank5
            };
            public readonly Rank Rank6 = new()
            {
                Name = moja_druzyna.Const.ScoutRanks.Rank6
            };

            public readonly List<Rank> Ranks = null;
        }

        public class AspNetUsers
        {
            public AspNetUsers()
            {
                SampleScouts scouts = new();

                TeamAlphaCaptainIdentity = new()
                {
                    Id = scouts.TeamAlphaCaptain.IdentityId,
                    UserName = scouts.TeamAlphaCaptain.Name
                };
                TeamAlphaHostCaptainIdentity = new()
                {
                    Id = scouts.TeamAlphaHost1Captain.IdentityId,
                    UserName = scouts.TeamAlphaHost1Captain.Name
                };
                TeamAlphaHostScoutIdentity = new()
                {
                    Id = scouts.TeamAlphaHost1Scout.IdentityId,
                    UserName = scouts.TeamAlphaHost1Scout.Name
                };
                TeamAlphaViceCaptainIdentity = new()
                {
                    Id = scouts.TeamAlphaViceCaptain.IdentityId,
                    UserName = scouts.TeamAlphaViceCaptain.Name
                };
                TeamAlphaQuatermasterIdentity = new()
                {
                    Id = scouts.TeamAlphaQuatermaster.IdentityId,
                    UserName = scouts.TeamAlphaQuatermaster.Name
                };
                TeamAlphaRegularScoutIdentity = new()
                {
                    Id = scouts.TeamAlphaRegularScout.IdentityId,
                    UserName = scouts.TeamAlphaRegularScout.Name
                };

                TeamBetaCaptainIdentity = new()
                {
                    Id = scouts.TeamBetaCaptain.IdentityId,
                    UserName = scouts.TeamBetaCaptain.Name
                };
                TeamBetaQuatermasterIdentity = new()
                {
                    Id = scouts.TeamBetaQuatermaster.IdentityId,
                    UserName = scouts.TeamBetaQuatermaster.Name
                };
                TeamBetaRegularScoutIdentity = new()
                {
                    Id = scouts.TeamBetaRegularScout.IdentityId,
                    UserName = scouts.TeamBetaRegularScout.Name
                };
            }

            public readonly IdentityUser TeamAlphaCaptainIdentity;
            public readonly IdentityUser TeamAlphaHostCaptainIdentity;
            public readonly IdentityUser TeamAlphaHostScoutIdentity;
            public readonly IdentityUser TeamAlphaViceCaptainIdentity;
            public readonly IdentityUser TeamAlphaQuatermasterIdentity;
            public readonly IdentityUser TeamAlphaRegularScoutIdentity;

            public readonly IdentityUser TeamBetaCaptainIdentity;
            public readonly IdentityUser TeamBetaQuatermasterIdentity;
            public readonly IdentityUser TeamBetaRegularScoutIdentity;

            public readonly List<IdentityUser> IdentityScouts;
        }

        public readonly List<ScoutTeam> ScoutTeams = null;

        public readonly List<ScoutHost> ScoutHosts = null;

        public readonly List<ScoutAchievement> ScoutAchievements = null;

        public readonly List<ScoutRank> ScoutRanks = null;
    }
}
