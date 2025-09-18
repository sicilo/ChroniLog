using ChroniLog.Core.Contracts;
using ChroniLog.Core.Extensions;
using ChroniLog.Flusher.PostgreSql.Resolvers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace ChroniLog.Flusher.PostgreSql.Extensions;

public static class ChroniLogFlusherExtensions
{
    public static ILoggingBuilder AddChroniLogFlushPostgreSql(this ILoggingBuilder builder)
    {
        builder.Services.AddSingleton<IFlushResolver, FlushResolver>();
        builder.AddChroniLogCore();
        builder.Services.AddHostedService<ChroniLogInitializer>();
        return builder;
    }
}