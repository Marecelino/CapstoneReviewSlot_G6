using Grpc.Core;
using GrpcContract.Identity;
using Identity.Application.Abstractions.Security;

namespace Identity.Api.GrpcServices;

public class IdentityGrpcService : IdentityGrpc.IdentityGrpcBase
{
    private readonly IJwtTokenService _tokenService;

    public IdentityGrpcService(IJwtTokenService tokenService)
    {
        _tokenService = tokenService;
    }

    public override Task<ValidateTokenResponse> ValidateToken(ValidateTokenRequest request, ServerCallContext context)
    {
        var isValid = _tokenService.ValidateToken(request.Token);
        return Task.FromResult(new ValidateTokenResponse { IsValid = isValid });
    }
}
