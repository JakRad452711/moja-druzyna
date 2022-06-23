using moja_druzyna.Lib.Order;
using System.Collections.Generic;

namespace moja_druzyna.ViewModels.DocumentsGenerators
{
    public class AppointmentsViewModel
    {
        public AppointmentsViewModel()
        {
            Appointments = new();
        }

        public string AddedScoutId { get; set; }
        public List<Appointment> Appointments { get; set; }
    }
}
