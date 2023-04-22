using PropertyManagement.Models;
using PropertyManagement.Data;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace PropertyManagement.Controllers
{
    public class LeaseController : Controller
    {
        public async Task<IActionResult> Index()
        {
            return View();
        }

        public async Task<IActionResult> EditLease(int id)
        {
            var lease = new Lease
            {
                LeaseID = id,
                StartDate = DateTime.Now,
                EndDate = DateTime.Now,
                MonthlyRent = 2000m,
                RentOutstanding = 0,
                SecurityDepositAmount = 0,
                SecurityDepositCharges = 0,
                SecurityDepositPaid = true,
                SecurityDepositReturned = false
            };
            //var lease = DataAccess.GetLease(id);

            return View(lease);
        }

        /*
        public async Task<IActionResult> UpdateLease(Lease lease)
        {
            var updateApplied = DataAccess.UpdateLease(lease);

            if (updateApplied)
            {
                return RedirectToAction("Index", "Properties");
            }

            return View("EditLease", lease); // Nothing changed, stay on Edit page
        }
        */

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

    }
}
