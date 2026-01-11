using System.ComponentModel.DataAnnotations;

namespace BD_LAB_TEST_PROJECT.Models
{
    public class TravelPlan
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        [Required, MaxLength(200)]
        public string Title { get; set; } = string.Empty;
        [Range(0, int.MaxValue)]
        public int BudgetEur { get; set; }
        [ConcurrencyCheck]
        public int Version { get; set; } = 1;
        public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
        public DateTimeOffset UpdatedAt { get; set; } = DateTimeOffset.UtcNow;
        public List<Location> Locations { get; set; } = new();
    }
}
