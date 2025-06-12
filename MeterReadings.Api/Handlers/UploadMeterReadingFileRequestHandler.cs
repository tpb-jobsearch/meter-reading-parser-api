using MeterReadings.Api.Requests;
using MeterReadings.Core;
using MeterReadings.Core.Exceptions;
using MeterReadings.Core.Models;
using MeterReadingContract = MeterReadings.Api.Contracts.MeterReading;
using MeterReadingUploadResultContract = MeterReadings.Api.Contracts.MeterReadingUploadResult;

namespace MeterReadings.Api.Handlers;

public class UploadMeterReadingFileRequestHandler
{
    private readonly IParser<MeterReadingContract> _parser;
    private readonly IMeterReadingRepository _meterReadingRepository;
    private readonly ILogger<UploadMeterReadingFileRequestHandler> _logger;

    public UploadMeterReadingFileRequestHandler(IParser<MeterReadingContract> parser,
                                                IMeterReadingRepository meterReadingRepository,
                                                ILogger<UploadMeterReadingFileRequestHandler> logger)
    {
        _parser = parser;
        _meterReadingRepository = meterReadingRepository;
        _logger = logger;
    }

    public async Task<MeterReadingUploadResultContract> HandleAsync(UploadMeterReadingFileRequest request, CancellationToken cancellationToken)
    {
        var validMeterReadings = new List<MeterReadingContract>();
        var invalidMeterReadings = new List<MeterReadingContract>();

        var parsedResults = _parser.ParseMany(request.File);
        foreach (var reading in parsedResults)
        {
            var successful = await TryAddMeterReadingAsync(reading, cancellationToken);

            if (successful)
            {
                validMeterReadings.Add(reading);
            }
            else
            {
                _logger.LogError("Failed to process meter reading for Account [{reading_account}], Date [{reading_date}, Value [{reading_value}]]", reading.AccountId, reading.ReadingDateTime, reading.Value);
                invalidMeterReadings.Add(reading);
            }
        }

        return new MeterReadingUploadResultContract
        {
            UploadedCount = validMeterReadings.Count,
            FailedCount = invalidMeterReadings.Count,
        };
    }

    private bool Validate(MeterReadingContract meterReading)
    {
        return meterReading.AccountId.HasValue && meterReading.AccountId.Value > 0 &&
               meterReading.ReadingDateTime.HasValue && meterReading.ReadingDateTime.Value != DateTime.MinValue &&
               meterReading.Value.HasValue && meterReading.Value.Value > 0 && meterReading.Value.Value < 100000;
        }

    private async Task<bool> TryAddMeterReadingAsync(MeterReadingContract meterReading, CancellationToken cancellationToken)
    {
        if (Validate(meterReading))
        {
            try
            {
                await _meterReadingRepository.SaveAsync(MapToModel(meterReading), cancellationToken);
                return true;
            }
            catch (PersistanceException) { }
        }

        return false;
    }

    private static MeterReading MapToModel(MeterReadingContract newMeterReading)
    {
        return new MeterReading(0, newMeterReading.AccountId!.Value, newMeterReading.ReadingDateTime!.Value, newMeterReading.Value!.Value);
    }
}
