using System.Collections.Generic;

namespace moja_druzyna.Const
{
    public static class HostRoles
    {
        public readonly static string HostCaptain = "captain";
        public readonly static string Scout = "scout";

        public readonly static List<string> HostRolesList = new() { HostCaptain, Scout };
    }
}
