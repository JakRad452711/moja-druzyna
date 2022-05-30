using System;
using System.Collections.Generic;

#nullable disable

namespace moja_druzyna.Models​
{
    public partial class TrainingCourse
    {
        public TrainingCourse()
        {
            ScoutCourses = new HashSet<ScoutCourse>();
        }

        public int IdCourse { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public virtual ICollection<ScoutCourse> ScoutCourses { get; set; }
    }
}
