using moja_druzyna.Lib.Services.Order;
using System;
using System.Collections.Generic;

namespace moja_druzyna.Data.Session
{
    public class SessionFormOrderContext
    {
        public SessionFormOrderContext()
        {
            Layoffs = new();
            Appointments = new();
            TrialClosings = new();
            TrialOpenings = new();
            ReprimendsAndPraises = new();
            Exclusions = new();
            Other = new();
        }

        public string Name { get; set; }
        public DateTime CreationDate { get; set; }
        public string CreationPlace { get; set; }

        public List<Layoff> Layoffs { get; set; }
        public List<Layoff> LayoffsSaved { get; set; }
        public List<Appointment> Appointments { get; set; }
        public List<Appointment> AppointmentsSaved { get; set; }
        public List<TrialClosing> TrialClosings { get; set; }
        public List<TrialClosing> TrialClosingsSaved { get; set; }
        public List<TrialOpening> TrialOpenings { get; set; }
        public List<TrialOpening> TrialOpeningsSaved { get; set; }
        public List<ReprimendsAndPraises> ReprimendsAndPraises { get; set; }
        public List<ReprimendsAndPraises> ReprimendsAndPraisesSaved { get; set; }
        public List<Exclusion> Exclusions { get; set; }
        public List<Exclusion> ExclusionsSaved { get; set; }
        public Other Other { get; set; }
        public Other OtherSaved { get; set; }
    }
}
