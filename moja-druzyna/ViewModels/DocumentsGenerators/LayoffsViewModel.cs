using moja_druzyna.Lib.Order;
using System.Collections.Generic;

namespace moja_druzyna.ViewModels.DocumentsGenerators
{
    public class LayoffsViewModel
    {
        public LayoffsViewModel()
        {
            Layoffs = new();
        }

        public string AddedScoutId { get; set; }
        public List<Layoff> Layoffs { get; set; }
    }
}
