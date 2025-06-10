using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace impact.Server.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/v1/Chat")]
    public class ChatController : ControllerBase
    {
        private readonly IChatService _ChatService;
        private readonly ConvertJWT _ConvertJwt;

        public ChatController(ConvertJWT convert,IChatService ChatService)
        {
            _ChatService = ChatService;
            _ConvertJwt = convert;
        }

        [HttpPost("sendWA")]
        public async Task<IActionResult> SendChatWA([FromBody] CreateChatDto dto)
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
                var result = await _ChatService.SendChatWAAsync(idUser,dto);
                return Ok(new { message = result });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("getWA")]
        public async Task<IActionResult> GetChatWA([FromBody] GetChatDto dto)
        {
            try
            {
                var result = await _ChatService.GetChatWAAsync(dto);
                return Ok(new { message = result });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
