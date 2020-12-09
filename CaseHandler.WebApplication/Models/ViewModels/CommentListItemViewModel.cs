using System.ComponentModel.DataAnnotations;

namespace CaseHandler.WebApplication.Models.ViewModels
{
    public class CommentListItemViewModel
    {
        public int Id { get; set; }
        [Display(Name = "Kommentelő")]
        public string CommentedBy { get; set; }
        [Display(Name = "Időpont")]
        public string CommentedAt { get; set; }
        [Display(Name = "Megjegyzés")]
        public string Content { get; set; }
    }
}
