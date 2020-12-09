using CaseHandler.WebApplication.Data;
using CaseHandler.WebApplication.Data.Models;
using System;

namespace CaseHandler.WebApplication.Services
{
    public class HistoryService : IHistoryService
    {
        private readonly ApplicationDbContext _context;

        public HistoryService(ApplicationDbContext context)
        {
            _context = context;
        }

        public void CreateHistoryForBlockedCase(Case blockedcase, string createdBy)
        {
            _context.Histories.Add(CreateHistoryModel(blockedcase.Id, createdBy, "Az ügy zárolásra került."));
            _context.SaveChanges();
        }

        public void CreateHistoryForCreatedComment(Comment comment)
        {
            _context.Histories.Add(CreateHistoryModel(comment.CaseId, comment.CommentedById, $"Új megjegyzés: '{comment.Content}'"));
            _context.SaveChanges();
        }

        public void CreateHistoryForCase(Case caseModel, string createdBy, string entry)
        {
            _context.Histories.Add(CreateHistoryModel(caseModel.Id, createdBy, entry));
            _context.SaveChanges();
        }

        public void CreateHistoryForNewCase(Case newCase, string createdBy)
        {
            _context.Histories.Add(CreateHistoryModel(newCase.Id, createdBy, "Az ügy létre lett hozva."));
            _context.SaveChanges();
        }

        private History CreateHistoryModel(int caseId, string createdBy, string entry)
        {
            return new History
            {
                CaseId = caseId,
                CreatedAt = DateTime.Now,
                CreatedById = createdBy,
                Entry = entry
            };
        }

        public void AddHistory(History history)
        {
            _context.Histories.Add(history);
            _context.SaveChanges();
        }
    }
}
