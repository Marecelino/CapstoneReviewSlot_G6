namespace Identity.Application.DTOs;

public record LoginRequestDto(string Email, string Password);

public record LoginResponseDto(string AccessToken, string TokenType, int ExpiresIn, UserDto User);
