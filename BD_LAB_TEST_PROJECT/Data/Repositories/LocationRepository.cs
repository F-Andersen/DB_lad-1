using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace BD_LAB_TEST_PROJECT.Data.Repositories
{
    public class LocationRepository(ApplicationDbContext database) : ILocationRepository
    {

        private static bool IsUniqueOrderViolation(DbUpdateException ex)
        {
            if (ex.InnerException is PostgresException pg && pg.SqlState == PostgresErrorCodes.UniqueViolation)
            {
                return string.Equals(pg.ConstraintName, "IX_locations_TravelPlanId_Order", StringComparison.OrdinalIgnoreCase)
                    || string.Equals(pg.ConstraintName, "IX_Locations_TravelPlanId_Order", StringComparison.OrdinalIgnoreCase)
                    || (pg.Detail?.Contains("(\"TravelPlanId\", \"Order\")", StringComparison.Ordinal) == true);
            }
            return false;
        }

        public async Task<(bool ok, Guid? id, string? error)> AddAsync(
            Guid planId,
            string name,
            int budgetEur,
            string? notes,
            CancellationToken ct)
        {
            var planExists = await database.TravelPlans.AnyAsync(p => p.Id == planId, ct);

            if (!planExists) return (false, null, "not_found");

            const int maxRetries = 5;

            for (var attempt = 1; attempt <= maxRetries; attempt++)
            {
                await using var transaction = await database.Database.BeginTransactionAsync(ct);
                try
                {
                    var nextOrder = await database.Locations
                        .Where(l => l.TravelPlanId == planId)
                        .Select(l => (int?)l.Order)
                        .MaxAsync(ct) ?? 0;

                    nextOrder += 1;

                    var location = new Location
                    {
                        TravelPlanId = planId,
                        Name = name.Trim(),
                        BudgetEur = budgetEur,
                        Notes = string.IsNullOrWhiteSpace(notes) ? null : notes!.Trim(),
                        Order = nextOrder
                    };

                    database.Locations.Add(location);

                    await database.SaveChangesAsync(ct);

                    await database.TravelPlans
                        .Where(p => p.Id == planId)
                        .ExecuteUpdateAsync(s => s
                            .SetProperty(p => p.Version, p => p.Version + 1)
                            .SetProperty(p => p.UpdatedAt, _ => DateTimeOffset.UtcNow), 
                            ct);

                    await transaction.CommitAsync(ct);

                    return (true, location.Id, null);
                }
                catch (DbUpdateException ex) when (IsUniqueOrderViolation(ex))
                {
                    await transaction.RollbackAsync(ct);
                    if (attempt == maxRetries) return (false, null, "order_conflict");
                }
            }

            return (false, null, "error");
        }

        public async Task<(bool ok, string? error)> UpdateAsync(
            Guid id,
            string name,
            int budgetEur,
            string? notes,
            int expectedVersion,
            int? newOrder,
            CancellationToken ct)
        {
            var head = await database.Locations
                .Where(l => l.Id == id)
                .Select(l => new { l.TravelPlanId, l.Order })
                .FirstOrDefaultAsync(ct);

            if (head is null) return (false, "not_found");

            await using var transaction = await database.Database.BeginTransactionAsync(ct);

            try
            {
                var affectedRows = await database.Locations
                    .Where(l => l.Id == id && l.Version == expectedVersion)
                    .ExecuteUpdateAsync(s => s
                        .SetProperty(l => l.Name, _ => name.Trim())
                        .SetProperty(l => l.BudgetEur, _ => budgetEur)
                        .SetProperty(l => l.Notes, _ => string.IsNullOrWhiteSpace(notes) ? null : notes!.Trim())
                        .SetProperty(l => l.Version, l => l.Version + 1)
                        .SetProperty(l => l.UpdatedAt, _ => DateTimeOffset.UtcNow), ct);

                if (affectedRows == 0)
                    throw new DbUpdateConcurrencyException();

                var currentOrder = head.Order;
                var targetOrder = newOrder.HasValue ? Math.Max(1, newOrder.Value) : currentOrder;

                if (targetOrder != currentOrder)
                {
                    if (targetOrder < currentOrder)
                    {
                        await database.Locations
                            .Where(l => l.TravelPlanId == head.TravelPlanId &&
                                        l.Order >= targetOrder &&
                                        l.Order < currentOrder &&
                                        l.Id != id)
                            .ExecuteUpdateAsync(s => s.SetProperty(l => l.Order, l => l.Order + 1), ct);
                    }
                    else
                    {
                        await database.Locations
                            .Where(l => l.TravelPlanId == head.TravelPlanId &&
                                        l.Order <= targetOrder &&
                                        l.Order > currentOrder &&
                                        l.Id != id)
                            .ExecuteUpdateAsync(s => s.SetProperty(l => l.Order, l => l.Order - 1), ct);
                    }

                    var moved = await database.Locations
                        .Where(l => l.Id == id)
                        .ExecuteUpdateAsync(s => s.SetProperty(l => l.Order, _ => targetOrder), ct);

                    if (moved == 0)
                        throw new DbUpdateConcurrencyException();
                }

                await database.TravelPlans
                    .Where(p => p.Id == head.TravelPlanId)
                    .ExecuteUpdateAsync(s => s
                        .SetProperty(p => p.Version, p => p.Version + 1)
                        .SetProperty(p => p.UpdatedAt, _ => DateTimeOffset.UtcNow), ct);

                await transaction.CommitAsync(ct);

                return (true, null);
            }
            catch (DbUpdateException ex) when (IsUniqueOrderViolation(ex))
            {
                await transaction.RollbackAsync(ct);
                return (false, "order_conflict");
            }
            catch (DbUpdateConcurrencyException)
            {
                await transaction.RollbackAsync(ct);
                return (false, "version_conflict");
            }
        }

        public async Task<bool> DeleteAsync(Guid id, CancellationToken ct)
        {
            var head = await database.Locations
                .Where(l => l.Id == id)
                .Select(l => new { l.TravelPlanId })
                .FirstOrDefaultAsync(ct);

            if (head is null) return false;

            await using var transaction = await database.Database.BeginTransactionAsync(ct);

            var affectedRows = await database.Locations
                .Where(l => l.Id == id)
                .ExecuteDeleteAsync(ct);

            if (affectedRows == 0)
            {
                await transaction.RollbackAsync(ct);
                return false;
            }

            await database.TravelPlans
                .Where(p => p.Id == head.TravelPlanId)
                .ExecuteUpdateAsync(s => s
                    .SetProperty(p => p.Version, p => p.Version + 1)
                    .SetProperty(p => p.UpdatedAt, _ => DateTimeOffset.UtcNow), ct);

            await transaction.CommitAsync(ct);
            return true;
        }
    }
}
