using Microsoft.Extensions.Logging;

namespace ChroniLog.Core.Models;

public sealed record ChroniLogEntry(
    LogLevel Level,
    EventId EventId,
    string Message,
    string? Exception,
    string? Category,
    DateTime TimestampUtc
);