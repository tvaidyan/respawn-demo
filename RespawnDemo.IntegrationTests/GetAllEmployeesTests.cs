using FluentAssertions;
using RespawnDemo.Api.Employee;
using RespawnDemo.IntegrationTests.Shared;
using System.Net;
using System.Text.Json;
using Xunit;

namespace RespawnDemo.IntegrationTests;

[Collection("EmployeeDbCollection")]
public class GetAllEmployeesTests
{
    private readonly DbFixture dbFixture;

    public GetAllEmployeesTests(DbFixture dbFixture)
    {
        this.dbFixture = dbFixture;
    }

    [Fact]
    public async Task CanGetAllEmployees()
    {
        var factory = new CustomWebApplicationFactory<Program>(services =>
        {
            services.SetupDatabaseConnection(dbFixture.DatabaseConnectionString);
        });

        var employeeFactory = new EmployeeFactory(dbFixture.DatabaseConnectionString);
        employeeFactory.CreateRandomSamplingOfEmployees(10);
        var createdEmployees = employeeFactory.GetAllEmployees();
        var client = factory.CreateClient();

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
}
