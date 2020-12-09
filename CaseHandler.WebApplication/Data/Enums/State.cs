using System.ComponentModel.DataAnnotations;

namespace CaseHandler.WebApplication.Data.Enums
{
    public enum State
    {
        [Display(Name = "Nyitott")]
        Open = 1,
        [Display(Name = "Feldolgozás alatt")]
        InProgress = 2,
        [Display(Name = "Válaszra vár")]
        WaitingForResponse = 3,
        [Display(Name = "Megoldva")]
        Resolved = 4,
        [Display(Name = "Lezárva")]
        Closed = 5
    }
}
