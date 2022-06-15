using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace moja_druzyna.Migrations
{
    public partial class test1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "achievement",
                columns: table => new
                {
                    IdAchievement = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Type = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_achievement", x => x.IdAchievement);
                });

            migrationBuilder.CreateTable(
                name: "agreement",
                columns: table => new
                {
                    IdAgreement = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_agreement", x => x.IdAgreement);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUsers",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    UserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SecurityStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "bit", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "bit", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUsers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "event",
                columns: table => new
                {
                    IdEvent = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DateStartDateNotNullDateEnd = table.Column<DateTime>(type: "datetime2", nullable: false),
                    HasCost = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Type = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Price = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_event", x => x.IdEvent);
                });

            migrationBuilder.CreateTable(
                name: "rank",
                columns: table => new
                {
                    Name = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_rank", x => x.Name);
                });

            migrationBuilder.CreateTable(
                name: "Teams",
                columns: table => new
                {
                    IdTeam = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Teams", x => x.IdTeam);
                });

            migrationBuilder.CreateTable(
                name: "TrainingCourses",
                columns: table => new
                {
                    IdCourse = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TrainingCourses", x => x.IdCourse);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    ProviderKey = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    RoleId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    LoginProvider = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "parent",
                columns: table => new
                {
                    Pesel = table.Column<string>(type: "nvarchar(11)", maxLength: 11, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    SecondName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Surname = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    IdentityId = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_parent", x => x.Pesel);
                    table.ForeignKey(
                        name: "FK_parent_AspNetUsers_IdentityId",
                        column: x => x.IdentityId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "collection",
                columns: table => new
                {
                    IdCollection = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Quarter = table.Column<int>(type: "int", nullable: true),
                    Type = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IdEvent = table.Column<int>(type: "int", nullable: true),
                    EventIdEvent = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_collection", x => x.IdCollection);
                    table.ForeignKey(
                        name: "FK_collection_event_EventIdEvent",
                        column: x => x.EventIdEvent,
                        principalTable: "event",
                        principalColumn: "IdEvent",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "event_team",
                columns: table => new
                {
                    EventIdEvent = table.Column<int>(type: "int", nullable: false),
                    TeamIdTeam = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__event_te__FD53622128F556BD", x => new { x.EventIdEvent, x.TeamIdTeam });
                    table.ForeignKey(
                        name: "FK_event_team_event_EventIdEvent",
                        column: x => x.EventIdEvent,
                        principalTable: "event",
                        principalColumn: "IdEvent",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_event_team_Teams_TeamIdTeam",
                        column: x => x.TeamIdTeam,
                        principalTable: "Teams",
                        principalColumn: "IdTeam",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "host",
                columns: table => new
                {
                    IdHost = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    TeamIdTeam = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_host", x => x.IdHost);
                    table.ForeignKey(
                        name: "FK_host_Teams_TeamIdTeam",
                        column: x => x.TeamIdTeam,
                        principalTable: "Teams",
                        principalColumn: "IdTeam",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "scout",
                columns: table => new
                {
                    PeselScout = table.Column<string>(type: "nvarchar(11)", maxLength: 11, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    SecondName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Surname = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    DateOfBirth = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Nationality = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    MembershipNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DateOfEntry = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Ns = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DateOfLeaving = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CrossNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ParentParentPesel = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IdentityId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    ParentPesel = table.Column<string>(type: "nvarchar(11)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_scout", x => x.PeselScout);
                    table.ForeignKey(
                        name: "FK_scout_AspNetUsers_IdentityId",
                        column: x => x.IdentityId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_scout_parent_ParentPesel",
                        column: x => x.ParentPesel,
                        principalTable: "parent",
                        principalColumn: "Pesel",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "adress",
                columns: table => new
                {
                    ParentPesel = table.Column<string>(type: "nvarchar(11)", nullable: false),
                    ScoutPeselScout = table.Column<string>(type: "nvarchar(11)", nullable: false),
                    AddresZam = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StreatZam = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    HouseZam = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ZipZam = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CountryZam = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CityZam = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    HouseKor = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ZipKor = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CountryKor = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CityKor = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AdressKor = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StreatKor = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_adresss_quicksolve", x => new { x.ParentPesel, x.ScoutPeselScout });
                    table.ForeignKey(
                        name: "FK_adress_parent_ParentPesel",
                        column: x => x.ParentPesel,
                        principalTable: "parent",
                        principalColumn: "Pesel",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_adress_scout_ScoutPeselScout",
                        column: x => x.ScoutPeselScout,
                        principalTable: "scout",
                        principalColumn: "PeselScout",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "attendance_list",
                columns: table => new
                {
                    IdList = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DateOfList = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EventIdEvent = table.Column<int>(type: "int", nullable: true),
                    HostIdHost = table.Column<int>(type: "int", nullable: true),
                    TeamIdTeam = table.Column<int>(type: "int", nullable: true),
                    ScoutIdScout = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ScoutPeselScout = table.Column<string>(type: "nvarchar(11)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_attendance_list", x => x.IdList);
                    table.ForeignKey(
                        name: "FK_attendance_list_event_EventIdEvent",
                        column: x => x.EventIdEvent,
                        principalTable: "event",
                        principalColumn: "IdEvent",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_attendance_list_host_HostIdHost",
                        column: x => x.HostIdHost,
                        principalTable: "host",
                        principalColumn: "IdHost",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_attendance_list_scout_ScoutPeselScout",
                        column: x => x.ScoutPeselScout,
                        principalTable: "scout",
                        principalColumn: "PeselScout",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_attendance_list_Teams_TeamIdTeam",
                        column: x => x.TeamIdTeam,
                        principalTable: "Teams",
                        principalColumn: "IdTeam",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "duty_history",
                columns: table => new
                {
                    DateStart = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Team = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Banner = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Host = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ScoutPeselScout = table.Column<string>(type: "nvarchar(11)", nullable: false),
                    DateEnd = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Detachment = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_duty_history", x => new { x.ScoutPeselScout, x.Team, x.Host, x.DateStart, x.Banner });
                    table.ForeignKey(
                        name: "FK_duty_history_scout_ScoutPeselScout",
                        column: x => x.ScoutPeselScout,
                        principalTable: "scout",
                        principalColumn: "PeselScout",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "scout_achievement",
                columns: table => new
                {
                    ScoutPeselScout = table.Column<string>(type: "nvarchar(11)", maxLength: 11, nullable: false),
                    AchievementIdAchievement = table.Column<int>(type: "int", nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__scout_achievement__14C192AA7E8B153B", x => new { x.ScoutPeselScout, x.AchievementIdAchievement });
                    table.ForeignKey(
                        name: "FK_scout_achievement_achievement_AchievementIdAchievement",
                        column: x => x.AchievementIdAchievement,
                        principalTable: "achievement",
                        principalColumn: "IdAchievement",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_scout_achievement_scout_ScoutPeselScout",
                        column: x => x.ScoutPeselScout,
                        principalTable: "scout",
                        principalColumn: "PeselScout",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "scout_agreement",
                columns: table => new
                {
                    ScoutPeselScout = table.Column<string>(type: "nvarchar(11)", maxLength: 11, nullable: false),
                    AgreementIdAgreement = table.Column<int>(type: "int", nullable: false),
                    DateSign = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DataCancel = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__scout_ag__14C192AA7E8B153B", x => new { x.ScoutPeselScout, x.AgreementIdAgreement });
                    table.ForeignKey(
                        name: "FK_scout_agreement_agreement_AgreementIdAgreement",
                        column: x => x.AgreementIdAgreement,
                        principalTable: "agreement",
                        principalColumn: "IdAgreement",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_scout_agreement_scout_ScoutPeselScout",
                        column: x => x.ScoutPeselScout,
                        principalTable: "scout",
                        principalColumn: "PeselScout",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "scout_collection",
                columns: table => new
                {
                    ScoutPeselScout = table.Column<string>(type: "nvarchar(11)", maxLength: 11, nullable: false),
                    CollectionIdCollection = table.Column<int>(type: "int", nullable: false),
                    Ammount = table.Column<int>(type: "int", nullable: false),
                    DateAcquirement = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Advance = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__scout_co__EDEC6AB95EAAD616", x => new { x.ScoutPeselScout, x.CollectionIdCollection });
                    table.ForeignKey(
                        name: "FK_scout_collection_collection_CollectionIdCollection",
                        column: x => x.CollectionIdCollection,
                        principalTable: "collection",
                        principalColumn: "IdCollection",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_scout_collection_scout_ScoutPeselScout",
                        column: x => x.ScoutPeselScout,
                        principalTable: "scout",
                        principalColumn: "PeselScout",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "scout_course",
                columns: table => new
                {
                    ScoutPeselScout = table.Column<string>(type: "nvarchar(11)", maxLength: 11, nullable: false),
                    TrainingCourseIdCourse = table.Column<int>(type: "int", nullable: false),
                    DateAcquirement = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__scout_co__F0AEC3E099E17797", x => new { x.ScoutPeselScout, x.TrainingCourseIdCourse });
                    table.ForeignKey(
                        name: "FK_scout_course_scout_ScoutPeselScout",
                        column: x => x.ScoutPeselScout,
                        principalTable: "scout",
                        principalColumn: "PeselScout",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_scout_course_TrainingCourses_TrainingCourseIdCourse",
                        column: x => x.TrainingCourseIdCourse,
                        principalTable: "TrainingCourses",
                        principalColumn: "IdCourse",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "scout_event",
                columns: table => new
                {
                    ScoutPeselScout = table.Column<string>(type: "nvarchar(11)", maxLength: 11, nullable: false),
                    EventIdEvent = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__scout_ev__A6050A5886C962DC", x => new { x.ScoutPeselScout, x.EventIdEvent });
                    table.ForeignKey(
                        name: "FK_scout_event_event_EventIdEvent",
                        column: x => x.EventIdEvent,
                        principalTable: "event",
                        principalColumn: "IdEvent",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_scout_event_scout_ScoutPeselScout",
                        column: x => x.ScoutPeselScout,
                        principalTable: "scout",
                        principalColumn: "PeselScout",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "scout_rank",
                columns: table => new
                {
                    ScoutPeselScout = table.Column<string>(type: "nvarchar(11)", maxLength: 11, nullable: false),
                    RankName = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    DateAcquirement = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__scout_ra__F838FC8F4D9C216B", x => new { x.ScoutPeselScout, x.RankName });
                    table.ForeignKey(
                        name: "FK_scout_rank_rank_RankName",
                        column: x => x.RankName,
                        principalTable: "rank",
                        principalColumn: "Name",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_scout_rank_scout_ScoutPeselScout",
                        column: x => x.ScoutPeselScout,
                        principalTable: "scout",
                        principalColumn: "PeselScout",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "scout_team",
                columns: table => new
                {
                    ScoutPeselScout = table.Column<string>(type: "nvarchar(11)", nullable: false),
                    HostIdHost = table.Column<int>(type: "int", nullable: false),
                    Role = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__scout_te__84F25C26A6B4D35B", x => new { x.ScoutPeselScout, x.HostIdHost });
                    table.ForeignKey(
                        name: "FK_scout_team_host_HostIdHost",
                        column: x => x.HostIdHost,
                        principalTable: "host",
                        principalColumn: "IdHost",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_scout_team_scout_ScoutPeselScout",
                        column: x => x.ScoutPeselScout,
                        principalTable: "scout",
                        principalColumn: "PeselScout",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_adress_ParentPesel",
                table: "adress",
                column: "ParentPesel",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_adress_ScoutPeselScout",
                table: "adress",
                column: "ScoutPeselScout",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoleClaims_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true,
                filter: "[NormalizedName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserClaims_UserId",
                table: "AspNetUserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserLogins_UserId",
                table: "AspNetUserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "AspNetUsers",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "AspNetUsers",
                column: "NormalizedUserName",
                unique: true,
                filter: "[NormalizedUserName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_attendance_list_EventIdEvent",
                table: "attendance_list",
                column: "EventIdEvent");

            migrationBuilder.CreateIndex(
                name: "IX_attendance_list_HostIdHost",
                table: "attendance_list",
                column: "HostIdHost");

            migrationBuilder.CreateIndex(
                name: "IX_attendance_list_ScoutPeselScout",
                table: "attendance_list",
                column: "ScoutPeselScout");

            migrationBuilder.CreateIndex(
                name: "IX_attendance_list_TeamIdTeam",
                table: "attendance_list",
                column: "TeamIdTeam");

            migrationBuilder.CreateIndex(
                name: "IX_collection_EventIdEvent",
                table: "collection",
                column: "EventIdEvent");

            migrationBuilder.CreateIndex(
                name: "IX_duty_history_ScoutPeselScout",
                table: "duty_history",
                column: "ScoutPeselScout",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_event_team_TeamIdTeam",
                table: "event_team",
                column: "TeamIdTeam");

            migrationBuilder.CreateIndex(
                name: "IX_host_TeamIdTeam",
                table: "host",
                column: "TeamIdTeam");

            migrationBuilder.CreateIndex(
                name: "IX_parent_IdentityId",
                table: "parent",
                column: "IdentityId");

            migrationBuilder.CreateIndex(
                name: "IX_scout_IdentityId",
                table: "scout",
                column: "IdentityId");

            migrationBuilder.CreateIndex(
                name: "IX_scout_ParentPesel",
                table: "scout",
                column: "ParentPesel");

            migrationBuilder.CreateIndex(
                name: "IX_scout_achievement_AchievementIdAchievement",
                table: "scout_achievement",
                column: "AchievementIdAchievement");

            migrationBuilder.CreateIndex(
                name: "IX_scout_achievement_ScoutPeselScout",
                table: "scout_achievement",
                column: "ScoutPeselScout",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_scout_agreement_AgreementIdAgreement",
                table: "scout_agreement",
                column: "AgreementIdAgreement");

            migrationBuilder.CreateIndex(
                name: "IX_scout_collection_CollectionIdCollection",
                table: "scout_collection",
                column: "CollectionIdCollection");

            migrationBuilder.CreateIndex(
                name: "IX_scout_course_TrainingCourseIdCourse",
                table: "scout_course",
                column: "TrainingCourseIdCourse");

            migrationBuilder.CreateIndex(
                name: "IX_scout_event_EventIdEvent",
                table: "scout_event",
                column: "EventIdEvent");

            migrationBuilder.CreateIndex(
                name: "IX_scout_rank_RankName",
                table: "scout_rank",
                column: "RankName");

            migrationBuilder.CreateIndex(
                name: "IX_scout_team_HostIdHost",
                table: "scout_team",
                column: "HostIdHost");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "adress");

            migrationBuilder.DropTable(
                name: "AspNetRoleClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserLogins");

            migrationBuilder.DropTable(
                name: "AspNetUserRoles");

            migrationBuilder.DropTable(
                name: "AspNetUserTokens");

            migrationBuilder.DropTable(
                name: "attendance_list");

            migrationBuilder.DropTable(
                name: "duty_history");

            migrationBuilder.DropTable(
                name: "event_team");

            migrationBuilder.DropTable(
                name: "scout_achievement");

            migrationBuilder.DropTable(
                name: "scout_agreement");

            migrationBuilder.DropTable(
                name: "scout_collection");

            migrationBuilder.DropTable(
                name: "scout_course");

            migrationBuilder.DropTable(
                name: "scout_event");

            migrationBuilder.DropTable(
                name: "scout_rank");

            migrationBuilder.DropTable(
                name: "scout_team");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "achievement");

            migrationBuilder.DropTable(
                name: "agreement");

            migrationBuilder.DropTable(
                name: "collection");

            migrationBuilder.DropTable(
                name: "TrainingCourses");

            migrationBuilder.DropTable(
                name: "rank");

            migrationBuilder.DropTable(
                name: "host");

            migrationBuilder.DropTable(
                name: "scout");

            migrationBuilder.DropTable(
                name: "event");

            migrationBuilder.DropTable(
                name: "Teams");

            migrationBuilder.DropTable(
                name: "parent");

            migrationBuilder.DropTable(
                name: "AspNetUsers");
        }
    }
}
