using Asp.Versioning;
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
        public VillaAPIController(IMapper mapper, IVillaRepository villaRepository)
        {
            _mapper = mapper;
            _villaRepository = villaRepository;
            response = new ApiResponse();
        }

		[HttpGet]
		[ProducesResponseType(StatusCodes.Status200OK)]
		public async Task<ActionResult<ApiResponse>> GetVillas([FromQuery(Name = "filterOccupancy")]int occupancy , 
			[FromQuery]int pageSize = 2 , 
			[FromQuery] int pageNumber = 1)
		{
			try
			{
				List<Villa> villas;
				if(occupancy > 0)
					villas = await _villaRepository.GetAllAsync(v => v.Occupancy == occupancy , pageSize:pageSize , pageNumber:pageNumber);
				else
					villas = await _villaRepository.GetAllAsync(pageSize: pageSize, pageNumber: pageNumber);

				response.Result = _mapper.Map<List<GetVillaDto>>(villas);
				response.IsSuccess = true;
				response.StatusCode = HttpStatusCode.OK;
				Pagination pagination = new Pagination() { PageSize = pageSize, PageNumber = pageNumber };
				Response.Headers.Append("X-Pagination", JsonSerializer.Serialize(pagination));
			}
			catch (Exception ex)
			{
				response.IsSuccess = false;
				response.Errors.Add(ex.Message);
			}
			return response;
		}

	}
}
