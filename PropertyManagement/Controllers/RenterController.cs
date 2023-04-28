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
            var renter = DataAccess.Get<Renter>(id);

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

        // The id argument is the parent property's id, to use as the FK
        public async Task<IActionResult> AddRenter(int id)
        {
            var renter = new Renter();

            return View((renter, id));
        }

        public async Task<IActionResult> InsertRenter(Renter renter, IFormCollection collection)
        {
            var parentId = collection["leaseId"];

            if (int.TryParse(parentId, out int leaseId) && leaseId > 0)
            {
                var insertSuccess = DataAccess.Insert<Renter>(renter, (leaseId, "Lease"));

                if (insertSuccess)
                {
                    return RedirectToAction("Index", "Properties");
                }
            }

            // Insert failed, stay on the page (add better indication to user that it failed)
            return View("AddRenter", renter);
        }

        #region API

        [HttpPost]
        public bool AddRenter([FromQuery] int leaseId, [FromBody] Renter renter)
        {
            if (leaseId > 0)
            {
                var insertSuccess = DataAccess.Insert<Renter>(renter, (leaseId, "Lease"));

                return insertSuccess;
            }

            return false;
        }

        #endregion API



        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}