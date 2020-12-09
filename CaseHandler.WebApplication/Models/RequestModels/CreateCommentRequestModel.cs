using System;
using System.ComponentModel.DataAnnotations;

namespace CaseHandler.WebApplication.Models.RequestModels
{
    public class CreateCommentRequestModel
    {
        [Display(Name ="Ügy azonosító")]
        [Range(1,int.MaxValue,ErrorMessage ="Érvénytelen {0}")]
        public int CaseId { get; set; }
        [Display(Name ="Megjegyzés")]
        [Required(ErrorMessage = "A {0} megadása kötelező")]
        public string Content { get; set; }
    }
}
