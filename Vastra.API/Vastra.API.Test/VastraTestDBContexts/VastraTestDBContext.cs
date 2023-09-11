using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vastra.API.DBContexts;
using Vastra.API.Entities;
using Vastra.API.Services;

namespace Vastra.API.Test.VastraTestDBContexts
{
    public class VastraTestDBContext : VastraContext
    {
        public VastraTestDBContext(IConfiguration configuration) : base(configuration)
        {
        }
        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            options.UseSqlite("Data Source=:memory:");
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}
