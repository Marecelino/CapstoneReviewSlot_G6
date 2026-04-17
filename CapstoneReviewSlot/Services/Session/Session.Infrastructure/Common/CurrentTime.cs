using Session.Infrastructure.Interfaces;

namespace Session.Infrastructure.Common
{
    public class CurrentTime : ICurrentTime
    {
        public DateTime GetCurrentTime()
        {
            return DateTime.UtcNow.ToUniversalTime();
        }
    }
}
