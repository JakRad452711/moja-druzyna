#nullable disable

namespace moja_druzyna.Models
{
    public partial class ScoutTeam
    {
        public int? TeamId { get; set; }
        public string Pesel { get; set; }
        public string TeamPosition { get; set; }
        /*
        public string date_of_interaction { get; set; }

        // True = joining Team; False = quiting team
        public bool type_of_interaction { get; set; } 

        public string Patrol { get; set; }
        */
        public virtual Scout PeselNavigation { get; set; }
        public virtual Team Team { get; set; }
    }
}
