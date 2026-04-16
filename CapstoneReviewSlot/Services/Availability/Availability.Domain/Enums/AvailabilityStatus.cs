namespace Availability.Domain.Enums;

public enum AvailabilityStatus
{
    Available = 0,    // Giảng viên đã đăng ký rảnh
    Cancelled = 1,    // Giảng viên huỷ đăng ký
    Assigned = 2      // Đã được phân công vào slot này
}
