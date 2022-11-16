using Dapper;
using MediatR;
using RespawnDemo.Api.Shared.DataAccess;

namespace RespawnDemo.Api.Employee;
public class DeleteEmployeeRequest : IRequest<Unit>
{
    public string EmployeeId { get; set; } = string.Empty;
}

public class DeleteEmployeeHandler : IRequestHandler<DeleteEmployeeRequest, Unit>
{
    private readonly IDatabase database;

    public DeleteEmployeeHandler(IDatabase database)
    {
        this.database = database;
    }

    public async Task<Unit> Handle(DeleteEmployeeRequest request, CancellationToken cancellationToken)
    {
        var parameters = new DynamicParameters();
        parameters.Add("@employeeId", request.EmployeeId);

        await database.ExecuteFileAsync("Employee/delete-employee.sql", parameters);

        return Unit.Value;
    }
}