namespace SmartX.Shared.Models;

public class SensorRegistration
{
    public string MacAddress { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public string Category { get; set; } = "Environmental";
    public DateTime RegisteredAt { get; set; } = DateTime.UtcNow;
    public List<string> Attachments { get; set; } = new();
}