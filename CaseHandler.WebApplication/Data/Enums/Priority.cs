using System.ComponentModel.DataAnnotations;

namespace CaseHandler.WebApplication.Data.Enums
{   
    public enum Priority
    {
        [Display(Name ="Alacsony")]
        Low = 1,
        [Display(Name = "Közepes")]
        Medium = 2,
        [Display(Name = "Magas")]
        High = 3
    }
}
