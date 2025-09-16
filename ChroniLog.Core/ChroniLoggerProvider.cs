using System.Threading.Channels;
using ChroniLog.Core.Models;
using Microsoft.Extensions.Logging;

namespace ChroniLog.Core;

public sealed class ChroniLoggerProvider : ILoggerProvider
{
    private readonly Channel<ChroniLogEntry> _entriesChannel;

    public ChroniLoggerProvider(Channel<ChroniLogEntry> entriesChannel)
    {
        _entriesChannel = entriesChannel;
    }

    public void Dispose()
    {
        _entriesChannel.Writer.Complete();
    }

    public ILogger CreateLogger(string categoryName)
    {
        return new ChroniLogger(_entriesChannel, categoryName);
    }
}