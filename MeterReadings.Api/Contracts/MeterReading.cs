namespace MeterReadings.Api.Contracts;

public class MeterReading
{
    public int? AccountId { get; set; }
    public DateTime? ReadingDateTime { get; set; }
    public int? Value { get; set; }
}
