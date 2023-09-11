using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Vastra.API.DBContexts;
using Vastra.API.Services;
using Vastra.API.Test.VastraTestDBContexts;

namespace Vastra.API.Test.Fixtures
{
    
    public class VastraTestDBClassFixture : IDisposable
    {
        public VastraRepository VastraRepository { get; }
        public VastraTestDBClassFixture()
        {
            //var connection = new SqliteConnection("Data Source=:memory:");
            //connection.Open();

            IConfigurationRoot configuration = new ConfigurationBuilder()
            .SetBasePath(@"./appsettings.json")
            .AddJsonFile("appsettings.json")
            .Build();

            //var optionsBuilder = new DbContextOptionsBuilder<VastraContext>()
            //    .UseSqlite(connection);

            var dbContext = new VastraTestDBContext(configuration);

            dbContext.Database.Migrate();

            VastraRepository = new VastraRepository(dbContext);

        }

        public void Dispose()
        {
            //cleanup
        }
    }
}
