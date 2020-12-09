using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CaseHandler.WebApplication.Data.Models
{
    public class Comment
    {
        [Key]
        [Display(Name ="Megjegyzés azonosító")]
        public int Id { get; set; }
        [Required]
        public int CaseId { get; set; }
        [Required]
        [Display(Name = "Kommentelő azonosító")]
        public string CommentedById { get; set; }
        [Required]
        [Column(TypeName = "datetime2")]
        public DateTime CommentedAt { get; set; }
        [Required(ErrorMessage = "A {0} megadása kötelező")]
        [Display(Name = "Megjegyzés")]
        public string Content { get; set; }
        [Display(Name = "Ügy")]
        public virtual Case Case { get; set; }
        [Display(Name = "Kommentelő")]
        public virtual ApplicationUser CommentedBy { get; set; }
    }
}
