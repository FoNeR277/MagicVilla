using AutoMapper;
using MagicVilla_API.Data;
using MagicVilla_API.Models;
using MagicVilla_API.Models.Dto;
using MagicVilla_API.Repository.IRepository;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MagicVilla_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VillaApiController : ControllerBase
    {

        private readonly IVillaRepository _repository;
        private readonly IMapper _mapper;
        public VillaApiController(ApplicationDbContext dbContext, IMapper mapper, IVillaRepository repository)
        {
            _mapper = mapper;
            _repository = repository;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<VillaDTO>>> GetVillas()
        {
            IEnumerable<Villa> villaList = await _repository.GetAllAsync();
            return Ok(_mapper.Map<List<VillaDTO>>(villaList));
        }
        
        [HttpGet("{id:int}", Name = "GetVilla")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<VillaDTO>> GEtVillas(int id)
        {
            if (id == 0)
            {
                return BadRequest();
            }
            var villa = await _repository.GetAsync(x => x.Id == id);
            if (villa is null)
            {
                return NotFound();
            }

            return Ok(_mapper.Map<VillaDTO>(villa));
        }


        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<VillaDTO>> CreateVilla([FromBody]VillaCreateDTO createDto)
        {
            if (await _repository.GetAsync(x=> x.Name.ToLower() == createDto.Name.ToLower()) != null)
            {
                ModelState.AddModelError("CustomError", "Villa already exist!");
                return BadRequest(ModelState);
            }
            if (createDto == null)
            {
                return BadRequest(createDto);
            }

            Villa villa = _mapper.Map<Villa>(createDto);
            
            await _repository.CreateAsync(villa);
            return CreatedAtRoute("GetVilla", new {id = villa.Id}, villa);
        }

        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpDelete("{id:int}", Name = "DeleteVilla")]
        public async Task<IActionResult> DeleteVilla(int id)
        {
            if (id == 0)
            {
                return BadRequest();
            }
            var villa = await _repository.GetAsync(x => x.Id == id);
            if (villa is null)
            {
                return NotFound();
            }

            await _repository.RemoveAsync(villa);
            return NoContent();
        }
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [HttpPut("{id:int}", Name = "UpdateVilla")]
        public async Task<IActionResult> UpdateVilla(int id, [FromBody]VillaUpdateDTO updateDto)
        {   
            if (updateDto is null || id != updateDto.Id)
            {
                return BadRequest();
            }

            // var villa = _dbContext.Villas.FirstOrDefault(x => x.Id == id);
            // villa.Name = villaDto.Name;
            // villa.Occupancy = villaDto.Occupancy;
            // villa.Sqft = villaDto.Sqft;

            Villa villa = _mapper.Map<Villa>(updateDto);

            await _repository.UpdateAsync(villa);
            return NoContent();
        }

        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpPatch("{id:int}", Name = "UpdatePartialVilla")]
        public async Task<IActionResult> UpdatePartialVilla(int id, JsonPatchDocument<VillaUpdateDTO> patchDto)
        {
            if (patchDto is null || id == 0)
            {
                return BadRequest();
            }
            var villa = await _repository.GetAsync(x => x.Id == id, tracked: false); // remember (AsNoTracking for patching db context model)!!!
            VillaUpdateDTO villaDTO = _mapper.Map<VillaUpdateDTO>(villa);
            
            if (villa is null)
            {
                return NotFound();
            }
            patchDto.ApplyTo(villaDTO, ModelState);
            Villa model = _mapper.Map<Villa>(villaDTO);
            await _repository.UpdateAsync(model);
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            return NoContent();
        }
    }
}
