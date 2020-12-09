using CaseHandler.WebApplication.Data;
using CaseHandler.WebApplication.Data.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CaseHandler.WebApplication.Areas.Identity.Pages.Account.Manage
{
    public partial class IndexModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;

        public IndexModel(UserManager<ApplicationUser> userManager, ApplicationDbContext applicationDbContext)
        {
            _userManager = userManager;
            _context = applicationDbContext;
        }

        [Display(Name ="Felhasználónév")]
        public string Username { get; set; }
        public string Email { get; set; }
        [Display(Name ="Megoldandó ügyek száma")]
        public int AssignedCaseCount { get; set; }
        [Display(Name = "Bejelentett ügyek száma")]
        public int ReportedCaseCount { get; set; }
        [Display(Name = "Megjegyzések száma")]
        public int CommentCount { get; set; }

        [TempData]
        public string StatusMessage { get; set; }

        private async Task LoadAsync(ApplicationUser user)
        {
            var userEntity = await _context.Users
                .Include(u => u.AssignedCases)
                .Include(u => u.ReportedCases)
                .Include(u => u.Comments)
                .Where(u => u.Id == user.Id)
                .SingleAsync();

            Username = user.UserName;
            Email = user.Email;
            if (user.AssignedCases.Any())
            {
                AssignedCaseCount = user.AssignedCases.Count();
            }
            if (user.ReportedCases.Any())
            {
                ReportedCaseCount = user.ReportedCases.Count();
            }
            if (user.Comments.Any())
            {
                CommentCount = user.Comments.Count;
            }
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"A megadott azonosítójú felhasználó nem található: '{_userManager.GetUserId(User)}'.");
            }

            await LoadAsync(user);
            return Page();
        }
    }
}
