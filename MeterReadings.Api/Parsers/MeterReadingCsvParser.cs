using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.TypeConversion;
using System.Globalization;
using MeterReadingContract = MeterReadings.Api.Contracts.MeterReading;

namespace MeterReadings.Api.Parsers;

public class MeterReadingCsvParser : IParser<MeterReadingContract>
{
    public IEnumerable<MeterReadingContract> ParseMany(IFormFile input)
    {
        using var stream = input.OpenReadStream();
        using var streamReader = new StreamReader(stream);  
        using var csvReader = new CsvReader(streamReader, CultureInfo.InvariantCulture);

        csvReader.Context.RegisterClassMap<MeterReadingContractClassMap>();

        return csvReader.GetRecords<MeterReadingContract>().ToArray();
    }

    private sealed class MeterReadingContractClassMap : ClassMap<MeterReadingContract> 
    {
        public MeterReadingContractClassMap()
        {
            Map(x => x.AccountId).Name("AccountId").TypeConverter<IntConverter>();
            Map(x => x.ReadingDateTime).Name("MeterReadingDateTime").TypeConverter<DateTimeConverter>();
            Map(x => x.Value).Name("MeterReadValue").TypeConverter<IntConverter>();
        }
    }

    public class DateTimeConverter : DefaultTypeConverter
    {
        public override object? ConvertFromString(string? text, IReaderRow row, MemberMapData memberMapData)
        {
            if(DateTime.TryParseExact(text, "dd/MM/yyyy HH:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out var dateTime))
            {
                return dateTime;
            }

            return null;
        }
    }

    public class IntConverter : DefaultTypeConverter
    {
        public override object? ConvertFromString(string? text, IReaderRow row, MemberMapData memberMapData)
        {
            if (int.TryParse(text, out var value))
            {
                return value;
            }

            return null;
        }
    }
}
