using netapi_template.DTOs;

namespace netapi_template.Services.Interfaces;

public interface ITaskService
{
    Task<ApiResponse<TaskDto>> GetTaskByIdAsync(int id);
    Task<ApiResponse<IEnumerable<TaskDto>>> GetAllTasksAsync();
    Task<ApiResponse<IEnumerable<TaskDto>>> GetTasksByUserIdAsync(int userId);
    Task<ApiResponse<TaskDto>> CreateTaskAsync(CreateTaskDto createTaskDto);
    Task<ApiResponse<TaskDto>> UpdateTaskAsync(int id, UpdateTaskDto updateTaskDto);
    Task<ApiResponse<bool>> DeleteTaskAsync(int id);
    Task<ApiResponse<IEnumerable<TaskDto>>> GetCompletedTasksAsync();
    Task<ApiResponse<IEnumerable<TaskDto>>> GetPendingTasksAsync();
}
