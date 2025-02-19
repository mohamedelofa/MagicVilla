using AutoMapper;
using MagicVilla_WebApp.Models;
using MagicVilla_WebApp.Models.Dtos;
using MagicVilla_WebApp.Services.IServices;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace MagicVilla_WebApp.Controllers
{
	public class HomeController(IMapper mapper, IVillaService villaService) : Controller
	{
		private readonly IMapper _mapper = mapper;
		private readonly IVillaService _service = villaService;

		public async Task<IActionResult> Index()
		{
			List<GetVillaDto> villas = new List<GetVillaDto>();
			ApiResponse? response = await _service.GetAllAsync("VillaAPI");
			if (response is not null && response.IsSuccess)
			{
				villas = JsonConvert.DeserializeObject<List<GetVillaDto>>(Convert.ToString(response.Result));
			}
			return View(villas);
		}

	}
}
