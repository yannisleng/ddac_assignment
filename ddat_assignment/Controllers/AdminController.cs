﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ddat_assignment.Controllers
{
    [Authorize(Roles = "Warehouse")]
    public class AdminController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Workspace()
        {
            return View();
        }

        public IActionResult StandardShipping()
        {
            return View();
        }
    }
}
