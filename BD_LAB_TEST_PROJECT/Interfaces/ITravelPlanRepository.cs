using BD_LAB_TEST_PROJECT.DTOs;

namespace BD_LAB_TEST_PROJECT.Interfaces
{
    public interface ITravelPlanRepository
    {
        Task<IReadOnlyList<TravelPlanListItemDto>> ListAsync(int page, int pageSize, CancellationToken ct);
        Task<TravelPlan?> GetAsync(Guid id, bool includeLocations, CancellationToken ct);
        Task<Guid> CreateAsync(TravelPlan plan, CancellationToken ct);
        Task<bool> UpdateAsync(Guid id, string title, int budgetEur, int expectedVersion, CancellationToken ct);
        Task<bool> DeleteAsync(Guid id, CancellationToken ct);
    }
}
