﻿using AutoMapper;
using MagicVilla_WebApp.Models;
using MagicVilla_WebApp.Models.Dtos;
using MagicVilla_WebApp.Models.ViewModels;
using MagicVilla_WebApp.Services.IServices;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace MagicVilla_WebApp.Controllers
{
	public class AuthenticationController : Controller
	{
		private readonly IAuthService _authenticationService;
		private readonly ITokenService _tokenService;
		private readonly IMapper _mapper;
		public AuthenticationController(IAuthService authenticationService,
			IMapper mapper,
			ITokenService tokenService)
		{
			_authenticationService = authenticationService;
			_mapper = mapper;
			_tokenService = tokenService;
		}
		public IActionResult Register()
		{
			var roleList = new List<SelectListItem>()
			{
				new SelectListItem(nameof(StaticDetails.Admin),StaticDetails.Admin),
				new SelectListItem(nameof(StaticDetails.User),StaticDetails.User)
			};
			ViewBag.RoleList = roleList;
			return View();
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Register(RegisterRequestViewModel model)
		{
			if (ModelState.IsValid)
			{
				ApiResponse? response = await _authenticationService.Register(_mapper.Map<RegisterRequestDto>(model));
				if (response is not null && response.IsSuccess)
				{
					TempData["Success"] = "Register done successfully";
					return RedirectToAction(nameof(LogIn));
				}
				if (response?.Errors.Count > 0)
				{
					ModelState.AddModelError(string.Empty, response.Errors.FirstOrDefault() ?? "");
				}
				TempData["Error"] = "Error encountered";
			}
			var roleList = new List<SelectListItem>()
			{
				new SelectListItem(nameof(StaticDetails.Admin),StaticDetails.Admin),
				new SelectListItem(nameof(StaticDetails.User),StaticDetails.User)
			};
			ViewBag.RoleList = roleList;
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
					//identity.AddClaim(new Claim(ClaimTypes.Name, logged.User.UserName));
					//identity.AddClaim(new Claim(ClaimTypes.Role, logged.User.Role));
					//foreach(var role in logged.Roles)
					//                   identity.AddClaim(new Claim(ClaimTypes.Role, role));

					var handler = new JwtSecurityTokenHandler();
					var jwt = handler.ReadJwtToken(logged.AccessToken);
					identity.AddClaim(
						new Claim(
							ClaimTypes.Name,
							jwt?.Claims?
							.FirstOrDefault(c => c.Type == "unique_name")?.Value ?? string.Empty));
					foreach (var role in jwt?.Claims?.Where(c => c.Type == "role")?.Select(c => c.Value)?.ToList())
					{
						identity.AddClaim(new Claim(ClaimTypes.Role, role));
					}
					var principal = new ClaimsPrincipal(identity);
					await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
					//HttpContext.Session.SetString(StaticDetails.sessionTokenKey, logged.AccessToken);

					_tokenService.SetToken(new TokenDto() { Accesstoken = logged.AccessToken, RefreshToken = logged.RefreshToken });
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
			var response = await _authenticationService.LogOut();
			if (response is not null && response.IsSuccess)
			{
				await HttpContext.SignOutAsync();
				_tokenService.DeleteToken();
				return RedirectToAction(nameof(LogIn));
			}
			else
			{
				TempData["Error"] = "Error encountered";
				return RedirectToAction("Index", "Home");
			}

			//HttpContext.Session.SetString(StaticDetails.sessionTokenKey, string.Empty);
		}

		public ActionResult AccessDenied()
		{
			return View();
		}


		public async Task<IActionResult> ConfirmEmail(string email, string token)
		{
			if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(token))
			{
				TempData["Error"] = "Invalid email or token";
				return RedirectToAction(nameof(LogIn));
			}
			var response = await _authenticationService.ConfirmEmailAsync(email, token);
			if (response.IsSuccess)
			{
				TempData["Success"] = "Email confirmed successfully";
				return RedirectToAction(nameof(LogIn));
			}
			else
			{
				TempData["Error"] = response.Errors.FirstOrDefault() ?? "Error confirming email";
				return RedirectToAction(nameof(LogIn));
			}
		}
	}
}
