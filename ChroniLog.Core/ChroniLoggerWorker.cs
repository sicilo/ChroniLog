using System.Threading.Channels;
using ChroniLog.Core.Contracts;
using ChroniLog.Core.Models;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace ChroniLog.Core;

internal sealed class ChroniLoggerWorker : BackgroundService
{
    private readonly Channel<ChroniLogEntry> _entriesChannel;
    private readonly IFlushResolver _flushResolver;
    private readonly ILogger<ChroniLoggerWorker> _logger;
    private readonly ChroniLogOptions _options;

    public ChroniLoggerWorker(
        Channel<ChroniLogEntry> entriesChannel,
        IFlushResolver  flushResolver,
        ILogger<ChroniLoggerWorker> logger,
        IOptions<ChroniLogOptions> options
        )
    {
        _entriesChannel = entriesChannel;
        _flushResolver = flushResolver;
        _logger = logger;
        _options = options.Value;
    }
    
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("✅ ChroniLogWorker started. BufferCapacity={Capacity}, FlushInterval={Interval}s",
            _options.BufferCapacity,
            _options.BufferFlushInterval);
        
        var buffer = new List<ChroniLogEntry>(capacity: _options.BufferCapacity);
        var lastFlush = DateTime.UtcNow;
        
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                if (await _entriesChannel.Reader.WaitToReadAsync(stoppingToken))
                {
                    while (_entriesChannel.Reader.TryRead(out var entry))
                    {
                        buffer.Add(entry);

                        if (buffer.Count < _options.BufferCapacity) continue;
                        _logger.LogInformation("ChroniLogWorker Flushing {Count} logs (buffer lleno)", buffer.Count);
                        await _flushResolver.FlushAsync(buffer, stoppingToken);
                        buffer.Clear();
                    }
                }

                if (buffer.Count < 0 ||
                    !((DateTime.UtcNow - lastFlush).TotalSeconds >= _options.BufferFlushInterval)) continue;

                _logger.LogInformation("ChroniLogWorker Flushing {Count} logs (intervalo)", buffer.Count);
                await _flushResolver.FlushAsync(buffer, stoppingToken);
                buffer.Clear();
                lastFlush = DateTime.UtcNow;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ ChroniLogWorker ChroniLogWorker");
                await Console.Error.WriteLineAsync($"Error en PostgresLogWorker: {ex}");
            }
        }
    }
}