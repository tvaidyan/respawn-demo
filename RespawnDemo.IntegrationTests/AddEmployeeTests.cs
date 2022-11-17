using FluentAssertions;
using RespawnDemo.Api.Employee;
using RespawnDemo.IntegrationTests.Shared;
using System.Net;
using System.Text;
using System.Text.Json;
using Xunit;

namespace RespawnDemo.IntegrationTests;

[Collection("EmployeeDbCollection")]
public class AddEmployeeTests : IAsyncLifetime
{
    private readonly string databaseConnectionString;
    private readonly Func<Task> resetDatabase;
    private readonly HttpClient client;

    public AddEmployeeTests(EmployeeApiFactory apiFactory)
    {
        databaseConnectionString = apiFactory.DatabaseConnectionString;
        this.client = apiFactory.HttpClient;
        resetDatabase = apiFactory.ResetDatabaseAsync;
    }

    [Fact]
    public async Task CanCreateAndRetrieveEmployee()
    {
        var employeeFactory = new EmployeeFactory(databaseConnectionString);
        employeeFactory.CreateRandomSamplingOfEmployees();

        var employee = new AddEmployeeRequest
        {
            HireDate = DateTime.UtcNow.AddDays(-90),
            FirstName = "Bruce",
            LastName = "Willis",
            FavoriteColor = "Red"
        };

        HttpContent createEmployeePayload = new StringContent(JsonSerializer.Serialize(employee), Encoding.UTF8, "application/json");
        var createEmployeeResponse = await client.PostAsync("/employees", createEmployeePayload);

        var response = (await client.SendAsync(new HttpRequestMessage
        {
            Method = HttpMethod.Get,
            RequestUri = new Uri(createEmployeeResponse.Headers.Location!.ToString().Replace("localhost", "localhost:7050"))
        })).Content.ReadAsStringAsync().Result;

        var employeeCreated = JsonSerializer.Deserialize<Employee>(response, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        })!;

        createEmployeeResponse.StatusCode.Should().Be(HttpStatusCode.Created);

        employeeCreated.Should().NotBeNull();
        employeeCreated!.FirstName.Should().Be("Bruce");
        employeeCreated.LastName.Should().Be("Willis");
        employeeCreated.FavoriteColor.Should().Be("Red");
    }

    public Task DisposeAsync() => resetDatabase();

    public Task InitializeAsync() => Task.CompletedTask;
}
