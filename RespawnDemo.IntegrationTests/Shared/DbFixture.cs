using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Configurations;
using DotNet.Testcontainers.Containers;
using DotNet.Testcontainers.Networks;

namespace RespawnDemo.IntegrationTests.Shared;

public class DbFixture : IDisposable
{
    private MsSqlTestcontainer dbContainer;
    private readonly TestcontainersContainer grate;
    private readonly IDockerNetwork testNetwork;
    private const string databaseName = "IntegrationTestsDb";

    public MsSqlTestcontainer DbContainer
    {
        get { return dbContainer; }
        set { dbContainer = value; }
    }

    public string DatabaseConnectionString
    {
        get { return this.dbContainer.ConnectionString.Replace("master", databaseName); }
    }

    public DbFixture()
    {
        // Create SQL Server and wait for it to be ready.
        const string started = "Recovery is complete. This is an informational message only. No user action is required.";
        using var stdout = new MemoryStream();
        using var stderr = new MemoryStream();
        using var consumer = Consume.RedirectStdoutAndStderrToStream(stdout, stderr);

        var dockerNetworkBuilder = new TestcontainersNetworkBuilder().WithName("testNetwork");
        testNetwork = dockerNetworkBuilder.Build();
        testNetwork.CreateAsync().Wait();

        var testSqlServerName = $"testSqlServer{Randomizer.GetRandomString(6)}";

        var dbBuilder = new TestcontainersBuilder<MsSqlTestcontainer>()
        .WithDatabase(new MsSqlTestcontainerConfiguration()
        {
            Password = "yourStrong(!)Password"
        })
        .WithImage("mcr.microsoft.com/mssql/server:2019-CU10-ubuntu-20.04")
        .WithOutputConsumer(consumer)
        .WithWaitStrategy(Wait.ForUnixContainer().UntilMessageIsLogged(consumer.Stdout, started))
        .WithNetwork(testNetwork)
        .WithName(testSqlServerName);

        dbContainer = dbBuilder.Build();
        dbContainer.StartAsync().Wait();

        // Create grate container for sql migrations.  Run migrations on SQL server.
        var sqlMigrationsBaseDirectory = Path.Combine(Environment.CurrentDirectory, "sql-migrations").ConvertToPosix();

        var grateBuilder = new TestcontainersBuilder<TestcontainersContainer>()
            .WithNetwork(testNetwork)
            .WithImage("erikbra/grate")
            .WithBindMount(sqlMigrationsBaseDirectory, "/sql-migrations")
            .WithCommand(@$"--connectionstring=Server={testSqlServerName};Database={databaseName};User ID=sa;Password=yourStrong(!)Password;TrustServerCertificate=True", "--files=/sql-migrations");

        grate = grateBuilder.Build();
        grate.StartAsync().Wait();
    }

    public void Dispose()
    {
        dbContainer.DisposeAsync();
        grate.DisposeAsync();
        testNetwork.DeleteAsync();
    }
}