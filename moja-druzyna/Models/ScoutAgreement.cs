using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

#nullable disable

namespace moja_druzyna.Models​
{
    public partial class ScoutAgreement
    {
        public DateTime DateSign { get; set; }
        public DateTime? DataCancel { get; set; }
        [ForeignKey("fk_scoutagreement_scout")]
        [RegularExpression("[0-9]{11}")]
        public string ScoutPeselScout { get; set; }
        [ForeignKey("fk_scoutagreement_agreement")]
        public int AgreementIdAgreement { get; set; }

        public virtual Agreement Agreement { get; set; }
        public virtual Scout Scout { get; set; }
    }
}
