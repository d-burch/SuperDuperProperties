﻿using PropertyManagement.Models;
using PropertyManagement.Data;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace PropertyManagement.Controllers
{
    public class PropertiesController : Controller
    {
        public async Task<IActionResult> Index()
        {
            var allOwners = DataAccess.GetFullDataGraph();

            // Owners are top-level, use each owner to get list of all properties
            return View(allOwners);
        }

        public async Task<IActionResult> EditProperty(int id)
        {
            var property = DataAccess.Get<Property>(id);

            return View(property);
        }

        public async Task<IActionResult> UpdateProperty(Property property)
        {
            var updateApplied = DataAccess.Update<Property>(property);

            if (updateApplied)
            {
                return RedirectToAction("Index", "Properties");
            }

            return View("EditProperty", property); // Nothing changed, stay on Edit page
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}