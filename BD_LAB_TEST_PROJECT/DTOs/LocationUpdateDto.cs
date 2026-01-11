using System.ComponentModel.DataAnnotations;

namespace BD_LAB_TEST_PROJECT.DTOs
{
    public record LocationUpdateDto
    (
        [Required, MaxLength(200)] string Name,
        [Range(0, int.MaxValue)] int BudgetEur,
        [MaxLength(1000)] string? Notes,
        [Range(1, int.MaxValue)] int Version,
        int? Order
    );
}
