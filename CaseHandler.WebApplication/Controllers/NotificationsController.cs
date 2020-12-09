using CaseHandler.WebApplication.Data;
using CaseHandler.WebApplication.Data.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace CaseHandler.WebApplication.Controllers
{
    [Authorize]
    public class NotificationsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public NotificationsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {            
            var applicationDbContext = _context.Notifications
                .Include(n => n.Case)
                .Include(n => n.CreatedBy);
            var notifications = await applicationDbContext
                .Where(a => a.RecipientId == _userManager.GetUserId(User))
                .OrderBy(a => a.Notified)
                .ThenByDescending(a => a.CreatedAt)
                .ToListAsync();

            return View(notifications);
        }
                        
        public async Task<IActionResult> Notified(int id, bool redirecToCaseDetails = false)
        {
            try
            {
                if (!NotificationExists(id))
                {
                    return RedirectToAction("ResourceNotFound", "Home");
                }
                var notification = _context.Notifications.Find(id);
                if (notification.RecipientId != _userManager.GetUserId(User))
                {
                    return LocalRedirect("/Identity/Account/AccessDenied");
                }

                notification.Notified = true;
                _context.Update(notification);
                await _context.SaveChangesAsync();
                if (redirecToCaseDetails)
                {
                    return RedirectToAction("Details", "Cases", new { @id = notification.CaseId});
                }
            }
            catch (DbUpdateConcurrencyException e)
            {
                throw e;
            }
            
            return RedirectToAction(nameof(Index));
        }

        private bool NotificationExists(int id)
        {
            return _context.Notifications.Any(e => e.Id == id);
        }
    }
}
