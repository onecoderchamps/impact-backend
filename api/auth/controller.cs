
using System.IdentityModel.Tokens.Jwt;
using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Trasgo.Server.Controllers
{
    [ApiController]
    [Route("api/v1/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _IAuthService;
        private readonly ConvertJWT _ConvertJwt;
        private readonly ErrorHandlingUtility _errorUtility;

        public AuthController(IAuthService authService, ConvertJWT convert)
        {
            _IAuthService = authService;
            _ConvertJwt = convert;
            _errorUtility = new ErrorHandlingUtility();

        }

        [Authorize]
        [HttpGet]
        [Route("verifySessions")]
        public async Task<object> VerifySessionsAsync()
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
                var data = await _IAuthService.Aktifasi(idUser);
                return data;
            }
            catch (CustomException ex)
            {
                int errorCode = ex.ErrorCode;
                var errorResponse = new ErrorResponse(errorCode, ex.ErrorHeader, ex.Message);
                return _errorUtility.HandleError(errorCode, errorResponse);
            }
        }

        [Authorize]
        [HttpPost]
        [Route("tiktok/exchange")]
        public async Task<object> tiktokExchange([FromBody] TikTokExchangeRequest request)
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
                var data = await _IAuthService.tiktokExchange(idUser, request);
                return data;
            }
            catch (CustomException ex)
            {
                int errorCode = ex.ErrorCode;
                var errorResponse = new ErrorResponse(errorCode, ex.ErrorHeader, ex.Message);
                return _errorUtility.HandleError(errorCode, errorResponse);
            }
        }

       

        [Authorize]
        [HttpPost]
        [Route("UpdateProfileSosmed")]
        public async Task<object> UpdateProfileSosmed([FromBody] UpdateProfileDto updateProfileDto)
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
                var data = await _IAuthService.UpdateProfileSosmed(idUser, updateProfileDto);
                return data;
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
        [Route("register")]
        public async Task<object> Register([FromBody] RegisterDto registerDto)
        {
            try
            {
                var data = await _IAuthService.Register(registerDto);
                return data;
            }
            catch (CustomException ex)
            {
                int errorCode = ex.ErrorCode;
                var errorResponse = new ErrorResponse(errorCode, ex.ErrorHeader, ex.Message);
                return _errorUtility.HandleError(errorCode, errorResponse);
            }
        }

        [HttpPost]
        [Route("login")]
        public async Task<object> Login([FromBody] LoginDto registerDto)
        {
            try
            {
                var data = await _IAuthService.Login(registerDto);
                return data;
            }
            catch (CustomException ex)
            {
                int errorCode = ex.ErrorCode;
                var errorResponse = new ErrorResponse(errorCode, ex.ErrorHeader, ex.Message);
                return _errorUtility.HandleError(errorCode, errorResponse);
            }
        }

        // [Authorize]
        // [HttpPost]
        // [Route("updateFCMUser")]
        // public async Task<object> UpdateUserProfile([FromBody] UpdateFCMProfileDto updateProfileDto)
        // {
        //     try
        //     {
        //         var claims = User.Claims;
        //         if (claims == null)
        //         {
        //             return new CustomException(400, "Error", "Unauthorized");
        //         }
        //         string accessToken = HttpContext.Request.Headers["Authorization"];
        //         string idUser = await _ConvertJwt.ConvertString(accessToken);
        //         var data = await _IAuthService.UpdateUserProfile(idUser, updateProfileDto);
        //         return data;
        //     }
        //     catch (CustomException ex)
        //     {
        //         int errorCode = ex.ErrorCode;
        //         var errorResponse = new ErrorResponse(errorCode, ex.ErrorHeader, ex.Message);
        //         return _errorUtility.HandleError(errorCode, errorResponse);
        //     }
        // }
        
    }

}