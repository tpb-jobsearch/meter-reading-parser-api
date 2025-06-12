using MeterReadings.Api.Parsers;
using Microsoft.AspNetCore.Http;
using Moq;

namespace MeterReadings.Api.Tests.Parsers;

public class MeterReadingCsvParserTests
{
    [Fact]
    public void ParseMany_AllValid_ReturnsContracts()
    {
        var parser = new MeterReadingCsvParser();

        var mockFile = new Mock<IFormFile>();
        mockFile.Setup(x => x.OpenReadStream())
                .Returns(CreateStreamFrom(MeterReadingCsvParserTestsResources.ParseMany_AllValid_ReturnsContracts));

        var results = parser.ParseMany(mockFile.Object);

        Assert.NotNull(results);
        Assert.Equal(3, results.Count());
        Assert.NotNull(results.SingleOrDefault(x => x.AccountId == 1 && x.Value == 2 && x.ReadingDateTime == new DateTime(2020, 12, 25, 12, 34, 0)));
        Assert.NotNull(results.SingleOrDefault(x => x.AccountId == 2 && x.Value == 123 && x.ReadingDateTime == new DateTime(2020, 12, 26, 12, 34, 0)));
        Assert.NotNull(results.SingleOrDefault(x => x.AccountId == 3 && x.Value == 432 && x.ReadingDateTime == new DateTime(2020, 12, 25, 12, 34, 0)));
    }

    [Fact]
    public void ParseMany_SingleMissingAccount_ReturnsContracts()
    {
        var parser = new MeterReadingCsvParser();

        var mockFile = new Mock<IFormFile>();
        mockFile.Setup(x => x.OpenReadStream())
                .Returns(CreateStreamFrom(MeterReadingCsvParserTestsResources.ParseMany_SingleMissingAccount_ReturnsContracts));

        var results = parser.ParseMany(mockFile.Object);

        Assert.NotNull(results);
        Assert.Equal(3, results.Count());
        Assert.NotNull(results.SingleOrDefault(x => x.AccountId == null && x.Value == 2 && x.ReadingDateTime == new DateTime(2020, 12, 25, 12, 34, 0)));
        Assert.NotNull(results.SingleOrDefault(x => x.AccountId == 2 && x.Value == 123 && x.ReadingDateTime == new DateTime(2020, 12, 26, 12, 34, 0)));
        Assert.NotNull(results.SingleOrDefault(x => x.AccountId == 3 && x.Value == 432 && x.ReadingDateTime == new DateTime(2020, 12, 25, 12, 34, 0)));
    }

    [Fact]
    public void ParseMany_SingleInvalidAccount_ReturnsContracts()
    {
        var parser = new MeterReadingCsvParser();

        var mockFile = new Mock<IFormFile>();
        mockFile.Setup(x => x.OpenReadStream())
                .Returns(CreateStreamFrom(MeterReadingCsvParserTestsResources.ParseMany_SingleInvalidAccount_ReturnsContracts));

        var results = parser.ParseMany(mockFile.Object);

        Assert.NotNull(results);
        Assert.Equal(3, results.Count());
        Assert.NotNull(results.SingleOrDefault(x => x.AccountId == null && x.Value == 2 && x.ReadingDateTime == new DateTime(2020, 12, 25, 12, 34, 0)));
        Assert.NotNull(results.SingleOrDefault(x => x.AccountId == 2 && x.Value == 123 && x.ReadingDateTime == new DateTime(2020, 12, 26, 12, 34, 0)));
        Assert.NotNull(results.SingleOrDefault(x => x.AccountId == 3 && x.Value == 432 && x.ReadingDateTime == new DateTime(2020, 12, 25, 12, 34, 0)));
    }

    [Fact]
    public void ParseMany_SingleMissingValue_ReturnsContracts()
    {
        var parser = new MeterReadingCsvParser();

        var mockFile = new Mock<IFormFile>();
        mockFile.Setup(x => x.OpenReadStream())
                .Returns(CreateStreamFrom(MeterReadingCsvParserTestsResources.ParseMany_SingleMissingValue_ReturnsContracts));

        var results = parser.ParseMany(mockFile.Object);

        Assert.NotNull(results);
        Assert.Equal(3, results.Count());
        Assert.NotNull(results.SingleOrDefault(x => x.AccountId == 1 && x.Value == 2 && x.ReadingDateTime == new DateTime(2020, 12, 25, 12, 34, 0)));
        Assert.NotNull(results.SingleOrDefault(x => x.AccountId == 2 && x.Value == null && x.ReadingDateTime == new DateTime(2020, 12, 26, 12, 34, 0)));
        Assert.NotNull(results.SingleOrDefault(x => x.AccountId == 3 && x.Value == 432 && x.ReadingDateTime == new DateTime(2020, 12, 25, 12, 34, 0)));
    }

    [Fact]
    public void ParseMany_SingleInvalidValue_ReturnsContracts()
    {
        var parser = new MeterReadingCsvParser();

        var mockFile = new Mock<IFormFile>();
        mockFile.Setup(x => x.OpenReadStream())
                .Returns(CreateStreamFrom(MeterReadingCsvParserTestsResources.ParseMany_SingleInvalidValue_ReturnsContracts));

        var results = parser.ParseMany(mockFile.Object);

        Assert.NotNull(results);
        Assert.Equal(3, results.Count());
        Assert.NotNull(results.SingleOrDefault(x => x.AccountId == 1 && x.Value == 2 && x.ReadingDateTime == new DateTime(2020, 12, 25, 12, 34, 0)));
        Assert.NotNull(results.SingleOrDefault(x => x.AccountId == 2 && x.Value == null && x.ReadingDateTime == new DateTime(2020, 12, 26, 12, 34, 0)));
        Assert.NotNull(results.SingleOrDefault(x => x.AccountId == 3 && x.Value == 432 && x.ReadingDateTime == new DateTime(2020, 12, 25, 12, 34, 0)));
    }

    [Fact]
    public void ParseMany_SingleInvalidDate_ReturnsContracts()
    {
        var parser = new MeterReadingCsvParser();

        var mockFile = new Mock<IFormFile>();
        mockFile.Setup(x => x.OpenReadStream())
                .Returns(CreateStreamFrom(MeterReadingCsvParserTestsResources.ParseMany_SingleInvalidDate_ReturnsContracts));

        var results = parser.ParseMany(mockFile.Object);

        Assert.NotNull(results);
        Assert.Equal(3, results.Count());
        Assert.NotNull(results.SingleOrDefault(x => x.AccountId == 1 && x.Value == 2 && x.ReadingDateTime == new DateTime(2020, 12, 25, 12, 34, 0)));
        Assert.NotNull(results.SingleOrDefault(x => x.AccountId == 2 && x.Value == 123 && x.ReadingDateTime == null));
        Assert.NotNull(results.SingleOrDefault(x => x.AccountId == 3 && x.Value == 432 && x.ReadingDateTime == new DateTime(2020, 12, 25, 12, 34, 0)));
    }

    [Fact]
    public void ParseMany_SingleMissingDate_ReturnsContracts()
    {
        var parser = new MeterReadingCsvParser();

        var mockFile = new Mock<IFormFile>();
        mockFile.Setup(x => x.OpenReadStream())
                .Returns(CreateStreamFrom(MeterReadingCsvParserTestsResources.ParseMany_SingleMissingDate_ReturnsContracts));

        var results = parser.ParseMany(mockFile.Object);

        Assert.NotNull(results);
        Assert.Equal(3, results.Count());
        Assert.NotNull(results.SingleOrDefault(x => x.AccountId == 1 && x.Value == 2 && x.ReadingDateTime == new DateTime(2020, 12, 25, 12, 34, 0)));
        Assert.NotNull(results.SingleOrDefault(x => x.AccountId == 2 && x.Value == 123 && x.ReadingDateTime == null));
        Assert.NotNull(results.SingleOrDefault(x => x.AccountId == 3 && x.Value == 432 && x.ReadingDateTime == new DateTime(2020, 12, 25, 12, 34, 0)));
    }

    private Stream CreateStreamFrom(string data)
    {
        var stream = new MemoryStream();
        var streamWriter = new StreamWriter(stream);

        streamWriter.Write(data);
        streamWriter.Flush();
        stream.Position = 0;

        return stream;
    }
}
