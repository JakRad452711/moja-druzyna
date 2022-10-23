using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using moja_druzyna.Data;
using moja_druzyna.Lib.Exceptions;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

#nullable disable

namespace moja_druzyna.Models​
{
    public partial class Scout
    {
        private ApplicationDbContext _dbContext;

        public Scout()
        {
            this.ScoutAgreements = new List<ScoutAgreement>();
            this.ScoutCollections = new List<ScoutCollection>();
            this.ScoutCourses = new List<ScoutCourse>();
            this.ScoutRanks = new List<ScoutRank>();
            this.ScoutHost = new List<ScoutHost>();
            this.ScoutTeam = new List<ScoutTeam>();
            this.AttendanceList = new List<AttendanceList>();
            this.Points = new List<Points>();
        }

        [Key]
        [Required(ErrorMessage = "ta rubryka musi zostać wypełniona")]
        [RegularExpression("[0-9]{11}", ErrorMessage = "niepoprawny numer PESEL został wprowadzony")]
        [MaxLength(11)]
        public string PeselScout { get; set; }
        [MaxLength(50)]
        [Required(ErrorMessage = "ta rubryka musi zostać wypełniona")]
        public string Name { get; set; }
        [MaxLength(50)]
        public string SecondName { get; set; }
        [MaxLength(50)]
        [Required(ErrorMessage = "ta rubryka musi zostać wypełniona")]
        public string Surname { get; set; }
        public DateTime? DateOfBirth { get; set; }
        [MaxLength(100)]
        public string Nationality { get; set; }
        [MaxLength(50)]
        public string MembershipNumber { get; set; }
        public DateTime? DateOfEntry { get; set; }
        public bool Ns { get; set; }
        public DateTime? DateOfLeaving { get; set; }
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

        public static Scout GetScout(ApplicationDbContext dbContext, string scoutPesel)
        {
            if (dbContext == null)
                throw new System.ArgumentNullException();

            bool scoutWithGivenPeselExists = dbContext.Scouts.Find(scoutPesel) != null;

            if (!scoutWithGivenPeselExists)
                throw new RecordNotFoundException(string.Format("Scout with pesel '{0}' doesn't exist", scoutPesel));

            Scout scout = dbContext.Scouts.Where(s => s.PeselScout == scoutPesel)
                .Include(s => s.Identity)
                .Include(s => s.Parent)
                .Include(s => s.DutyHistory)
                .Include(s => s.ScoutAchievement)
                .Include(s => s.Adress)
                .Include(s => s.ScoutAgreements)
                .Include(s => s.ScoutCollections)
                .Include(s => s.ScoutCourses)
                .Include(s => s.ScoutEvents)
                .Include(s => s.ScoutRanks)
                .Include(s => s.ScoutHost)
                .Include(s => s.ScoutTeam)
                .Include(s => s.AttendanceList)
                .Include(s => s.Points)
                .First();

            scout._dbContext = dbContext;

            return scout;
        }

        public static Scout GetScoutById(ApplicationDbContext dbContext, string scoutId)
        {
            if (dbContext == null)
                throw new System.ArgumentNullException();

            bool scoutWithGivenIdExists = dbContext.Scouts.Any(s => s.IdentityId == scoutId);

            if (!scoutWithGivenIdExists)
                throw new RecordNotFoundException(string.Format("Scout with identity id '{0}' doesn't exist", scoutId));

            Scout scout = dbContext.Scouts.Where(s => s.IdentityId == scoutId)
                .Include(s => s.Identity)
                .Include(s => s.Parent)
                .Include(s => s.DutyHistory)
                .Include(s => s.ScoutAchievement)
                .Include(s => s.Adress)
                .Include(s => s.ScoutAgreements)
                .Include(s => s.ScoutCollections)
                .Include(s => s.ScoutCourses)
                .Include(s => s.ScoutEvents)
                .Include(s => s.ScoutRanks)
                .Include(s => s.ScoutHost)
                .Include(s => s.ScoutTeam)
                .Include(s => s.AttendanceList)
                .Include(s => s.Points)
                .First();

            scout._dbContext = dbContext;

            return scout;
        }

        public void Edit(Scout newScout)
        {
            CheckDbContextInstance("void Edit(Scout newScout)");

            Scout oldScout = _dbContext.Scouts.Find(newScout.PeselScout);

            oldScout.Name = ReturnNotNullValue<string> (newScout.Name, oldScout.Name);
            oldScout.Surname = ReturnNotNullValue<string>(newScout.Surname, oldScout.Surname);
            oldScout.SecondName = ReturnNotNullValue<string>(newScout.SecondName, oldScout.SecondName);
            oldScout.MembershipNumber = ReturnNotNullValue<string>(newScout.MembershipNumber, oldScout.MembershipNumber);
            oldScout.DateOfBirth = ReturnNotNullValue<DateTime?>(newScout.DateOfBirth, oldScout.DateOfBirth);
            oldScout.Nationality = ReturnNotNullValue<string>(newScout.Nationality, oldScout.Nationality);
            oldScout.Ns = newScout.Ns;
            oldScout.DateOfEntry = ReturnNotNullValue<DateTime?>(newScout.DateOfEntry, oldScout.DateOfEntry);
            oldScout.DateOfLeaving = ReturnNotNullValue<DateTime?>(newScout.DateOfLeaving, oldScout.DateOfLeaving);

            if (newScout.Adress != null)
            {
                Address address = _dbContext.Adresses.First(_address => _address.ScoutPeselScout == newScout.PeselScout);

                address.AddressKor = ReturnNotNullValue<string>(newScout.Adress.AddressKor, address.AddressKor);
                address.CityKor = ReturnNotNullValue<string>(newScout.Adress.CityKor, address.CityKor);
                address.CountryKor = ReturnNotNullValue<string>(newScout.Adress.CountryKor, address.CountryKor);
                address.NumberHouseKor = ReturnNotNullValue<string>(newScout.Adress.NumberHouseKor, address.NumberHouseKor);
                address.StreetKor = ReturnNotNullValue<string>(newScout.Adress.StreetKor, address.StreetKor);
                address.ZipKor = ReturnNotNullValue<string>(newScout.Adress.ZipKor, address.ZipKor);
                address.AddressZam = ReturnNotNullValue<string>(newScout.Adress.AddressZam, address.AddressZam);
                address.CityZam = ReturnNotNullValue<string>(newScout.Adress.CityZam, address.CityZam);
                address.CountryZam = ReturnNotNullValue<string>(newScout.Adress.CountryZam, address.CountryZam);
                address.NumberHouseZam = ReturnNotNullValue<string>(newScout.Adress.NumberHouseZam, address.NumberHouseZam);
                address.StreetZam = ReturnNotNullValue<string>(newScout.Adress.StreetZam, address.StreetZam);
                address.ZipZam = ReturnNotNullValue<string>(newScout.Adress.ZipZam, address.ZipZam);

                this.Adress = address;
                _dbContext.Adresses.Update(address);
            }

            if (newScout.Parent != null)
            {
                Parent parent = _dbContext.Parents.Where(_parent => _parent.Pesel == newScout.ParentParentPesel).First();

                parent.Adresses = ReturnNotNullValue<Address>(newScout.Parent.Adresses, parent.Adresses);
                parent.Scouts = ReturnNotNullValue<ICollection<Scout>>(newScout.Parent.Scouts, parent.Scouts);
                parent.Name = ReturnNotNullValue<string>(newScout.Parent.Name, parent.Name);
                parent.Surname = ReturnNotNullValue<string>(newScout.Parent.Surname, parent.Surname);

                _dbContext.Parents.Update(parent);
            }

            _dbContext.Scouts.Update(oldScout);
            _dbContext.SaveChanges();
        }

        public List<ScoutAchievement> GetAchievements()
        {
            CheckDbContextInstance("List<ScoutAchievement> GetAchievements()");

            return _dbContext.ScoutAchievements.Where(sa => sa.ScoutPeselScout == PeselScout).ToList();
        }

        public Rank GetRank()
        {
            CheckDbContextInstance("string GetRank()");

            ScoutRank scoutRank = ScoutRanks.FirstOrDefault(sr => sr.IsCurrent);

            if (scoutRank == null)
                return null;

            return _dbContext.Ranks.First(r => r.Name == scoutRank.RankName);
        }

        private T ReturnNotNullValue<T>(T a, T b)
        {
            return (a == null ? b : a);
        }

        private void CheckDbContextInstance(string methodName)
        {
            string message = string.Format("Scout instance needs to be initialized with a dbContext instance to use '{0}' method. " +
                "Use 'Scout GetScout(ApplicationDbContext dbContext, string scoutPesel)' or " +
                "'Scout GetScoutById(ApplicationDbContext dbContext, string scoutId)' static method to get a Team object with a dbContext instance.", methodName);

            if (_dbContext == null)
                throw new MissingDbContextInstanceException(message);
        }
    }
}
