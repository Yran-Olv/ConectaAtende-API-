using ConectaAtende.Domain.Services;
using Microsoft.AspNetCore.Mvc;

namespace ConectaAtende.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TriageController : ControllerBase
{
    private readonly ITriagePolicyService _triagePolicyService;

    public TriageController(ITriagePolicyService triagePolicyService)
    {
        _triagePolicyService = triagePolicyService;
    }

    [HttpGet("policy")]
    public IActionResult GetPolicy()
    {
        var policyName = _triagePolicyService.GetCurrentPolicyName();
        return Ok(new { policy = policyName });
    }

    [HttpPost("policy")]
    public IActionResult SetPolicy([FromBody] SetPolicyDto dto)
    {
        try
        {
            _triagePolicyService.SetPolicy(dto.Policy);
            return Ok(new { message = $"Política alterada para: {dto.Policy}" });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }
}

public class SetPolicyDto
{
    public string Policy { get; set; } = string.Empty;
}
