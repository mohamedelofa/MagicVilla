using AutoMapper;
using MagicVilla_WebApp.Models;
using MagicVilla_WebApp.Services.IServices;
using MagicVilla_WebApp.Services;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Diagnostics;
using MagicVilla_WebApp.Models.Dtos;

namespace MagicVilla_WebApp.Controllers
{
    public class HomeController(IMapper mapper , IVillaService villaService) : Controller
    {
        private readonly IMapper _mapper = mapper;
        private readonly IVillaService _service = villaService;

        public async Task<IActionResult> Index()
        {
            List<GetVillaDto> villas = new List<GetVillaDto>();
            ApiResponse? response = await _service.GetAllAsync("VillaAPI" , HttpContext.Session.GetString(StaticDetails.sessionTokenKey));
            if (response is not null && response.IsSuccess)
            {
                villas = JsonConvert.DeserializeObject<List<GetVillaDto>>(Convert.ToString(response.Result));
            }
            return View(villas);
        }

    }
}
