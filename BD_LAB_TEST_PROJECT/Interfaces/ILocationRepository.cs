namespace BD_LAB_TEST_PROJECT.Interfaces
{
    public interface ILocationRepository
    {
        Task<(bool ok, Guid? id, string? error)> AddAsync(Guid planId, string name, int budgetEur, string? notes, CancellationToken ct);
        Task<(bool ok, string? error)> UpdateAsync(Guid id, string name, int budgetEur, string? notes, int expectedVersion, int? newOrder, CancellationToken ct);
        Task<bool> DeleteAsync(Guid id, CancellationToken ct);
    }
}
