using AutoMapper;
using MagicVilla_VillaAPI.Data;
using MagicVilla_VillaAPI.Models;
using MagicVilla_VillaAPI.Models.Dtos;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MagicVilla_VillaAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VillaAPIController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;
        public VillaAPIController(AppDbContext context , IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<List<GetVillaDto>>> GetVillas()
        {
            var villas =await _context.Villas.ToListAsync();
            List<GetVillaDto> villaDtos = _mapper.Map<List<GetVillaDto>>(villas);
            return Ok(villaDtos);
        }

        [HttpGet("{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async  Task<ActionResult<GetVillaDto>> GetById(int id)
        {
            if (id <= 0) return BadRequest();
            var villa = await _context.Villas.SingleOrDefaultAsync(v => v.Id == id);
            if (villa is null) return NotFound();
            GetVillaDto villaDto = _mapper.Map<GetVillaDto>(villa);
            return Ok(villaDto);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(Villa))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<Villa>> Create(CreateUpdateVillaDto model)
        {
            if(model is null) return BadRequest();
            Villa villa = _mapper.Map<Villa>(model);
            await _context.Villas.AddAsync(villa);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetById), new { id = villa.Id }, villa);
        }

        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(int id)
        {
            if (id <= 0) return BadRequest();
            Villa? villa = _context.Villas.Find(id);
            if (villa is null) return NotFound();
            _context.Villas.Remove(villa);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpPut("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update(int id , CreateUpdateVillaDto model)
        {
            if(id <= 0 || model is null) return BadRequest();
            Villa? villa = await _context.Villas.AsNoTracking().SingleOrDefaultAsync(x => x.Id == id);
            if(villa is null ) return NotFound();
            var createAt = villa.CreatedDate;
            villa = _mapper.Map<Villa>(model);
            villa.Id = id;
            villa.CreatedDate = createAt;
            villa.UpdatedDate = DateTime.Now;
            _context.Villas.Update(villa);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
