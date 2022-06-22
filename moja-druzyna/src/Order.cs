using System.Collections.Generic;

namespace moja_druzyna.src
{
    public class Order
    {
        private string number;
        private string team;
        private string date;
        private string location;
        public List<Releasing> releasings;
        public List<Appointment> appointments;

        public Order(string number, string team, string date, string location, List<Releasing> releasings, List<Appointment> appointments)
        {
            this.number = number;
            this.team = team;
            this.date = date;
            this.location = location;
            this.releasings = releasings;
            this.appointments = appointments;
        }

        public string Number
        {
            get { return this.number; }
        }

        public string Team
        {
            get { return this.team; }
        }

        public string Date
        {
            get { return this.date; }
        }

        public string Location
        {
            get { return this.location; }
        }
    }
}
