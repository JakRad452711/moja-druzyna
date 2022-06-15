using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

#nullable disable

namespace moja_druzyna.Models​
{
    public partial class Scout
    {
        
        [Key]
        [Required(ErrorMessage = "Ta rubryka musi zostać wypełniona")]
        [RegularExpression("[0-9]{11}", ErrorMessage = "niepoprawny numer PESEL został wprowadzony")]
        public string PeselScout { get; set; }
        [MaxLength(50)]
        [Required(ErrorMessage = "Ta rubryka musi zostać wypełniona")]
        public string Name { get; set; }
        [MaxLength(50)]
        public string SecondName { get; set; }
        [MaxLength(50)]
        [Required(ErrorMessage = "Ta rubryka musi zostać wypełniona")]
        public string Surname { get; set; }
        public DateTime DateOfBirth { get; set; }
        [MaxLength(50)]
        public string Nationality { get; set; }
        public string MembershipNumber { get; set; }
        public DateTime DateOfEntry { get; set; }
        public string Ns { get; set; }
        public DateTime? DateOfLeaving { get; set; }
        public string CrossNumber { get; set; }
        [ForeignKey("fk_scout_parent")]
        public string ParentParentPesel { get; set; }
        [ForeignKey("fk_scout_identity")]
        public string IdentityId { get; set; }

        public IdentityUser Identity { get; set; }
        public virtual Parent Parent { get; set; }
        public virtual DutyHistory DutyHistory { get; set; }
        public virtual ScoutAchievement ScoutAchievement { get; set; }
        public virtual Address Adress { get; set; }
        public virtual ICollection<ScoutAgreement> ScoutAgreements { get; set; }
        public virtual ICollection<ScoutCollection> ScoutCollections { get; set; }
        public virtual ICollection<ScoutCourse> ScoutCourses { get; set; }
        public virtual ICollection<ScoutEvent> ScoutEvents { get; set; }
        public virtual ICollection<ScoutRank> ScoutRanks { get; set; }
        public virtual ICollection<ScoutHost> ScoutHost { get; set; }
        public virtual ICollection<ScoutTeam> ScoutTeam { get; set; }
        public virtual ICollection<AttendanceList> AttendanceList { get; set; }
        public virtual ICollection<Points> Points { get; set; }
    }
}
