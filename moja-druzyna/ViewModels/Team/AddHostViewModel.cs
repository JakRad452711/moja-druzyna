using System.ComponentModel.DataAnnotations;

namespace moja_druzyna.ViewModels.Team
{
    public class AddHostViewModel
    {
        [Required(ErrorMessage = "ta rubryka musi zostać wypełniona")]
        public string HostName { get; set; }
        public string HostCaptainPesel { get; set; }
    }
}
