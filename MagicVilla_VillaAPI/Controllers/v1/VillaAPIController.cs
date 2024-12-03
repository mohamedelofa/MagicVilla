using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;

namespace MagicVilla_VillaAPI.Controllers.v1
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    [ApiVersion("1")]
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
		//[MapToApiVersion("1")]
		public async Task<ActionResult<ApiResponse>> GetVillas()
        {
            try
            {
                var villas = await _villaRepository.GetAllAsync();
                response.Result = _mapper.Map<List<GetVillaDto>>(villas);
                response.IsSuccess = true;
                response.StatusCode = HttpStatusCode.OK;
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
        public async Task<ActionResult<ApiResponse>> Create(CreateUpdateVillaDto model)
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
                if (await _villaRepository.CreateAsync(villa) == true)
                {
                    response.IsSuccess = true;
                    response.StatusCode = HttpStatusCode.Created;
                    response.Result = model;
                    return CreatedAtAction(nameof(GetById), new { id = villa.Id }, response);
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
                return response;
            }
            try
            {
                Villa? villa = await _villaRepository.GetAsync(v => v.Id == id, true);
                if (villa is null)
                {
                    response.IsSuccess = false;
                    response.StatusCode = HttpStatusCode.NotFound;
                    return response;
                }
                if (await _villaRepository.DeleteAsync(villa) == true)
                {
                    response.IsSuccess = true;
                    response.StatusCode = HttpStatusCode.NoContent;
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
        public async Task<ActionResult<ApiResponse>> Update(int id, CreateUpdateVillaDto model)
        {
            if (id <= 0 || model is null)
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
				Villa? villa = await _villaRepository.GetAsync(v => v.Id == id, false);
                if (villa is null)
                {
                    response.IsSuccess = false;
                    response.StatusCode = HttpStatusCode.NotFound;
                    return response;
                }
                var createAt = villa.CreatedDate;
                villa = _mapper.Map<Villa>(model);
                villa.Id = id;
                villa.CreatedDate = createAt;
                if (await _villaRepository.UpdateAsync(villa) == true)
                {
                    response.IsSuccess = true;
                    response.StatusCode = HttpStatusCode.NoContent;
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
    }
}
