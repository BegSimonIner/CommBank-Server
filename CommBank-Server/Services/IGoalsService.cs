using CommBank.Models;
using MongoDB.Bson;

namespace CommBank.Services
{
    public interface IGoalsService
    {
        Task CreateAsync(Goal newGoal);
        Task<List<Goal>> GetAsync();
        Task<List<Goal>?> GetForUserAsync(ObjectId id);
        public Task<Goal?> GetAsync(ObjectId id);
        Task RemoveAsync(ObjectId id);
        Task UpdateAsync(ObjectId id, Goal updatedGoal);
        Task SeedDataAsync();
    }
}