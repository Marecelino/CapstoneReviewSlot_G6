using Grpc.Core;
using GrpcContract.Session;
using Session.Domain.Interfaces;

namespace Session.Api.GrpcServices;

public class SessionGrpcService : SessionGrpc.SessionGrpcBase
{
    private readonly IUnitOfWork _uow;
    private readonly ILogger<SessionGrpcService> _logger;

    public SessionGrpcService(IUnitOfWork uow, ILogger<SessionGrpcService> logger)
    {
        _uow = uow;
        _logger = logger;
    }

    public override async Task<SessionResponse> GetSessionInfo(
        GetSessionRequest request, ServerCallContext context)
    {
        if (!Guid.TryParse(request.SessionId, out var slotId))
        {
            _logger.LogWarning("Invalid session_id: {Id}", request.SessionId);
            throw new RpcException(new Status(StatusCode.InvalidArgument, "session_id phải là GUID hợp lệ."));
        }

        var slot = await _uow.Slots.GetByIdAsync(slotId, context.CancellationToken);
        if (slot is null)
            throw new RpcException(new Status(StatusCode.NotFound, $"Slot {slotId} không tồn tại."));

        return new SessionResponse { Id = slot.Id.ToString() };
    }
}
