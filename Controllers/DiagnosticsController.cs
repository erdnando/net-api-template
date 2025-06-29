using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using netapi_template.Data;
using netapi_template.Models;

namespace netapi_template.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DiagnosticsController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<DiagnosticsController> _logger;

    public DiagnosticsController(ApplicationDbContext context, ILogger<DiagnosticsController> logger)
    {
        _context = context;
        _logger = logger;
    }

    [HttpGet("database-state")]
    public async Task<IActionResult> GetDatabaseState()
    {
        try
        {
            var result = new
            {
                DatabaseConnection = await _context.Database.CanConnectAsync(),
                Users = new
                {
                    Total = await _context.Users.CountAsync(),
                    Active = await _context.Users.CountAsync(u => u.Status == UserStatus.Active),
                    ByStatus = await _context.Users
                        .GroupBy(u => u.Status)
                        .Select(g => new { Status = g.Key.ToString(), Count = g.Count() })
                        .ToListAsync(),
                    NotDeleted = await _context.Users.CountAsync(u => !u.IsDeleted)
                },
                PasswordResetTokens = new
                {
                    Total = await _context.PasswordResetTokens.CountAsync(),
                    Last24Hours = await _context.PasswordResetTokens
                        .CountAsync(t => t.CreatedAt > DateTime.UtcNow.AddDays(-1)),
                    Expired = await _context.PasswordResetTokens
                        .CountAsync(t => t.ExpiresAt < DateTime.UtcNow),
                    Valid = await _context.PasswordResetTokens
                        .CountAsync(t => t.ExpiresAt > DateTime.UtcNow && !t.UsedAt.HasValue),
                    Recent = await _context.PasswordResetTokens
                        .OrderByDescending(t => t.CreatedAt)
                        .Take(5)
                        .Select(t => new { 
                            t.Id, 
                            t.UserId, 
                            UserEmail = t.User!.Email,
                            t.CreatedAt, 
                            t.ExpiresAt, 
                            t.UsedAt 
                        })
                        .ToListAsync()
                },
                Roles = await _context.Roles
                    .Where(r => !r.IsDeleted)
                    .Select(r => new { r.Id, r.Name, UserCount = r.Users.Count })
                    .ToListAsync()
            };

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting database state");
            return StatusCode(500, new { Error = ex.Message });
        }
    }

    [HttpPost("create-test-reset-token")]
    public async Task<IActionResult> CreateTestResetToken([FromBody] CreateTestTokenRequest request)
    {
        try
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email);
            if (user == null)
            {
                return NotFound(new { Error = "User not found" });
            }

            var token = new PasswordResetToken
            {
                Token = Guid.NewGuid().ToString("N"),
                UserId = user.Id,
                ExpiresAt = DateTime.UtcNow.AddHours(1),
                CreatedAt = DateTime.UtcNow
            };

            _context.PasswordResetTokens.Add(token);
            await _context.SaveChangesAsync();

            return Ok(new { 
                Message = "Test token created successfully",
                TokenId = token.Id,
                UserId = user.Id,
                UserEmail = user.Email,
                ExpiresAt = token.ExpiresAt
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating test reset token");
            return StatusCode(500, new { Error = ex.Message });
        }
    }

    [HttpGet("test-utils-endpoints")]
    public async Task<IActionResult> TestUtilsEndpoints()
    {
        try
        {
            // We'll simulate what the UtilsService methods do to test them directly
            var utilsService = HttpContext.RequestServices.GetRequiredService<Services.Interfaces.IUtilsService>();
            
            var results = new
            {
                SystemHealth = await utilsService.GetSystemHealthAsync(),
                PasswordResetStats = await utilsService.GetPasswordResetStatsAsync(),
                UserExists_ValidEmail = await utilsService.UserExistsAsync("erdnando@gmail.com"),
                UserExists_InvalidEmail = await utilsService.UserExistsAsync("nonexistent@test.com"),
                IsAdmin_AdminUser = await utilsService.IsAdminUserAsync("admin@sistema.com"),
                IsAdmin_RegularUser = await utilsService.IsAdminUserAsync("erdnando@gmail.com"),
                SystemConfig = await utilsService.GetSystemConfigAsync()
            };

            return Ok(new { 
                Message = "Utils service methods tested directly", 
                Results = results,
                TestTime = DateTime.UtcNow 
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error testing utils endpoints");
            return StatusCode(500, new { Error = ex.Message, StackTrace = ex.StackTrace });
        }
    }

    [HttpGet("debug-password-reset-stats")]
    public async Task<IActionResult> DebugPasswordResetStats()
    {
        try
        {
            var maxDaily = 3; // Default from config
            
            // Step 1: Get raw tokens from last 24h
            var rawTokens = await _context.PasswordResetTokens
                .Where(t => t.CreatedAt > DateTime.UtcNow.AddDays(-1))
                .Select(t => new { 
                    t.Id, 
                    t.UserId, 
                    t.CreatedAt,
                    t.ExpiresAt 
                })
                .ToListAsync();

            // Step 2: Try with Include
            var tokensWithUser = await _context.PasswordResetTokens
                .Include(t => t.User)
                .Where(t => t.CreatedAt > DateTime.UtcNow.AddDays(-1))
                .Select(t => new { 
                    t.Id, 
                    t.UserId, 
                    t.CreatedAt,
                    UserEmail = t.User!.Email
                })
                .ToListAsync();

            // Step 3: Try without Include but join manually
            var tokensWithUserManual = await _context.PasswordResetTokens
                .Where(t => t.CreatedAt > DateTime.UtcNow.AddDays(-1))
                .Join(_context.Users, 
                    t => t.UserId, 
                    u => u.Id, 
                    (t, u) => new { 
                        t.Id, 
                        t.UserId, 
                        t.CreatedAt, 
                        UserEmail = u.Email 
                    })
                .ToListAsync();

            // Step 4: Try the original grouping query step by step
            var groupedQuery = _context.PasswordResetTokens
                .Include(t => t.User)
                .Where(t => t.CreatedAt > DateTime.UtcNow.AddDays(-1))
                .GroupBy(t => new { t.User!.Email, t.UserId });

            var groupedResults = await groupedQuery
                .Select(g => new {
                    Email = g.Key.Email,
                    UserId = g.Key.UserId,
                    Count = g.Count(),
                    LastAttempt = g.Max(t => t.CreatedAt)
                })
                .ToListAsync();

            return Ok(new {
                RawTokensCount = rawTokens.Count,
                RawTokens = rawTokens,
                
                TokensWithUserCount = tokensWithUser.Count,
                TokensWithUser = tokensWithUser,
                
                TokensWithUserManualCount = tokensWithUserManual.Count,
                TokensWithUserManual = tokensWithUserManual,
                
                GroupedResultsCount = groupedResults.Count,
                GroupedResults = groupedResults,
                
                DebugInfo = new {
                    CurrentTime = DateTime.UtcNow,
                    Last24Hours = DateTime.UtcNow.AddDays(-1),
                    MaxDaily = maxDaily
                }
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error debugging password reset stats");
            return StatusCode(500, new { 
                Error = ex.Message, 
                StackTrace = ex.StackTrace?.Split('\n').Take(10).ToArray()
            });
        }
    }

    [HttpGet("debug-time-comparison")]
    public async Task<IActionResult> DebugTimeComparison()
    {
        try
        {
            var now = DateTime.UtcNow;
            var yesterday = now.AddDays(-1);
            
            // Get all tokens (without calculated fields that can't be translated to SQL)
            var allTokensRaw = await _context.PasswordResetTokens
                .Select(t => new { 
                    t.Id, 
                    t.CreatedAt
                })
                .ToListAsync();

            // Calculate in memory
            var allTokens = allTokensRaw.Select(t => new {
                t.Id,
                t.CreatedAt,
                IsWithin24Hours = t.CreatedAt > yesterday,
                MinutesAgo = (int)(now - t.CreatedAt).TotalMinutes
            }).ToList();

            // Test the exact query from stats
            var statsTokens = await _context.PasswordResetTokens
                .Where(t => t.CreatedAt > yesterday)
                .Select(t => new { 
                    t.Id, 
                    t.CreatedAt, 
                    t.UserId 
                })
                .ToListAsync();

            return Ok(new {
                CurrentUtcTime = now,
                Yesterday = yesterday,
                AllTokens = allTokens,
                StatsTokensFound = statsTokens.Count,
                StatsTokens = statsTokens
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { Error = ex.Message, StackTrace = ex.StackTrace?.Split('\n').Take(5).ToArray() });
        }
    }
}

public class CreateTestTokenRequest
{
    public string Email { get; set; } = string.Empty;
}
