using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace RespawnDemo.Api.Employee;

[ApiController]
[Route("[controller]")]
public class EmployeeController : ControllerBase
{
    private readonly IMediator mediator;

    public EmployeeController(IMediator mediator)
    {
        this.mediator = mediator;
    }

    [HttpGet]
    public async Task<ActionResult<Employee>> GetEmployee(GetEmployeeRequest request)
    {
        var employee = await mediator.Send(request);

        if (employee is null)
            return NotFound();

        return Ok(employee);
    }

    [HttpPost]
    public async Task<IActionResult> PostEmployee(AddEmployeeRequest request)
    {
        await mediator.Send(request);
        return new OkResult();
    }

    [HttpPut]
    public async Task<IActionResult> PutEmployee(UpdateEmployeeRequest request)
    {
        await mediator.Send(request);
        return new OkResult();
    }

    [HttpDelete]
    public async Task<IActionResult> DeleteEmployee(DeleteEmployeeRequest request)
    {
        await mediator.Send(request);
        return new OkResult();
    }
}
