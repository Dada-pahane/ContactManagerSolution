﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace CRUDExample.Controllers
{
    public class HomeController : Controller
    {
        [Route("/Error")]
        [AllowAnonymous]
        public IActionResult Error()
        {
            var exceptionHandlerPathFeature =
         HttpContext.Features.Get<IExceptionHandlerPathFeature>();

            ViewBag.ExceptionPath = exceptionHandlerPathFeature?.Path;
            ViewBag.ExceptionMessage = exceptionHandlerPathFeature?.Error.Message;
            ViewBag.StackTrace = exceptionHandlerPathFeature?.Error.StackTrace;
            return View();
        }
    }
}
