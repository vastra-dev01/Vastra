//using Microsoft.AspNetCore.Builder;
//using Microsoft.Data.Sqlite;
//using Microsoft.EntityFrameworkCore;
//using Microsoft.Extensions.Configuration;
//using Vastra.API.DBContexts;
//using Vastra.API.Services;

//namespace Vastra.API.Test.Fixtures
//{
    
//    public class VastraTestDBClassFixture : IDisposable
//    {
//        public VastraRepository VastraRepository { get; }
//        public VastraTestDBClassFixture()
//        {
//            var connection = new SqliteConnection("Data Source=:memory:");
//            connection.Open();

            

//            var optionsBuilder = new DbContextOptionsBuilder<VastraContext>()
//                .UseSqlite(connection);

//            var dbContext = new VastraContext(optionsBuilder.Options);

//            dbContext.Database.Migrate();

//            VastraRepository = new VastraRepository(dbContext);

//        }

//        public void Dispose()
//        {
//            //cleanup
//        }
//    }
//}
