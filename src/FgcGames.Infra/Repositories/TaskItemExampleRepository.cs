using FgcGames.Domain.Entities;
using FgcGames.Domain.Interfaces.Repositories;
using FgcGames.Infra.Database;
using Microsoft.EntityFrameworkCore;

namespace FgcGames.Infra.Repositories;

public class TaskItemExampleRepository(FgcGamesContext dbContext) : ITaskItemExampleRepository
{
    private readonly FgcGamesContext _dbContext = dbContext;

    public void Add(TaskItemExample taskItem)
        => _dbContext.Set<TaskItemExample>().Add(taskItem);

    public void Update(TaskItemExample taskItem)
        => _dbContext.Set<TaskItemExample>().Update(taskItem);

    public void Delete(TaskItemExample taskItem)
        => _dbContext.Set<TaskItemExample>().Remove(taskItem);

    public async Task<TaskItemExample?> GetByIdAsync(int id)
        => await _dbContext.Set<TaskItemExample>()
            .FirstOrDefaultAsync(x => x.Id == id);

    public async Task<IReadOnlyList<TaskItemExample>> GetAllAsync() 
        => await _dbContext.Set<TaskItemExample>()
            .AsNoTracking()
            .ToListAsync();

    public async Task<bool> ExistsByTitleAsync(string title)
        => await _dbContext.Set<TaskItemExample>()
            .AnyAsync(x => x.Title.ToUpper() == title.ToUpper());

    public async Task<int> SaveChangesAsync() 
        => await _dbContext.SaveChangesAsync();
}
