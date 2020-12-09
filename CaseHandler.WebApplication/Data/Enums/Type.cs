using System.ComponentModel.DataAnnotations;

namespace CaseHandler.WebApplication.Data.Enums
{
    public enum Type
    {
        [Display(Name ="Kérelem")]
        Request = 1,
        [Display(Name = "Hibajelenség")]
        Failure =2
    }
}
