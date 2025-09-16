using System.Threading.Channels;
using ChroniLog.Core.Models;
using Microsoft.Extensions.Logging;

namespace ChroniLog.Core;

internal class ChroniLogger : ILogger
{
    private readonly Channel<ChroniLogEntry> _entriesChannel;
    private readonly string _categoryName;

    public ChroniLogger(Channel<ChroniLogEntry> entriesChannel, string categoryName)
    {
        _entriesChannel = entriesChannel;
        _categoryName = categoryName;
    }

    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception,
        Func<TState, Exception?, string> formatter)
    {
        var entry = new ChroniLogEntry(
            logLevel,
            eventId,
            formatter(state, exception),
            exception?.ToString(),
            _categoryName,
            DateTime.UtcNow
        );
        
        _entriesChannel.Writer.TryWrite(entry);
    }

    public bool IsEnabled(LogLevel logLevel)
    {
        return logLevel != LogLevel.None;
    }

    public IDisposable? BeginScope<TState>(TState state) where TState : notnull
    {
        return null;
    }
}