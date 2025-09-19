using ChroniLog.Core.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace ChroniLog.Core.Configurations;

public class ConfigureChroniLogOptions(IConfiguration config) : IConfigureOptions<ChroniLogSettings>
{
    public void Configure(ChroniLogSettings settings)
    {
        config.GetSection(nameof(ChroniLogSettings)).Bind(settings);
    }
}