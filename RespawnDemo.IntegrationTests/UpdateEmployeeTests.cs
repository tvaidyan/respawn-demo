using FluentAssertions;
using RespawnDemo.Api.Employee;
using RespawnDemo.IntegrationTests.Shared;
using System.Net;
using System.Text;
using System.Text.Json;
using Xunit;

namespace RespawnDemo.IntegrationTests;

[Collection("EmployeeDbCollection")]
public class UpdateEmployeeTests : IAsyncLifetime
{
    private readonly string databaseConnectionString;
    private readonly Func<Task> resetDatabase;
    private readonly HttpClient client;

    public UpdateEmployeeTests(EmployeeApiFactory apiFactory)
    {
        databaseConnectionString = apiFactory.DatabaseConnectionString;
        this.client = apiFactory.HttpClient;
        resetDatabase = apiFactory.ResetDatabaseAsync;
    }

    [Fact]
    public async Task CanCreateAndRetrieveEmployee()
    {
        var employeeFactory = new EmployeeFactory(databaseConnectionString);
        employeeFactory.CreateRandomSamplingOfEmployees(10);
        var createdEmployees = employeeFactory.GetAllEmployees();
        var employeeToUpdate = createdEmployees[new Random().Next(0, 9)];

        var employee = new UpdateEmployeeRequest
        {
            EmployeeId = employeeToUpdate.EmployeeId,
            HireDate = DateTime.UtcNow.AddDays(-90),
            FirstName = "Tom",
            LastName = "Hanks",
            FavoriteColor = "Blue"
        };

        HttpContent updateEmployeePayload = new StringContent(JsonSerializer.Serialize(employee), Encoding.UTF8, "application/json");
        var updateEmployeeResponse = await client.PutAsync("/employees", updateEmployeePayload);

        var response = (await client.SendAsync(new HttpRequestMessage
        {
            Method = HttpMethod.Get,
            RequestUri = new Uri($@"http://localhost:7050/employees/{employeeToUpdate.EmployeeId}")
        })).Content.ReadAsStringAsync().Result;

        var updatedEmployee = JsonSerializer.Deserialize<Employee>(response, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        })!;

        updateEmployeeResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        updatedEmployee.Should().NotBeNull();
        updatedEmployee!.FirstName.Should().Be("Tom");
        updatedEmployee.LastName.Should().Be("Hanks");
        updatedEmployee.FavoriteColor.Should().Be("Blue");
    }

    public Task DisposeAsync() => resetDatabase();

    public Task InitializeAsync() => Task.CompletedTask;
}
