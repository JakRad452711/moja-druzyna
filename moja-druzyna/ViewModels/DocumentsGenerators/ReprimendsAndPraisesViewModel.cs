using moja_druzyna.Lib.Order;
using System.Collections.Generic;

namespace moja_druzyna.ViewModels.DocumentsGenerators
{
    public class ReprimendsAndPraisesViewModel
    {
        public ReprimendsAndPraisesViewModel()
        {
            ReprimendsAndPraises = new();
        }

        public string AddedScoutId { get; set; }
        public List<ReprimendsAndPraises> ReprimendsAndPraises { get; set; }
    }
}
