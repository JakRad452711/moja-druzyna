using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;

#nullable disable

namespace moja_druzyna.Models​
{
    public partial class Scout: IdentityUser
    {
        public Scout()
        {
            Adresses = new HashSet<Adress>();
            ScoutAgreements = new HashSet<ScoutAgreement>();
            ScoutCollections = new HashSet<ScoutCollection>();
            ScoutCourses = new HashSet<ScoutCourse>();
            ScoutEvents = new HashSet<ScoutEvent>();
            ScoutRanks = new HashSet<ScoutRank>();
            ScoutTeams = new HashSet<ScoutTeam>();
            AttendanceList = new HashSet<AttendanceList>();
        }

        public string PeselScout { get; set; }
        public string SecondName { get; set; }
        public string Surname { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string Nationality { get; set; }
        public string MembershipNumber { get; set; }
        public DateTime DateOfEntry { get; set; }
        public string Ns { get; set; }
        public DateTime? DateOfLeaving { get; set; }
        public string CrossNumber { get; set; }
        public string PeselParent { get; set; }
        
        public virtual Parent PeselParentNavigation { get; set; }
        public virtual DutyHistory DutyHistory { get; set; }
        public virtual ScoutAchievement ScoutAchievement { get; set; }
        public virtual ICollection<Adress> Adresses { get; set; }
        public virtual ICollection<ScoutAgreement> ScoutAgreements { get; set; }
        public virtual ICollection<ScoutCollection> ScoutCollections { get; set; }
        public virtual ICollection<ScoutCourse> ScoutCourses { get; set; }
        public virtual ICollection<ScoutEvent> ScoutEvents { get; set; }
        public virtual ICollection<ScoutRank> ScoutRanks { get; set; }
        public virtual ICollection<ScoutTeam> ScoutTeams { get; set; }
        public virtual ICollection<AttendanceList> AttendanceList { get; set; }
    }
}
