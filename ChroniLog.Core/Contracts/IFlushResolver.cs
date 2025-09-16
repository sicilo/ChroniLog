using ChroniLog.Core.Models;

namespace ChroniLog.Core.Contracts;

public interface IFlushResolver
{
    Task FlushAsync(List<ChroniLogEntry> logs, CancellationToken stoppingToken);
}