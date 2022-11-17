using FluentAssertions;
using RespawnDemo.Api.Employee;
using RespawnDemo.IntegrationTests.Shared;
using System.Net;
using System.Text.Json;
using Xunit;

namespace RespawnDemo.IntegrationTests;

[Collection("EmployeeDbCollection")]
public class GetAllEmployeesTests : IAsyncLifetime
{
    private readonly string databaseConnectionString;
    private readonly Func<Task> resetDatabase;
    private readonly HttpClient client;

    public GetAllEmployeesTests(EmployeeApiFactory apiFactory)
    {
        databaseConnectionString = apiFactory.DatabaseConnectionString;
        this.client = apiFactory.HttpClient;
        resetDatabase = apiFactory.ResetDatabaseAsync;
    }

    [Fact]
    public async Task CanGetAllEmployees()
    {
        var employeeFactory = new EmployeeFactory(databaseConnectionString);
        employeeFactory.CreateRandomSamplingOfEmployees(10);

        var response = await client.SendAsync(new HttpRequestMessage
        {
            Method = HttpMethod.Get,
            RequestUri = new Uri($@"http://localhost:7050/employees")
        });
        var result = response.Content.ReadAsStringAsync().Result;

        var employeesFetched = JsonSerializer.Deserialize<List<Employee>>(result, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        })!;

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        employeesFetched.Should().NotBeNull();
        employeesFetched.Count.Should().Be(10);
    }

    public Task DisposeAsync() => resetDatabase();

    public Task InitializeAsync() => Task.CompletedTask;
}
