using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Configurations;
using DotNet.Testcontainers.Containers;
using DotNet.Testcontainers.Networks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Respawn;
using System.Data.Common;
using System.Data.SqlClient;
using Xunit;

namespace RespawnDemo.IntegrationTests.Shared;
public class EmployeeApiFactory : WebApplicationFactory<IApiMarker>, IAsyncLifetime
{
    private Respawner respawner = default!;
    private DbConnection dbConnection = default!;
    private MsSqlTestcontainer dbContainer = default!;

    private TestcontainersContainer grate = default!;
    private IDockerNetwork testNetwork = default!;
    private readonly string databaseName = "IntegrationTestsDb";
    private readonly string databasePassword = "yourStrong(!)Password";
    private readonly string databaseServerName = "db-testcontainer";

    public string DatabaseConnectionString
    {
        get
        {
            var connStr = this.dbContainer.ConnectionString.Replace("master", databaseName);
            return connStr;
        }
    }

    public HttpClient HttpClient { get; private set; } = default!;

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureTestServices(services =>
        {
            services.SetupDatabaseConnection(DatabaseConnectionString);
        });
    }

    public async Task InitializeAsync()
    {
        // Create SQL Server and wait for it to be ready.
        const string started = "Recovery is complete. This is an informational message only. No user action is required.";
        const string grateFinished = "Skipping 'AfterMigration', afterMigration does not exist.";

        using var stdout = new MemoryStream();
        using var stderr = new MemoryStream();
        using var consumer = Consume.RedirectStdoutAndStderrToStream(stdout, stderr);

        var dockerNetworkBuilder = new TestcontainersNetworkBuilder().WithName("testNetwork");
        testNetwork = dockerNetworkBuilder.Build();
        testNetwork.CreateAsync().Wait();

        dbContainer = new TestcontainersBuilder<MsSqlTestcontainer>()
           .WithDatabase(new MsSqlTestcontainerConfiguration
           {
               Password = databasePassword,
           })
        .WithImage("mcr.microsoft.com/mssql/server:2019-CU10-ubuntu-20.04")
        .WithOutputConsumer(consumer)
        .WithWaitStrategy(Wait.ForUnixContainer().UntilMessageIsLogged(consumer.Stdout, started))
        .WithNetwork(testNetwork)
        .WithName(databaseServerName)
        .Build();
        await dbContainer.StartAsync();

        // Create grate container for sql migrations.  Run migrations on SQL server.
        var sqlMigrationsBaseDirectory = Path.Combine(Environment.CurrentDirectory, "sql-migrations").ConvertToPosix();

        using var gratestdout = new MemoryStream();
        using var gratestderr = new MemoryStream();
        using var grateconsumer = Consume.RedirectStdoutAndStderrToStream(gratestdout, gratestderr);

        var grateBuilder = new TestcontainersBuilder<TestcontainersContainer>()
            .WithNetwork(testNetwork)
            .WithImage("erikbra/grate")
            .WithBindMount(sqlMigrationsBaseDirectory, "/sql-migrations")
            .WithCommand(@$"--connectionstring=Server={databaseServerName};Database={databaseName};User ID=sa;Password={databasePassword};TrustServerCertificate=True;Encrypt=false", "--files=/sql-migrations")
            .WithOutputConsumer(grateconsumer)
            .WithWaitStrategy(Wait.ForUnixContainer().UntilMessageIsLogged(grateconsumer.Stdout, grateFinished));

        grate = grateBuilder.Build();
        await grate.StartAsync();

        dbConnection = new SqlConnection(DatabaseConnectionString);
        HttpClient = CreateClient(new WebApplicationFactoryClientOptions { AllowAutoRedirect = false });
        await InitializeRespawner();
    }

    public async Task ResetDatabaseAsync() => await respawner.ResetAsync(dbConnection);

    private async Task InitializeRespawner()
    {
        await dbConnection.OpenAsync();
        respawner = await Respawner.CreateAsync(dbConnection, new RespawnerOptions()
        {
            DbAdapter = DbAdapter.SqlServer,
            SchemasToInclude = new[] { "dbo" }
        });
    }

    async Task IAsyncLifetime.DisposeAsync()
    {
        await dbContainer.DisposeAsync();
    }
}