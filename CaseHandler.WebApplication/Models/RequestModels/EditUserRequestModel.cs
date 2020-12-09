using System.ComponentModel.DataAnnotations;

namespace CaseHandler.WebApplication.Models.RequestModels
{
    public class EditUserRequestModel
    {
        [Display(Name = "Azonosító")]
        public string Id { get; set; }
        [Display(Name = "Felhasználónév")]
        public string UserName { get; set; }
        [Display(Name = "Zárolás")]
        public bool IsLockedOut { get; set; }
        [Display(Name = "Adminisztrátor")]
        public bool IsAdmin { get; set; }
    }
}
