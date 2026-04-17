using Entities;
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
        public Session.Domain.Enums.CampaignStatus Status { get; set; }

        public ICollection<ReviewSlot> ReviewSlots { get; set; } = new List<ReviewSlot>();

        private ReviewCampaign() { }

        public static ReviewCampaign Create(string name, DateTime startTime, DateTime endTime, int maxGroups, int requiredReviewers)
        {
            return new ReviewCampaign
            {
                Name = name,
                StartTime = startTime,
                EndTime = endTime,
                MaxGroupsPerLecturer = maxGroups,
                RequiredReviewersPerGroup = requiredReviewers,
                Status = Session.Domain.Enums.CampaignStatus.Draft
            };
        }
    }
}
