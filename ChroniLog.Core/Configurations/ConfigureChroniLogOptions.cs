using ChroniLog.Core.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace ChroniLog.Core.Configurations;

public class ConfigureChroniLogOptions(IConfiguration config) : IConfigureOptions<ChroniLogOptions>
{
    public void Configure(ChroniLogOptions options)
    {
        config.GetSection(nameof(ChroniLogOptions)).Bind(options);
    }
}