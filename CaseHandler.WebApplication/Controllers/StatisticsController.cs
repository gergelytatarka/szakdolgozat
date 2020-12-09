using CaseHandler.WebApplication.Data;
using CaseHandler.WebApplication.Data.Enums;
using CaseHandler.WebApplication.Data.Models;
using CaseHandler.WebApplication.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CaseHandler.WebApplication.Controllers
{
    [Authorize]
    public class StatisticsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public StatisticsController(ApplicationDbContext applicationDbContext, UserManager<ApplicationUser> userManager)
        {
            _context = applicationDbContext;
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            if (!_userManager.GetUserAsync(User).Result.IsAdmin)
            {
                return Redirect("/Identity/Account/AccessDenied");
            }

            var cases = _context.Cases.Include(c => c.Assignee);

            return View(new CaseStatisticsViewModel
            {
                AllFailureCount = cases.Where(c => c.Type == Data.Enums.Type.Failure).Count(),
                AllRequestCount = cases.Where(c => c.Type == Data.Enums.Type.Request).Count(),
                AllBlockedFailureCount = cases.Where(c => c.Type == Data.Enums.Type.Failure && c.Blocked).Count(),
                AllBlockedRequestCount = cases.Where(c => c.Type == Data.Enums.Type.Request && c.Blocked).Count(),
                AllFinishedFailureCount = cases.Where(c => !c.Blocked && c.Type == Data.Enums.Type.Failure && (c.State == State.Closed || c.State == State.Resolved)).Count(),
                AllFinishedRequestCount = cases.Where(c => !c.Blocked && c.Type == Data.Enums.Type.Request && (c.State == State.Closed || c.State == State.Resolved)).Count(),
                AllOpenFailureCount = cases.Where(c => !c.Blocked && c.Type == Data.Enums.Type.Failure && (c.State == State.Open || c.State == State.Resolved)).Count(),
                AllOpenRequestCount = cases.Where(c => !c.Blocked && c.Type == Data.Enums.Type.Request && (c.State == State.Open || c.State == State.Resolved)).Count(),
                AllPendingFailureCount = cases.Where(c => !c.Blocked && c.Type == Data.Enums.Type.Failure && (c.State == State.InProgress || c.State == State.WaitingForResponse)).Count(),
                AllPendingRequestCount = cases.Where(c => !c.Blocked && c.Type == Data.Enums.Type.Request && (c.State == State.InProgress || c.State == State.WaitingForResponse)).Count(),
                MonthlyFailuresThisYear = GetMonthlyCaseCount(cases, DateTime.Now.Year, Data.Enums.Type.Failure),
                MonthlyRequestThisYear = GetMonthlyCaseCount(cases, DateTime.Now.Year, Data.Enums.Type.Request),
                PendingCasesByAssignee = GetPendingCaseCountByAssignee(cases)
            });
        }

        private Dictionary<string, int> GetPendingCaseCountByAssignee(IEnumerable<Case> cases)
        {
            var result = cases
                .Where(c => !c.Blocked && c.Assignee != null && (c.State != State.Resolved || c.State != State.Closed))
                .OrderBy(c => c.Assignee.UserName)
                .GroupBy(c => c.Assignee.UserName).ToDictionary(g => g.Key, g => g.Count());

            return result;
        }

        private Dictionary<string, int> GetMonthlyCaseCount(IEnumerable<Case> cases, int year, Data.Enums.Type type)
        {
            var hungarianMonthNames = new List<string> { "Január", "Február", "Március", "Április", "Május", "Június", "Július", "Augusztus", "Szeptember", "Október", "November", "December" };
            var result = new Dictionary<string, int>();
            var monthIndex = 1;

            foreach (var month in hungarianMonthNames)
            {
                var casesInMonth = cases
                    .Where(c => c.ReportedAt.Month == monthIndex && c.ReportedAt.Year == year && c.Type == type)
                    .Count();
                result.Add(month, casesInMonth);
                monthIndex++;
            }

            return result;
        }
    }
}
