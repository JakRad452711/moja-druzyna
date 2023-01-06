using moja_druzyna.Lib.Order;
using System.Collections.Generic;

namespace moja_druzyna.ViewModels.DocumentsGenerators
{
    public class GamePointsViewModel : IFormOrderViewModel
    {
        public GamePointsViewModel()
        {
            GamePointEntries = new();
        }

        public string AddedScoutId { get; set; }
        public List<GamePointsEntry> GamePointEntries { get; set; }

        public void AddElement(string scoutId, string pesel, string name, string surname)
        {
            GamePointEntries.Add(
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
            return GamePointEntries.ConvertAll(x => (IOrderElement)x);
        }

        public string GetScoutId()
        {
            return AddedScoutId;
        }
    }
}
