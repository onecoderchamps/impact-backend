using Microsoft.AspNetCore.Mvc;
using RepositoryPattern.Services.RateCardService;

namespace impact.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class RateCardController : ControllerBase
    {
        private readonly IRateCardService _rateCardService;
        private readonly ConvertJWT _ConvertJwt;

        public RateCardController(IRateCardService rateCardService, ConvertJWT convert)
        {
            _rateCardService = rateCardService;
            _ConvertJwt = convert;
        }

        // GET api/ratecard/user/{idUser}
        [HttpGet]
        public async Task<IActionResult> GetByUser()
        {
            var claims = User.Claims;
            if (claims == null)
            {
                return Unauthorized(new { code = 400, error = "Error", message = "Unauthorized" });
            }
            string accessToken = HttpContext.Request.Headers["Authorization"];
            string idUser = await _ConvertJwt.ConvertString(accessToken);
            var result = await _rateCardService.GetByUser(idUser);
            return Ok(result);
        }

        // POST api/ratecard
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] CreateOrUpdateRateCardDto dto)
        {
            var claims = User.Claims;
            if (claims == null)
            {
                return Unauthorized(new { code = 400, error = "Error", message = "Unauthorized" });
            }
            string accessToken = HttpContext.Request.Headers["Authorization"];
            string idUser = await _ConvertJwt.ConvertString(accessToken);
            var result = await _rateCardService.Post(dto, idUser);
            return Ok(result);
        }

        // PUT api/ratecard/{id}
        [HttpPut]
        public async Task<IActionResult> Put([FromBody] CreateOrUpdateRateCardDto dto)
        {
            var claims = User.Claims;
            if (claims == null)
            {
                return Unauthorized(new { code = 400, error = "Error", message = "Unauthorized" });
            }
            string accessToken = HttpContext.Request.Headers["Authorization"];
            string idUser = await _ConvertJwt.ConvertString(accessToken);
            var result = await _rateCardService.Put(idUser, dto);
            return Ok(result);
        }
    }
}
