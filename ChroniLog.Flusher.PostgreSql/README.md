<img src="https://raw.githubusercontent.com/sicilo/ChroniLog/master/ChroniLog.Core/logo.png" alt="ChroniLog Logo" width="150"/>

# ChroniLog Flush PostgreSQL 🎯

## Installation

Add the following NuGet packages to your project:

```bash
dotnet add package Npgsql
dotnet add package ChroniLog.Core
dotnet add package ChroniLog.Flusher.PostgreSql
```

## Configuration

1. **Register `appsettings.json` configuration**

   In the code below, you can see `ChroniLogOptions`, which is mandatory if you want to customize buffer and database table name options, and `ConnectionOptions`, which is just an example for the next steps about how to provide `NpgsqlDataSource` using dependency injection.
 
    ```json
   {
        "Logging": {
            "LogLevel": {
                "Default": "Information",
                "Microsoft.AspNetCore": "Warning"
            }
        },
        "AllowedHosts": "*",
        "ConnectionOptions": {
          "Database":"Host=<your_host>;Port=<your_port>;Database=<your_db>;Username=<your_user>;Password=<your_pass>"
        },
        "ChroniLogOptions": {
            "BufferCapacity": 50,
            "BufferFlushInterval": 10,
            "TableName": "audit"
        }
    }
   ```
   
2. **Register `NpgsqlDataSource` with Dependency Injection**

   ```csharp 
   //ConnectionOptions appSettings Model
    public class ConnectionOptions
    {
        public string Database { get; set; } = null!;
    }
   ```

   ```csharp 
   //Configure ConnectionOptions
    public class ConfigureConnectionOptions(IConfiguration configuration) : IConfigureOptions<ConnectionOptions>
    {
        public void Configure(ConnectionOptions options)
        {
            configuration.GetSection(nameof(ConnectionOptions)).Bind(options);
        }
    }
   ```

   ```csharp 
   //Configure ConnectionOptions
    public static class DatabaseExtensions
    {
        public static IServiceCollection AddDatabase(this IServiceCollection builder)
        {
            builder.ConfigureOptions<ConfigureConnectionOptions>();
    
                builder.AddSingleton<NpgsqlDataSource>(serviceProvider =>
                {
                    var options = serviceProvider.GetRequiredService<IOptions<ConnectionOptions>>().Value;
                    var npgsqlDataSourceBuilder = new NpgsqlDataSourceBuilder(options.Database);
                    return npgsqlDataSourceBuilder.Build();
                });
            
            return builder;
        }
    }
   ```

   ```csharp 
   //Program.cs
    var builder = WebApplication.CreateBuilder(args);
    builder.Services.AddDatabase();
   ```

3. **Register `AddNpgLog` with Dependency Injection**
   ```csharp 
   //Program.cs
    var builder = WebApplication.CreateBuilder(args);
    builder.Services.AddDatabase();
    builder.Logging.AddNpgLog();
   ```


## Notes

* `Npgsql` provides PostgreSQL database handler.
* `ChroniLog.Core` provides the core logging infrastructure.
* `ChroniLog.Flusher.PostgreSql` is the PostgreSQL-specific implementation.
* Ensure your PostgreSQL database and connection string are properly configured.

---
