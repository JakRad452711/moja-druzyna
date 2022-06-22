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
        public List<ClosingTrial> closings;
        public List<OpenTrial> opens;
        public List<Game> games;
        public List<Extraordinary> extras;
        public List<Deletion> deletions;
        public List<string> others;

        public Order(string number, string team, string date, string location, 
            List<Releasing> releasings, List<Appointment> appointments, List<ClosingTrial> closings, List<OpenTrial> opens,
            List<Game> games, List<Extraordinary> extras, List<Deletion> deletions, List<string> others)
        {
            this.number = number;
            this.team = team;
            this.date = date;
            this.location = location;
            this.releasings = releasings;
            this.appointments = appointments;
            this.closings = closings;  
            this.opens = opens;
            this.games = games;
            this.extras = extras;
            this.deletions = deletions;
            this.others = others;
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
