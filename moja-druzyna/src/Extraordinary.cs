using moja_druzyna.Models;

namespace moja_druzyna.src
{
    public class Extraordinary
    {
        public Scout person { get; set; }
        public string type { get; set; }    
        public string justification { get; set; }

        public Extraordinary(Scout person, string type, string justification)
        {
            this.person = person;
            this.type = type;
            this.justification = justification;
        }
    }
}