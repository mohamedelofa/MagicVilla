using Asp.Versioning;
using MagicVilla_VillaAPI.Helpers;
using MagicVilla_VillaAPI.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using System.Text.Json;

namespace MagicVilla_VillaAPI.Controllers.v2
{
	[Route("api/v{version:apiVersion}/[controller]")]
	[ApiController]
	[ApiVersion("2")]
	public class VillaAPIController : ControllerBase
	{
		private readonly IMapper _mapper;
		private readonly IVillaRepository _villaRepository;
		private readonly ApiResponse response;
		private readonly IWebHostEnvironment _environment;
		private readonly ICacheService _cacheService;
		public VillaAPIController(IMapper mapper,
			IVillaRepository villaRepository,
			IWebHostEnvironment environment,
			ICacheService cacheService)
		{
			_mapper = mapper;
			_villaRepository = villaRepository;
			response = new ApiResponse();
			_environment = environment;
			_cacheService = cacheService;
		}

		[HttpGet]
		[ProducesResponseType(StatusCodes.Status200OK)]
		public async Task<ActionResult<ApiResponse>> GetVillas([FromQuery(Name = "filterOccupancy")] int occupancy,
			[FromQuery] int pageSize = 0,
			[FromQuery] int pageNumber = 1)
		{
			try
			{
				// Check cache first
				var cacheKey = RedisKeys.VillasKey(occupancy, pageSize, pageNumber);
				var cachedVillas = await _cacheService.GetAsync<List<GetVillaDto>>(cacheKey);
				if (cachedVillas is not null)
				{
					response.IsSuccess = true;
					response.StatusCode = HttpStatusCode.OK;
					response.Result = cachedVillas;
					Response.Headers.Append("X-Pagination", JsonSerializer.Serialize(new Pagination() { PageSize = pageSize, PageNumber = pageNumber }));
					return response;
				}
				List<Villa> villas;
				if (occupancy > 0)
					villas = await _villaRepository.GetAllAsync(v => v.Occupancy == occupancy, pageSize: pageSize, pageNumber: pageNumber);
				else
					villas = await _villaRepository.GetAllAsync(pageSize: pageSize, pageNumber: pageNumber);

				response.Result = _mapper.Map<List<GetVillaDto>>(villas);
				response.IsSuccess = true;
				response.StatusCode = HttpStatusCode.OK;
				Pagination pagination = new Pagination() { PageSize = pageSize, PageNumber = pageNumber };
				Response.Headers.Append("X-Pagination", JsonSerializer.Serialize(pagination));
				// Set cache
				await _cacheService.SetAsync(cacheKey, response.Result, DateTimeOffset.Now.AddMinutes(15));
			}
			catch (Exception ex)
			{
				response.IsSuccess = false;
				response.Errors.Add(ex.Message);
			}
			return response;
		}


		[HttpGet("{id:int}")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		public async Task<ActionResult<ApiResponse>> GetById(int id)
		{
			if (id <= 0)
			{
				response.IsSuccess = false;
				response.StatusCode = HttpStatusCode.BadRequest;
				return BadRequest(response);
			}
			try
			{
				//Check cache first
				var cacheKey = $"{RedisKeys.VillaKey}{id}";
				var cachedVilla = await _cacheService.GetAsync<GetVillaDto>(cacheKey);
				if (cachedVilla is not null)
				{
					response.IsSuccess = true;
					response.StatusCode = HttpStatusCode.OK;
					response.Result = cachedVilla;
					return response;
				}
				var villa = await _villaRepository.GetAsync(v => v.Id == id, false);
				if (villa is null)
				{
					response.IsSuccess = false;
					response.StatusCode = HttpStatusCode.NotFound;
					return NotFound(response);
				}
				response.IsSuccess = true;
				response.StatusCode = HttpStatusCode.OK;
				response.Result = _mapper.Map<GetVillaDto>(villa);
				// Set cache
				await _cacheService.SetAsync(cacheKey, response.Result, DateTimeOffset.Now.AddMinutes(15));
			}
			catch (Exception ex)
			{
				response.IsSuccess = false;
				response.Errors.Add(ex.Message);
			}
			return response;
		}

		[HttpPost]
		[ProducesResponseType(StatusCodes.Status201Created)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[Authorize]
		public async Task<ActionResult<ApiResponse>> Create([FromForm] CreateVillaDto model)
		{
			if (model is null)
			{
				response.IsSuccess = false;
				response.StatusCode = HttpStatusCode.BadRequest;
				return response;
			}
			try
			{
				if (await _villaRepository.GetAsync(v => v.Name.ToLower() == model.Name.ToLower(), false) is not null)
				{
					response.IsSuccess = false;
					response.Errors.Add("Villa Already Exist");
					response.StatusCode = HttpStatusCode.BadRequest;
					return response;
				}
				Villa villa = _mapper.Map<Villa>(model);
				var fileName = await SaveIamge(model.Image);
				villa.ImageName = fileName;
				villa.ImageUrl = GetImageUrl(fileName);
				if (await _villaRepository.CreateAsync(villa) == true)
				{
					response.IsSuccess = true;
					response.StatusCode = HttpStatusCode.Created;
					response.Result = _mapper.Map<GetVillaDto>(villa);
					// clean cache
					await _cacheService.RemoveAllAsync(RedisKeys.AllVillasKeys);
					return response;//CreatedAtAction(nameof(GetById), new { id = villa.Id }, response.Result);
				}
				else
				{
					response.IsSuccess = false;
					response.StatusCode = HttpStatusCode.InternalServerError;
				}
			}
			catch (Exception ex)
			{
				response.IsSuccess = false;
				response.Errors.Add(ex.Message);
			}
			return response;
		}

		[HttpDelete("{id:int}")]
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		[Authorize]
		public async Task<ActionResult<ApiResponse>> Delete(int id)
		{
			if (id <= 0)
			{
				response.IsSuccess = false;
				response.StatusCode = HttpStatusCode.BadRequest;
				return BadRequest(response);
			}
			try
			{
				Villa? villa = await _villaRepository.GetAsync(v => v.Id == id, true);
				if (villa is null)
				{
					response.IsSuccess = false;
					response.StatusCode = HttpStatusCode.NotFound;
					return NotFound(response);
				}
				DeleteFile(villa.ImageName);
				if (await _villaRepository.DeleteAsync(villa) == true)
				{
					response.IsSuccess = true;
					response.StatusCode = HttpStatusCode.NoContent;
					// clean cache
					await _cacheService.RemoveAllAsync(RedisKeys.AllVillasKeys);
					await _cacheService.RemoveAsync($"{RedisKeys.VillaKey}{id}");
				}
				else
				{
					response.IsSuccess = false;
					response.StatusCode = HttpStatusCode.InternalServerError;
				}
			}
			catch (Exception ex)
			{
				response.IsSuccess = false;
				response.Errors.Add(ex.Message);
			}
			return response;
		}

		[HttpPut("{id:int}")]
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		[Authorize]
		public async Task<ActionResult<ApiResponse>> Update(int id, [FromForm] UpdateVillaDto model)
		{
			if (id <= 0 || model is null)
			{
				response.IsSuccess = false;
				response.StatusCode = HttpStatusCode.BadRequest;
				return response;
			}
			try
			{
				Villa? villa = await _villaRepository.GetAsync(v => v.Id == id, false);
				if (villa is null)
				{
					response.IsSuccess = false;
					response.StatusCode = HttpStatusCode.NotFound;
					return response;
				}
				if (await _villaRepository.GetAsync(v => v.Name.ToLower() == model.Name.ToLower() && v.Name != villa.Name, false) is not null)
				{
					response.IsSuccess = false;
					response.Errors.Add("Villa Name Already Exist");
					response.StatusCode = HttpStatusCode.BadRequest;
					return response;
				}
				villa = _mapper.Map(model, villa);
				if (model.Image is not null)
				{
					DeleteFile(villa.ImageName);
					var newImageName = await SaveIamge(model.Image);
					villa.ImageName = newImageName;
					villa.ImageUrl = GetImageUrl(newImageName);
				}
				if (await _villaRepository.UpdateAsync(villa) == true)
				{
					response.IsSuccess = true;
					response.StatusCode = HttpStatusCode.NoContent;
					// clean cache
					await _cacheService.RemoveAllAsync(RedisKeys.AllVillasKeys);
					await _cacheService.RemoveAsync($"{RedisKeys.VillaKey}{id}");
				}
				else
				{
					response.IsSuccess = false;
					response.StatusCode = HttpStatusCode.InternalServerError;
				}
			}
			catch (Exception ex)
			{
				response.IsSuccess = false;
				response.Errors.Add(ex.Message);
			}
			return response;
		}


		private async Task<string> SaveIamge(IFormFile image)
		{
			// Save image
			var directory = Path.Combine(_environment.WebRootPath, "Images");
			if (!Directory.Exists(directory))
				Directory.CreateDirectory(directory);
			var fileName = $"{Guid.NewGuid().ToString()}.{Path.GetExtension(image.FileName)}";
			var path = Path.Combine(directory, fileName);
			using (var stream = new FileStream(path, FileMode.Create))
			{
				await image.CopyToAsync(stream);
			}
			return fileName;
		}

		private string GetImageUrl(string fileName)
		{
			var baseUrl = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host.Value}{HttpContext.Request.PathBase.Value ?? string.Empty}";
			var imageUrl = $"{baseUrl}/Images/{fileName}";
			return imageUrl;
		}

		private void DeleteFile(string fileName)
		{
			var oldPath = Path.Combine(_environment.WebRootPath, "Images", fileName);
			if (System.IO.File.Exists(oldPath))
				System.IO.File.Delete(oldPath);
		}
	}
}
