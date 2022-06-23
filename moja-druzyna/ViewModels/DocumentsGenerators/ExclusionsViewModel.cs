using moja_druzyna.Lib.Order;
using System.Collections.Generic;

namespace moja_druzyna.ViewModels.DocumentsGenerators
{
    public class ExclusionsViewModel
    {
        public ExclusionsViewModel()
        {
            Exclusions = new();
        }

        public string AddedScoutId { get; set; }
        public List<Exclusion> Exclusions { get; set; }
    }
}
