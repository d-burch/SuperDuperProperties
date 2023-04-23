using PropertyManagement.Models;
using PropertyManagement.Data;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace PropertyManagement.Controllers
{
    public class OwnerController : Controller
    {
        public async Task<IActionResult> Index()
        {
            return View();
        }

        public async Task<IActionResult> EditOwner(int id)
        {
            var owner = DataAccess.Get<Owner>(id);

            return View(owner);
        }

        public async Task<IActionResult> UpdateOwner(Owner owner)
        {
            var updateApplied = DataAccess.Update<Owner>(owner);

            if (updateApplied)
            {
                return RedirectToAction("Index", "Properties");
            }

            return View("EditOwner", owner); // Nothing changed, stay on Edit page
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}