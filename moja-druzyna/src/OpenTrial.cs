using moja_druzyna.Models;

namespace moja_druzyna.src
{
    public class OpenTrial
    {
        public Scout person { get; set; }
        public string trialType { get; set; }
        public string trialName { get; set; }

        public OpenTrial(Scout person, string trialType, string trialName)
        {
            this.person = person;
            this.trialType = trialType;
            this.trialName = trialName;
        }
    }

    
}