using Microsoft.EntityFrameworkCore;
using PuppetMaster.WebApi.Repositories;

namespace PuppetMaster.WebApi.Tests
{
    public static class DbContextHelper
    {
        public static ApplicationDbContext GetInMemoryDbContext()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: "puppetmaster")
            .Options;

            return new ApplicationDbContext(options);
        }
    }
}
