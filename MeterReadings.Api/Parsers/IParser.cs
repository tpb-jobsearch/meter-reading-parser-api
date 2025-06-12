namespace MeterReadings.Api;

public interface IParser<T>
{
    IEnumerable<T> ParseMany(IFormFile input);
}
