using Microsoft.AspNetCore.Mvc;
using CommBank.Services;
using CommBank.Models;
using MongoDB.Bson;

namespace CommBank.Controllers;

[ApiController]
[Route("api/[controller]")]
public class GoalController : ControllerBase
{
    private readonly IGoalsService _goalsService;
    private readonly IUsersService _usersService;

    public GoalController(IGoalsService goalsService, IUsersService usersService)
    {
        _goalsService = goalsService;
        _usersService = usersService;
    }

    [HttpGet]
    public async Task<List<Goal>> Get() =>
        await _goalsService.GetAsync();

    [HttpGet("{id}")]
    public async Task<ActionResult<Goal>> Get(string id)
    {
        try
        {
            // Check if ID has correct format
            if (!ObjectId.TryParse(id, out var objectId))
            {
                return BadRequest($"Invalid ID format: {id}");
            }

            // Try to get the goal
            var goal = await _goalsService.GetAsync(objectId);

            if (goal is null)
            {
                // Get all goals to see what we have in the database
                var allGoals = await _goalsService.GetAsync();
                var availableIds = string.Join(", ", allGoals.Select(g => g.Id));
                
                return NotFound($"Goal with ID {id} not found. Available IDs: {availableIds}");
            }

            return Ok(goal);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error: {ex.Message}");
        }
    }


    [HttpGet("User/{id:length(24)}")]
    public async Task<ActionResult<List<Goal>>> GetForUser(string id)
    {
        if (!ObjectId.TryParse(id, out var objectId)) 
            return BadRequest("Invalid User ID format"); 

        var goals = await _goalsService.GetForUserAsync(objectId);

        if (goals == null || goals.Count == 0)
            return NotFound("No goals found for this user."); // 

        return Ok(goals); // 
    }


    [HttpPost]
    public async Task<IActionResult> Post(Goal newGoal)
    {
        await _goalsService.CreateAsync(newGoal);

        if (newGoal.Id is not null && newGoal.UserId is not null)
        {
            if (!ObjectId.TryParse(newGoal.UserId, out var userId)) 
                return BadRequest("Invalid User ID format");

            var user = await _usersService.GetAsync(userId.ToString());

            if (user is not null && user.Id is not null)
            {
                if (user.GoalIds is not null)
                {
                    user.GoalIds.Add(newGoal.Id);
                }
                else
                {
                    user.GoalIds = new() { newGoal.Id };
                }

                await _usersService.UpdateAsync(user.Id, user);
            }
        }

        return CreatedAtAction(nameof(Get), new { id = newGoal.Id }, newGoal);
    }

    [HttpPut("{id:length(24)}")]
    public async Task<IActionResult> Update(string id, Goal updatedGoal)
    {
        if (!ObjectId.TryParse(id, out var objectId)) // 
            return BadRequest("Invalid ID format");

        var goal = await _goalsService.GetAsync(objectId); // 

        if (goal is null)
        {
            return NotFound();
        }

        updatedGoal.Id = goal.Id;

        await _goalsService.UpdateAsync(objectId, updatedGoal); // 

        return NoContent();
    }

    [HttpDelete("{id:length(24)}")]
    public async Task<IActionResult> Delete(string id)
    {
        if (!ObjectId.TryParse(id, out var objectId)) // 
            return BadRequest("Invalid ID format");

        var goal = await _goalsService.GetAsync(objectId); // 

        if (goal is null)
        {
            return NotFound();
        }

        await _goalsService.RemoveAsync(objectId); // 

        return NoContent();
    }
}
