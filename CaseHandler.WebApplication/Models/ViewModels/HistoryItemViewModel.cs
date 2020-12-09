using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CaseHandler.WebApplication.Models.ViewModels
{
    public class HistoryViewModel
    {
        [Display(Name = "Ügy azonosító")]
        public string CaseNumber { get; set; }
        public List<HistoryItemViewModel> HistoryItems { get; set; }
    }

    public class HistoryItemViewModel
    {
        [Display(Name = "Bejegyzés")]
        public string Entry { get; set; }
        [Display(Name = "Bejegyzés létrehozója")]
        public string CreatedByUserName { get; set; }
        [Display(Name = "Létrehozás dátuma")]
        public DateTime CreatedAt { get; set; }
    }
}
