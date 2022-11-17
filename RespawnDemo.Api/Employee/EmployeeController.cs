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

    [HttpGet("employees/{employeeId:guid}")]
    public async Task<ActionResult<Employee>> Get([FromRoute] Guid employeeId)
    {
        var employee = await mediator.Send(new GetEmployeeRequest { EmployeeId = employeeId });

        if (employee is null)
            return NotFound();

        return Ok(employee);
    }

    [HttpPost("employees")]
    public async Task<IActionResult> Create(AddEmployeeRequest request)
    {
        var newEmployee = await mediator.Send(request);
        var employeeId = newEmployee.EmployeeId.ToString();
        return CreatedAtAction("Get", new { employeeId }, newEmployee);
    }

    [HttpPut("employees")]
    public async Task<IActionResult> Update(UpdateEmployeeRequest request)
    {
        await mediator.Send(request);
        return new OkResult();
    }

    [HttpDelete("employees/{employeeId:guid}")]
    public async Task<IActionResult> Delete([FromRoute] Guid employeeId)
    {
        await mediator.Send(new DeleteEmployeeRequest { EmployeeId = employeeId });
        return new NoContentResult();
    }
}
