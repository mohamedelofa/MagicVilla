using AutoMapper;
using MagicVilla_WebApp.Models;
using MagicVilla_WebApp.Models.Dtos;
using MagicVilla_WebApp.Models.ViewModels;
using MagicVilla_WebApp.Services;
using MagicVilla_WebApp.Services.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace MagicVilla_WebApp.Controllers
{
	public class VillaNumberController(IMapper mapper , IVillaNumberService villaNumberService , IVillaService villaService) : Controller
	{
		private readonly IMapper _mapper = mapper;
		private readonly IVillaNumberService _villaNumberService = villaNumberService;
		private readonly IVillaService _villaService = villaService;
		private string endPoint = "VillaNumberAPI";

		public async Task<IActionResult> Index()
		{
			List<GetVillaNumberDto> lst = new List<GetVillaNumberDto>();

            ApiResponse? response = await _villaNumberService.GetAllAsync(endPoint, HttpContext.Session.GetString(StaticDetails.sessionTokenKey));
			if(response is not null && response.IsSuccess)
			{
				lst = JsonConvert.DeserializeObject<List<GetVillaNumberDto>>(Convert.ToString(response.Result));
			}
            return View(lst);
        }

		[HttpGet]
        [Authorize]
        public async Task<IActionResult> Create()
		{
			ApiResponse? response = await _villaService.GetAllAsync("VillaAPI",HttpContext.Session.GetString(StaticDetails.sessionTokenKey));
			if(response is not null && response.IsSuccess)
			{
				CreateVillaNumberViewModel model = new CreateVillaNumberViewModel();
				model.Villas = JsonConvert.DeserializeObject<IEnumerable<GetVillaDto>>(Convert.ToString(response.Result));
				return View(model);
			}
			return BadRequest();
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Create(CreateVillaNumberViewModel model)
		{
			if(ModelState.IsValid)
			{
				ApiResponse? createResponse = await _villaNumberService.CreateAsync<CreateVillaNumberDto>(_mapper.Map<CreateVillaNumberDto>(model), endPoint , HttpContext.Session.GetString(StaticDetails.sessionTokenKey));
				if(createResponse is not null && createResponse.IsSuccess)
				{
					TempData["Success"] = "Villa Number Created Successfully";
					return RedirectToAction(nameof(Index));
				}
				else
				{
					if (createResponse?.Errors.Count > 0)
					{
						ModelState.AddModelError("", createResponse?.Errors.FirstOrDefault() ?? " ");
					}
				}
			}
			ApiResponse? response = await _villaService.GetAllAsync("VillaAPI" , HttpContext.Session.GetString(StaticDetails.sessionTokenKey));
			if(response is not null && response.IsSuccess)
			{
				model.Villas = JsonConvert.DeserializeObject<IEnumerable<GetVillaDto>>(Convert.ToString(response.Result));
				TempData["Error"] = "Error encountered";
				return View(model);
			}
			return BadRequest();
		}

		[HttpGet]
		[Authorize]
        public async Task<IActionResult> Update(int id)
		{
			if(id <= 0) return BadRequest();
			ApiResponse? villaNumberResponse = await _villaNumberService.GetAsync(id , endPoint , HttpContext.Session.GetString(StaticDetails.sessionTokenKey));

			if (villaNumberResponse is not null && villaNumberResponse.IsSuccess)
			{
				var dto = JsonConvert.DeserializeObject<GetVillaNumberDto>(Convert.ToString(villaNumberResponse.Result));
				UpdateVillaNumberViewModel model = _mapper.Map<UpdateVillaNumberViewModel>(dto);
				ApiResponse? villaResponse = await _villaService.GetAllAsync("VillaAPI" , HttpContext.Session.GetString(StaticDetails.sessionTokenKey));
				if (villaResponse is not null && villaResponse.IsSuccess)
				{
					model.Villas = JsonConvert.DeserializeObject<IEnumerable<GetVillaDto>>(Convert.ToString(villaResponse.Result));
				}
				return View(model);
			}
			return NotFound();
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Update(UpdateVillaNumberViewModel model , int id)
		{
			if(ModelState.IsValid)
			{
				ApiResponse? updateResponse = await _villaNumberService.UpdateAsync<UpdateVillaNumberDto>(_mapper.Map<UpdateVillaNumberDto>(model), id , endPoint , HttpContext.Session.GetString(StaticDetails.sessionTokenKey));
				if(updateResponse is not null && updateResponse.IsSuccess)
				{
					TempData["Success"] = "Villa Number Updated Successfully";
					return RedirectToAction(nameof(Index));
				}
				else
				{
					if(updateResponse?.Errors.Count > 0)
					{
						ModelState.AddModelError("", updateResponse?.Errors.FirstOrDefault() ?? " ");
					}
				}
			}
			ApiResponse? response = await _villaService.GetAllAsync("VillaAPI" , HttpContext.Session.GetString(StaticDetails.sessionTokenKey));
			if (response is not null && response.IsSuccess)
			{
				model.Villas = JsonConvert.DeserializeObject<IEnumerable<GetVillaDto>>(Convert.ToString(response.Result));
				TempData["Error"] = "Error encountered";
				return View(model);
			}
			return BadRequest();
		}

		[HttpGet]
        [Authorize]
        public async Task<IActionResult> Delete(int id)
		{
			if(id <= 0) return BadRequest();
			ApiResponse? response = await _villaNumberService.GetAsync(id , endPoint , HttpContext.Session.GetString(StaticDetails.sessionTokenKey));
			if(response is not null && response.IsSuccess)
			{
				var dto = JsonConvert.DeserializeObject<GetVillaNumberDto>(Convert.ToString(response.Result));
				return View(dto);
			}
			return NotFound();
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Delete(GetVillaNumberDto model , int id)
		{
			ApiResponse? response = await _villaNumberService.DeleteAsync(id , endPoint , HttpContext.Session.GetString(StaticDetails.sessionTokenKey));
			if(response is not null && response.IsSuccess)
			{
				TempData["Success"] = "Villa Number Deleted Successfully";
				return RedirectToAction(nameof(Index));
			}
			else
			{
				if(response?.Errors.Count > 0)
				{
					ModelState.AddModelError("", response.Errors.FirstOrDefault()??"");
				}
			}
			TempData["Error"] = "Error encountered";
			model.VillaNo = id;
			return View(model);
		}
	}
}
