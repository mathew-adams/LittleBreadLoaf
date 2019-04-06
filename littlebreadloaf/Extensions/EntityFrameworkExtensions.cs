using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using littlebreadloaf.ConfigurationProvider;

namespace littlebreadloaf.Extensions
{
    public static class EntityFrameworkExtensions
    {
        public static IConfigurationBuilder AddEFConfiguration(
            this IConfigurationBuilder builder,
            Action<DbContextOptionsBuilder> optionsAction)
        {
            return builder.Add(new ConfigurationSource(optionsAction));
        }
    }
}
