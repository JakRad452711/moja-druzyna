using System.Collections.Generic;

namespace moja_druzyna.Const
{
    public class ScoutAbilities
    {
        public static readonly string Hygenist = "hygenist";
        public static readonly string Paramedic = "paramedic";
        public static readonly string Lifesaver = "lifesaver";
        public static readonly string Glimmer = "glimmer";
        public static readonly string FireGuard = "fire guard";
        public static readonly string FireplaceMaster = "fireplace master";
        public static readonly string DrillExpert = "drill expert";
        public static readonly string DrillMaster = "drill master";
        public static readonly string Needle = "needle";
        public static readonly string Tailor = "tailor";
        public static readonly string YoungSwimmer = "young swimmer";
        public static readonly string Swimmer = "swimmer";
        public static readonly string ExcellentSwimmer = "excellent swimmer";
        public static readonly string Internaut = "internaut";
        public static readonly string FamilyHistorian = "family historian";
        public static readonly string European = "european";
        public static readonly string HealthLeader = "health leader";
        public static readonly string NatureFriend = "nature friend";
        public static readonly string Photograph = "photograph";

        public static readonly Dictionary<string, string> ScoutAbilitiesTranslation = new()
        {
            { Hygenist, "higienista" },
            { Paramedic, "sanitariusz" },
            { Lifesaver, "ratownik" },
            { Glimmer, "ognik" },
            { FireGuard, "straznik ognia" },
            { FireplaceMaster, "mistrz ognisk" },
            { DrillExpert, "znawca musztry" },
            { DrillMaster, "mistrz musztry" },
            { Needle, "igielka" },
            { Tailor, "krawiec" },
            { YoungSwimmer, "mlody plywak" },
            { Swimmer, "plywak" },
            { ExcellentSwimmer, "plywak doskonaly" },
            { Internaut, "internauta" },
            { FamilyHistorian, "historyk rodzinny" },
            { European, "europejczyk" },
            { HealthLeader, "lider zdrowia" },
            { NatureFriend, "przyjaciel przyrody" },
            { Photograph, "fotograf" }
        };

        public static readonly Dictionary<string, string> ScoutAbilitiesTranslationWithPolishLetters = new()
        {
            { Hygenist, "Higienista" },
            { Paramedic, "Sanitariusz" },
            { Lifesaver, "Ratownik" },
            { Glimmer, "Ognik" },
            { FireGuard, "Strażnik ognia" },
            { FireplaceMaster, "Mistrz ognisk" },
            { DrillExpert, "Znawca musztry" },
            { DrillMaster, "Mistrz musztry" },
            { Needle, "Igiełka" },
            { Tailor, "Krawiec" },
            { YoungSwimmer, "Młody pływak" },
            { Swimmer, "Pływak" },
            { ExcellentSwimmer, "Pływak doskonały" },
            { Internaut, "Internauta" },
            { FamilyHistorian, "Historyk rodzinny" },
            { European, "Europejczyk" },
            { HealthLeader, "Lider zdrowia" },
            { NatureFriend, "Przyjaciel przyrody" },
            { Photograph, "Fotograf" }
        };
    }
}