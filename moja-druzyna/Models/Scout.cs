using System.ComponentModel.DataAnnotations;

#nullable disable

namespace moja_druzyna.Models
{
    public partial class Scout
    {
        public string Name { get; set; }
        public string SecondName { get; set; }
        public string Surname { get; set; }
        [Key]
        public string Pesel { get; set; }
        public string DateOfBirth { get; set; }
        public string Nationality { get; set; }
        public string ScoutId { get; set; }
    }
}
