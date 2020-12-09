using CaseHandler.WebApplication.Data.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CaseHandler.WebApplication.Models.ViewModels
{
    public class CaseViewModel
    {
        public string ReturnUrl { get; set; }
        public string SearchFilter { get; set; }
        public List<CaseItemViewModel> CaseItems { get; set; }
    }

    public class CaseItemViewModel
    {
        [Display(Name = "Azonosító")]
        public string Number { get; set; }
        public int Id { get; set; }
        [Display(Name = "Összegzés")]
        public string Summary { get; set; }
        [Display(Name = "Típus")]
        public Data.Enums.Type Type { get; set; }
        [Display(Name = "Státusz")]
        public Data.Enums.State State { get; set; }
        [Display(Name = "Prioritás")]
        public Data.Enums.Priority Priority { get; set; }
        [Display(Name = "Határidő")]        
        public DateTime? Deadline { get; set; }
        [Display(Name = "Bejelentés ideje")]
        public DateTime ReportedAt { get; set; }
        [Display(Name = "Felelős")]
        public virtual ApplicationUser Assignee { get; set; }
        [Display(Name = "Bejelentő")]
        public virtual ApplicationUser ReportedBy { get; set; }
        public bool Blocked { get; set; }
        [Display(Name ="Folyamatban töltött napok")]
        public int OpenDaysCount { get; set; }
        [Display(Name = "Folyamatban töltött napok")]
        public int DaysUntilDeadline { get; set; }
    }
}
