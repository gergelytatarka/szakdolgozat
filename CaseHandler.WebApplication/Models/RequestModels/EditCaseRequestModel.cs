using CaseHandler.WebApplication.Data.Models;
using System;
using System.ComponentModel.DataAnnotations;

namespace CaseHandler.WebApplication.Models.RequestModels
{
    public class EditCaseRequestModel
    {
        public string Number { get => $"ÜGY{DateTime.Now.Year - 2000}{GetValueOfCaseType()}{Id.ToString("D4")}"; }
        public int Id { get; set; }
        [Display(Name = "Összegzés")]
        [Required(ErrorMessage = "Az {0} megadása kötelező")]
        public string Summary { get; set; }
        [Display(Name = "Részletezés")]
        [Required(ErrorMessage = "Az {0} megadása kötelező")]
        public string Detail { get; set; }
        public string AssigneeId { get; set; }
        public string ReportedById { get; set; }
        [Display(Name = "Típus")]
        [Required(ErrorMessage = "Az {0} megadása kötelező")]
        public Data.Enums.Type Type { get; set; }
        [Display(Name = "Státusz")]
        [Required(ErrorMessage = "Az {0} megadása kötelező")]
        public Data.Enums.State State { get; set; }
        [Display(Name = "Prioritás")]
        [Required(ErrorMessage = "Az {0} megadása kötelező")]
        public Data.Enums.Priority Priority { get; set; }
        [Display(Name = "Megoldás")]
        public string Resolution { get; set; }
        [Display(Name = "Határidő")]
        public DateTime? Deadline { get; set; }
        [Display(Name = "Felelős")]
        public virtual ApplicationUser Assignee { get; set; }

        private int GetValueOfCaseType()
        {
            return (int)Type;
        }
    }
}
