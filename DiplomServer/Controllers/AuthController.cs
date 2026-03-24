using Microsoft.AspNetCore.Mvc;
using DiplomServer.Interfaces.Service;
using DiplomServer.Models.DTO.Auth;

namespace DiplomServer.Controllers
{
    [ApiController]
    [Route("api/auth/")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("login")]
        public async Task<ActionResult> Login([FromBody] LoginDto dto)
        {
            try
            {
                var token = await _authService.LoginAsync(dto.Email, dto.Password);
                return Ok(new { token });
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized(new { message = "Неверный email или пароль" });
            }
        }

        [HttpPost("register")]
        public async Task<ActionResult> Register([FromBody] RegisterDto dto)
        {
            try
            {
                var token = await _authService.RegisterAsync(dto);
                return Ok(new { token, message = "Пользователь зарегистрирован!" });
            }
            catch (InvalidOperationException ex) when (ex.Message.Contains("уже зарегистрирован"))
            {
                return Conflict(new { message = "Email уже зарегистрирован" });
            }
            catch (Exception)
            {
                return StatusCode(500, new { message = "Ошибка сервера" });
            }
        }
    }

}
