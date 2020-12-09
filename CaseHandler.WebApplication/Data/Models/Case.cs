using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CaseHandler.WebApplication.Data.Models
{
    public class Case
    {
        [NotMapped]
        public string Number { get => $"ÜGY{DateTime.Now.Year - 2000}{GetValueOfCaseType()}{Id.ToString("D4")}"; }
        [Key]
        public int Id { get; set; }
        [Display(Name = "Összegzés")]
        [Required(ErrorMessage = "Az {0} megadása kötelező")]
        public string Summary { get; set; }
        [Display(Name = "Részletezés")]
        [Required(ErrorMessage = "Az {0} megadása kötelező")]
        public string Detail { get; set; }
        public string AssigneeId { get; set; }
        [Display(Name = "Típus")]
        [Required]
        public Enums.Type Type { get; set; }
        [Display(Name = "Státusz")]
        [Required]
        public Enums.State State { get; set; }
        [Display(Name = "Prioritás")]
        [Required]
        public Enums.Priority Priority { get; set; }
        [Required]
        public string ReportedById { get; set; }
        [Display(Name = "Határidő")]
        [Column(TypeName = "datetime2")]
        public DateTime? Deadline { get; set; }
        [Display(Name = "Bejelentés ideje")]
        [Column(TypeName = "datetime2")]
        public DateTime ReportedAt { get; set; }
        [Display(Name = "Zárolva")]
        public bool Blocked { get; set; }
        [Display(Name = "Megoldás")]
        public string Resolution { get; set; }

        [Display(Name = "Felelős")]
        public virtual ApplicationUser Assignee { get; set; }
        [Display(Name = "Bejelentő")]
        public virtual ApplicationUser ReportedBy { get; set; }
        public virtual ICollection<Comment> Comments { get; set; }
        public virtual ICollection<History> HistoryItems { get; set; }
        public virtual ICollection<Notification> Notifications { get; set; }

        private int GetValueOfCaseType()
        {
            return (int)Type;
        }
    }
}
