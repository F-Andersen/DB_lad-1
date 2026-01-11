using Microsoft.EntityFrameworkCore;
using System;
using BD_LAB_TEST_PROJECT.DTOs;
using BD_LAB_TEST_PROJECT.Interfaces;

namespace BD_LAB_TEST_PROJECT.Data.Repositories
{
    public class TravelPlanRepository(ApplicationDbContext database) : ITravelPlanRepository
    {
        public async Task<IReadOnlyList<TravelPlanListItemDto>> ListAsync(int page, int pageSize, CancellationToken ct)
        {
            return await database.TravelPlans
                .AsNoTracking()
                .OrderByDescending(p => p.UpdatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(p => new TravelPlanListItemDto(p.Id, p.Title, p.BudgetEur, p.Version, p.UpdatedAt))
                .ToListAsync(ct);
        }


        public async Task<TravelPlan?> GetAsync(Guid id, bool includeLocations, CancellationToken ct)
        {
            var query = database.TravelPlans
                .AsQueryable()
                .AsNoTracking();

            if (includeLocations) 
            { 
                query = query
                    .Include(p => p.Locations
                    .OrderBy(l => l.Order)); 
            }

            return await query.FirstOrDefaultAsync(p => p.Id == id, ct);
        }


        public async Task<Guid> CreateAsync(TravelPlan plan, CancellationToken ct)
        {
            database.TravelPlans.Add(plan);
            await database.SaveChangesAsync(ct);
            return plan.Id;
        }


        public async Task<bool> UpdateAsync(Guid id, string title, int budgetEur, int expectedVersion, CancellationToken ct)
        {
            var now = DateTimeOffset.UtcNow;

            var affectedRows = await database.TravelPlans
                .ExecuteUpdateAsync(x => x
                    .SetProperty(p => p.Title, title)
                    .SetProperty(p => p.BudgetEur, budgetEur)
                    .SetProperty(p => p.Version, p => p.Version + 1)
                    .SetProperty(p => p.UpdatedAt, now),
                    ct
                );

            if(affectedRows == 0) throw new InvalidOperationException("Failed to update the travel plan.");

            await database.SaveChangesAsync(ct);
            return true;
        }


        public async Task<bool> DeleteAsync(Guid id, CancellationToken ct)
        {
            var affectedRows = await database.TravelPlans
                .Where(p => p.Id == id)
                .ExecuteDeleteAsync(ct);

            if (affectedRows == 0) throw new DbUpdateConcurrencyException("Failed to update the travel plan.");

            await database.SaveChangesAsync(ct);

            return true;
        }
    }
}
