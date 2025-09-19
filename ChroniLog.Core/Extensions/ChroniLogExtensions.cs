using System.Threading.Channels;
using ChroniLog.Core.Configurations;
using ChroniLog.Core.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace ChroniLog.Core.Extensions;

public static class ChroniLogExtensions
{
    public static ILoggingBuilder AddChroniLogCore(this ILoggingBuilder builder)
    {
        builder.Services.ConfigureOptions<ConfigureChroniLogOptions>();
        
        builder.Services.AddSingleton<Channel<ChroniLogEntry>>(sp =>
        {
            var opts = sp.GetRequiredService<IOptions<ChroniLogSettings>>().Value;

            return Channel.CreateBounded<ChroniLogEntry>(new BoundedChannelOptions(opts.ChannelCapacity)
            {
                FullMode = BoundedChannelFullMode.DropWrite
            });
            
        });
        
        builder.Services.AddSingleton<ILoggerProvider, ChroniLoggerProvider>();
        builder.Services.AddHostedService<ChroniLoggerWorker>();
        return builder;
    }
}