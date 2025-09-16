using ChroniLog.Core.Models;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Npgsql;

namespace ChroniLog.Flusher.PostgreSql;

internal class ChroniLogInitializer :IHostedService
{
    private readonly NpgsqlDataSource _npgsqlDataSource;
    private readonly ChroniLogOptions _options;

    public ChroniLogInitializer(NpgsqlDataSource npgsqlDataSource, IOptions<ChroniLogOptions> options)
    {
        _npgsqlDataSource = npgsqlDataSource;
        _options = options.Value;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await using var connection = await _npgsqlDataSource.OpenConnectionAsync(cancellationToken);

        var createTableQuery = $"""
                                create table if not exists {_options.TableName} (
                                    event_id int,
                                    event_name text,
                                    category text,
                                    log_level text not null,
                                    message text not null,
                                    exception text null,
                                    created_at timestamptz not null default now()
                                );
                                """;
        
        await using var createTableCommand = new NpgsqlCommand(createTableQuery, connection);
        
        await createTableCommand.ExecuteNonQueryAsync(cancellationToken);
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}