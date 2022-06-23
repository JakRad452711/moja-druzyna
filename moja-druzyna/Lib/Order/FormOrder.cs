using System;
using System.Collections.Generic;

namespace moja_druzyna.Lib.Order
{
    public class FormOrder
    {
        public FormOrder()
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
        public List<Appointment> Appointments { get; set; }
        public List<TrialClosing> TrialClosings { get; set; }
        public List<TrialOpening> TrialOpenings { get; set; }
        public List<ReprimendsAndPraises> ReprimendsAndPraises { get; set; }
        public List<Exclusion> Exclusions { get; set; }
        public Other Other { get; set; }
    }
}
