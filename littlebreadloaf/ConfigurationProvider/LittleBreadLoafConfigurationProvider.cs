using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Primitives;

namespace littlebreadloaf.ConfigurationProvider
{
    public class LittleBreadLoafConfigurationProvider : Microsoft.Extensions.Configuration.ConfigurationProvider
    {
        public LittleBreadLoafConfigurationProvider(Action<DbContextOptionsBuilder> optionsAction)
        {
            OptionsAction = optionsAction;
        }

        Action<DbContextOptionsBuilder> OptionsAction { get; }

        public override void Load()
        {
            var builder = new DbContextOptionsBuilder<ConfigurationContext>();

            OptionsAction(builder);

            using (var dbContext = new ConfigurationContext(builder.Options))
            {
                Data = dbContext.LittleBreadLoafSystem.ToDictionary(c => c.Key, c => c.Value);
            }
        }
    }
}