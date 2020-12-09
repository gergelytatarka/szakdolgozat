using CaseHandler.WebApplication.Configuration;
using CaseHandler.WebApplication.Models;
using CaseHandler.WebApplication.Models.RequestModels;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Threading.Tasks;

namespace CaseHandler.WebApplication.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IEmailSender _emailSender;
        private readonly ApplicationConfiguration _applicationConfiguration;

        public HomeController(ILogger<HomeController> logger,
            IEmailSender emailSender,
            ApplicationConfiguration applicationConfiguration)
        {
            _logger = logger;
            _emailSender = emailSender;
            _applicationConfiguration = applicationConfiguration;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult Contact()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Contact(ContactRequestModel contactRequestModel)
        {
            if (ModelState.IsValid)
            {
                await SendMailToApplicationAdmin(contactRequestModel);
                await SendMailToContact(contactRequestModel);
                return RedirectToAction(nameof(ContactSuccess));
            }

            return View(contactRequestModel);
        }

        private async Task SendMailToApplicationAdmin(ContactRequestModel contactRequestModel)
        {
            await _emailSender.SendEmailAsync(_applicationConfiguration.ApplicationAdminEmailAddress,
                    "Kapcsolatfelvételi kérelem",
                    $"Új kapcsolatfelvételi kérelem érkezett.<br/><br/>" +
                    $"Tárgy: {contactRequestModel.Subject}<br/>" +
                    $"Kérdés: {contactRequestModel.Question}<br/>" +
                    $"Email: {contactRequestModel.Email}");
        }

        private async Task SendMailToContact(ContactRequestModel contactRequestModel)
        {
            await _emailSender.SendEmailAsync(contactRequestModel.Email,
                "Kapcsolatfelvétel sikeres",
                $"Kapcsolatfelvételi kérelmét megkaptuk, hamarosan felvesszük önnel a kapcsolatot. " +
                $"A kapcsolatfelvételi kérelmének részletei:<br/><br/>" +
                $"Tárgy: {contactRequestModel.Subject}<br/>" +
                $"Kérdés: {contactRequestModel.Question}<br/>" +
                $"Email: {contactRequestModel.Email}");
        }

        public IActionResult ContactSuccess()
        {
            return View();
        }

        public IActionResult SiteMap()
        {
            return View();
        }

        public IActionResult ResourceNotFound()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
