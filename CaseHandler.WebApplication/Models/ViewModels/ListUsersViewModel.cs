using System.ComponentModel.DataAnnotations;

namespace CaseHandler.WebApplication.Models.ViewModels
{
    public class ListUsersViewModel
    {
        [Display(Name = "Azonosító")]
        public string Id { get; set; }
        [Display(Name = "Felhasználónév")]
        public string UserName { get; set; }
        [Display(Name = "Email cím")]
        public string Email { get; set; }
        [Display(Name = "Email cím megerősítve")]
        public bool EmailConfirmed { get; set; }
        [Display(Name = "Zárolva")]
        public bool IsLockedOut { get; set; }
        [Display(Name = "Adminisztrátor")]
        public bool IsAdmin { get; set; }
        public int CaseCount { get; set; }
        public int CommentCount { get; set; }
    }
}
