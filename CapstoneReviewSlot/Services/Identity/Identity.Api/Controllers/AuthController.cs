using Identity.Application.Abstractions.Services;
using Identity.Application.DTOs.Auth;
using Identity.Domain.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;


namespace Identity.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterRequest request, CancellationToken cancellationToken)
        {
            try
            {
                var result = await _authService.RegisterAsync(request, cancellationToken);
                return Ok(new
                {
                    message = "Đăng ký tài khoản thành công.",
                    data = result
                });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new
                {
                    message = ex.Message
                });
            }
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginRequest request, CancellationToken cancellationToken)
        {
            try
            {
                var result = await _authService.LoginAsync(request, cancellationToken);
                return Ok(new
                {
                    message = "Đăng nhập thành công.",
                    data = result
                });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new
                {
                    message = ex.Message
                });
            }
        }

        //[Authorize(Roles = SystemRoles.Admin)]
        [AllowAnonymous]
        [HttpPost("create-lecturer-account")]
        public async Task<IActionResult> CreateLecturerAccount(CreateLecturerAccountRequest request, CancellationToken cancellationToken)
        {
            try
            {
                var result = await _authService.CreateLecturerAccountAsync(request, cancellationToken);
                return Ok(new
                {
                    message = "Tạo tài khoản giảng viên thành công.",
                    data = result
                });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new
                {
                    message = ex.Message
                });
            }
        }
    }

}
