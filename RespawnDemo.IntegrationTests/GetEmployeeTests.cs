using FluentAssertions;
using RespawnDemo.Api.Employee;
using RespawnDemo.IntegrationTests.Shared;
using System.Net;
using System.Text.Json;
using Xunit;

namespace RespawnDemo.IntegrationTests;

[Collection("EmployeeDbCollection")]
public class GetEmployeeTests
{
    private readonly DbFixture dbFixture;

    public GetEmployeeTests(DbFixture dbFixture)
    {
        this.dbFixture = dbFixture;
    }

    [Fact]
    public async Task CanGetSpecificEmployee()
    {
        var factory = new CustomWebApplicationFactory<Program>(services =>
        {
            services.SetupDatabaseConnection(dbFixture.DatabaseConnectionString);
        });

        var employeeFactory = new EmployeeFactory(dbFixture.DatabaseConnectionString);
        employeeFactory.CreateRandomSamplingOfEmployees(10);
        var createdEmployees = employeeFactory.GetAllEmployees();
        var employeeToFetch = createdEmployees[new Random().Next(0, 9)];
        var client = factory.CreateClient();

        var response = await client.SendAsync(new HttpRequestMessage
        {
            Method = HttpMethod.Get,
            RequestUri = new Uri($@"http://localhost:7050/employees/{employeeToFetch.EmployeeId}")
        });
        var result = response.Content.ReadAsStringAsync().Result;

        var employeeFetched = JsonSerializer.Deserialize<Employee>(result, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        })!;

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        employeeFetched.Should().NotBeNull();
        employeeFetched!.FirstName.Should().Be(employeeToFetch.FirstName);
        employeeFetched.LastName.Should().Be(employeeToFetch.LastName);
        employeeFetched.FavoriteColor.Should().Be(employeeToFetch.FavoriteColor);
    }
}
