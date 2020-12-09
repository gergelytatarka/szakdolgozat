using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;

namespace CaseHandler.WebApplication.Data.Models
{
    public class ApplicationUser : IdentityUser
    {
        public bool IsAdmin { get; set; }

        public virtual ICollection<Case> AssignedCases { get; set; }
        public virtual ICollection<Case> ReportedCases { get; set; }
        public virtual ICollection<Comment> Comments { get; set; }
        public virtual ICollection<History> HistoryItems { get; set; }
        public virtual ICollection<Notification> CreatedNotifications { get; set; }
        public virtual ICollection<Notification> ReceiptNotifications { get; set; }
    }
}
