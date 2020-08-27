using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Text;

namespace DataExtractor.Service.Data
{
    public class DEDbContext : DbContext 
    {
        public DEDbContext(DbContextOptions<DEDbContext> options) : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var dbConnectionString = ConfigurationManager.AppSettings["DBConnection"];
            optionsBuilder.UseSqlServer(dbConnectionString);
        }
    }
}
