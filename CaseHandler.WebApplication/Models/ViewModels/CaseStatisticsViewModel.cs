using System.Collections.Generic;

namespace CaseHandler.WebApplication.Models.ViewModels
{
    public class CaseStatisticsViewModel
    {
        public int AllFailureCount { get; set; }
        public int AllRequestCount { get; set; }
        public int AllOpenFailureCount { get; set; }
        public int AllOpenRequestCount { get; set; }
        public int AllPendingFailureCount { get; set; }
        public int AllPendingRequestCount { get; set; }
        public int AllFinishedFailureCount { get; set; }
        public int AllFinishedRequestCount { get; set; }
        public int AllBlockedFailureCount { get; set; }
        public int AllBlockedRequestCount { get; set; }
        public Dictionary<string, int> MonthlyFailuresThisYear { get; set; }
        public Dictionary<string, int> MonthlyRequestThisYear { get; set; }
        public Dictionary<string, int> PendingCasesByAssignee { get; set; }
    }
}
