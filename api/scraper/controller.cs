

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Trasgo.Server.Controllers
{
    [Authorize]
    [ApiController]
    [Route("/api/v1/[controller]")]
    public class ScraperController : ControllerBase
    {
        private readonly IScraperService _IScraperService;
        private readonly ErrorHandlingUtility _errorUtility;
        private readonly ValidationMasterDto _masterValidationService;
        private readonly ConvertJWT _ConvertJwt;
        public ScraperController(IScraperService ScraperService, ConvertJWT convert)
        {
            _IScraperService = ScraperService;
            _ConvertJwt = convert;
            _errorUtility = new ErrorHandlingUtility();
            _masterValidationService = new ValidationMasterDto();
        }

        [HttpGet("{id}")]
        public async Task<object> GetById([FromRoute] string id)
        {
            try
            {
                var data = await _IScraperService.GetById(id);
                return Ok(data);
            }
            catch (CustomException ex)
            {
                int errorCode = ex.ErrorCode;
                var errorResponse = new ErrorResponse(errorCode, ex.ErrorHeader, ex.Message);
                return _errorUtility.HandleError(errorCode, errorResponse);
            }
        }


        // [Authorize]
        [HttpPost("scraperTiktok")]
        public async Task<object> scraperTiktok([FromBody] TikTokProfileRequest item)
        {
            try
            {
                var claims = User.Claims;
                if (claims == null)
                {
                    return new CustomException(400, "Error", "Unauthorized");
                }
                string accessToken = HttpContext.Request.Headers["Authorization"];
                string idUser = await _ConvertJwt.ConvertString(accessToken);
                var data = await _IScraperService.scraperTiktok(item, idUser);
                return Ok(data);
            }
            catch (CustomException ex)
            {
                int errorCode = ex.ErrorCode;
                var errorResponse = new ErrorResponse(errorCode, ex.ErrorHeader, ex.Message);
                return _errorUtility.HandleError(errorCode, errorResponse);
            }
        }

        [HttpPost("scraperInstagram")]
        public async Task<object> scraperInstagram([FromBody] InstagramProfileRequest item)
        {
            try
            {
                var claims = User.Claims;
                if (claims == null)
                {
                    return new CustomException(400, "Error", "Unauthorized");
                }
                string accessToken = HttpContext.Request.Headers["Authorization"];
                string idUser = await _ConvertJwt.ConvertString(accessToken);
                var data = await _IScraperService.scraperInstagram(item, idUser);
                return Ok(data);
            }
            catch (CustomException ex)
            {
                int errorCode = ex.ErrorCode;
                var errorResponse = new ErrorResponse(errorCode, ex.ErrorHeader, ex.Message);
                return _errorUtility.HandleError(errorCode, errorResponse);
            }
        }

        [HttpPost("scraperYoutube")]
        public async Task<object> scraperYoutube([FromBody] YoutubeProfileRequest item)
        {
            try
            {
                var claims = User.Claims;
                if (claims == null)
                {
                    return new CustomException(400, "Error", "Unauthorized");
                }
                string accessToken = HttpContext.Request.Headers["Authorization"];
                string idUser = await _ConvertJwt.ConvertString(accessToken);
                var data = await _IScraperService.scraperYoutube(item, idUser);
                return Ok(data);
            }
            catch (CustomException ex)
            {
                int errorCode = ex.ErrorCode;
                var errorResponse = new ErrorResponse(errorCode, ex.ErrorHeader, ex.Message);
                return _errorUtility.HandleError(errorCode, errorResponse);
            }
        }

        [HttpPost("scraperLinkin")]
        public async Task<object> scraperLinkin([FromBody] LinkinProfileRequest item)
        {
            try
            {
                var claims = User.Claims;
                if (claims == null)
                {
                    return new CustomException(400, "Error", "Unauthorized");
                }
                string accessToken = HttpContext.Request.Headers["Authorization"];
                string idUser = await _ConvertJwt.ConvertString(accessToken);
                var data = await _IScraperService.scraperLinkin(item, idUser);
                return Ok(data);
            }
            catch (CustomException ex)
            {
                int errorCode = ex.ErrorCode;
                var errorResponse = new ErrorResponse(errorCode, ex.ErrorHeader, ex.Message);
                return _errorUtility.HandleError(errorCode, errorResponse);
            }
        }

    }
}