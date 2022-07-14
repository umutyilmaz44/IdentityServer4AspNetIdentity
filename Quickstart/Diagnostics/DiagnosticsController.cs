// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Serilog;

namespace IdentityServerHost.Quickstart.UI
{
    [SecurityHeaders]
    [Authorize]
    public class DiagnosticsController : Controller
    {
        public async Task<IActionResult> Index()
        {
            var localAddresses = new string[] { "127.0.0.1", "::1", HttpContext.Connection.LocalIpAddress.ToString() };

            string localGateway = HttpContext.Connection.LocalIpAddress.ToString().Substring(0, HttpContext.Connection.LocalIpAddress.ToString().LastIndexOf("."));
            string remoteGateway = HttpContext.Connection.RemoteIpAddress.ToString().Substring(0, HttpContext.Connection.RemoteIpAddress.ToString().LastIndexOf("."));
            Log.Debug($"localGateway: {localGateway}");
            Log.Debug($"remoteGateway: {remoteGateway}");
            if (!localAddresses.Contains(HttpContext.Connection.RemoteIpAddress.ToString()) && localGateway != remoteGateway)
            {
                Log.Debug($"RemoteIpAddress: {Environment.NewLine}{HttpContext.Connection.RemoteIpAddress.ToString()}");
                Log.Debug($"localAddresses: {Environment.NewLine}{JsonConvert.SerializeObject(localAddresses, Formatting.Indented)}");
                return NotFound();
            }

            var model = new DiagnosticsViewModel(await HttpContext.AuthenticateAsync());
            return View(model);
        }
    }
}