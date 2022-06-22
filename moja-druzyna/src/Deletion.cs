using moja_druzyna.Models;

namespace moja_druzyna.src
{
    public class Deletion
    {
        public Scout person { get; set; }
        public string justification { get; set; }

        public Deletion(Scout person, string justification)
        {
            this.person = person;
            this.justification = justification;
        }
    }
}