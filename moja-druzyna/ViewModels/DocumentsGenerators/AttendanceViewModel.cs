using System;
using System.Collections.Generic;
using System.Linq;

namespace moja_druzyna.ViewModels.DocumentsGenerators
{
    public class AttendanceViewModel
    {
        public AttendanceViewModel()
        {
            AttendanceViewModel_Lists = new List<AttendanceViewModel_List>();
        }

        public List<AttendanceViewModel_List> AttendanceViewModel_Lists { get; set; }
        public int EventId { get; set; }

        public class AttendanceViewModel_List
        {
            public string IdScout { get; set; }
            public string Name { get; set; }
            public string Surname { get; set; }
            public string Host { get; set; }
            public bool IsPresent { get; set; }
            public int EventId { get; set; }

        }
    }
}
