using CaseHandler.WebApplication.Data.Models;

namespace CaseHandler.WebApplication.Services
{
    public interface IHistoryService
    {
        void CreateHistoryForNewCase(Case newCase, string createdBy);
        void CreateHistoryForCreatedComment(Comment comment);
        void CreateHistoryForCase(Case caseModel, string createdBy, string entry);
        void CreateHistoryForBlockedCase(Case blockedCase, string createdBy);
        void AddHistory(History history);
    }
}
