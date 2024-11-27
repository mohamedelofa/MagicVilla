﻿using AutoMapper;
using MagicVilla_WebApp.Models;
using MagicVilla_WebApp.Models.Dtos;
using MagicVilla_WebApp.Models.ViewModels;
using MagicVilla_WebApp.Services.IServices;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Security.Claims;

namespace MagicVilla_WebApp.Controllers
{
	public class AuthenticationController : Controller
	{
		private readonly IAuthService _authenticationService;
		private readonly IMapper _mapper;
        public AuthenticationController(IAuthService authenticationService , IMapper mapper)
        {
			_authenticationService = authenticationService;
			_mapper = mapper;
        }
        public IActionResult Register()
		{
			return View();
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Register(RegisterRequestViewModel model)
		{
			if(ModelState.IsValid)
			{
				ApiResponse? response = await _authenticationService.Register(_mapper.Map<RegisterRequestDto>(model));
				if(response is not null && response.IsSuccess)
				{
					TempData["Success"] = "Register done successfully";
					return RedirectToAction(nameof(LogIn));
				}
				if(response?.Errors.Count > 0)
				{
					ModelState.AddModelError(string.Empty, response.Errors.FirstOrDefault() ?? "");
				}
				TempData["Error"] = "Error encountered";
			}
			return View(model);
		}

		public IActionResult LogIn()
		{
			return View();
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> LogIn(LogInRequestViewModel model)
		{
			if (ModelState.IsValid)
			{
				ApiResponse? response = await _authenticationService.LogIn(_mapper.Map<LogInRequestDto>(model));
				if (response is not null && response.IsSuccess)
				{
					LogInResponseDto logged = JsonConvert.DeserializeObject<LogInResponseDto>(Convert.ToString(response.Result));
					var identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme);
					identity.AddClaim(new Claim(ClaimTypes.Name, logged.User.UserName));
					identity.AddClaim(new Claim(ClaimTypes.Role, logged.User.Role));
					var principal = new ClaimsPrincipal(identity);
					await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,principal);
					HttpContext.Session.SetString(StaticDetails.sessionTokenKey,logged.Token);
					TempData["Success"] = "LogIn done successfully";
					return RedirectToAction("Index", "Home");
				}
				if (response?.Errors.Count > 0)
				{
					ModelState.AddModelError(string.Empty, response.Errors.FirstOrDefault() ?? "");
				}
				TempData["Error"] = "Error encountered";
			}
			return View(model);
		}

		public async Task<IActionResult> LogOut()
		{
			await HttpContext.SignOutAsync();
			HttpContext.Session.SetString(StaticDetails.sessionTokenKey, string.Empty);
			return RedirectToAction(nameof(LogIn));
		}

		public ActionResult AccessDenied()
		{
			return View();
		}
	}
}