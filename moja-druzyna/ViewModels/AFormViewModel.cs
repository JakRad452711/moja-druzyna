﻿using moja_druzyna.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace moja_druzyna.ViewModels
{
    public class AFormViewModel
    {
        public AFormViewModel()
        {
            AForm_Scouts = new List<AForm_Scout>();
        }

        public void AddScout(Scout scout)
        {
            AForm_Scouts.Add(new AForm_Scout() { Id = scout.PeselScout, Label = string.Format("{0} {1}\t({2})", scout.Name, scout.Surname, scout.PeselScout), Prop1 = "", Prop2 = "" });
        }

        public List<AForm_Scout> AForm_Scouts { get; set; }

        public class AForm_Scout
        {
            public string Id { get; set; }
            public string Label { get; set; }
            public string Prop1 { get; set; }
            public string Prop2 { get; set; }
        }
    }
}
