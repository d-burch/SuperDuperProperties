﻿using PropertyManagement.Models;
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

        public async Task<IActionResult> AddOwner()
        {
            return View();
        }

        public async Task<IActionResult> InsertOwner(Owner owner)
        {
            var insertSuccess = DataAccess.Insert<Owner>(owner, (null, null));

            if (insertSuccess)
            {
                return RedirectToAction("Index", "Properties");
            }

            return View("AddOwner", owner);
        }

        #region API

        [HttpPut]
        public bool AddOwner([FromBody] Owner owner)
        {
            var insertSuccess = DataAccess.Insert<Owner>(owner, (null, null));

            return insertSuccess;
        }

        [HttpPut]
        public bool Update([FromQuery] int ownerId, [FromBody] Owner owner)
        {
            if (owner.OwnerID == 0)
            {
                owner.OwnerID = ownerId;
            }

            var updateApplied = DataAccess.Update<Owner>(owner);

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