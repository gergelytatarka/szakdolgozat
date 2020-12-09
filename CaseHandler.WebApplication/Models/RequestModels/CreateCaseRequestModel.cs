using System;
using System.ComponentModel.DataAnnotations;

namespace CaseHandler.WebApplication.Models.RequestModels
{
    public class CreateCaseRequestModel
    {        
        [Display(Name ="Típus")]
        [Required(ErrorMessage ="A {0} megadása kötelező")]
        public int Type { get; set; }
        [Display(Name = "Összegzés")]
        [Required(ErrorMessage = "A {0} megadása kötelező")]
        [StringLength(maximumLength: 100, ErrorMessage = "Az {0} hosszának {2} és {1} karakter között kell lennie", MinimumLength = 10)]
        public string Summary { get; set; }
        [Display(Name="Részletezés")]
        [Required(ErrorMessage = "A {0} megadása kötelező")]
        [StringLength(maximumLength: 2000, ErrorMessage = "Az {0} hosszának {2} és {1} karakter között kell lennie", MinimumLength = 20)]
        public string Detail { get; set; }
        [Display(Name = "Prioritás")]
        [Required(ErrorMessage = "A {0} megadása kötelező")]
        public int Priority { get; set; }
        [Display(Name = "Határidő")]
        public DateTime? Deadline { get; set; }
    }
}
