

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Trasgo.Server.Controllers
{
    // [Authorize]
    [ApiController]
    [Route("/api/v1/[controller]")]
    public class CampaignController : ControllerBase
    {
        private readonly ICampaignService _ICampaignService;
        private readonly ErrorHandlingUtility _errorUtility;
        private readonly ConvertJWT _ConvertJwt;
        
        public CampaignController(ConvertJWT convert,ICampaignService CampaignService)
        {
            _ICampaignService = CampaignService;
            _errorUtility = new ErrorHandlingUtility();
             _ConvertJwt = convert;
        }

        // [Authorize]
        [HttpGet]
        public async Task<object> Get()
        {
            try
            {
                var claims = User.Claims;
                if (claims == null)
                {
                    return Unauthorized(new { code = 400, error = "Error", message = "Unauthorized" });
                }
                string accessToken = HttpContext.Request.Headers["Authorization"];
                string idUser = await _ConvertJwt.ConvertString(accessToken);
                var data = await _ICampaignService.Get(idUser);
                return Ok(data);
            }
            catch (CustomException ex)
            {
                int errorCode = ex.ErrorCode;
                var errorResponse = new ErrorResponse(errorCode, ex.ErrorHeader, ex.Message);
                return _errorUtility.HandleError(errorCode, errorResponse);
            }
        }

        [HttpGet("Kontrak")]
        public async Task<object> GetKontrak()
        {
            try
            {
                var claims = User.Claims;
                if (claims == null)
                {
                    return Unauthorized(new { code = 400, error = "Error", message = "Unauthorized" });
                }
                string accessToken = HttpContext.Request.Headers["Authorization"];
                string idUser = await _ConvertJwt.ConvertString(accessToken);
                var data = await _ICampaignService.GetKontrak(idUser);
                return Ok(data);
            }
            catch (CustomException ex)
            {
                int errorCode = ex.ErrorCode;
                var errorResponse = new ErrorResponse(errorCode, ex.ErrorHeader, ex.Message);
                return _errorUtility.HandleError(errorCode, errorResponse);
            }
        }

        [HttpGet("all")]
        public async Task<object> GetAll()
        {
            try
            {
                var data = await _ICampaignService.GetAll();
                return Ok(data);
            }
            catch (CustomException ex)
            {
                int errorCode = ex.ErrorCode;
                var errorResponse = new ErrorResponse(errorCode, ex.ErrorHeader, ex.Message);
                return _errorUtility.HandleError(errorCode, errorResponse);
            }
        }

        [HttpGet("{id}")]
        public async Task<object> GetById([FromRoute] string id)
        {
            try
            {
                var data = await _ICampaignService.GetById(id);
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
        [HttpPost]
        public async Task<object> Post([FromBody] CreateCampaignDto item)
        {
            try
            {
                var claims = User.Claims;
                if (claims == null)
                {
                    return Unauthorized(new { code = 400, error = "Error", message = "Unauthorized" });
                }
                string accessToken = HttpContext.Request.Headers["Authorization"];
                string idUser = await _ConvertJwt.ConvertString(accessToken);
                var data = await _ICampaignService.Post(item, idUser);
                return Ok(data);
            }
            catch (CustomException ex)
            {
                int errorCode = ex.ErrorCode;
                var errorResponse = new ErrorResponse(errorCode, ex.ErrorHeader, ex.Message);
                return _errorUtility.HandleError(errorCode, errorResponse);
            }
        }

        [HttpPost("verifCampaign")]
        public async Task<object> PostActivate([FromForm] PayCallbackCampaignDto item)
        {
            try
            {
                var data = await _ICampaignService.PayCallback(item);
                return Ok(data);
            }
            catch (CustomException ex)
            {
                int errorCode = ex.ErrorCode;
                var errorResponse = new ErrorResponse(errorCode, ex.ErrorHeader, ex.Message);
                return _errorUtility.HandleError(errorCode, errorResponse);
            }
        }

        [HttpPost("payCampaign")]
        public async Task<object> PayCampaign([FromBody] PayCampaignDto item)
        {
            try
            {
                var data = await _ICampaignService.PostActivate(item);
                return Ok(data);
            }
            catch (CustomException ex)
            {
                int errorCode = ex.ErrorCode;
                var errorResponse = new ErrorResponse(errorCode, ex.ErrorHeader, ex.Message);
                return _errorUtility.HandleError(errorCode, errorResponse);
            }
        }

        [HttpPost("register")]
        public async Task<object> RegisterCampaign([FromBody] RegisterCampaignDto item)
        {
            try
            {
                var claims = User.Claims;
                if (claims == null)
                {
                    return Unauthorized(new { code = 400, error = "Error", message = "Unauthorized" });
                }
                string accessToken = HttpContext.Request.Headers["Authorization"];
                string idUser = await _ConvertJwt.ConvertString(accessToken);
                var data = await _ICampaignService.RegisterCampaign(item, idUser);
                return Ok(data);
            }
            catch (CustomException ex)
            {
                int errorCode = ex.ErrorCode;
                var errorResponse = new ErrorResponse(errorCode, ex.ErrorHeader, ex.Message);
                return _errorUtility.HandleError(errorCode, errorResponse);
            }
        }

        [HttpPost("registerByBrand")]
        public async Task<object> RegisterbyBrandCampaign([FromBody] RegisterCampaignDto item)
        {
            try
            {
                var data = await _ICampaignService.RegisterByBrandCampaign(item, item.IdUser);
                return Ok(data);
            }
            catch (CustomException ex)
            {
                int errorCode = ex.ErrorCode;
                var errorResponse = new ErrorResponse(errorCode, ex.ErrorHeader, ex.Message);
                return _errorUtility.HandleError(errorCode, errorResponse);
            }
        }

        [HttpGet("registerMember/{id}")]
        public async Task<object> registerMember([FromRoute] string id)
        {
            try
            {
                var data = await _ICampaignService.RegisterMemberCampaign(id);
                return Ok(data);
            }
            catch (CustomException ex)
            {
                int errorCode = ex.ErrorCode;
                var errorResponse = new ErrorResponse(errorCode, ex.ErrorHeader, ex.Message);
                return _errorUtility.HandleError(errorCode, errorResponse);
            }
        }
        [HttpPut("MemberCampaign")]
        public async Task<object> MemberCampaign([FromBody] UpdateCampaignDto item)
        {
            try
            {
                var data = await _ICampaignService.MemberCampaign(item);
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
        [HttpPut("{id}")]
        public async Task<object> Put([FromRoute] string id, [FromBody] CreateCampaignDto item)
        {
            try
            {
                var data = await _ICampaignService.Put(id, item);
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
        [HttpDelete("{id}")]
        public async Task<object> Delete([FromRoute] string id)
        {
            try
            {
                var data = await _ICampaignService.Delete(id);
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