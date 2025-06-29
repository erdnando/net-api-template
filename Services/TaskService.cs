using AutoMapper;
using netapi_template.DTOs;
using netapi_template.Models;
using netapi_template.Repositories.Interfaces;
using netapi_template.Services.Interfaces;

namespace netapi_template.Services;

public class TaskService : ITaskService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILogger<TaskService> _logger;

    public TaskService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<TaskService> logger)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<ApiResponse<TaskDto>> GetTaskByIdAsync(int id)
    {
        try
        {
            var task = await _unitOfWork.Tasks.GetByIdAsync(id);
            if (task == null)
            {
                return new ApiResponse<TaskDto>(false, "Tarea no encontrada");
            }

            var taskDto = _mapper.Map<TaskDto>(task);
            return new ApiResponse<TaskDto>(true, "Tarea encontrada", taskDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener tarea con ID {TaskId}", id);
            return new ApiResponse<TaskDto>(false, "Error interno del servidor");
        }
    }

    public async Task<ApiResponse<IEnumerable<TaskDto>>> GetAllTasksAsync()
    {
        try
        {
            var tasks = await _unitOfWork.Tasks.GetAllAsync();
            var taskDtos = _mapper.Map<IEnumerable<TaskDto>>(tasks);
            return new ApiResponse<IEnumerable<TaskDto>>(true, "Tareas obtenidas exitosamente", taskDtos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener todas las tareas");
            return new ApiResponse<IEnumerable<TaskDto>>(false, "Error interno del servidor");
        }
    }

    public async Task<ApiResponse<IEnumerable<TaskDto>>> GetTasksByUserIdAsync(int userId)
    {
        try
        {
            var tasks = await _unitOfWork.Tasks.GetTasksByUserIdAsync(userId);
            var taskDtos = _mapper.Map<IEnumerable<TaskDto>>(tasks);
            return new ApiResponse<IEnumerable<TaskDto>>(true, "Tareas del usuario obtenidas exitosamente", taskDtos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener tareas del usuario {UserId}", userId);
            return new ApiResponse<IEnumerable<TaskDto>>(false, "Error interno del servidor");
        }
    }

    public async Task<ApiResponse<TaskDto>> CreateTaskAsync(CreateTaskDto createTaskDto)
    {
        try
        {
            // Verificar que el usuario existe
            var user = await _unitOfWork.Users.GetByIdAsync(createTaskDto.UserId);
            if (user == null)
            {
                return new ApiResponse<TaskDto>(false, "Usuario no encontrado");
            }

            var task = _mapper.Map<TaskItem>(createTaskDto);
            await _unitOfWork.Tasks.AddAsync(task);
            await _unitOfWork.SaveChangesAsync();

            var taskDto = _mapper.Map<TaskDto>(task);
            return new ApiResponse<TaskDto>(true, "Tarea creada exitosamente", taskDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al crear tarea");
            return new ApiResponse<TaskDto>(false, "Error interno del servidor");
        }
    }

    public async Task<ApiResponse<TaskDto>> UpdateTaskAsync(int id, UpdateTaskDto updateTaskDto)
    {
        try
        {
            var task = await _unitOfWork.Tasks.GetByIdAsync(id);
            if (task == null)
            {
                return new ApiResponse<TaskDto>(false, "Tarea no encontrada");
            }

            _mapper.Map(updateTaskDto, task);
            task.UpdatedAt = DateTime.UtcNow;
            _unitOfWork.Tasks.Update(task);
            await _unitOfWork.SaveChangesAsync();

            var taskDto = _mapper.Map<TaskDto>(task);
            return new ApiResponse<TaskDto>(true, "Tarea actualizada exitosamente", taskDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar tarea con ID {TaskId}", id);
            return new ApiResponse<TaskDto>(false, "Error interno del servidor");
        }
    }

    public async Task<ApiResponse<bool>> DeleteTaskAsync(int id)
    {
        try
        {
            var task = await _unitOfWork.Tasks.GetByIdAsync(id);
            if (task == null)
            {
                return new ApiResponse<bool>(false, "Tarea no encontrada");
            }

            _unitOfWork.Tasks.Remove(task);
            await _unitOfWork.SaveChangesAsync();

            return new ApiResponse<bool>(true, "Tarea eliminada exitosamente", true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al eliminar tarea con ID {TaskId}", id);
            return new ApiResponse<bool>(false, "Error interno del servidor");
        }
    }

    public async Task<ApiResponse<IEnumerable<TaskDto>>> GetCompletedTasksAsync()
    {
        try
        {
            var tasks = await _unitOfWork.Tasks.GetCompletedTasksAsync();
            var taskDtos = _mapper.Map<IEnumerable<TaskDto>>(tasks);
            return new ApiResponse<IEnumerable<TaskDto>>(true, "Tareas completadas obtenidas exitosamente", taskDtos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener tareas completadas");
            return new ApiResponse<IEnumerable<TaskDto>>(false, "Error interno del servidor");
        }
    }

    public async Task<ApiResponse<IEnumerable<TaskDto>>> GetPendingTasksAsync()
    {
        try
        {
            var tasks = await _unitOfWork.Tasks.GetPendingTasksAsync();
            var taskDtos = _mapper.Map<IEnumerable<TaskDto>>(tasks);
            return new ApiResponse<IEnumerable<TaskDto>>(true, "Tareas pendientes obtenidas exitosamente", taskDtos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener tareas pendientes");
            return new ApiResponse<IEnumerable<TaskDto>>(false, "Error interno del servidor");
        }
    }
}
