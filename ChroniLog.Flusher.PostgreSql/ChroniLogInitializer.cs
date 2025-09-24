using ChroniLog.Core.Models;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Npgsql;

namespace ChroniLog.Flusher.PostgreSql;

internal class ChroniLogInitializer : IHostedService
{
    private readonly NpgsqlDataSource _npgsqlDataSource;
    private readonly ChroniLogSettings _settings;

    public ChroniLogInitializer(NpgsqlDataSource npgsqlDataSource, IOptions<ChroniLogSettings> settings)
    {
        _npgsqlDataSource = npgsqlDataSource;
        _settings = settings.Value;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await using var connection = await _npgsqlDataSource.OpenConnectionAsync(cancellationToken);
        await using var transaction = await connection.BeginTransactionAsync(cancellationToken);

        try
        {
            if (!string.IsNullOrEmpty(_settings.SchemaName))
            {
                var createSchemaQuery = $"create schema if not exists {_settings.SchemaName};";
                await using var createSchemaCommand = new NpgsqlCommand(createSchemaQuery, connection, transaction);
                await createSchemaCommand.ExecuteNonQueryAsync(cancellationToken);
            }

            var tableName = string.IsNullOrEmpty(_settings.SchemaName)
                ? _settings.TableName
                : $"{_settings.SchemaName}.{_settings.TableName}";

            var createTableQuery = $"""
                                    create table if not exists {tableName} (
                                        event_id int,
                                        event_name text,
                                        category text,
                                        log_level text not null,
                                        message text not null,
                                        exception text null,
                                        created_at timestamptz not null default now()
                                    );
                                    """;

            await using var createTableCommand = new NpgsqlCommand(createTableQuery, connection, transaction);

            await createTableCommand.ExecuteNonQueryAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);
        }
        catch (Exception e)
        {
            await transaction.RollbackAsync(cancellationToken);
            Console.WriteLine(e);
            throw;
        }
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}