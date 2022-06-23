using moja_druzyna.Lib.Order;
using System.Collections.Generic;

namespace moja_druzyna.ViewModels.DocumentsGenerators
{
    public class TrialClosingsViewModel
    {
        public TrialClosingsViewModel()
        {
            TrialClosings = new();
        }

        public string AddedScoutId { get; set; }
        public List<TrialClosing> TrialClosings { get; set; }
    }
}
