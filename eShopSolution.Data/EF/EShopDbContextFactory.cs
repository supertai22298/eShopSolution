using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace eShopSolution.Data.EF
{
    public class EShopDbContextFactory : IDesignTimeDbContextFactory<EShopDbContext>
    {
        public EShopDbContext CreateDbContext(string[] args)
        {
            IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json").Build();

            string connectionString = configuration.GetConnectionString("eShopSolutionDatabase");

            var optionBuilder = new DbContextOptionsBuilder<EShopDbContext>();

            optionBuilder.UseSqlServer(connectionString);

            return new EShopDbContext(optionBuilder.Options);
        }
    }
}
