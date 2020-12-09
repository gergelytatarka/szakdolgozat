using System;
using System.ComponentModel.DataAnnotations;

namespace CaseHandler.WebApplication.Models.RequestModels
{
    public class EditCommentRequestModel
    {
        [Display(Name = "Komment azonosító")]
        [Range(0, int.MaxValue, ErrorMessage = "Érvénytelen {0}")]
        public int Id { get; set; }
        [Display(Name = "Megjegyzés")]
        [Required(ErrorMessage = "A {0} megadása kötelező")]
        public string Content { get; set; }
    }
}
