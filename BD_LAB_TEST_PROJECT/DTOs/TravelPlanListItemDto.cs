namespace BD_LAB_TEST_PROJECT.DTOs
{
    public record TravelPlanListItemDto
    (
        Guid Id, 
        string Title, 
        int BudgetEur, 
        int Version, 
        DateTimeOffset UpdatedAt
    );
}
