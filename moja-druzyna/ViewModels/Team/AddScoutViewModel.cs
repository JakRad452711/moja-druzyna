using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace moja_druzyna.ViewModels.Team
{
    public class AddScoutViewModel
    {
        [Required(ErrorMessage = "ta rubryka musi zostać wypełniona")]
        [RegularExpression("[0-9]{11}", ErrorMessage = "niepoprawny numer PESEL został wprowadzony")]
        public string Pesel { get; set; }
        public string MembershipNumber { get; set; }
        [Required(ErrorMessage = "ta rubryka musi zostać wypełniona")]
        public string Name { get; set; }
        [Required(ErrorMessage = "ta rubryka musi zostać wypełniona")]
        public string Surname { get; set; }
        public string SecondName { get; set; }
        public string Nationality { get; set; }
        public DateTime? DateOfEntry { get; set; }
        public bool Ns { get; set; }
    }
}
