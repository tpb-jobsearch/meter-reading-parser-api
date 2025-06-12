using MeterReadings.Core.Models;

namespace MeterReadings.Core;

public interface IMeterReadingRepository
{
    Task<MeterReading> SaveAsync(MeterReading meterReading, CancellationToken cancellationToken);
}
