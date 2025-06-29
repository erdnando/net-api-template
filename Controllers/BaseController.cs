using Microsoft.AspNetCore.Mvc;

namespace netapi_template.Controllers;

[ApiController]
[Route("api/[controller]")]
public abstract class BaseController : ControllerBase
{
    protected IActionResult HandleResponse<T>(DTOs.ApiResponse<T> response)
    {
        if (response.Success)
        {
            return Ok(response);
        }

        return BadRequest(response);
    }
}
