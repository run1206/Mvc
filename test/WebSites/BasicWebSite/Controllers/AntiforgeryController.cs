// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using BasicWebSite.Models;
using Microsoft.AspNet.Authorization;
using Microsoft.AspNet.Mvc;

namespace BasicWebSite.Controllers
{
    // This controller is reachable via traditional routing.
    public class AntiforgeryController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        // GET: /Antiforgery/Login
        [AllowAnonymous]
        public ActionResult Login(string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;

            return View();
        }

        [AllowAnonymous]
        public string UseFacebookLogin()
        {
            return "somestring";
        }

        // POST: /Antiforgery/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public string Login(LoginViewModel model)
        {
            return "OK";
        }

        // GET: /Antiforgery/FlushAsyncLogin
        [AllowAnonymous]
        public ActionResult FlushAsyncLogin(string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;

            return View();
        }

        // POST: /Antiforgery/FlushAsyncLogin
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public string FlushAsyncLogin(LoginViewModel model)
        {
            return "OK";
        }
    }
}