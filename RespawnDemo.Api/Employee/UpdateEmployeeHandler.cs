using Dapper;
using MediatR;
using RespawnDemo.Api.Shared.DataAccess;

namespace RespawnDemo.Api.Employee;
public class UpdateEmployeeRequest : IRequest<Unit>
{
    public DateTime HireDate { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string EmployeeId { get; set; } = Guid.NewGuid().ToString();
    public string FavoriteColor { get; set; } = string.Empty;
}

public class UpdateEmployeeHandler : IRequestHandler<UpdateEmployeeRequest, Unit>
{
    private readonly IDatabase database;

    public UpdateEmployeeHandler(IDatabase database)
    {
        this.database = database;
    }

    public async Task<Unit> Handle(UpdateEmployeeRequest request, CancellationToken cancellationToken)
    {
        var parameters = new DynamicParameters();
        parameters.Add("@employeeId", request.EmployeeId);
        parameters.Add("@firstName", request.FirstName);
        parameters.Add("@lastName", request.LastName);
        parameters.Add("@favoriteColor", request.FavoriteColor);
        parameters.Add("@hireDate", request.HireDate);

        await database.ExecuteFileAsync("Employee/update-employee.sql", parameters);

        return Unit.Value;
    }
}