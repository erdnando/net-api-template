using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using netapi_template.Data;
using System.Threading.Tasks;
using Swashbuckle.AspNetCore.Annotations;

namespace netapi_template.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HealthController : ControllerBase
    {
        private readonly ApplicationDbContext _dbContext;

        public HealthController(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        /// <summary>
        /// Health check endpoint
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(object), 200)]
        [ProducesResponseType(typeof(object), 503)]
        [SwaggerOperation(Summary = "Health check endpoint", Description = "Returns API and DB health status.")]
        public async Task<IActionResult> Get()
        {
            // Simple DB check
            try
            {
                await _dbContext.Database.ExecuteSqlRawAsync("SELECT 1");
                return Ok(new { status = "Healthy", db = "Up" });
            }
            catch
            {
                return StatusCode(503, new { status = "Unhealthy", db = "Down" });
            }
        }
    }
}
