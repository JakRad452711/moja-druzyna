using System.Collections.Generic;

namespace moja_druzyna.Const
{
    public static class ScoutRanks
    {
        public static readonly string Rank1 = "1";
        public static readonly string Rank2 = "2";
        public static readonly string Rank3 = "3";
        public static readonly string Rank4 = "4";
        public static readonly string Rank5 = "5";
        public static readonly string Rank6 = "6";

        public static readonly List<string> ScoutRanksList = new() { Rank1, Rank2, Rank3, Rank4, Rank5, Rank6 };
        public static readonly Dictionary<string, string> ScoutRanksTranslation = new()
        {
            { Rank1, "Młodzik" },
            { Rank2, "Wywiadowca" },
            { Rank3, "Odkrywca" },
            { Rank4, "Ćwik" },
            { Rank5, "Harcerz orli" },
            { Rank6, "Harcerz rzeczypospolitej" }
        };
    }
}
