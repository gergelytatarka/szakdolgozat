using AutoMapper;
using CaseHandler.WebApplication.Data;
using CaseHandler.WebApplication.Data.Enums;
using CaseHandler.WebApplication.Data.Models;
using CaseHandler.WebApplication.Models.RequestModels;
using CaseHandler.WebApplication.Models.ViewModels;
using CaseHandler.WebApplication.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CaseHandler.WebApplication.Controllers
{
    [Authorize]
    public class CasesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IMapper _mapper;
        private readonly INotificationService _notificationService;
        private readonly IHistoryService _historyService;

        public CasesController(ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            IMapper mapper,
            INotificationService notificationService,
            IHistoryService historyService)
        {
            _context = context;
            _userManager = userManager;
            _mapper = mapper;
            _notificationService = notificationService;
            _historyService = historyService;
        }

        // GET: Cases
        public IActionResult Index(string filter, string sortOrder)
        {
            if (!UserIdAdmin())
            {
                return Redirect($"https://{Request.Host}/Identity/Account/AccessDenied");
            };

            var allCases = ListAllCases(sortOrder);            
            SetSortParemeters(sortOrder);
            ApplyFilter(filter, ref allCases);

            return View(GetCaseViewModel(allCases.ToList(), nameof(Index), filter));
        }

        private void ApplyFilter(string filter, ref IEnumerable<Case> allCases)
        {
            if (!string.IsNullOrWhiteSpace(filter))
            {
                allCases = allCases
                    .Where(
                    c => c.Summary.Contains(filter, StringComparison.OrdinalIgnoreCase) ||
                    c.Detail.Contains(filter, StringComparison.OrdinalIgnoreCase) ||
                    c.Number.Contains(filter, StringComparison.OrdinalIgnoreCase) ||
                    c.ReportedBy.UserName.Contains(filter, StringComparison.OrdinalIgnoreCase) ||
                    (c.AssigneeId != null && c.Assignee.UserName.Contains(filter, StringComparison.OrdinalIgnoreCase)) ||
                    (!string.IsNullOrWhiteSpace(c.Resolution) && c.Resolution.Contains(filter, StringComparison.OrdinalIgnoreCase)));
            }
        }

        public IActionResult Own(string filter, string sortOrder)
        {
            var myCases = ListAllCases(sortOrder)
                .Where(c => c.ReportedById == _userManager.GetUserId(User));
            ApplyFilter(filter, ref myCases);
            SetSortParemeters(sortOrder);

            return View(nameof(Index), GetCaseViewModel(myCases.ToList(), nameof(Own), filter));
        }

        private void SetSortParemeters(string sortOrder)
        {
            ViewData["SortByNumber"] = sortOrder == nameof(SortBy.Number) ? nameof(SortBy.NumberDesc) : nameof(SortBy.Number);
            ViewData["SortByDeadline"] = sortOrder == nameof(SortBy.Deadline) ? nameof(SortBy.DeadlineDesc) : nameof(SortBy.Deadline);
            ViewData["SortBySummary"] = sortOrder == nameof(SortBy.Summary) ? nameof(SortBy.SummaryDesc) : nameof(SortBy.Summary);
            ViewData["SortByType"] = sortOrder == nameof(SortBy.Type) ? nameof(SortBy.TypeDesc) : nameof(SortBy.Type);
            ViewData["SortByState"] = sortOrder == nameof(SortBy.State) ? nameof(SortBy.StateDesc) : nameof(SortBy.State);
            ViewData["SortByPriority"] = sortOrder == nameof(SortBy.Priority) ? nameof(SortBy.PriorityDesc) : nameof(SortBy.Priority);
            ViewData["SortByAssignee"] = sortOrder == nameof(SortBy.Assignee) ? nameof(SortBy.AssigneeDesc) : nameof(SortBy.Assignee);
            ViewData["SortByReportedBy"] = sortOrder == nameof(SortBy.ReportedBy) ? nameof(SortBy.ReportedByDesc) : nameof(SortBy.ReportedBy);
            ViewData["SortByReportedAt"] = sortOrder == nameof(SortBy.ReportedAt) ? nameof(SortBy.ReportedAtDesc) : nameof(SortBy.ReportedAt);
        }

        // GET: Cases/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            var contextCase = await _context.Cases
                .Include(c => c.Assignee)
                .Include(c => c.ReportedBy)
                .Include(c => c.Comments)
                .SingleOrDefaultAsync(c => c.Id == id);
            if (contextCase == null)
            {
                return RedirectToAction("ResourceNotFound", "Home");
            }
            if (!UserIdAdmin() && contextCase.ReportedById != _userManager.GetUserId(User))
            {
                return Own(string.Empty, string.Empty);
            }

            var caseDetails = _mapper.Map<CaseDetailsViewModel>(contextCase);
            caseDetails.Comments = caseDetails.Comments
                .OrderByDescending(c => c.CommentedAt)
                .ToList();
            caseDetails.Nofications = _context.Notifications
                .Where(n => n.RecipientId == _userManager.GetUserId(User) && n.CaseId == id)
                .OrderBy(n => n.Notified)
                .ThenByDescending(n => n.CreatedAt)
                .ToList();

            return View(caseDetails);
        }

        // GET: Cases/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Cases/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateCaseRequestModel caseRequestModel)
        {
            if (ModelState.IsValid)
            {
                var newCase = _mapper.Map<Case>(caseRequestModel);

                newCase.ReportedAt = DateTime.Now;
                newCase.State = Data.Enums.State.Open;
                newCase.ReportedById = _userManager.GetUserId(User);

                _context.Add(newCase);
                await _context.SaveChangesAsync();
                _historyService.CreateHistoryForNewCase(newCase, newCase.ReportedById);
                _notificationService.SendNotificationAboutCreatedCase(newCase.Number, newCase.ReportedById);

                return RedirectToAction(nameof(Own));
            }

            return View(caseRequestModel);
        }

        // GET: Cases/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("ResourceNotFound", "Home");
            }
            if (!UserIdAdmin() || CaseIsBlocked((int)id))
            {
                return Redirect($"https://{Request.Host}/Identity/Account/AccessDenied");
            };
            var editedCase = await _context.Cases.FindAsync(id);
            if (editedCase == null)
            {
                return RedirectToAction("ResourceNotFound", "Home");
            }
            var admins = _context.Users.Where(u => u.IsAdmin);
            ViewData["AssigneeId"] = new SelectList(admins, "Id", "UserName", editedCase.AssigneeId);
            ViewData["ReportedById"] = new SelectList(admins, "Id", "UserName", editedCase.ReportedById);

            return View(_mapper.Map<EditCaseRequestModel>(editedCase));
        }

        // POST: Cases/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, EditCaseRequestModel editCaseRequestModel)
        {
            if (id != editCaseRequestModel.Id)
            {
                return RedirectToAction("ResourceNotFound", "Home");
            }
            if (!UserIdAdmin() || CaseIsBlocked(id))
            {
                return Redirect($"https://{Request.Host}/Identity/Account/AccessDenied");
            };
            if (ModelState.IsValid)
            {
                try
                {
                    var editedCase = _mapper.Map<Case>(editCaseRequestModel);
                    _context.Entry(editedCase).State = EntityState.Modified;
                    _context.Update(editedCase);
                    await _context.SaveChangesAsync();
                    _historyService.AddHistory(new History
                    {
                        CaseId = editedCase.Id,
                        CreatedAt = DateTime.Now,
                        CreatedById = _userManager.GetUserId(User),
                        Entry = "Az ügy részletei szerkesztve lettek."
                    });

                    _notificationService.CreateAndSendNotification(CreateNewNotificationModel(editedCase, editedCase.ReportedById, "Szerkesztési művelet történt"));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CaseExists(editCaseRequestModel.Id))
                    {
                        return RedirectToAction("ResourceNotFound", "Home");
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["AssigneeId"] = new SelectList(_context.Users, "Id", "UserName", editCaseRequestModel.AssigneeId);
            return View(editCaseRequestModel);
        }

        public async Task<IActionResult> Block(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("ResourceNotFound", "Home");
            }
            if (!UserIdAdmin() || CaseIsBlocked((int)id))
            {
                return Redirect($"https://{Request.Host}/Identity/Account/AccessDenied");
            };

            var caseToBlock = await _context.Cases
                .Include(c => c.Assignee)
                .Include(c => c.ReportedBy)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (caseToBlock == null)
            {
                return RedirectToAction("ResourceNotFound", "Home");
            }

            return View(caseToBlock);
        }

        [HttpPost, ActionName("Block")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> BlockConfirmed(int id)
        {
            if (!UserIdAdmin() || CaseIsBlocked(id))
            {
                return Redirect($"https://{Request.Host}/Identity/Account/AccessDenied");
            };
            var caseToBlock = await _context.Cases.FindAsync(id);
            caseToBlock.Blocked = true;
            await _context.SaveChangesAsync();

            SendNotificationAboutModifiedCase(caseToBlock, $"Az alábbi ügy zárolva: {caseToBlock.Summary}. Ez azt jelenti, hogy az ügy a továbbiakban már nem módosítható.");
            _historyService.CreateHistoryForBlockedCase(caseToBlock, _userManager.GetUserId(User));
            return RedirectToAction(nameof(Details), new { @id = id });
        }

        // GET: Cases/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("ResourceNotFound", "Home");
            }
            if (!UserIdAdmin() || CaseIsBlocked((int)id))
            {
                return Redirect($"https://{Request.Host}/Identity/Account/AccessDenied");
            };

            var caseToDelete = await _context.Cases
                .Include(c => c.Assignee)
                .Include(c => c.ReportedBy)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (caseToDelete == null)
            {
                return RedirectToAction("ResourceNotFound", "Home");
            }

            return View(caseToDelete);
        }

        // POST: Cases/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (!UserIdAdmin() || CaseIsBlocked(id))
            {
                return Redirect($"https://{Request.Host}/Identity/Account/AccessDenied");
            };
            var caseToDelete = await _context.Cases
                .Include(c => c.Comments)
                .Include(c => c.Notifications)
                .Include(c => c.HistoryItems)
                .Where(c => c.Id == id)
                .FirstOrDefaultAsync();

            _context.Comments.RemoveRange(caseToDelete.Comments);
            _context.Notifications.RemoveRange(caseToDelete.Notifications);
            _context.Histories.RemoveRange(caseToDelete.HistoryItems);
            _context.Cases.Remove(caseToDelete);
            await _context.SaveChangesAsync();
            _notificationService.SendNotificationAboutDeletedCase(caseToDelete.Number, caseToDelete.ReportedById, _userManager.GetUserId(User));

            return RedirectToAction(nameof(Index));
        }

        private void SendNotificationAboutModifiedCase(Case modifiedCase, string entry)
        {
            _notificationService.CreateAndSendNotification(CreateNewNotificationModel(modifiedCase, modifiedCase.ReportedById, entry));
            if (_userManager.GetUserId(User) != modifiedCase.AssigneeId && modifiedCase.AssigneeId != null)
            {
                _notificationService.CreateAndSendNotification(CreateNewNotificationModel(modifiedCase, modifiedCase.AssigneeId, entry));
            }
        }

        [HttpGet]
        public IActionResult History(int caseId)
        {
            var caseElement = _context.Cases
                .Include(c => c.HistoryItems)
                .ThenInclude(h => h.CreatedBy)
                .Where(c => c.Id == caseId)
                .FirstOrDefault();
            var caseHistoryItems = _mapper.Map<List<HistoryItemViewModel>>(caseElement.HistoryItems);

            return View(new HistoryViewModel { CaseNumber = caseElement.Number, HistoryItems = caseHistoryItems });
        }

        public IActionResult Free(string filter, string sortOrder)
        {
            if (!UserIdAdmin())
            {
                return Redirect($"https://{Request.Host}/Identity/Account/AccessDenied");
            };
            var freeCases = ListAllCases(sortOrder)
                .Where(c => c.AssigneeId == null && (!c.Blocked && (c.State != State.Closed || c.State != State.Resolved)));
            ApplyFilter(filter, ref freeCases);
            SetSortParemeters(sortOrder);

            return View(nameof(Index), GetCaseViewModel(freeCases.ToList(), nameof(Free), filter)); ;
        }

        public IActionResult AssignedToMe(string filter, string sortOrder)
        {
            if (!UserIdAdmin())
            {
                return Redirect($"https://{Request.Host}/Identity/Account/AccessDenied");
            };
            var casesAssignedToMe = ListAllCases(sortOrder)
                .Where(c => c.AssigneeId == _userManager.GetUserId(User) && !c.Blocked && (c.State != State.Closed || c.State != State.Resolved));
            ApplyFilter(filter, ref casesAssignedToMe);
            SetSortParemeters(sortOrder);

            return View(nameof(Index), GetCaseViewModel(casesAssignedToMe.ToList(), nameof(AssignedToMe), filter));
        }

        public IActionResult Pending(string filter, string sortOrder)
        {
            if (!UserIdAdmin())
            {
                return Redirect($"https://{Request.Host}/Identity/Account/AccessDenied");
            };
            var pendingCases = ListAllCases(sortOrder)
                .Where(c => !c.Blocked && (c.State == State.InProgress || c.State == State.WaitingForResponse));
            ApplyFilter(filter, ref pendingCases);
            SetSortParemeters(sortOrder);

            return View(nameof(Index), GetCaseViewModel(pendingCases.ToList(), nameof(Pending), filter));
        }

        public IActionResult Finished(string filter, string sortOrder)
        {
            if (!UserIdAdmin())
            {
                return Redirect($"https://{Request.Host}/Identity/Account/AccessDenied");
            };
            var finishedCases = ListAllCases(sortOrder)
                .Where(c => !c.Blocked && (c.State == State.Closed || c.State == State.Resolved));
            ApplyFilter(filter, ref finishedCases);
            SetSortParemeters(sortOrder);

            return View(nameof(Index), GetCaseViewModel(finishedCases.ToList(), nameof(Finished), filter));
        }

        public IActionResult Blocked(string filter, string sortOrder)
        {
            if (!UserIdAdmin())
            {
                return Redirect($"https://{Request.Host}/Identity/Account/AccessDenied");
            };
            var blockedCases = ListAllCases(sortOrder)
                .Where(c => c.Blocked);
            ApplyFilter(filter, ref blockedCases);
            SetSortParemeters(sortOrder);

            return View(nameof(Index), GetCaseViewModel(blockedCases.ToList(), nameof(Blocked), filter));
        }

        private Notification CreateNewNotificationModel(Case notifiedCase, string recipientId, string entry)
        {
            return new Notification
            {
                CaseId = notifiedCase.Id,
                RecipientId = recipientId,
                Entry = entry,
                CreatedAt = DateTime.Now,
                CreatedById = _userManager.GetUserId(User),
                Notified = false
            };
        }

        private bool CaseExists(int id)
        {
            return _context.Cases.Any(c => c.Id == id);
        }

        private bool UserIdAdmin()
        {
            var currentUserId = _userManager.GetUserId(User);

            return _userManager.FindByIdAsync(currentUserId).Result.IsAdmin;
        }

        private bool CaseIsBlocked(int caseId)
        {
            return _context.Cases
                .AsNoTracking()
                .Where(c => c.Id == caseId)
                .FirstOrDefault()
                .Blocked;
        }

        private IEnumerable<Case> ListAllCases(string sortOrder)
        {
            var cases = _context.Cases
                .Include(c => c.Assignee)
                .Include(c => c.ReportedBy)
                .AsEnumerable();

            switch (sortOrder)
            {
                case nameof(SortBy.Number):
                    cases = cases.OrderBy(c => c.Blocked).ThenBy(c => c.Number);
                    break;
                case nameof(SortBy.NumberDesc):
                    cases = cases.OrderBy(c => c.Blocked).ThenByDescending(c => c.Number);
                    break;
                case nameof(SortBy.Summary):
                    cases = cases.OrderBy(c => c.Blocked).ThenBy(c => c.Summary);
                    break;
                case nameof(SortBy.SummaryDesc):
                    cases = cases.OrderBy(c => c.Blocked).ThenByDescending(c => c.Summary);
                    break;
                case nameof(SortBy.Priority):
                    cases = cases.OrderBy(c => c.Blocked).ThenBy(c => c.Priority);
                    break;
                case nameof(SortBy.PriorityDesc):
                    cases = cases.OrderBy(c => c.Blocked).ThenByDescending(c => c.Priority);
                    break;
                case nameof(SortBy.Assignee):
                    cases = cases.OrderBy(c => c.Blocked).ThenBy(c => c.Assignee?.UserName);
                    break;
                case nameof(SortBy.AssigneeDesc):
                    cases = cases.OrderBy(c => c.Blocked).ThenByDescending(c => c.Assignee?.UserName);
                    break;
                case nameof(SortBy.ReportedAt):
                    cases = cases.OrderBy(c => c.Blocked).ThenBy(c => c.ReportedAt);
                    break;
                case nameof(SortBy.ReportedAtDesc):
                    cases = cases.OrderBy(c => c.Blocked).ThenByDescending(c => c.ReportedAt);
                    break;
                case nameof(SortBy.ReportedBy):
                    cases = cases.OrderBy(c => c.Blocked).ThenBy(c => c.ReportedBy.UserName);
                    break;
                case nameof(SortBy.ReportedByDesc):
                    cases = cases.OrderBy(c => c.Blocked).ThenByDescending(c => c.ReportedBy.UserName);
                    break;
                case nameof(SortBy.State):
                    cases = cases.OrderBy(c => c.Blocked).ThenBy(c => c.State);
                    break;
                case nameof(SortBy.StateDesc):
                    cases = cases.OrderBy(c => c.Blocked).ThenByDescending(c => c.State);
                    break;
                case nameof(SortBy.Type):
                    cases = cases.OrderBy(c => c.Blocked).ThenBy(c => c.Type);
                    break;
                case nameof(SortBy.TypeDesc):
                    cases = cases.OrderBy(c => c.Blocked).ThenByDescending(c => c.Type);
                    break;
                case nameof(SortBy.Deadline):
                    cases = cases.OrderBy(c => c.Blocked).ThenBy(c => c.Deadline == null).ThenBy(c => c.Deadline.GetValueOrDefault());
                    break;
                default:
                    cases = cases.OrderBy(c => c.Blocked).ThenByDescending(c => c.Deadline == null).ThenByDescending(c => c.Deadline.GetValueOrDefault());
                    break;
            }

            return cases;
        }

        private CaseViewModel GetCaseViewModel(List<Case> cases, string returnUrl, string searchFilter)
        {
            var caseItems = _mapper.Map<List<CaseItemViewModel>>(cases);

            return new CaseViewModel
            {
                SearchFilter = searchFilter,
                ReturnUrl = returnUrl,
                CaseItems = caseItems
            };
        }
    }
}
