﻿using Microsoft.AspNetCore.Mvc;
using MyPharmacy.Models;

namespace MyPharmacy.Areas.Inventory.Controllers
{
    [Area("Inventory")]
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            ViewData["Title"] = "Inventory Home";
            HttpContext.Session.Remove(SessionVariable.SessionKeyMessageType);
            HttpContext.Session.Remove(SessionVariable.SessionKeyMessage);
            return View();
        }
    }
}
