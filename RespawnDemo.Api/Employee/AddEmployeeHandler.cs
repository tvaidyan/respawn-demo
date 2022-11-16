using Dapper;
using MediatR;
using RespawnDemo.Api.Shared.DataAccess;

namespace RespawnDemo.Api.Employee;
public class AddEmployeeRequest : IRequest<Unit>
{
    public DateTime HireDate { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string FavoriteColor { get; set; } = string.Empty;
}

public class AddEmployeeHandler : IRequestHandler<AddEmployeeRequest, Unit>
{
    private readonly IDatabase database;

    public AddEmployeeHandler(IDatabase database)
    {
        this.database = database;
    }

    public async Task<Unit> Handle(AddEmployeeRequest request, CancellationToken cancellationToken)
    {
        var parameters = new DynamicParameters();
        parameters.Add("@hireDate", request.HireDate);
        parameters.Add("@firstName", request.FirstName);
        parameters.Add("@lastName", request.LastName);
        parameters.Add("@favoriteColor", request.FavoriteColor);

        await database.ExecuteFileAsync("Employee/insert-employee.sql", parameters);

        return Unit.Value;
    }
}