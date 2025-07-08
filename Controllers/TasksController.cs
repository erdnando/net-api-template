using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using netapi_template.DTOs;
using netapi_template.Services.Interfaces;
using netapi_template.Attributes;
using netapi_template.Models;

namespace netapi_template.Controllers;

/// <summary>
/// Controlador para gestión de tareas
/// </summary>
[Produces("application/json")]
public class TasksController : BaseController
{
    private readonly ITaskService _taskService;

    public TasksController(ITaskService taskService)
    {
        _taskService = taskService;
    }

    /// <summary>
    /// Obtiene todas las tareas
    /// </summary>
    /// <returns>Lista de tareas</returns>
    /// <response code="200">Tareas obtenidas exitosamente</response>
    /// <response code="400">Error en la petición</response>
    [Authorize]
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<TaskDto>>), 200)]
    [ProducesResponseType(400)]
    public async Task<IActionResult> GetAllTasks()
    {
        var response = await _taskService.GetAllTasksAsync();
        return HandleResponse(response);
    }

    /// <summary>
    /// Obtiene una tarea por su ID
    /// </summary>
    /// <param name="id">ID de la tarea</param>
    /// <returns>Tarea encontrada</returns>
    /// <response code="200">Tarea encontrada</response>
    /// <response code="400">Tarea no encontrada</response>
    [Authorize]
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ApiResponse<TaskDto>), 200)]
    [ProducesResponseType(400)]
    public async Task<IActionResult> GetTaskById([FromRoute] int id)
    {
        var response = await _taskService.GetTaskByIdAsync(id);
        return HandleResponse(response);
    }

    /// <summary>
    /// Obtiene tareas por ID de usuario
    /// </summary>
    /// <param name="userId">ID del usuario</param>
    /// <returns>Lista de tareas del usuario</returns>
    /// <response code="200">Tareas del usuario obtenidas exitosamente</response>
    /// <response code="400">Error en la petición</response>
    [Authorize]
    [HttpGet("user/{userId}")]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<TaskDto>>), 200)]
    [ProducesResponseType(400)]
    public async Task<IActionResult> GetTasksByUserId([FromRoute] int userId)
    {
        var response = await _taskService.GetTasksByUserIdAsync(userId);
        return HandleResponse(response);
    }

    /// <summary>
    /// Obtiene tareas completadas
    /// </summary>
    /// <returns>Lista de tareas completadas</returns>
    /// <response code="200">Tareas completadas obtenidas exitosamente</response>
    /// <response code="400">Error en la petición</response>
    [Authorize]
    [HttpGet("completed")]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<TaskDto>>), 200)]
    [ProducesResponseType(400)]
    public async Task<IActionResult> GetCompletedTasks()
    {
        var response = await _taskService.GetCompletedTasksAsync();
        return HandleResponse(response);
    }

    /// <summary>
    /// Obtiene tareas pendientes
    /// </summary>
    /// <returns>Lista de tareas pendientes</returns>
    /// <response code="200">Tareas pendientes obtenidas exitosamente</response>
    /// <response code="400">Error en la petición</response>
    [Authorize]
    [HttpGet("pending")]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<TaskDto>>), 200)]
    [ProducesResponseType(400)]
    public async Task<IActionResult> GetPendingTasks()
    {
        var response = await _taskService.GetPendingTasksAsync();
        return HandleResponse(response);
    }

    /// <summary>
    /// Crea una nueva tarea
    /// </summary>
    /// <param name="createTaskDto">Datos de la tarea a crear</param>
    /// <returns>Tarea creada</returns>
    /// <response code="200">Tarea creada exitosamente</response>
    /// <response code="400">Error en los datos proporcionados</response>
    [Authorize]
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<TaskDto>), 200)]
    [ProducesResponseType(400)]
    public async Task<IActionResult> CreateTask([FromBody] CreateTaskDto createTaskDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var response = await _taskService.CreateTaskAsync(createTaskDto);
        return HandleResponse(response);
    }

    /// <summary>
    /// Actualiza una tarea existente
    /// </summary>
    /// <param name="id">ID de la tarea a actualizar</param>
    /// <param name="updateTaskDto">Datos a actualizar</param>
    /// <returns>Tarea actualizada</returns>
    /// <response code="200">Tarea actualizada exitosamente</response>
    /// <response code="400">Error en los datos o tarea no encontrada</response>
    [Authorize]
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(ApiResponse<TaskDto>), 200)]
    [ProducesResponseType(400)]
    public async Task<IActionResult> UpdateTask([FromRoute] int id, [FromBody] UpdateTaskDto updateTaskDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var response = await _taskService.UpdateTaskAsync(id, updateTaskDto);
        return HandleResponse(response);
    }

    /// <summary>
    /// Elimina una tarea
    /// </summary>
    /// <param name="id">ID de la tarea a eliminar</param>
    /// <returns>Confirmación de eliminación</returns>
    /// <response code="200">Tarea eliminada exitosamente</response>
    /// <response code="400">Tarea no encontrada</response>
    [Authorize]
    [HttpDelete("{id}")]
    [ProducesResponseType(typeof(ApiResponse<bool>), 200)]
    [ProducesResponseType(400)]
    public async Task<IActionResult> DeleteTask([FromRoute] int id)
    {
        var response = await _taskService.DeleteTaskAsync(id);
        return HandleResponse(response);
    }
}
