using Identity.Application.Abstractions.Services;
using Identity.Application.DTOs.Auth;
using Identity.Domain.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

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
        [Authorize(Roles = SystemRoles.Manager)]
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

        [AllowAnonymous]
        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordRequest request, CancellationToken cancellationToken)
        {
            try
            {
                await _authService.ForgotPasswordAsync(request, cancellationToken);
                return Ok(new
                {
                    message = "Xin vui lòng kiểm tra Gmail, mã OTP đã được gửi."
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
        [HttpPost("verify-forgot-password-otp")]
        public async Task<IActionResult> VerifyForgotPasswordOtp(VerifyForgotPasswordOtpRequest request, CancellationToken cancellationToken)
        {
            try
            {
                await _authService.VerifyForgotPasswordOtpAsync(request, cancellationToken);
                return Ok(new
                {
                    message = "Xác thực OTP thành công."
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
        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword(ResetPasswordRequest request, CancellationToken cancellationToken)
        {
            try
            {
                await _authService.ResetPasswordAsync(request, cancellationToken);
                return Ok(new
                {
                    message = "Đặt lại mật khẩu thành công."
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

        [Authorize]
        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword(ChangePasswordRequest request, CancellationToken cancellationToken)
        {
            try
            {
                var userIdClaim =
                    User.FindFirst(ClaimTypes.NameIdentifier)?.Value ??
                    User.FindFirst("sub")?.Value;

                if (string.IsNullOrWhiteSpace(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
                {
                    return Unauthorized(new
                    {
                        message = "Không xác định được người dùng từ token."
                    });
                }

                await _authService.ChangePasswordAsync(userId, request, cancellationToken);

                return Ok(new
                {
                    message = "Đổi mật khẩu thành công."
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