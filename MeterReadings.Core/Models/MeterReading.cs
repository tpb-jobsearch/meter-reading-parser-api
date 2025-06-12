namespace MeterReadings.Core.Models;

public record MeterReading(int Id, int AccountId, DateTime ReadingDateTime, int Value);
