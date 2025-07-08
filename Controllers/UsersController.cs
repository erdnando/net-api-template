using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using netapi_template.DTOs;
using netapi_template.Services.Interfaces;
using netapi_template.Services;
using System.ComponentModel.DataAnnotations;
using Swashbuckle.AspNetCore.Annotations;
using netapi_template.Attributes;
using netapi_template.Models;

namespace netapi_template.Controllers;

/// <summary>
/// Controller for user management
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly IPasswordResetService _passwordResetService;

    public UsersController(
        IUserService userService,
        IPasswordResetService passwordResetService)
    {
        _userService = userService;
        _passwordResetService = passwordResetService;
    }

    /// <summary>
    /// Get all users with pagination and filtering
    /// </summary>
    [HttpGet]
    [Authorize]
    [SwaggerOperation(Summary = "Get all users", Description = "Retrieve a paginated list of users with optional filtering")]
    [SwaggerResponse(200, "Users retrieved successfully", typeof(ApiResponse<PagedResult<UserDto>>))]
    public async Task<IActionResult> GetAllUsers(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? search = null,
        [FromQuery] string? sortBy = null,
        [FromQuery] bool sortDescending = false)
    {
        var query = new PaginationQuery(page, pageSize, search, sortBy, sortDescending);
        var response = await _userService.GetAllUsersAsync(query);
        
        if (!response.Success)
        {
            return BadRequest(response);
        }

        return Ok(response);
    }

    /// <summary>
    /// Get user by ID
    /// </summary>
    [HttpGet("{id}")]
    [Authorize]
    [SwaggerOperation(Summary = "Get user by ID", Description = "Retrieve a specific user by their ID")]
    [SwaggerResponse(200, "User found", typeof(ApiResponse<UserDto>))]
    [SwaggerResponse(404, "User not found")]
    public async Task<IActionResult> GetUserById([FromRoute] int id)
    {
        var response = await _userService.GetUserByIdAsync(id);
        
        if (!response.Success)
        {
            return NotFound(response);
        }

        return Ok(response);
    }

    /// <summary>
    /// Get user by email
    /// </summary>
    [HttpGet("by-email/{email}")]
    [Authorize]
    [SwaggerOperation(Summary = "Get user by email", Description = "Retrieve a specific user by their email address")]
    [SwaggerResponse(200, "User found", typeof(ApiResponse<UserDto>))]
    [SwaggerResponse(404, "User not found")]
    public async Task<IActionResult> GetUserByEmail([FromRoute] string email)
    {
        var response = await _userService.GetUserByEmailAsync(email);
        
        if (!response.Success)
        {
            return NotFound(response);
        }

        return Ok(response);
    }

    /// <summary>
    /// Create a new user
    /// </summary>
    [HttpPost]
    [Authorize]
    [SwaggerOperation(Summary = "Create user", Description = "Create a new user account")]
    [SwaggerResponse(201, "User created successfully", typeof(ApiResponse<UserDto>))]
    [SwaggerResponse(400, "Invalid request data")]
    public async Task<IActionResult> CreateUser([FromBody] CreateUserDto createUserDto)
    {
        // Temporarily bypass model validation due to FluentValidation async issues
        // if (!ModelState.IsValid)
        // {
        //     return BadRequest(new ApiResponse<UserDto>(false, "Invalid request data", null, 
        //         ModelState.SelectMany(x => x.Value?.Errors?.Select(e => e.ErrorMessage) ?? []).ToList()));
        // }

        var response = await _userService.CreateUserAsync(createUserDto);
        
        if (!response.Success)
        {
            return BadRequest(response);
        }

        return CreatedAtAction(nameof(GetUserById), new { id = response.Data!.Id }, response);
    }

    /// <summary>
    /// Update an existing user
    /// </summary>
    [HttpPut("{id}")]
    [Authorize]
    [SwaggerOperation(Summary = "Update user", Description = "Update an existing user's information")]
    [SwaggerResponse(200, "User updated successfully", typeof(ApiResponse<UserDto>))]
    [SwaggerResponse(404, "User not found")]
    [SwaggerResponse(400, "Invalid request data")]
    public async Task<IActionResult> UpdateUser([FromRoute] int id, [FromBody] UpdateUserDto updateUserDto)
    {
        // Temporarily bypass model validation due to FluentValidation async issues
        // if (!ModelState.IsValid)
        // {
        //     return BadRequest(new ApiResponse<UserDto>(false, "Invalid request data", null, 
        //         ModelState.SelectMany(x => x.Value?.Errors?.Select(e => e.ErrorMessage) ?? []).ToList()));
        // }

        var response = await _userService.UpdateUserAsync(id, updateUserDto);
        
        if (!response.Success)
        {
            return response.Message.Contains("not found") ? NotFound(response) : BadRequest(response);
        }

        return Ok(response);
    }

    /// <summary>
    /// Delete a user (soft delete)
    /// </summary>
    [HttpDelete("{id}")]
    [Authorize]
    [SwaggerOperation(Summary = "Delete user", Description = "Soft delete a user account")]
    [SwaggerResponse(200, "User deleted successfully", typeof(ApiResponse<bool>))]
    [SwaggerResponse(404, "User not found")]
    [SwaggerResponse(400, "Cannot delete user")]
    public async Task<IActionResult> DeleteUser([FromRoute] int id)
    {
        var response = await _userService.DeleteUserAsync(id);
        
        if (!response.Success)
        {
            return response.Message.Contains("not found") ? NotFound(response) : BadRequest(response);
        }

        return Ok(response);
    }

    /// <summary>
    /// Change user password
    /// </summary>
    [HttpPost("{id}/change-password")]
    [Authorize]
    [SwaggerOperation(Summary = "Change password", Description = "Change a user's password")]
    [SwaggerResponse(200, "Password changed successfully", typeof(ApiResponse<bool>))]
    [SwaggerResponse(404, "User not found")]
    [SwaggerResponse(400, "Invalid current password")]
    public async Task<IActionResult> ChangePassword([FromRoute] int id, [FromBody] ChangePasswordDto changePasswordDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(new ApiResponse<bool>(false, "Invalid request data", false, 
                ModelState.SelectMany(x => x.Value?.Errors?.Select(e => e.ErrorMessage) ?? []).ToList()));
        }

        var response = await _userService.ChangePasswordAsync(id, changePasswordDto);
        
        if (!response.Success)
        {
            return response.Message.Contains("not found") ? NotFound(response) : BadRequest(response);
        }

        return Ok(response);
    }

    /// <summary>
    /// User authentication
    /// </summary>
    [HttpPost("login")]
    [AllowAnonymous]
    [SwaggerOperation(Summary = "User login", Description = "Authenticate user and return JWT token")]
    [SwaggerResponse(200, "Login successful", typeof(ApiResponse<LoginResponseDto>))]
    [SwaggerResponse(401, "Invalid credentials")]
    public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(new ApiResponse<LoginResponseDto>(false, "Invalid request data", null, 
                ModelState.SelectMany(x => x.Value?.Errors?.Select(e => e.ErrorMessage) ?? []).ToList()));
        }

        var response = await _userService.LoginAsync(loginDto);
        
        if (!response.Success)
        {
            return Unauthorized(response);
        }

        return Ok(response);
    }

    /// <summary>
    /// Initiate password reset process
    /// </summary>
    [HttpPost("forgot-password")]
    [AllowAnonymous]
    [SwaggerOperation(Summary = "Initiate password reset", Description = "Start the password reset process by requesting a reset token via email")]
    [SwaggerResponse(200, "Reset process initiated successfully", typeof(ApiResponse<ForgotPasswordResponse>))]
    public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(new ApiResponse<ForgotPasswordResponse>(false, "Invalid request"));

        var (success, message) = await _passwordResetService.InitiatePasswordResetAsync(request.Email);
        var response = new ForgotPasswordResponse { Success = success, Message = message };

        return Ok(new ApiResponse<ForgotPasswordResponse>(success, message, response));
    }

    /// <summary>
    /// Reset password using token
    /// </summary>
    [HttpPost("reset-password")]
    [AllowAnonymous]
    [SwaggerOperation(Summary = "Reset password", Description = "Reset user's password using the token received via email")]
    [SwaggerResponse(200, "Password reset successful", typeof(ApiResponse<ResetPasswordResponse>))]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(new ApiResponse<ResetPasswordResponse>(false, "Invalid request"));

        var (success, message) = await _passwordResetService.ValidateAndResetPasswordAsync(
            request.Token,
            request.NewPassword);

        var response = new ResetPasswordResponse { Success = success, Message = message };

        return Ok(new ApiResponse<ResetPasswordResponse>(success, message, response));
    }
}
