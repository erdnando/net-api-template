using netapi_template.Models;

namespace netapi_template.Repositories.Interfaces;

public interface ITaskRepository : IRepository<TaskItem>
{
    Task<IEnumerable<TaskItem>> GetTasksByUserIdAsync(int userId);
    Task<IEnumerable<TaskItem>> GetCompletedTasksAsync();
    Task<IEnumerable<TaskItem>> GetPendingTasksAsync();
    Task<IEnumerable<TaskItem>> GetTasksByPriorityAsync(string priority);
}
