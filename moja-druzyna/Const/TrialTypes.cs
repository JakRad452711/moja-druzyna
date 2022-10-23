using System.Collections.Generic;

namespace moja_druzyna.Const
{
    public static class TrialTypes
    {
        public static readonly string Rank = "rank";
        public static readonly string Ability = "ability";
        public static readonly string ScoutCross = "scout cross";

        public static readonly List<string> TrialTypesList = new() { Rank, Ability, ScoutCross };
    }
}
