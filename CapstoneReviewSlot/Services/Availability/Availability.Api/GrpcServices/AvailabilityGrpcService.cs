using Grpc.Core;
using GrpcContract.Availability;
using Availability.Domain.Interfaces;
using Availability.Domain.Enums;

namespace Availability.Api.GrpcServices;

/// <summary>
/// gRPC server: trả lời các service khác muốn kiểm tra
/// giảng viên X có đăng ký rảnh hay không.
/// </summary>
public class AvailabilityGrpcService : AvailabilityGrpc.AvailabilityGrpcBase
{
    private readonly IUnitOfWork _uow;
    private readonly ILogger<AvailabilityGrpcService> _logger;

    public AvailabilityGrpcService(IUnitOfWork uow, ILogger<AvailabilityGrpcService> logger)
    {
        _uow = uow;
        _logger = logger;
    }

    public override async Task<CheckResponse> CheckLecturerAvailable(
        CheckRequest request, ServerCallContext context)
    {
        if (!Guid.TryParse(request.LecturerId, out var lecturerId))
            throw new RpcException(new Status(StatusCode.InvalidArgument, "lecturer_id phải là Guid hợp lệ."));

        var records = await _uow.Availabilities.GetByLecturerIdAsync(lecturerId, context.CancellationToken);
        var isAvailable = records.Any(r => r.Status == AvailabilityStatus.Available);

        _logger.LogInformation("gRPC CheckLecturerAvailable: LecturerId={Id}, IsAvailable={Result}",
            lecturerId, isAvailable);

        return new CheckResponse { IsAvailable = isAvailable };
    }
}
