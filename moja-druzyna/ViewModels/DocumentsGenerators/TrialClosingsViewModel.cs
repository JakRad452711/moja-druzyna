using moja_druzyna.Const;
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

        public string AddedScoutIdRank { get; set; }
        public string AddedScoutIdAbility { get; set; }
        public string AddedScoutIdCross { get; set; }
        public string TrialType { get; set; }
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
            if (TrialType == TrialTypes.Rank)
                return AddedScoutIdRank;
            else if (TrialType == TrialTypes.Ability)
                return AddedScoutIdAbility;
            else
                return AddedScoutIdCross;
        }
    }
}
