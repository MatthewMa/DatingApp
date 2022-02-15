using API.DTOS;
using API.Entities;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Authorize]
    public class ApiExceptionController : BaseApiController
    {
        private readonly IApiExceptionRepository _apiExceptionRepository;
        private readonly IMapper _mapper;

        public ApiExceptionController(IApiExceptionRepository apiExceptionRepository, IMapper mapper)
        {
            _apiExceptionRepository = apiExceptionRepository;
            _mapper = mapper;
        }

        [HttpPost]
        public async Task<ActionResult<ApiException>> AddApiException(ApiExceptionCreateDTO apiException)
        {
            var exception = _mapper.Map<ApiException>(apiException);
            _apiExceptionRepository.AddException(exception);
            if (!await _apiExceptionRepository.SaveAllAsync()) return BadRequest("Add api exception failed");
            return Ok(exception);
        }

        [HttpGet("all")]
        public async Task<ActionResult<IEnumerable<ApiException>>> GetApiExceptions()
        {
            return Ok(await _apiExceptionRepository.GetExceptionsAsync());
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<IEnumerable<ApiException>>> GetApiException(int id)
        {
            return Ok(await _apiExceptionRepository.GetApiExceptionByIdAsync(id));
        }

    }
}
