using Xunit;

namespace RespawnDemo.IntegrationTests.Shared
{
    [CollectionDefinition("EmployeeDbCollection")]
    public class SharedDbCollection : ICollectionFixture<EmployeeApiFactory>
    {

    }
}
