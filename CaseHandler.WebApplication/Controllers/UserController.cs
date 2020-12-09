using AutoMapper;
using CaseHandler.WebApplication.Data.Models;
using CaseHandler.WebApplication.Models.RequestModels;
using CaseHandler.WebApplication.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CaseHandler.WebApplication.Controllers
{
    [Authorize]
    public class UserController : Controller
    {
        private readonly IMapper _mapper;
        private readonly UserManager<ApplicationUser> _userManager;

        public UserController(IMapper mapper, UserManager<ApplicationUser> userManager)
        {
            _mapper = mapper;
            _userManager = userManager;
        }

        public IActionResult ListUsers()
        {
            if (!UserIsAdmin())
            {
                return Redirect("/Identity/Account/AccessDenied");
            };
            var users = _userManager.Users
                .Include(u => u.ReportedCases)
                .Include(u => u.Comments)
                .ToList();

            var viewModel = _mapper.Map<List<ListUsersViewModel>>(users);

            return View(viewModel);
        }

        public async Task<IActionResult> Edit(string id)
        {
            if (!UserIsAdmin() || id == _userManager.GetUserId(User))
            {
                return Redirect("/Identity/Account/AccessDenied");
            };
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return RedirectToAction("ResourceNotFound", "Home");
            }

            var userViewModel = _mapper.Map<EditUserRequestModel>(user);

            return View(userViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, EditUserRequestModel user)
        {
            if (!UserIsAdmin() || user.Id == _userManager.GetUserId(User))
            {
                return Redirect("/Identity/Account/AccessDenied");
            };
            if (id != user.Id)
            {
                return RedirectToAction("ResourceNotFound", "Home");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var applicationUser = await _userManager.FindByIdAsync(id);                    
                    applicationUser.IsAdmin = user.IsAdmin;
                    applicationUser.UserName = user.UserName;

                    if(user.IsLockedOut)
                    {
                        applicationUser.LockoutEnd = DateTime.MaxValue;
                    }
                    else
                    {
                        applicationUser.LockoutEnd = null;
                    }
                    await _userManager.UpdateAsync(applicationUser);
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    return View(ex);
                }
                return RedirectToAction(nameof(ListUsers));
            }
            return View(user);
        }
        private bool UserIsAdmin()
        {
            var currentUserId = _userManager.GetUserId(User);

            return _userManager.FindByIdAsync(currentUserId).Result.IsAdmin;
        }
    }
}