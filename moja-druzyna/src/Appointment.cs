using moja_druzyna.Models;

namespace moja_druzyna.src
{
    public class Appointment
    {
        public string peselScout { get; set; }
        public string nameScout { get; set; }
        public string surnameScout { get; set; }
        private string function;
        private string patrol;

        public Appointment(Scout person, string function, string patrol)
        {
            peselScout = person.PeselScout;
            nameScout = person.Name;
            surnameScout = person.Surname;
            this.function = function;
            this.patrol = patrol;
        }

        public string Function
        {
            get { return function; }
            set { }
        }

        public string Patrol
        {
            get { return patrol; }
            set { }
        }
    }
}
