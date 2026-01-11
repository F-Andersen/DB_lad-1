using Microsoft.AspNetCore.Mvc;

namespace BD_LAB_TEST_PROJECT.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LocationsController : ControllerBase
    {
        private readonly ITravelPlanRepository _plans;
        private readonly ILocationRepository _locations;
        public LocationsController(ITravelPlanRepository plans, ILocationRepository locations)
        {
            _plans = plans;
            _locations = locations;
        }

        [HttpPost("{planId:guid}/locations")]
        public async Task<ActionResult> Add(Guid planId, [FromBody] LocationCreateDto dto, CancellationToken ct)
        {
            if (!ModelState.IsValid) return ValidationProblem(ModelState);

            var plan = await _plans.GetAsync(planId, includeLocations: false, ct);

            if (plan is null) return NotFound();

            var (ok, id, error) = await _locations.AddAsync(
                planId,
                dto.Name,
                dto.BudgetEur,
                dto.Notes,
                ct
            );

            if (!ok)
            {
                return error switch
                {
                    "order_conflict" => Conflict(new { message = "Concurrent add caused order conflict. Retry later." }),
                    _ => StatusCode(500)
                };
            }
            return CreatedAtAction(nameof(TravelPlansController.Get), "TravelPlans", new { id = planId }, new { id });
        }

        [HttpPut("{id:guid}")]
        public async Task<ActionResult> Update(Guid id, [FromBody] LocationUpdateDto dto, CancellationToken ct)
        {
            if (!ModelState.IsValid) return ValidationProblem(ModelState);

            var (ok, error) = await _locations.UpdateAsync(
                id,
                dto.Name,
                dto.BudgetEur,
                dto.Notes,
                dto.Version,
                dto.Order,
                ct
            );

            if (!ok)
            {
                return error switch
                {
                    "not_found" => NotFound(),
                    "order_conflict" => Conflict(new { message = "Order conflict" }),
                    "version_conflict" => Conflict(new { message = "Version conflict" }),
                    _ => StatusCode(500)
                };
            }
            return Ok();
        }

        [HttpDelete("{id:guid}")]
        public async Task<ActionResult> Delete(Guid id, CancellationToken ct)
        {
            var result = await _locations.DeleteAsync(id, ct);

            return result ? NoContent() : NotFound();
        }
    }
}
