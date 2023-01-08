using moja_druzyna.Lib.Order;
using System.Collections.Generic;

namespace moja_druzyna.ViewModels.DocumentsGenerators
{
    public class ReprimendsAndPraisesViewModel : IFormOrderViewModel
    {
        public ReprimendsAndPraisesViewModel()
        {
            ReprimendsAndPraises = new();
        }

        public string AddedScoutId { get; set; }
        public List<ReprimendsAndPraises> ReprimendsAndPraises { get; set; }

        public void AddElement(string scoutId, string pesel, string name, string surname)
        {
            ReprimendsAndPraises.Add(
                new()
                {
                    ScoutId = scoutId,
                    ScoutPesel = pesel,
                    ScoutName = name,
                    ScoutSurname = surname
                });
        }

        public List<IOrderElement> GetList()
        {
            return ReprimendsAndPraises.ConvertAll(x => (IOrderElement)x);
        }

        public string GetScoutId()
        {
            return AddedScoutId;
        }
    }
}
