using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace moja_druzyna.Models
{
    public class OrderInfo
    {
        [Key]
        public int Id { get; set; }
        [MaxLength(100)]
        public string Name { get; set; }
        public DateTime CreationDate { get; set; }

        [ForeignKey("fk_orderInfo_order")]
        public int OrderId { get; set; }
        [ForeignKey("fk_orderInfo_scout")]
        public string ScoutPeselScout { get; set; }
        [ForeignKey("fk_orderInfo_team")]
        public int TeamIdTeam { get; set; }
        public virtual Order Order { get; set; }
        public virtual Scout Scout { get; set; }
        public virtual Team Team { get; set; }
    }
}
