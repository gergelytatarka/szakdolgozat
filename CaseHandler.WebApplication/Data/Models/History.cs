using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CaseHandler.WebApplication.Data.Models
{
    public class History
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public int CaseId { get; set; }
        [Required]
        public string Entry { get; set; }
        [Required]
        public string CreatedById { get; set; }
        [Required]
        [Column(TypeName = "datetime2")]
        public DateTime CreatedAt { get; set; }

        public virtual Case Case { get; set; }
        public virtual ApplicationUser CreatedBy { get; set; }
    }
}
