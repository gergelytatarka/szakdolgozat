using AutoMapper;
using CaseHandler.WebApplication.Data;
using CaseHandler.WebApplication.Data.Models;
using CaseHandler.WebApplication.Models.RequestModels;
using CaseHandler.WebApplication.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace CaseHandler.WebApplication.Controllers
{
    [Authorize]
    public class CommentsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly INotificationService _notificationService;
        private readonly IHistoryService _historyService;
        private readonly UserManager<ApplicationUser> _userManager;

        public CommentsController(ApplicationDbContext context, 
            UserManager<ApplicationUser> userManager, 
            IMapper mapper, 
            INotificationService notificationService,
            IHistoryService historyService)
        {
            _context = context;
            _mapper = mapper;
            _notificationService = notificationService;
            _historyService = historyService;
            _userManager = userManager;
        }

        // GET: Comments/Create
        public IActionResult Create(int caseId)
        {
            if (CaseIsBlocked(caseId))
            {
                return Redirect("/Identity/Account/AccessDenied");
            }

            return View(new CreateCommentRequestModel { CaseId = caseId });
        }

        // POST: Comments/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateCommentRequestModel newComment)
        {
            if (CaseIsBlocked(newComment.CaseId))
            {
                return Redirect("/Identity/Account/AccessDenied");
            }
            var comment = new Comment
            {
                CaseId = newComment.CaseId,
                Content = newComment.Content,
                CommentedById = _userManager.GetUserId(User),
                CommentedAt = DateTime.Now
            };
            if (ModelState.IsValid)
            {
                _context.Add(comment);
                await _context.SaveChangesAsync();
                var createdComment = _context.Comments
                    .Include(c => c.Case)
                    .Single(c => c.Id == comment.Id);
                SendNotificationAboutNewComment(createdComment);
                _historyService.CreateHistoryForCreatedComment(createdComment);

                return RedirectToAction("Details", "Cases", new { @id = createdComment.CaseId });
            }

            return View(newComment);
        }

        private void SendNotificationAboutNewComment(Comment comment)
        {
            var currentUserId = _userManager.GetUserId(User);
            var notification = new Notification
            {
                CaseId = comment.CaseId,
                CreatedAt = DateTime.Now,
                CreatedById = currentUserId,
                Entry = $"Az ügyhöz új megjegyzés: '{comment.Content}'",
            };

            if (currentUserId != comment.Case.ReportedById)
            {
                notification.RecipientId = comment.Case.ReportedById;
                _notificationService.CreateAndSendNotification(notification);
            }
            if (!string.IsNullOrWhiteSpace(comment.Case.AssigneeId) & currentUserId != comment.Case.AssigneeId)
            {
                notification.RecipientId = comment.Case.AssigneeId;
                _notificationService.CreateAndSendNotification(notification);
            }
        }

        // GET: Comments/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("ResourceNotFound", "Home");
            }
            if (!UserIsAdmin())
            {
                return Redirect("/Identity/Account/AccessDenied");
            };
            var comment = await _context.Comments.FindAsync(id);
            if (comment == null)
            {
                return RedirectToAction("ResourceNotFound", "Home");
            }

            var editCommentRequestModel = _mapper.Map<EditCommentRequestModel>(comment);

            return View(editCommentRequestModel);
        }

        // POST: Comments/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, EditCommentRequestModel editCommentRequestModel)
        {
            if (id != editCommentRequestModel.Id)
            {
                return RedirectToAction("ResourceNotFound", "Home");
            }
            if (!UserIsAdmin())
            {
                return Redirect("/Identity/Account/AccessDenied");
            };
            if (ModelState.IsValid)
            {
                try
                {
                    var comment = _context.Comments
                        .Include(c => c.Case)
                        .Single(c => c.Id == editCommentRequestModel.Id);
                    var originalCommentContent = comment.Content;
                    comment.Content = editCommentRequestModel.Content;

                    _context.Update(comment);
                    await _context.SaveChangesAsync();
                    SendNotificationToOriginalCommenter(originalCommentContent, comment);
                    _historyService.CreateHistoryForCase(comment.Case, comment.CommentedById, "Megjegyzés szerkesztése");

                    return RedirectToAction("Details", "Cases", new { @id = comment.CaseId });
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CommentExists(editCommentRequestModel.Id))
                    {
                        return RedirectToAction("ResourceNotFound", "Home");
                    }
                    else
                    {
                        throw;
                    }
                }
            }

            return View(editCommentRequestModel);
        }

        private void SendNotificationToOriginalCommenter(string originalCommentContent, Comment comment)
        {
            var currentUserId = _userManager.GetUserId(User);

            if (currentUserId != comment.CommentedById)
            {
                _notificationService.CreateAndSendNotification(new Notification
                {
                    CaseId = comment.CaseId,
                    CreatedAt = DateTime.Now,
                    CreatedById = currentUserId,
                    Entry = $"A megjegyzését szerkesztették. Az eredeti megjegyzés: '{originalCommentContent}' A módosított megjegyzés: '{comment.Content}'.",
                    RecipientId = comment.CommentedById
                });
            }
        }

        // GET: Comments/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("ResourceNotFound", "Home");
            }
            if (!UserIsAdmin())
            {
                return Redirect("/Identity/Account/AccessDenied");
            };
            var comment = await _context.Comments
                .Include(c => c.Case)
                .Include(c => c.CommentedBy)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (comment == null)
            {
                return RedirectToAction("ResourceNotFound", "Home");
            }

            return View(comment);
        }

        // POST: Comments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (!UserIsAdmin())
            {
                return Redirect("/Identity/Account/AccessDenied");
            };
            var comment = _context.Comments
                .Include(c => c.Case)
                .Single(c => c.Id == id);

            _context.Comments.Remove(comment);
            await _context.SaveChangesAsync();
            SendNotificationAboutDeletedComment(comment);
            _historyService.CreateHistoryForCase(comment.Case, _userManager.GetUserId(User), "Megjegyzés törlése");

            return RedirectToAction("Details", "Cases", new { @id = comment.CaseId });
        }

        private void SendNotificationAboutDeletedComment(Comment comment)
        {
            _notificationService.CreateAndSendNotification(new Notification
            {
                CaseId = comment.CaseId,
                CreatedAt = DateTime.Now,
                CreatedById = _userManager.GetUserId(User),
                Entry = $"Az alábbi megjegyzése törölve lett: '{comment.Content}'",
                RecipientId = comment.CommentedById
            });
        }
        private bool UserIsAdmin()
        {
            var currentUserId = _userManager.GetUserId(User);

            return _userManager.FindByIdAsync(currentUserId).Result.IsAdmin;
        }

        private bool CaseIsBlocked(int caseId)
        {            
            return _context.Cases.Find(caseId).Blocked;
        }
        private bool CommentExists(int id)
        {
            return _context.Comments.Any(e => e.Id == id);
        }
    }
}
