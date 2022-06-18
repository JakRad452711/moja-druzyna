using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
#nullable disable

namespace moja_druzyna.Models​
{
    public partial class Host
    {
        public Host()
        {
            this.AttendanceLists = new List<AttendanceList>();
            this.ScoutHost       = new List<ScoutHost>();
        }

        [Key]
        public int IdHost { get; set; }
        [MaxLength(50)]
        public string Name { get; set; }
        [ForeignKey("fk_host_team")]
        public int TeamIdTeam { get; set; }
        public bool IsDefaultHost { get; set; }

        public virtual Team Team { get; set; }
        public virtual ICollection<AttendanceList> AttendanceLists { get; set; }
        public virtual ICollection<ScoutHost> ScoutHost { get; set; }
    }
}
