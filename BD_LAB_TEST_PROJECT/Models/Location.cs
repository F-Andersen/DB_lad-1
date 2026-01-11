using System.ComponentModel.DataAnnotations;

namespace BD_LAB_TEST_PROJECT.Models
{
    public class Location
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid TravelPlanId { get; set; }
        public TravelPlan? TravelPlan { get; set; }
        [Required, MaxLength(200)]
        public string Name { get; set; } = string.Empty;
        [Range(1, int.MaxValue)]
        public int Order { get; set; }
        [Range(0, int.MaxValue)]
        public int BudgetEur { get; set; }
        [MaxLength(1000)]
        public string? Notes { get; set; }
        [ConcurrencyCheck]
        public int Version { get; set; } = 1;


        public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
        public DateTimeOffset UpdatedAt { get; set; } = DateTimeOffset.UtcNow;
    }
}
