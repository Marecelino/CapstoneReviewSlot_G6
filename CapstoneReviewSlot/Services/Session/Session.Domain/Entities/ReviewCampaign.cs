using Entities;
using Session.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Session.Domain.Entities
{
    public class ReviewCampaign : BaseEntity
    {
        public string Name { get; set; } = default!;
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public int MaxGroupsPerLecturer { get; set; }
        public int RequiredReviewersPerGroup { get; set; }
        public string Status { get; set; }

        public ICollection<ReviewSlot> ReviewSlots { get; set; } = new List<ReviewSlot>();

        public ReviewCampaign() { }

        public static ReviewCampaign Create(string name, DateTime startTime, DateTime endTime, int maxGroups, int requiredReviewers)
        {
            return new ReviewCampaign
            {
                Name = name,
                StartTime = startTime,
                EndTime = endTime,
                MaxGroupsPerLecturer = maxGroups,
                RequiredReviewersPerGroup = requiredReviewers,
                Status = CampaignStatus.Draft.ToString()
            };
        }
    }
}
