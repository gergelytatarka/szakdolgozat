﻿using CaseHandler.WebApplication.Data.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
namespace CaseHandler.WebApplication.Areas.Identity.Pages.Account.Manage
{
    public class ChangePasswordModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ILogger<ChangePasswordModel> _logger;

        public ChangePasswordModel(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            ILogger<ChangePasswordModel> logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        [TempData]
        public string StatusMessage { get; set; }

        public class InputModel
        {
            [StringLength(maximumLength: 255, ErrorMessage = "A {0} hosszának {2} és {1} karakter között kell lennie.", MinimumLength = 6)]
            [DataType(DataType.Password, ErrorMessage = "A jelszó formátuma nem megfelelő.")]
            [Display(Name = "Jelenlegi jelszó")]
            public string OldPassword { get; set; }

            [Required(ErrorMessage = "Az {0} megadása kötelező")]
            [StringLength(maximumLength: 255, ErrorMessage = "A {0} hosszának {2} és {1} karakter között kell lennie.", MinimumLength = 6)]
            [DataType(DataType.Password, ErrorMessage = "A jelszó formátuma nem megfelelő.")]
            [Display(Name = "Új jelszó")]
            public string NewPassword { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "Jelszó megerősítése")]
            [Compare("NewPassword", ErrorMessage = "A két megadott jelszó nem egyezik.")]
            public string ConfirmPassword { get; set; }
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"A megadott azonosítójú felhasználó nem található: '{_userManager.GetUserId(User)}'.");
            }

            var hasPassword = await _userManager.HasPasswordAsync(user);
            if (!hasPassword)
            {
                return RedirectToPage("./SetPassword");
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"A megadott azonosítójú felhasználó nem található: '{_userManager.GetUserId(User)}'.");
            }

            var changePasswordResult = await _userManager.ChangePasswordAsync(user, Input.OldPassword, Input.NewPassword);
            if (!changePasswordResult.Succeeded)
            {
                foreach (var error in changePasswordResult.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                return Page();
            }

            await _signInManager.RefreshSignInAsync(user);
            _logger.LogInformation("User changed their password successfully.");
            StatusMessage = "Sikeresen megváltoztatta a jelszavát.";

            return RedirectToPage();
        }
    }
}
