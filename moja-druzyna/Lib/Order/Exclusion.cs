using moja_druzyna.Data;
using moja_druzyna.Models;
using System.Collections.Generic;
using System.Linq;

namespace moja_druzyna.Lib.Order
{
    public class Exclusion : IOrderElement
    {
        public string ScoutId { get; set; }
        public string ScoutPesel { get; set; }
        public string ScoutName { get; set; }
        public string ScoutSurname { get; set; }
        public string Reason { get; set; }

        public string GetScoutId()
        {
            return ScoutId;
        }
    }
}
