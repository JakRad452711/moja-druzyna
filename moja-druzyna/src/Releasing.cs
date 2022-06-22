using moja_druzyna.Models;

namespace moja_druzyna.src
{
    public class Releasing
    {
        public string peselScout { get; set; }
        public string nameScout { get; set; }
        public string surnameScout { get; set; }

        public string function { get; set; }
        public string patrol { get; set; }

        public Releasing(Scout person, string function, string patrol)
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
