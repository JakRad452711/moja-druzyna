using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

#nullable disable

namespace moja_druzyna.Models​
{
    public partial class ScoutCourse
    {
        public DateTime DateAcquirement { get; set; }
        [ForeignKey("fk_scoutcourse_scout")]
        [RegularExpression("[0-9]{11}")]
        public string ScoutPeselScout { get; set; }
        [ForeignKey("fk_scoutcourse_course")]
        public int TrainingCourseIdCourse { get; set; }

        public virtual TrainingCourse TrainingCourse { get; set; }
        public virtual Scout Scout { get; set; }
    }
}
