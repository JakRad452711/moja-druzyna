﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

#nullable disable

namespace moja_druzyna.Models​
{
    public partial class DutyHistory
    {
        public DateTime DateStart { get; set; }
        public DateTime? DateEnd { get; set; }
        public string Team { get; set; }
        public string Detachment { get; set; }
        public string Banner { get; set; }
        public string Host { get; set; }
        [ForeignKey("fk_DutyHistory_scout")]
        [RegularExpression("[0-9]{11}")]
        public string ScoutPeselScout { get; set; }

        public virtual Scout Scout { get; set; }
    }
}
