using System.Collections.Generic;

namespace moja_druzyna.ViewModels.Ranking
{
    public class ScoresViewModel
    {
        public ScoresViewModel()
        {
            Scores = new();
        }

        public List<ScoreEntry> Scores { get; set; }

        public class ScoreEntry
        {
            public string Name { get; set; }
            public string Surname { get; set; }
            public string HostName { get; set; }
            public string Rank { get; set; }
            public int Points { get; set; }
        }
    }
}
