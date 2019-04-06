using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace littlebreadloaf.ConfigurationProvider
{
    public class ConfigurationContext : DbContext
    {
        public ConfigurationContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<LittleBreadLoafSystem> LittleBreadLoafSystem { get; set; }
    }
}
