using MeterReadings.Api.Handlers;
using MeterReadings.Api.Requests;
using MeterReadings.Core;
using MeterReadings.Core.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MeterReadingContract = MeterReadings.Api.Contracts.MeterReading;
using MeterReadingModel = MeterReadings.Core.Models.MeterReading;
using MeterReadingUploadResultContract = MeterReadings.Api.Contracts.MeterReadingUploadResult;

namespace MeterReadings.Api.Tests.Handlers;

public class UploadMeterReadingFileRequestHandlerTests
{
    private readonly MeterReadingModel _defaultMeterReadingModel = new MeterReadingModel(0, 0, new DateTime(2020, 1, 1), 1);

    private readonly MeterReadingContract _validMeterReadingContract1 = new MeterReadingContract() { AccountId = 1, ReadingDateTime = new DateTime(2020, 1, 1), Value = 2101 };
    private readonly MeterReadingContract _validMeterReadingContract2 = new MeterReadingContract() { AccountId = 3, ReadingDateTime = new DateTime(2020, 1, 2), Value = 123 };
    private readonly MeterReadingContract _validMeterReadingContract3 = new MeterReadingContract() { AccountId = 5, ReadingDateTime = new DateTime(2020, 1, 1), Value = 52353 };

    private readonly MeterReadingModel _validMeterReadingModel1 = new MeterReadingModel(0, 1, new DateTime(2020, 1, 1), 2101);
    private readonly MeterReadingModel _validMeterReadingModel2 = new MeterReadingModel(0, 3, new DateTime(2020, 1, 2), 123);
    private readonly MeterReadingModel _validMeterReadingModel3 = new MeterReadingModel(0, 5, new DateTime(2020, 1, 1), 52353);


    private readonly MeterReadingContract _invalidMeterReadingContract_NullAccount = new MeterReadingContract() { AccountId = null, ReadingDateTime = new DateTime(2020, 1, 1), Value = 2101 };

    private readonly Mock<IFormFile> _defaultMockFile;
    private readonly UploadMeterReadingFileRequest _defaultRequest;
    private readonly CancellationToken _defaultCancellationToken;


    public UploadMeterReadingFileRequestHandlerTests()
    {
        _defaultMockFile = new Mock<IFormFile>();
        _defaultRequest = new UploadMeterReadingFileRequest(_defaultMockFile.Object);
        _defaultCancellationToken = new CancellationTokenSource().Token;
    }

    [Fact]
    public void CanCreate()
    {
        var exception = Record.Exception(() => new UploadMeterReadingFileRequestHandler(Mock.Of<IParser<MeterReadingContract>>(), Mock.Of<IMeterReadingRepository>(), Mock.Of<ILogger<UploadMeterReadingFileRequestHandler>>()));

        Assert.Null(exception);
    }

    [Fact]
    public async Task HandleAsync_WithAllValid_ReturnsUploadedCount()
    {
        var mockParser = new Mock<IParser<MeterReadingContract>>();
        var mockRepository = new Mock<IMeterReadingRepository>();
        var handler = new UploadMeterReadingFileRequestHandler(mockParser.Object, mockRepository.Object, Mock.Of<ILogger<UploadMeterReadingFileRequestHandler>>());

        var parsedResults = new List<MeterReadingContract>()
        {
            _validMeterReadingContract1,
            _validMeterReadingContract2,
            _validMeterReadingContract3
        };

        mockParser.Setup(x => x.ParseMany(It.IsAny<IFormFile>()))
                  .Returns(parsedResults);

        mockRepository.Setup(x => x.SaveAsync(It.IsAny<MeterReadingModel>(), It.IsAny<CancellationToken>()))
                      .ReturnsAsync(_defaultMeterReadingModel);

        var result = await handler.HandleAsync(_defaultRequest, _defaultCancellationToken);

        Assert.NotNull(result);
        Assert.Equal(3, result.UploadedCount);
        Assert.Equal(0, result.FailedCount);

        mockParser.Verify(x => x.ParseMany(_defaultMockFile.Object), Times.Once);
        mockParser.VerifyNoOtherCalls();
        mockRepository.Verify(x => x.SaveAsync(_validMeterReadingModel1, _defaultCancellationToken), Times.Once);
        mockRepository.Verify(x => x.SaveAsync(_validMeterReadingModel2, _defaultCancellationToken), Times.Once);
        mockRepository.Verify(x => x.SaveAsync(_validMeterReadingModel3, _defaultCancellationToken), Times.Once);
        mockRepository.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task HandleAsync_WithOneInValidWithoutAccount_ReturnsUploadedCountAndFailedCount()
    {
        var mockParser = new Mock<IParser<MeterReadingContract>>();
        var mockRepository = new Mock<IMeterReadingRepository>();
        var handler = new UploadMeterReadingFileRequestHandler(mockParser.Object, mockRepository.Object, Mock.Of<ILogger<UploadMeterReadingFileRequestHandler>>());

        var parsedResults = new List<MeterReadingContract>()
        {
            _validMeterReadingContract1,
            new MeterReadingContract() { AccountId = null, ReadingDateTime = new DateTime(2020, 1, 1), Value = 2101 },
            _validMeterReadingContract3
        };

        mockParser.Setup(x => x.ParseMany(It.IsAny<IFormFile>()))
                  .Returns(parsedResults);

        mockRepository.Setup(x => x.SaveAsync(It.IsAny<MeterReadingModel>(), It.IsAny<CancellationToken>()))
                      .ReturnsAsync(_defaultMeterReadingModel);

        var result = await handler.HandleAsync(_defaultRequest, _defaultCancellationToken);

        Assert.NotNull(result);
        Assert.Equal(2, result.UploadedCount);
        Assert.Equal(1, result.FailedCount);

        mockParser.Verify(x => x.ParseMany(_defaultMockFile.Object), Times.Once);
        mockParser.VerifyNoOtherCalls();
        mockRepository.Verify(x => x.SaveAsync(_validMeterReadingModel1, _defaultCancellationToken), Times.Once);
        mockRepository.Verify(x => x.SaveAsync(_validMeterReadingModel3, _defaultCancellationToken), Times.Once);
        mockRepository.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task HandleAsync_WithOneInValidWithZeroAccount_ReturnsUploadedCountAndFailedCount()
    {
        var mockParser = new Mock<IParser<MeterReadingContract>>();
        var mockRepository = new Mock<IMeterReadingRepository>();
        var handler = new UploadMeterReadingFileRequestHandler(mockParser.Object, mockRepository.Object, Mock.Of<ILogger<UploadMeterReadingFileRequestHandler>>());

        var parsedResults = new List<MeterReadingContract>()
        {
            _validMeterReadingContract1,
            new MeterReadingContract() { AccountId = 0, ReadingDateTime = new DateTime(2020, 1, 1), Value = 2101 },
            _validMeterReadingContract3
        };

        mockParser.Setup(x => x.ParseMany(It.IsAny<IFormFile>()))
                  .Returns(parsedResults);

        mockRepository.Setup(x => x.SaveAsync(It.IsAny<MeterReadingModel>(), It.IsAny<CancellationToken>()))
                      .ReturnsAsync(_defaultMeterReadingModel);

        var result = await handler.HandleAsync(_defaultRequest, _defaultCancellationToken);

        Assert.NotNull(result);
        Assert.Equal(2, result.UploadedCount);
        Assert.Equal(1, result.FailedCount);

        mockParser.Verify(x => x.ParseMany(_defaultMockFile.Object), Times.Once);
        mockParser.VerifyNoOtherCalls();
        mockRepository.Verify(x => x.SaveAsync(_validMeterReadingModel1, _defaultCancellationToken), Times.Once);
        mockRepository.Verify(x => x.SaveAsync(_validMeterReadingModel3, _defaultCancellationToken), Times.Once);
        mockRepository.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task HandleAsync_WithOneInValidWithNullDate_ReturnsUploadedCountAndFailedCount()
    {
        var mockParser = new Mock<IParser<MeterReadingContract>>();
        var mockRepository = new Mock<IMeterReadingRepository>();
        var handler = new UploadMeterReadingFileRequestHandler(mockParser.Object, mockRepository.Object, Mock.Of<ILogger<UploadMeterReadingFileRequestHandler>>());

        var parsedResults = new List<MeterReadingContract>()
        {
            _validMeterReadingContract1,
            new MeterReadingContract() { AccountId = 1, ReadingDateTime = null, Value = 2101 },
            _validMeterReadingContract3
        };

        mockParser.Setup(x => x.ParseMany(It.IsAny<IFormFile>()))
                  .Returns(parsedResults);

        mockRepository.Setup(x => x.SaveAsync(It.IsAny<MeterReadingModel>(), It.IsAny<CancellationToken>()))
                      .ReturnsAsync(_defaultMeterReadingModel);

        var result = await handler.HandleAsync(_defaultRequest, _defaultCancellationToken);

        Assert.NotNull(result);
        Assert.Equal(2, result.UploadedCount);
        Assert.Equal(1, result.FailedCount);

        mockParser.Verify(x => x.ParseMany(_defaultMockFile.Object), Times.Once);
        mockParser.VerifyNoOtherCalls();
        mockRepository.Verify(x => x.SaveAsync(_validMeterReadingModel1, _defaultCancellationToken), Times.Once);
        mockRepository.Verify(x => x.SaveAsync(_validMeterReadingModel3, _defaultCancellationToken), Times.Once);
        mockRepository.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task HandleAsync_WithOneInValidWithMinDate_ReturnsUploadedCountAndFailedCount()
    {
        var mockParser = new Mock<IParser<MeterReadingContract>>();
        var mockRepository = new Mock<IMeterReadingRepository>();
        var handler = new UploadMeterReadingFileRequestHandler(mockParser.Object, mockRepository.Object, Mock.Of<ILogger<UploadMeterReadingFileRequestHandler>>());

        var parsedResults = new List<MeterReadingContract>()
        {
            _validMeterReadingContract1,
            new MeterReadingContract() { AccountId = 1, ReadingDateTime = DateTime.MinValue, Value = 2101 },
            _validMeterReadingContract3
        };

        mockParser.Setup(x => x.ParseMany(It.IsAny<IFormFile>()))
                  .Returns(parsedResults);

        mockRepository.Setup(x => x.SaveAsync(It.IsAny<MeterReadingModel>(), It.IsAny<CancellationToken>()))
                      .ReturnsAsync(_defaultMeterReadingModel);

        var result = await handler.HandleAsync(_defaultRequest, _defaultCancellationToken);

        Assert.NotNull(result);
        Assert.Equal(2, result.UploadedCount);
        Assert.Equal(1, result.FailedCount);

        mockParser.Verify(x => x.ParseMany(_defaultMockFile.Object), Times.Once);
        mockParser.VerifyNoOtherCalls();
        mockRepository.Verify(x => x.SaveAsync(_validMeterReadingModel1, _defaultCancellationToken), Times.Once);
        mockRepository.Verify(x => x.SaveAsync(_validMeterReadingModel3, _defaultCancellationToken), Times.Once);
        mockRepository.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task HandleAsync_WithOneInValidWithNullValue_ReturnsUploadedCountAndFailedCount()
    {
        var mockParser = new Mock<IParser<MeterReadingContract>>();
        var mockRepository = new Mock<IMeterReadingRepository>();
        var handler = new UploadMeterReadingFileRequestHandler(mockParser.Object, mockRepository.Object, Mock.Of<ILogger<UploadMeterReadingFileRequestHandler>>());

        var parsedResults = new List<MeterReadingContract>()
        {
            _validMeterReadingContract1,
            new MeterReadingContract() { AccountId = 1, ReadingDateTime = new DateTime(2020,1,1), Value = null },
            _validMeterReadingContract3
        };

        mockParser.Setup(x => x.ParseMany(It.IsAny<IFormFile>()))
                  .Returns(parsedResults);

        mockRepository.Setup(x => x.SaveAsync(It.IsAny<MeterReadingModel>(), It.IsAny<CancellationToken>()))
                      .ReturnsAsync(_defaultMeterReadingModel);

        var result = await handler.HandleAsync(_defaultRequest, _defaultCancellationToken);

        Assert.NotNull(result);
        Assert.Equal(2, result.UploadedCount);
        Assert.Equal(1, result.FailedCount);

        mockParser.Verify(x => x.ParseMany(_defaultMockFile.Object), Times.Once);
        mockParser.VerifyNoOtherCalls();
        mockRepository.Verify(x => x.SaveAsync(_validMeterReadingModel1, _defaultCancellationToken), Times.Once);
        mockRepository.Verify(x => x.SaveAsync(_validMeterReadingModel3, _defaultCancellationToken), Times.Once);
        mockRepository.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task HandleAsync_WithOneInValidWithNegativeValue_ReturnsUploadedCountAndFailedCount()
    {
        var mockParser = new Mock<IParser<MeterReadingContract>>();
        var mockRepository = new Mock<IMeterReadingRepository>();
        var handler = new UploadMeterReadingFileRequestHandler(mockParser.Object, mockRepository.Object, Mock.Of<ILogger<UploadMeterReadingFileRequestHandler>>());

        var parsedResults = new List<MeterReadingContract>()
        {
            _validMeterReadingContract1,
            new MeterReadingContract() { AccountId = 1, ReadingDateTime = new DateTime(2020,1,1), Value = -1 },
            _validMeterReadingContract3
        };

        mockParser.Setup(x => x.ParseMany(It.IsAny<IFormFile>()))
                  .Returns(parsedResults);

        mockRepository.Setup(x => x.SaveAsync(It.IsAny<MeterReadingModel>(), It.IsAny<CancellationToken>()))
                      .ReturnsAsync(_defaultMeterReadingModel);

        var result = await handler.HandleAsync(_defaultRequest, _defaultCancellationToken);

        Assert.NotNull(result);
        Assert.Equal(2, result.UploadedCount);
        Assert.Equal(1, result.FailedCount);

        mockParser.Verify(x => x.ParseMany(_defaultMockFile.Object), Times.Once);
        mockParser.VerifyNoOtherCalls();
        mockRepository.Verify(x => x.SaveAsync(_validMeterReadingModel1, _defaultCancellationToken), Times.Once);
        mockRepository.Verify(x => x.SaveAsync(_validMeterReadingModel3, _defaultCancellationToken), Times.Once);
        mockRepository.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task HandleAsync_WithOneInValidWithZeroValue_ReturnsUploadedCountAndFailedCount()
    {
        var mockParser = new Mock<IParser<MeterReadingContract>>();
        var mockRepository = new Mock<IMeterReadingRepository>();
        var handler = new UploadMeterReadingFileRequestHandler(mockParser.Object, mockRepository.Object, Mock.Of<ILogger<UploadMeterReadingFileRequestHandler>>());

        var parsedResults = new List<MeterReadingContract>()
        {
            _validMeterReadingContract1,
            new MeterReadingContract() { AccountId = 1, ReadingDateTime = new DateTime(2020,1,1), Value = 0 },
            _validMeterReadingContract3
        };

        mockParser.Setup(x => x.ParseMany(It.IsAny<IFormFile>()))
                  .Returns(parsedResults);

        mockRepository.Setup(x => x.SaveAsync(It.IsAny<MeterReadingModel>(), It.IsAny<CancellationToken>()))
                      .ReturnsAsync(_defaultMeterReadingModel);

        var result = await handler.HandleAsync(_defaultRequest, _defaultCancellationToken);

        Assert.NotNull(result);
        Assert.Equal(2, result.UploadedCount);
        Assert.Equal(1, result.FailedCount);

        mockParser.Verify(x => x.ParseMany(_defaultMockFile.Object), Times.Once);
        mockParser.VerifyNoOtherCalls();
        mockRepository.Verify(x => x.SaveAsync(_validMeterReadingModel1, _defaultCancellationToken), Times.Once);
        mockRepository.Verify(x => x.SaveAsync(_validMeterReadingModel3, _defaultCancellationToken), Times.Once);
        mockRepository.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task HandleAsync_WithOneInValidWithLargeValue_ReturnsUploadedCountAndFailedCount()
    {
        var mockParser = new Mock<IParser<MeterReadingContract>>();
        var mockRepository = new Mock<IMeterReadingRepository>();
        var handler = new UploadMeterReadingFileRequestHandler(mockParser.Object, mockRepository.Object, Mock.Of<ILogger<UploadMeterReadingFileRequestHandler>>());

        var parsedResults = new List<MeterReadingContract>()
        {
            _validMeterReadingContract1,
            new MeterReadingContract() { AccountId = 1, ReadingDateTime = new DateTime(2020,1,1), Value = 9999999 },
            _validMeterReadingContract3
        };

        mockParser.Setup(x => x.ParseMany(It.IsAny<IFormFile>()))
                  .Returns(parsedResults);

        mockRepository.Setup(x => x.SaveAsync(It.IsAny<MeterReadingModel>(), It.IsAny<CancellationToken>()))
                      .ReturnsAsync(_defaultMeterReadingModel);

        var result = await handler.HandleAsync(_defaultRequest, _defaultCancellationToken);

        Assert.NotNull(result);
        Assert.Equal(2, result.UploadedCount);
        Assert.Equal(1, result.FailedCount);

        mockParser.Verify(x => x.ParseMany(_defaultMockFile.Object), Times.Once);
        mockParser.VerifyNoOtherCalls();
        mockRepository.Verify(x => x.SaveAsync(_validMeterReadingModel1, _defaultCancellationToken), Times.Once);
        mockRepository.Verify(x => x.SaveAsync(_validMeterReadingModel3, _defaultCancellationToken), Times.Once);
        mockRepository.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task HandleAsync_WithAllInValid_ReturnsUploadedCount()
    {
        var mockParser = new Mock<IParser<MeterReadingContract>>();
        var mockRepository = new Mock<IMeterReadingRepository>();
        var handler = new UploadMeterReadingFileRequestHandler(mockParser.Object, mockRepository.Object, Mock.Of<ILogger<UploadMeterReadingFileRequestHandler>>());

        var parsedResults = new List<MeterReadingContract>()
        {
            new MeterReadingContract() { AccountId = 1, ReadingDateTime = new DateTime(2020,1,1), Value = 0 },
            new MeterReadingContract() { AccountId = 1, ReadingDateTime = DateTime.MinValue, Value = 2101 }
        };

        mockParser.Setup(x => x.ParseMany(It.IsAny<IFormFile>()))
                  .Returns(parsedResults);

        mockRepository.Setup(x => x.SaveAsync(It.IsAny<MeterReadingModel>(), It.IsAny<CancellationToken>()))
                      .ReturnsAsync(_defaultMeterReadingModel);

        var result = await handler.HandleAsync(_defaultRequest, _defaultCancellationToken);

        Assert.NotNull(result);
        Assert.Equal(0, result.UploadedCount);
        Assert.Equal(2, result.FailedCount);

        mockParser.Verify(x => x.ParseMany(_defaultMockFile.Object), Times.Once);
        mockParser.VerifyNoOtherCalls();
        mockRepository.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task HandleAsync_WithSomeInValidDueToFailedPersistance_ReturnsUploadedCount()
    {
        var mockParser = new Mock<IParser<MeterReadingContract>>();
        var mockRepository = new Mock<IMeterReadingRepository>();
        var handler = new UploadMeterReadingFileRequestHandler(mockParser.Object, mockRepository.Object, Mock.Of<ILogger<UploadMeterReadingFileRequestHandler>>());

        var parsedResults = new List<MeterReadingContract>()
        {
            _validMeterReadingContract1,
            _validMeterReadingContract2
        };

        mockParser.Setup(x => x.ParseMany(It.IsAny<IFormFile>()))
                  .Returns(parsedResults);

        mockRepository.Setup(x => x.SaveAsync(_validMeterReadingModel1, It.IsAny<CancellationToken>()))
                      .ThrowsAsync(new PersistanceException());

        mockRepository.Setup(x => x.SaveAsync(_validMeterReadingModel2, It.IsAny<CancellationToken>()))
                      .ReturnsAsync(_defaultMeterReadingModel);

        var result = await handler.HandleAsync(_defaultRequest, _defaultCancellationToken);

        Assert.NotNull(result);
        Assert.Equal(1, result.UploadedCount);
        Assert.Equal(1, result.FailedCount);

        mockParser.Verify(x => x.ParseMany(_defaultMockFile.Object), Times.Once);
        mockParser.VerifyNoOtherCalls();
        mockRepository.Verify(x => x.SaveAsync(_validMeterReadingModel1, _defaultCancellationToken), Times.Once);
        mockRepository.Verify(x => x.SaveAsync(_validMeterReadingModel2, _defaultCancellationToken), Times.Once);
        mockRepository.VerifyNoOtherCalls();
    }
}

