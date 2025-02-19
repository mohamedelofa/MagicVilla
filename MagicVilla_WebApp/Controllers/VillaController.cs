using AutoMapper;
using MagicVilla_WebApp.Models;
using MagicVilla_WebApp.Models.Dtos;
using MagicVilla_WebApp.Services.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace MagicVilla_WebApp.Controllers
{
	public class VillaController(IVillaService villaService, IMapper mapper) : Controller
	{
		private readonly IMapper _mapper = mapper;
		private readonly IVillaService _service = villaService;
		private string endPoint = "VillaAPI";

		public async Task<IActionResult> Index()
		{
			List<GetVillaDto> villas = new List<GetVillaDto>();
			ApiResponse? response = await _service.GetAllAsync(endPoint);
			if (response is not null && response.IsSuccess)
			{
				villas = JsonConvert.DeserializeObject<List<GetVillaDto>>(Convert.ToString(response.Result));
			}
			return View(villas);
		}

		[Authorize]
		public IActionResult Create()
		{
			return View();
		}
		[HttpPost]
		[ValidateAntiForgeryToken]
		[Authorize]
		public async Task<IActionResult> Create(CreateVillaDto model)
		{
			if (ModelState.IsValid)
			{
				ApiResponse? response = await _service.CreateAsync<CreateVillaDto>(model, endPoint);
				if (response is not null && response.IsSuccess)
				{
					TempData["Success"] = "Villa Created Successfully";
					return RedirectToAction(nameof(Index));
				}
				else
				{
					if (response?.Errors.Count > 0)
					{
						ModelState.AddModelError("", response?.Errors.FirstOrDefault() ?? " ");
					}
					TempData["Error"] = "Error encountered";
				}
			}
			return View(model);
		}

		[HttpGet]
		[Authorize]
		public async Task<IActionResult> Update(int id)
		{
			if (id <= 0) return BadRequest();
			ApiResponse? response = await _service.GetAsync(id, endPoint);
			if (response is not null && response.IsSuccess)
			{
				var model = JsonConvert.DeserializeObject<GetVillaDto>(Convert.ToString(response.Result));
				return View(_mapper.Map<UpdateVillaDto>(model));
			}
			return NotFound();
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		[Authorize]
		public async Task<IActionResult> Update(UpdateVillaDto model, int id)
		{
			if (ModelState.IsValid)
			{
				ApiResponse? response = await _service.UpdateAsync<UpdateVillaDto>(model, id, endPoint);
				if (response is not null && response.IsSuccess)
				{
					TempData["Success"] = "Villa Updated Successfully";
					return RedirectToAction(nameof(Index));
				}
				else
				{
					if (response?.Errors.Count > 0)
					{
						ModelState.AddModelError("", response?.Errors.FirstOrDefault() ?? " ");
					}
					TempData["Error"] = "Error encountered";
				}
			}
			model.Id = id;
			return View(model);
		}

		[HttpGet]
		[Authorize]
		public async Task<IActionResult> Delete(int id)
		{
			if (id <= 0) return BadRequest();
			ApiResponse? response = await _service.GetAsync(id, endPoint);
			if (response is not null && response.IsSuccess)
			{
				var dto = JsonConvert.DeserializeObject<GetVillaDto>(Convert.ToString(response.Result));
				return View(dto);
			}
			return NotFound();
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		[Authorize]
		public async Task<IActionResult> Delete(GetVillaDto model)
		{
			ApiResponse? response = await _service.DeleteAsync(model.Id, endPoint);
			if (response is not null && response.IsSuccess)
			{
				TempData["Success"] = "Villa Deleted Successfully";
				return RedirectToAction(nameof(Index));
			}
			else
			{
				if (response?.Errors.Count > 0)
				{
					ModelState.AddModelError("", response.Errors.FirstOrDefault() ?? "");
				}
				TempData["Error"] = "Error encountered";
			}
			return View(model);
		}
	}
}
