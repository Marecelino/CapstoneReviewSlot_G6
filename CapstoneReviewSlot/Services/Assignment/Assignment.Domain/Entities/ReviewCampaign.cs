using Common;

namespace Assignment.Domain.Entities
{
    public class ReviewCampaign : BaseClass
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int MaxGroupsPerLecturer { get; set; }
        public int RequiredReviewersPerGroup { get; set; }

        public string Status { get; set; }

        // Navigation properties

    }
}
