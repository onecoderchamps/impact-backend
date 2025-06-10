using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Trasgo.Server.Controllers
{
    [ApiController]
    [Route("api/v1/otp")]
    public class OtpController : ControllerBase
    {
        private readonly IOtpService _otpService;

        public OtpController(IOtpService otpService)
        {
            _otpService = otpService;
        }

        [HttpPost("sendWA")]
        public async Task<IActionResult> SendOtpWA([FromBody] CreateOtpDto dto)
        {
            try
            {
                var result = await _otpService.SendOtpWAAsync(dto);
                return Ok(new { message = result });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("validateWA")]
        public async Task<IActionResult> ValidateOtpWA([FromBody] ValidateOtpDto dto)
        {
            try
            {
                var result = await _otpService.ValidateOtpWAAsync(dto);
                return Ok(new { message = result });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
