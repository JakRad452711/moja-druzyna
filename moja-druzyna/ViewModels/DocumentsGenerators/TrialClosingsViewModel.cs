using moja_druzyna.Lib.Order;
using System.Collections.Generic;

namespace moja_druzyna.ViewModels.DocumentsGenerators
{
    public class TrialClosingsViewModel : IFormOrderViewModel
    {
        public TrialClosingsViewModel()
        {
            TrialClosings = new();
        }

        public string AddedScoutId { get; set; }
        public List<TrialClosing> TrialClosings { get; set; }

        public void AddElement(string scoutId, string pesel, string name, string surname)
        {
            TrialClosings.Add(
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
            return TrialClosings.ConvertAll(x => (IOrderElement) x);
        }

        public string GetScoutId()
        {
            return AddedScoutId;
        }
    }
}
