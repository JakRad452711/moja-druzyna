using moja_druzyna.Lib.Order;
using System.Collections.Generic;

namespace moja_druzyna.ViewModels.DocumentsGenerators
{
    public class ExclusionsViewModel : IFormOrderViewModel
    {
        public ExclusionsViewModel()
        {
            Exclusions = new();
        }

        public string AddedScoutId { get; set; }
        public List<Exclusion> Exclusions { get; set; }

        public void AddElement(string scoutId, string pesel, string name, string surname)
        {
            Exclusions.Add(
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
            return Exclusions.ConvertAll(x => (IOrderElement)x);
        }

        public string GetScoutId()
        {
            return AddedScoutId;
        }
    }
}
