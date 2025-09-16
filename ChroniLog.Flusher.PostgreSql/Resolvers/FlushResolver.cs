using ChroniLog.Core.Contracts;
using ChroniLog.Core.Models;
using Microsoft.Extensions.Options;
using Npgsql;

namespace ChroniLog.Flusher.PostgreSql.Resolvers;

public class FlushResolver : IFlushResolver
{
    private readonly NpgsqlDataSource _npgsqlDataSource;
    private readonly ChroniLogOptions _options;

    public FlushResolver(
        NpgsqlDataSource npgsqlDataSource,
        IOptions<ChroniLogOptions> options
            )
    {
        _npgsqlDataSource = npgsqlDataSource;
        _options = options.Value;
    }
    public async Task FlushAsync(List<ChroniLogEntry> logs, CancellationToken stoppingToken)
    {
        await using var connection = await _npgsqlDataSource.OpenConnectionAsync(stoppingToken);

        var query = $"""
                     COPY {_options.TableName} (event_id, event_name, category, log_level, message, exception, created_at)
                     FROM STDIN (FORMAT BINARY)
                     """;

        await using var writer = await connection.BeginBinaryImportAsync(query, stoppingToken);

        try
        {
            foreach (var npgLogEntry in logs)
            {
                await writer.StartRowAsync(stoppingToken);
                await writer.WriteAsync(npgLogEntry.EventId.Id, NpgsqlTypes.NpgsqlDbType.Integer, stoppingToken);
                await writer.WriteAsync(npgLogEntry.EventId.Name, NpgsqlTypes.NpgsqlDbType.Text, stoppingToken);
                await writer.WriteAsync(npgLogEntry.Category, NpgsqlTypes.NpgsqlDbType.Text, stoppingToken);
                await writer.WriteAsync(npgLogEntry.Level.ToString(), NpgsqlTypes.NpgsqlDbType.Text, stoppingToken);
                await writer.WriteAsync(npgLogEntry.Message, NpgsqlTypes.NpgsqlDbType.Text, stoppingToken);
                await writer.WriteAsync(npgLogEntry.Exception, NpgsqlTypes.NpgsqlDbType.Text, stoppingToken);
                await writer.WriteAsync(npgLogEntry.TimestampUtc, NpgsqlTypes.NpgsqlDbType.TimestampTz, stoppingToken);
            }

            await writer.CompleteAsync(stoppingToken);
        }
        catch
        {
            await writer.CloseAsync(stoppingToken);
            throw;
        }
    }
}