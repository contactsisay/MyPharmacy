﻿using Microsoft.AspNetCore.Mvc;
using MyPharmacy.Models;

namespace MyPharmacy.Areas.Sales.Controllers
{
    [Area("Sales")]
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            ViewData["Title"] = "Sales";
            HttpContext.Session.Remove(SessionVariable.SessionKeyMessageType);
            HttpContext.Session.Remove(SessionVariable.SessionKeyMessage);
            return View();
        }
    }
}
