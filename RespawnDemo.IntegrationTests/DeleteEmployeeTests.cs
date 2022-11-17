﻿using FluentAssertions;
using RespawnDemo.IntegrationTests.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace RespawnDemo.IntegrationTests;
public class DeleteEmployeeTests : IClassFixture<DbFixture>
{
    private readonly DbFixture dbFixture;

    public DeleteEmployeeTests(DbFixture dbFixture)
    {
        this.dbFixture = dbFixture;
    }

    [Fact]
    public async Task CanDeleteEmployee()
    {
        var factory = new CustomWebApplicationFactory<Program>(services =>
        {
            services.SetupDatabaseConnection(dbFixture.DatabaseConnectionString);
        });

        var employeeFactory = new EmployeeFactory(dbFixture.DatabaseConnectionString);
        employeeFactory.CreateRandomSamplingOfEmployees(10);
        var createdEmployees = employeeFactory.GetAllEmployees();
        var employeeToDelete = createdEmployees[new Random().Next(0, 9)];

        var client = factory.CreateClient();
        var deleteEmployeeResponse = await client.DeleteAsync($"/employees/{employeeToDelete.EmployeeId}");

        deleteEmployeeResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var remainingEmployees = employeeFactory.GetAllEmployees();
        var deletedEmployee = remainingEmployees.Where(x => x.EmployeeId == employeeToDelete.EmployeeId).FirstOrDefault();

        deletedEmployee.Should().BeNull();
    }

}
