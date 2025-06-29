using Microsoft.EntityFrameworkCore;
using netapi_template.Data;
using netapi_template.Models;
using netapi_template.Repositories.Interfaces;

namespace netapi_template.Repositories;

public class TaskRepository : Repository<TaskItem>, ITaskRepository
{
    public TaskRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<TaskItem>> GetTasksByUserIdAsync(int userId)
    {
        return await _dbSet
            .Where(t => t.UserId == userId)
            .Include(t => t.User)
            .OrderByDescending(t => t.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<TaskItem>> GetCompletedTasksAsync()
    {
        return await _dbSet
            .Where(t => t.Completed)
            .Include(t => t.User)
            .OrderByDescending(t => t.UpdatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<TaskItem>> GetPendingTasksAsync()
    {
        return await _dbSet
            .Where(t => !t.Completed)
            .Include(t => t.User)
            .OrderBy(t => t.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<TaskItem>> GetTasksByPriorityAsync(string priority)
    {
        return await _dbSet
            .Where(t => t.Priority == priority)
            .Include(t => t.User)
            .OrderByDescending(t => t.CreatedAt)
            .ToListAsync();
    }
}
