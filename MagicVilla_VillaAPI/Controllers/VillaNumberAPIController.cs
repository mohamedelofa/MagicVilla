using Azure;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Migrations.Internal;

namespace MagicVilla_VillaAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VillaNumberAPIController : ControllerBase
    {
        private readonly IVillaNumberRepository _villaNumberRepository;
        private readonly IVillaRepository _villaRepository;
        private readonly IMapper _mapper;
        private readonly ApiResponse Response;
        public VillaNumberAPIController(IVillaNumberRepository villaNumberRepository , IMapper mapper , IVillaRepository villaRepository)
        {
            _mapper = mapper;
            _villaNumberRepository = villaNumberRepository;
            _villaRepository = villaRepository;
            Response = new ApiResponse();
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<ApiResponse>> GetVillaNumbers()
        {
            try
            {
                var VillaNumbers = await _villaNumberRepository.GetAllAsync(includeProperties:"Villa");
                Response.Result = _mapper.Map<List<GetVillaNumberDto>>(VillaNumbers);
                Response.StatusCode = HttpStatusCode.OK;
                Response.IsSuccess = true;
            }
            catch (Exception ex)
            {
                Response.IsSuccess = false;
                Response.Errors.Add(ex.Message);
            }
            return Response;
        }

        [HttpGet("{number:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ApiResponse>> GetByNo(int number)
        {
            if(number <= 0 )
            {
                Response.StatusCode = HttpStatusCode.BadRequest;
                Response.IsSuccess = false;
                return BadRequest(Response);
            }
            try
            {
                var villaNumber = await _villaNumberRepository.GetAsync(v => v.VillaNo == number, false , includeProperties:"Villa");
                if (villaNumber is null)
                {
                    Response.IsSuccess = false;
                    Response.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(Response);
                }
                Response.Result = _mapper.Map<GetVillaNumberDto>(villaNumber);
                Response.StatusCode = HttpStatusCode.OK;
                Response.IsSuccess = true;
            }
            catch (Exception ex)
            {
                Response.IsSuccess = false;
                Response.Errors.Add(ex.Message);
            }
            return Response;
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ApiResponse>> Create(CreateVillaNumberDto model)
        {
            if(model is null)
            {
                Response.StatusCode = HttpStatusCode.BadRequest;
                Response.IsSuccess = false;
                return BadRequest(Response);
            }
            try
            {
                if(await _villaNumberRepository.GetAsync(v => v.VillaNo == model.VillaNo , false) is not null)
                {
                    Response.IsSuccess = false;
                    Response.StatusCode=HttpStatusCode.BadRequest;
                    Response.Errors.Add("Villa Number already exist.");
                    return BadRequest(Response);
                }
                if (await _villaRepository.GetAsync(v => v.Id == model.VillaId, false) is null)
                {
                    Response.Errors.Add("Villa Id Is invalid");
                    Response.IsSuccess = false;
                    Response.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(Response);
                }
                var villaNumber = _mapper.Map<VillaNumber>(model);
                if(await _villaNumberRepository.CreateAsync(villaNumber) == true)
                {
                    Response.IsSuccess = true;
                    Response.StatusCode = HttpStatusCode.Created;
                    Response.Result = model;
                    return CreatedAtAction(nameof(GetByNo), new { number = villaNumber.VillaNo }, Response);
                }
                else
                {
                    Response.IsSuccess = false;
                    Response.StatusCode = HttpStatusCode.InternalServerError;
                    return StatusCode(StatusCodes.Status500InternalServerError , Response);
                }
            }
            catch (Exception ex)
            {
                Response.IsSuccess = false;
                Response.Errors.Add(ex.Message);
            }
            return Response;
        }




        [HttpDelete("{number:int}")]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]

        public async Task<ActionResult<ApiResponse>> Delete(int number)
        {
            if (number <= 0)
            {
                Response.StatusCode = HttpStatusCode.BadRequest;
                Response.IsSuccess = false;
                return BadRequest(Response);
            }
            try
            {
                var villaNumber = await _villaNumberRepository.GetAsync(v => v.VillaNo == number, true);
                if (villaNumber is null)
                {
                    Response.IsSuccess = false;
                    Response.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(Response);
                }
                if(await _villaNumberRepository.DeleteAsync(villaNumber) == true )
                {
                    Response.IsSuccess = true;
                    Response.StatusCode = HttpStatusCode.NoContent;
                    return Response;
                }
                else
                {
                    Response.IsSuccess = false;
                    Response.StatusCode = HttpStatusCode.InternalServerError;
                    return StatusCode(StatusCodes.Status500InternalServerError,Response);
                }
            }
            catch (Exception ex)
            {
                Response.IsSuccess = false;
                Response.Errors.Add(ex.Message);
            }
            return Response;
        }



        [HttpPut("{number:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ApiResponse>> Update(int number, UpdateVillaNumberDto model)
        {
            if (number <= 0 || model is null)
            {
                Response.StatusCode = HttpStatusCode.BadRequest;
                Response.IsSuccess = false;
                return BadRequest(Response);
            }
            try
            {
                if(await _villaRepository.GetAsync(v => v.Id == model.VillaId,false) is null)
                {
                    Response.Errors.Add("Villa Id Is invalid");
                    Response.IsSuccess = false;
                    Response.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(Response);
                }
                var villaNumber = await _villaNumberRepository.GetAsync(v => v.VillaNo == number,false);
                if (villaNumber is null)
                {
                    Response.IsSuccess = false;
                    Response.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(Response);
                }
                DateTime Create = villaNumber.CreatedDate;
                villaNumber = _mapper.Map<VillaNumber>(model);
                villaNumber.CreatedDate = Create;
                villaNumber.VillaNo = number;
                if(await _villaNumberRepository.UpdateAsync(villaNumber) == true)
                {
                    Response.IsSuccess = true;
                    Response.StatusCode = HttpStatusCode.NoContent;
                }
                else
                {
                    Response.IsSuccess = false;
                    Response.StatusCode = HttpStatusCode.InternalServerError;
                }
            }
            catch (Exception ex)
            {
                Response.IsSuccess = false;
                Response.Errors.Add(ex.Message);
            }
            return Response;
        }
    }
}
