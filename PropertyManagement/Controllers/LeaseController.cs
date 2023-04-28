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
            var lease = DataAccess.Get<Lease>(id);

            return View(lease);
        }

        public async Task<IActionResult> UpdateLease(Lease lease)
        {
            var updateApplied = DataAccess.Update<Lease>(lease);

            if (updateApplied)
            {
                return RedirectToAction("Index", "Properties");
            }

            return View("EditLease", lease); // Nothing changed, stay on Edit page
        }

        // The id argument is the parent property's id, to use as the FK
        public async Task<IActionResult> AddLease(int id)
        {
            var lease = new Lease();

            return View((lease, id));
        }

        public async Task<IActionResult> InsertLease(Lease lease, IFormCollection collection)
        {
            var parentId = collection["propertyId"];

            if (int.TryParse(parentId, out int propertyId) && propertyId > 0)
            {
                var insertSuccess = DataAccess.Insert<Lease>(lease, (propertyId, "Property"));

                if (insertSuccess)
                {
                    return RedirectToAction("Index", "Properties");
                }
            }

            // Insert failed, stay on the page (add better indication to user that it failed)
            return View("AddLease", lease);
        }

        #region API

        [HttpPut]
        public bool AddLease([FromQuery] int propertyId, [FromBody] Lease lease)
        {
            //if (!DateTime.TryParse(lease.StartDate, out var _start) || !DateTime.TryParse(lease.EndDate, out var _end))
            //{
                //return false;
            //}

            if (propertyId > 0)
            {
                var insertSuccess = DataAccess.Insert<Lease>(lease, (propertyId, "Property"));

                return insertSuccess;
            }

            return false;
        }

        [HttpPut]
        public bool Update([FromQuery] int leaseId, [FromBody] Lease lease)
        {
            if (lease.LeaseID == 0)
            {
                lease.LeaseID = leaseId;
            }

            var updateApplied = DataAccess.Update<Lease>(lease);

            return updateApplied;
        }


        #endregion API

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

    }
}
