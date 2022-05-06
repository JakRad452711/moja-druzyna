using System.ComponentModel.DataAnnotations;

#nullable disable

namespace moja_druzyna.Models
{
    public partial class Team
    {
        public string TeamName { get; set; }
        [Key]
        public int TeamId { get; set; }
    }
}
