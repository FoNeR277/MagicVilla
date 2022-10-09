using System.Net;
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
    public class VillaNumberApiController : ControllerBase
    {
        protected APIResponse _response;
        private readonly IVillaNumberRepository _repository;
        private readonly IVillaRepository _villaRepository;
        private readonly IMapper _mapper;

        public VillaNumberApiController(IMapper mapper, IVillaNumberRepository repository, IVillaRepository villaRepository)
        {
            _mapper = mapper;
            _repository = repository;
            _villaRepository = villaRepository;
            _response = new();
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<APIResponse>> GetVillaNumbers()
        {
            try
            {
                IEnumerable<VillaNumber> villaNumbers = await _repository.GetAllAsync();
                _response.Result = _mapper.Map<List<VillaNumberDto>>(villaNumbers);
                _response.StatusCode = HttpStatusCode.OK;
                _response.IsSuccess = true;
                return Ok(_response);
            }
            catch (Exception e)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages
                    = new List<string>() { e.Message };
            }

            return _response;
        }

        [HttpGet("{id:int}", Name = "GetVillaNumber")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<APIResponse>> GetVillaNumber(int id)
        {
            try
            {
                if (id == 0)
                {
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(_response);
                }

                var villa = await _repository.GetAsync(x => x.VillaNo == id);
                if (villa is null)
                {
                    _response.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(_response);
                }

                _response.Result = _mapper.Map<VillaNumberDto>(villa);
                _response.StatusCode = HttpStatusCode.OK;
                _response.IsSuccess = true;
                return Ok(_response);
            }
            catch (Exception e)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages
                    = new List<string>() { e.Message };
            }

            return _response;
        }


        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<APIResponse>> CreateVillaNumber([FromBody] VillaNumberCreateDto createDto)
        {
            try
            {
                if (await _repository.GetAsync(x => x.VillaNo == createDto.VillaNo) != null)
                {
                    ModelState.AddModelError("CustomError", "Villa already exist!");
                    return BadRequest(ModelState);
                }

                if (await _villaRepository.GetAsync(x => x.Id == createDto.VillaId) == null)
                {
                    ModelState.AddModelError("CustomError", "Villa does not exist!");
                    return BadRequest(ModelState);
                }
                
                if (createDto == null)
                {
                    return BadRequest(createDto);
                }

                VillaNumber villa = _mapper.Map<VillaNumber>(createDto);

                await _repository.CreateAsync(villa);
                _response.Result = _mapper.Map<VillaNumberDto>(villa);
                _response.StatusCode = HttpStatusCode.Created;
                return CreatedAtRoute("GetVillaNumber", new { id = villa.VillaNo }, _response);
            }
            catch (Exception e)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages
                    = new List<string>() { e.Message };
            }

            return _response;
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpDelete("{id:int}", Name = "DeleteVillaNumber")]
        public async Task<ActionResult<APIResponse>> DeleteVillaNumber(int id)
        {
            try
            {
                if (id == 0)
                {
                    return BadRequest();
                }

                var villaNumber = await _repository.GetAsync(x => x.VillaNo == id);
                if (villaNumber is null)
                {
                    return NotFound();
                }

                await _repository.RemoveAsync(villaNumber);
                _response.Result = _mapper.Map<VillaNumberDto>(villaNumber);
                _response.StatusCode = HttpStatusCode.OK;
                return Ok(_response);
            }
            catch (Exception e)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages
                    = new List<string>() { e.Message };
            }

            return _response;
        }

        [HttpPut("{id:int}", Name = "UpdateVillaNumber")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<APIResponse>> UpdateVillaNumber(int id, [FromBody] VillaNumberUpdateDto updateDto)
        {
            try
            {
                if (updateDto is null || id != updateDto.VillaNo)
                {
                    return BadRequest();
                }

                if (await _villaRepository.GetAsync(x => x.Id == updateDto.VillaId) == null)
                {
                    ModelState.AddModelError("CustomError", "Villa does not exist!");
                    return BadRequest(ModelState);
                }
                VillaNumber villa = _mapper.Map<VillaNumber>(updateDto);

                await _repository.UpdateAsync(villa);
                _response.Result = _mapper.Map<VillaNumberDto>(villa);
                _response.StatusCode = HttpStatusCode.OK;
                return Ok(_response);
            }
            catch (Exception e)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages
                    = new List<string>() { e.Message };
            }

            return _response;
        }
    }
}