using CaseHandler.WebApplication.Data.Models;

namespace CaseHandler.WebApplication.Services
{
    public interface INotificationService
    {
        void CreateAndSendNotification(Notification newNotification);
        int GetUnNotifiedNotificationCount(string userId);
        void SendNotificationAboutCreatedCase(string caseNumber, string recipientId);
        void SendNotificationAboutDeletedCase(string caseNumber, string recipientId, string deletedById);
    }
}
