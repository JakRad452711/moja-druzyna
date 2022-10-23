using moja_druzyna.Models;

namespace moja_druzyna.Lib.Order
{
    public class Game : IOrderElement
    {
        public Scout Person { get; set; }
        public string GameName { get; set; }
        public int Points { get; set; }

        public string GetScoutId()
        {
            return Person.IdentityId;
        }
    }
}