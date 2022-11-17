using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace RespawnDemo.IntegrationTests.Shared
{
    [CollectionDefinition("EmployeeDbCollection")]
    public class SharedDbCollection : ICollectionFixture<DbFixture>
    {

    }
}
