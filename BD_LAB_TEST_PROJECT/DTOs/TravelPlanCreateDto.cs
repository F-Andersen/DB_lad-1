using System.ComponentModel.DataAnnotations;

namespace BD_LAB_TEST_PROJECT.DTOs
{
    public record TravelPlanCreateDto
    (
        [Required, MaxLength(200)] string Title,
        [Range(0, int.MaxValue)] int BudgetEur
    );   
}
