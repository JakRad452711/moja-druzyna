using moja_druzyna.Lib.Order;
using System.Collections.Generic;

namespace moja_druzyna.ViewModels.DocumentsGenerators
{
    public class LayoffsViewModel : IFormOrderViewModel
    {
        public LayoffsViewModel()
        {
            Layoffs = new();
        }

        public string AddedScoutId { get; set; }
        public List<Layoff> Layoffs { get; set; }

        public void AddElement(string scoutId, string pesel, string name, string surname)
        {
            Layoffs.Add(
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
            return Layoffs.ConvertAll(x => (IOrderElement)x);
        }

        public string GetScoutId()
        {
            return AddedScoutId;
        }
    }
}
