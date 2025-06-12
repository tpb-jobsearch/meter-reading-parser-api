namespace MeterReadings.Api.Contracts;

public class MeterReadingUploadResult
{
    public int UploadedCount { get; set; } = 0;
    public int FailedCount { get; set; } = 0;
}
