using FluentAssertions;
using RespawnDemo.IntegrationTests.Shared;
using System.Net;
using Xunit;

namespace RespawnDemo.IntegrationTests;

[Collection("EmployeeDbCollection")]
public class DeleteEmployeeTests : IAsyncLifetime
{
    private readonly string databaseConnectionString;
    private readonly Func<Task> resetDatabase;
    private readonly HttpClient client;

    public DeleteEmployeeTests(EmployeeApiFactory apiFactory)
    {
        databaseConnectionString = apiFactory.DatabaseConnectionString;
        this.client = apiFactory.HttpClient;
        resetDatabase = apiFactory.ResetDatabaseAsync;
    }

    [Fact]
    public async Task CanDeleteEmployee()
    {
        var employeeFactory = new EmployeeFactory(databaseConnectionString);
        employeeFactory.CreateRandomSamplingOfEmployees(10);
        var createdEmployees = employeeFactory.GetAllEmployees();
        var employeeToDelete = createdEmployees[new Random().Next(0, 9)];


        var deleteEmployeeResponse = await client.DeleteAsync($"/employees/{employeeToDelete.EmployeeId}");

        deleteEmployeeResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var remainingEmployees = employeeFactory.GetAllEmployees();
        var deletedEmployee = remainingEmployees.Where(x => x.EmployeeId == employeeToDelete.EmployeeId).FirstOrDefault();

        deletedEmployee.Should().BeNull();
    }

    public Task DisposeAsync() => resetDatabase();

    public Task InitializeAsync() => Task.CompletedTask;
}
