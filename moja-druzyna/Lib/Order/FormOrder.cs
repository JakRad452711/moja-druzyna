﻿using System;
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
            GamePointsEntries = new();
            ReprimendsAndPraises = new();
            Exclusions = new();
            Other = new();
        }

        public string OrderNumber { get; set; }
        public string TeamName { get; set; }
        public DateTime CreationDate { get; set; }
        public string Location { get; set; }

        public List<Layoff> Layoffs { get; set; }
        public List<Appointment> Appointments { get; set; }
        public List<TrialClosing> TrialClosings { get; set; }
        public List<TrialOpening> TrialOpenings { get; set; }
        public List<GamePointsEntry> GamePointsEntries { get; set; }
        public List<ReprimendsAndPraises> ReprimendsAndPraises { get; set; }
        public List<Exclusion> Exclusions { get; set; }
        public Other Other { get; set; }
    }
}
