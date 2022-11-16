using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace RespawnDemo.Api.Employee;

[ApiController]
public class EmployeeController : ControllerBase
{
    private readonly IMediator mediator;

    public EmployeeController(IMediator mediator)
    {
        this.mediator = mediator;
    }

    [HttpGet("employees/{id:guid}")]
    public async Task<ActionResult<Employee>> Get([FromRoute] Guid id)
    {
        var employee = await mediator.Send(new GetEmployeeRequest { EmployeeId = id });

        if (employee is null)
            return NotFound();

        return Ok(employee);
    }

    [HttpPost("employees")]
    public async Task<IActionResult> Create(AddEmployeeRequest request)
    {
        var newEmployee = await mediator.Send(request);
        return CreatedAtAction("Get", new { newEmployee.EmployeeId }, newEmployee);
    }

    [HttpPut("employees")]
    public async Task<IActionResult> PutEmployee(UpdateEmployeeRequest request)
    {
        await mediator.Send(request);
        return new OkResult();
    }

    [HttpDelete("employees")]
    public async Task<IActionResult> DeleteEmployee(DeleteEmployeeRequest request)
    {
        await mediator.Send(request);
        return new OkResult();
    }
}
