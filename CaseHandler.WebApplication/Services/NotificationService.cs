using CaseHandler.WebApplication.Data;
using CaseHandler.WebApplication.Data.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace CaseHandler.WebApplication.Services
{
    public class NotificationService : INotificationService
    {
        private readonly IEmailSender _emailSender;
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public NotificationService(IEmailSender emailSender,
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager)
        {
            _emailSender = emailSender;
            _context = context;
            _userManager = userManager;
        }
        public void CreateAndSendNotification(Notification newNotification)
        {
            newNotification = CreateNotification(newNotification);
            SendNotificationToRecipient(newNotification);
        }

        public int GetUnNotifiedNotificationCount(string userId)
        {
            return _context.Notifications.Where(n => n.RecipientId == userId && !n.Notified).Count();
        }

        public void SendNotificationAboutCreatedCase(string caseNumber, string recipientId)
        {
            var recipient = _userManager.FindByIdAsync(recipientId).Result;
            _emailSender.SendEmailAsync(recipient.Email,
                "Ügyintéző alkalmazás - Az imént egy új ügyet hozott létre",
                $"Ön az imént egy új ügyet hozott létre, melynek azonósítója: {caseNumber}.<br/>" +
                $"Az ügyével kapcsolatban minden információt megtalál a weboldalunkon.");
        }

        public void SendNotificationAboutDeletedCase(string caseNumber, string recipientId, string deletedById)
        {
            var recipient = _userManager.FindByIdAsync(recipientId).Result;
            var deletedBy = _userManager.FindByIdAsync(deletedById).Result;

            _emailSender.SendEmailAsync(recipient.Email,
                "Ügyintéző alkalmazás - Az ön egyik ügye véglegesen törölve lett.",
                $"Értesítjük, hogy az alábbi ügye törölve lett: {caseNumber}.<br/><br/>" +
                $"Az értesítés feladója: {deletedBy.UserName}");
        }

        private Notification CreateNotification(Notification newNotification)
        {
            _context.Notifications.Add(newNotification);
            _context.SaveChanges();

            return _context.Notifications
                .Include(n => n.CreatedBy)
                .Include(n => n.Recipient)
                .Single(n => n.Id == newNotification.Id);
        }

        private void SendNotificationToRecipient(Notification newNotification)
        {
            var recipientEmailAddress = _userManager.GetEmailAsync(newNotification.Recipient).Result;

            _emailSender.SendEmailAsync(recipientEmailAddress,
                "Ügyintéző alkalmazás - Új értesítése érkezett.",
                $"Új értesítés érkezett az ön számára az alábbi üggyel kapcsolatban: {newNotification.Case.Summary}.<br/><br/>" +
                $"Az értesítés feladója: {newNotification.CreatedBy.UserName}<br/>" +
                $"Az értesítés tartalma: {newNotification.Entry}");
        }
    }
}
