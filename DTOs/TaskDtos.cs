using System.ComponentModel.DataAnnotations;

namespace netapi_template.DTOs;

public record CreateTaskDto(
    [Required][StringLength(200)] string Title,
    [StringLength(1000)] string? Description,
    [Required][StringLength(10)] string Priority, // low, medium, high
    int UserId
);

public record UpdateTaskDto(
    [StringLength(200)] string? Title,
    [StringLength(1000)] string? Description,
    bool? Completed,
    [StringLength(10)] string? Priority
);

public class TaskDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public bool Completed { get; set; }
    public string Priority { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public int UserId { get; set; }
    public string? UserName { get; set; }
    
    public TaskDto() { }
    
    public TaskDto(int id, string title, string description, bool completed, string priority, DateTime createdAt, DateTime updatedAt, int userId, string? userName)
    {
        Id = id;
        Title = title;
        Description = description;
        Completed = completed;
        Priority = priority;
        CreatedAt = createdAt;
        UpdatedAt = updatedAt;
        UserId = userId;
        UserName = userName;
    }
}
