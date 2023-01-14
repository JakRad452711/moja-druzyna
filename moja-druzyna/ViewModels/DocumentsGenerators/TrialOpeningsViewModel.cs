using moja_druzyna.Const;
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

        public string AddedScoutIdRank { get; set; }
        public string AddedScoutIdAbility { get; set; }
        public string AddedScoutIdCross { get; set; }
        public string TrialType { get; set; }
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
            if (TrialType == TrialTypes.Rank)
                return AddedScoutIdRank;
            else if (TrialType == TrialTypes.Ability)
                return AddedScoutIdAbility;
            else
                return AddedScoutIdCross;
        }
    }
}
