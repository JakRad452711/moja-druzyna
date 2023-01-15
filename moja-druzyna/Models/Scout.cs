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
        public virtual Address Adress { get; set; }

        public virtual ICollection<ScoutAgreement> ScoutAgreements { get; set; }
        public virtual ICollection<ScoutAchievement> ScoutAchievements { get; set; }
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
                .Include(s => s.ScoutAchievements)
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
                .Include(s => s.ScoutAchievements)
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

        public void Edit(Scout changes)
        {
            CheckDbContextInstance("void Edit(Scout newScout)");

            Scout oldScout = this;

            oldScout.Name = ReturnNotNullValue<string> (changes.Name, oldScout.Name);
            oldScout.Surname = ReturnNotNullValue<string>(changes.Surname, oldScout.Surname);
            oldScout.SecondName = ReturnNotNullValue<string>(changes.SecondName, oldScout.SecondName);
            oldScout.MembershipNumber = ReturnNotNullValue<string>(changes.MembershipNumber, oldScout.MembershipNumber);
            oldScout.DateOfBirth = ReturnNotNullValue<DateTime?>(changes.DateOfBirth, oldScout.DateOfBirth);
            oldScout.Nationality = ReturnNotNullValue<string>(changes.Nationality, oldScout.Nationality);
            oldScout.Ns = changes.Ns;
            oldScout.DateOfEntry = ReturnNotNullValue<DateTime?>(changes.DateOfEntry, oldScout.DateOfEntry);
            oldScout.DateOfLeaving = ReturnNotNullValue<DateTime?>(changes.DateOfLeaving, oldScout.DateOfLeaving);

            Address address = _dbContext.Adresses.FirstOrDefault(_address => _address.ScoutPeselScout == changes.PeselScout);

            if (changes.Adress != null && address != null)
            {
                address.AddressKor = ReturnNotNullValue<string>(changes.Adress.AddressKor, address.AddressKor);
                address.CityKor = ReturnNotNullValue<string>(changes.Adress.CityKor, address.CityKor);
                address.CountryKor = ReturnNotNullValue<string>(changes.Adress.CountryKor, address.CountryKor);
                address.NumberHouseKor = ReturnNotNullValue<string>(changes.Adress.NumberHouseKor, address.NumberHouseKor);
                address.StreetKor = ReturnNotNullValue<string>(changes.Adress.StreetKor, address.StreetKor);
                address.ZipKor = ReturnNotNullValue<string>(changes.Adress.ZipKor, address.ZipKor);
                address.AddressZam = ReturnNotNullValue<string>(changes.Adress.AddressZam, address.AddressZam);
                address.CityZam = ReturnNotNullValue<string>(changes.Adress.CityZam, address.CityZam);
                address.CountryZam = ReturnNotNullValue<string>(changes.Adress.CountryZam, address.CountryZam);
                address.NumberHouseZam = ReturnNotNullValue<string>(changes.Adress.NumberHouseZam, address.NumberHouseZam);
                address.StreetZam = ReturnNotNullValue<string>(changes.Adress.StreetZam, address.StreetZam);
                address.ZipZam = ReturnNotNullValue<string>(changes.Adress.ZipZam, address.ZipZam);

                this.Adress = address;
                _dbContext.Adresses.Update(address);
            }
            else if(changes.Adress != null)
            {
                changes.Adress.ScoutPeselScout = PeselScout;
                changes.Adress.ParentPesel = ParentParentPesel == null ? PeselScout : ParentParentPesel;
                this.Adress = changes.Adress;
                _dbContext.Adresses.Add(changes.Adress);
            }

            Parent parent = _dbContext.Parents.FirstOrDefault(_parent => _parent.Pesel == changes.ParentParentPesel);

            if (changes.Parent != null && parent != null)
            {
                parent.Adresses = ReturnNotNullValue<Address>(changes.Parent.Adresses, parent.Adresses);
                parent.Scouts = ReturnNotNullValue<ICollection<Scout>>(changes.Parent.Scouts, parent.Scouts);
                parent.Name = ReturnNotNullValue<string>(changes.Parent.Name, parent.Name);
                parent.Surname = ReturnNotNullValue<string>(changes.Parent.Surname, parent.Surname);

                this.Parent = parent;
                _dbContext.Parents.Update(parent);
            }
            else if(changes.Parent != null)
            {
                if (changes.Parent.Pesel == null)
                    throw new ArgumentException("void Edit(Scout newScout): Parent object has to have a key");

                this.Parent = changes.Parent;
                _dbContext.Parents.Add(changes.Parent);
            }

            _dbContext.Scouts.Update(oldScout);
            _dbContext.SaveChanges();
        }

        public Scout GetShallowCopy()
        {
            Scout scout = new()
            {
                PeselScout = this.PeselScout,
                Name = this.Name,
                SecondName = this.SecondName,
                Surname = this.Surname,
                DateOfBirth = this.DateOfBirth,
                Nationality = this.Nationality,
                MembershipNumber = this.MembershipNumber,
                DateOfEntry = this.DateOfEntry,
                Ns = this.Ns,
                DateOfLeaving = this.DateOfLeaving,
                ParentParentPesel = this.ParentParentPesel,
                IdentityId = this.IdentityId
            };

            return scout;
        }

        public List<ScoutAchievement> GetScoutAchievements()
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
