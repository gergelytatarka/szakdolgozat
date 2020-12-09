using CaseHandler.WebApplication.Data;
using CaseHandler.WebApplication.Data.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CaseHandler.WebApplication.Areas.Identity.Pages.Account.Manage
{
    public class DeletePersonalDataModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public DeletePersonalDataModel(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager)
        {
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            [Display(Name = "Jelszó")]
            [Required(ErrorMessage = "A {0} megadása kötelező")]
            [DataType(DataType.Password)]
            public string Password { get; set; }
        }

        public bool RequirePassword { get; set; }

        public async Task<IActionResult> OnGet()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"A megadott azonosítójú felhasználó nem található: '{_userManager.GetUserId(User)}'.");
            }

            RequirePassword = await _userManager.HasPasswordAsync(user);
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"A megadott azonosítójú felhasználó nem található: '{_userManager.GetUserId(User)}'.");
            }

            RequirePassword = await _userManager.HasPasswordAsync(user);
            if (RequirePassword)
            {
                if (!await _userManager.CheckPasswordAsync(user, Input.Password))
                {
                    ModelState.AddModelError(string.Empty, "Helytelen jelszó.");
                    return Page();
                }
            }

            RemoveAllUserRelatedEntity(user.Id);

            var result = await _userManager.DeleteAsync(user);
            var userId = await _userManager.GetUserIdAsync(user);
            if (!result.Succeeded)
            {
                throw new InvalidOperationException($"Váratlan hiba történt a felhasználói fiók törlése során: '{userId}'.");
            }

            await _signInManager.SignOutAsync();

            return Redirect("~/");
        }

        private void RemoveAllUserRelatedEntity(string userId)
        {
            var casesOfUser = _context.Cases
                .Where(c => c.ReportedById == userId || c.AssigneeId == userId).ToList();
            var caseIds = casesOfUser.Select(c => c.Id).ToList();

            var notificationsOfUser = _context.Notifications
                .Where(n => caseIds.Contains(n.CaseId));
            var historyItemsOfUser = _context.Histories
                .Where(n => caseIds.Contains(n.CaseId)).ToList();
            var commentsOfUser = _context.Comments
                .Where(n => caseIds.Contains(n.CaseId)).ToList();

            casesOfUser.ToList().ForEach(c => c.Assignee = null);
            _context.Notifications.RemoveRange(notificationsOfUser);
            _context.Histories.RemoveRange(historyItemsOfUser);
            _context.Comments.RemoveRange(commentsOfUser);
            _context.Cases.RemoveRange(casesOfUser);

            _context.SaveChanges();
        }
    }
}
