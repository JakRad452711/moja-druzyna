using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace moja_druzyna.Models
{
    public class Order
    {
        [Key]
        public int Id { get; set; }
        [MaxLength(12000)]
        public string Contents { get; set; }
    }
}