using moja_druzyna.Lib.Order;
using System.Collections.Generic;

namespace moja_druzyna.ViewModels.DocumentsGenerators
{
    public class AppointmentsViewModel : IFormOrderViewModel
    {
        public AppointmentsViewModel()
        {
            Appointments = new();
        }

        public string AddedScoutId { get; set; }
        public List<Appointment> Appointments { get; set; }

        public void AddElement(string scoutId, string pesel, string name, string surname)
        {
            Appointments.Add(
                new()
                {
                    ScoutId = scoutId,
                    ScoutPesel = pesel,
                    ScoutName = name,
                    ScoutSurname = surname
                });
        }

        public List<IOrderElement> GetList()
        {
            return Appointments.ConvertAll(x => (IOrderElement)x);
        }

        public string GetScoutId()
        {
            return AddedScoutId;
        }
    }
}
