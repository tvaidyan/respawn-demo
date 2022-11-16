using Dapper;
using MediatR;
using RespawnDemo.Api.Shared.DataAccess;

namespace RespawnDemo.Api.Employee;

public class GetEmployeeRequest : IRequest<Employee?>
{
    public Guid EmployeeId { get; set; }
}

public class GetEmployeeHandler : IRequestHandler<GetEmployeeRequest, Employee?>
{
    private readonly IDatabase database;

    public GetEmployeeHandler(IDatabase database)
    {
        this.database = database;
    }

    public async Task<Employee?> Handle(GetEmployeeRequest request, CancellationToken cancellationToken)
    {
        var parameters = new DynamicParameters();
        parameters.Add("@employeeId", request.EmployeeId);

        return (await database.ExecuteFileAsync<Employee>("Employee/select-employee.sql", parameters)).FirstOrDefault();
    }
}
