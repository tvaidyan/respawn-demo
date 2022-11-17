using Dapper;
using MediatR;
using RespawnDemo.Api.Shared.DataAccess;

namespace RespawnDemo.Api.Employee;

public class GetAllEmployeesRequest : IRequest<List<Employee>>
{
}

public class GetAllEmployeesHandler : IRequestHandler<GetAllEmployeesRequest, List<Employee>>
{
    private readonly IDatabase database;

    public GetAllEmployeesHandler(IDatabase database)
    {
        this.database = database;
    }

    public async Task<List<Employee>> Handle(GetAllEmployeesRequest request, CancellationToken cancellationToken)
    {
        return (await database.ExecuteFileAsync<Employee>("Employee/select-all-employees.sql"))
            .ToList();
    }
}
