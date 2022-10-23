using System.Collections.Generic;

namespace moja_druzyna.Const
{
    public static class TeamRoles
    {
        public readonly static string Captain = "captain";
        public readonly static string ViceCaptain = "vice captain";
        public readonly static string HostCaptain = "host captain";
        public readonly static string Ensign = "ensign";
        public readonly static string Chronicler = "chronicler";
        public readonly static string Quatermaster = "quatermaster";
        public readonly static string Scout = "scout";
        public readonly static string Parent = "parent";

        public readonly static List<string> TeamRolesList = new() { Captain, ViceCaptain, HostCaptain, Ensign, Chronicler, Quatermaster, Scout, Parent };

        public readonly static Dictionary<string, string> TeamRolesTranslations = new() 
        {
            { Captain, "Druzynowy" },
            { ViceCaptain, "Przyboczny" },
            { HostCaptain, "Zastepowy" },
            { Ensign, "Chorazy druzyny" },
            { Chronicler, "Kronikarz" },
            { Quatermaster, "Kwatermistrz" }
        };
    }
}
