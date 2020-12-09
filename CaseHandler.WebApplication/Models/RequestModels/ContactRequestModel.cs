using System.ComponentModel.DataAnnotations;

namespace CaseHandler.WebApplication.Models.RequestModels
{
    public class ContactRequestModel
    {
        [Display(Name = "Tárgy")]
        [Required(ErrorMessage = "A {0} megadása kötelező")]
        public string Subject { get; set; }
        [Display(Name = "Kérdés")]
        [Required(ErrorMessage = "A {0} megadása kötelező")]
        public string Question { get; set; }
        [Display(Name = "Email cím")]
        [Required(ErrorMessage = "A {0} megadása kötelező")]
        [EmailAddress(ErrorMessage ="Az {0} formátuma nem megfelelő")]
        public string Email { get; set; }
    }
}
