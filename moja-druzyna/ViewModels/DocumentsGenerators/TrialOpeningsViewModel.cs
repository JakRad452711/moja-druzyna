using moja_druzyna.Lib.Order;
using System.Collections.Generic;

namespace moja_druzyna.ViewModels.DocumentsGenerators
{
    public class TrialOpeningsViewModel : IFormOrderViewModel
    {
        public TrialOpeningsViewModel()
        {
            TrialOpenings = new();
        }

        public string AddedScoutId { get; set; }
        public List<TrialOpening> TrialOpenings { get; set; }

        public void AddElement(string scoutId, string pesel, string name, string surname)
        {
            TrialOpenings.Add(
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
            return TrialOpenings.ConvertAll(x => (IOrderElement)x);
        }

        public string GetScoutId()
        {
            return AddedScoutId;
        }
    }
}
