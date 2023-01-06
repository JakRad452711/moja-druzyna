using moja_druzyna.Models;

namespace moja_druzyna.Lib.Order
{
    public class GamePointsEntry : IOrderElement
    {
        public string ScoutId { get; set; }
        public string ScoutPesel { get; set; }
        public string ScoutName { get; set; }
        public string ScoutSurname { get; set; }
        public string GameName { get; set; }
        public int Points { get; set; }

        public string GetScoutId()
        {
            return ScoutId;
        }
    }
}