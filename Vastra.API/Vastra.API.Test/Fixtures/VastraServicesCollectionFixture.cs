using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vastra.API.Test.Fixtures
{
    [CollectionDefinition("VastraServicesCollection")]
    public class VastraServicesCollectionFixture 
        : ICollectionFixture<VastraServicesClassFixture>
        //,ICollectionFixture<VastraTestDBClassFixture>
    {
    }
}
