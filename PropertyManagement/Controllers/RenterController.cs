using PropertyManagement.Models;
using PropertyManagement.Data;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace PropertyManagement.Controllers
{
    public class RenterController : Controller
    {
        public async Task<IActionResult> Index()
        {
            return View();
        }

        public async Task<IActionResult> EditRenter(int id)
        {
            var renter = DataAccess.GetRenter(id);

            return View(renter);
        }

        public async Task<IActionResult> UpdateRenter(Renter renter)
        {
            var updateApplied = DataAccess.Update<Renter>(renter);

            if (updateApplied)
            {
                return RedirectToAction("Index", "Properties");
            }

            return View("EditRenter", renter); // Nothing changed, stay on Edit page
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}