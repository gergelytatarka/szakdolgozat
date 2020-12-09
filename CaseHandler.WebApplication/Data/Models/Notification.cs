using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CaseHandler.WebApplication.Data.Models
{
    public class Notification
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public int CaseId { get; set; }
        [Required]
        [Display(Name = "Értesítés")]
        public string Entry { get; set; }
        //[Required]
        public string CreatedById { get; set; }
        [Required]
        public string RecipientId { get; set; }
        [Display(Name = "Létrehozva")]
        [Column(TypeName = "datetime2")]
        public DateTime CreatedAt { get; set; }
        [Display(Name = "Olvasottnak jelölve")]
        public bool Notified { get; set; }

        [Display(Name = "Ügy")]
        public virtual Case Case { get; set; }
        [Display(Name = "Feladó")]
        public virtual ApplicationUser CreatedBy { get; set; }
        [Display(Name = "Címzett")]
        public virtual ApplicationUser Recipient { get; set; }
    }
}
