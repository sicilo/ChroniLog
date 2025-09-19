namespace ChroniLog.Core.Models;

public class ChroniLogSettings
{
    public int ChannelCapacity { get; set; } = 1000;
    public int BufferCapacity { get; set; } = 50;
    public int BufferFlushInterval { get; set; } = 10;
    public string? SchemaName { get; set; }
    public string TableName { get; set; } =  "logs";
}