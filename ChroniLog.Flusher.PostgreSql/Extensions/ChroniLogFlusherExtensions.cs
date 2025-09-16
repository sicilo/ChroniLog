using ChroniLog.Core.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace ChroniLog.Flusher.PostgreSql.Extensions;

public static class ChroniLogFlusherExtensions
{
    public static ILoggingBuilder AddChroniLog(this ILoggingBuilder builder)
    {
        builder.AddChroniLogCore();

        builder.Services.AddHostedService<ChroniLogInitializer>();
        return builder;
    }
}