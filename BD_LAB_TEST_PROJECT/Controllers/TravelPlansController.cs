using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BD_LAB_TEST_PROJECT.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TravelPlansController : ControllerBase
    {
        private readonly ITravelPlanRepository _plans;
        public TravelPlansController(ITravelPlanRepository plans)
        {
            _plans = plans;
        }
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 20, CancellationToken ct = default)
        {
            var result = await _plans.ListAsync(
                Math.Max(1, page),
                Math.Clamp(pageSize, 1, 100),
                ct);

            return Ok(result);
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> Get(Guid id, CancellationToken ct)
        {
            var plan = await _plans.GetAsync(id, includeLocations: true, ct);

            if (plan is null) return NotFound();

            var dto = new TravelPlanDetailsDto(
                plan.Id,
                plan.Title,
                plan.BudgetEur,
                plan.Version,
                plan.CreatedAt,
                plan.UpdatedAt,

                plan.Locations
                .OrderBy(l => l.Order)
                .Select(l => 
                    new LocationItemDto
                    (
                        l.Id,
                        l.Name,
                        l.Order,
                        l.BudgetEur,
                        l.Notes,
                        l.Version
                    )
                ).ToList()
            );

            return Ok(dto);
        }


        [HttpPost]
        public async Task<IActionResult> Create([FromBody] TravelPlanCreateDto dto, CancellationToken ct)
        {
            if (!ModelState.IsValid) return ValidationProblem(ModelState);

            var plan = new TravelPlan 
            { 
                Title = dto.Title.Trim(),
                BudgetEur = dto.BudgetEur
            };

            var id = await _plans.CreateAsync(plan, ct);

            Response.Headers.Location = $"/api/travel-plans/{id}";
            return StatusCode(StatusCodes.Status201Created, new { id });
        }


        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] TravelPlanUpdateDto dto, CancellationToken ct)
        {
            if (!ModelState.IsValid) return ValidationProblem(ModelState);

            try
            {
                var ok = await _plans.UpdateAsync(id, dto.Title, dto.BudgetEur, dto.Version, ct);
                if (!ok) return NotFound();
                return Ok();
            }
            catch (DbUpdateConcurrencyException)
            {
                return Conflict(new { message = "Version conflict" });
            }
        }


        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
        { 
            var result = await _plans.DeleteAsync(id, ct);

            return result ? NoContent() : NotFound();
        }
    }
}
