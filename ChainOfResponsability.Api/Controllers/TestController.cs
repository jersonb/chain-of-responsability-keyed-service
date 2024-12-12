using ChainOfResponsability.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace ChainOfResponsability.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class TestController(ImportFileService importFileService) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Post(IFormFile file)
    {
        var stream = file.OpenReadStream();
         var validations = await importFileService.Import(stream);

        if (validations.Any())
        {
            return BadRequest(validations);
        }
        return Ok();
    }
}