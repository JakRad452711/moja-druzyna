using moja_druzyna.Lib.Order;
using System.Collections.Generic;

namespace moja_druzyna.ViewModels.DocumentsGenerators
{
    public class TrialOpeningsViewModel
    {
        public TrialOpeningsViewModel()
        {
            TrialOpenings = new();
        }

        public string AddedScoutId { get; set; }
        public List<TrialOpening> TrialOpenings { get; set; }
    }
}
