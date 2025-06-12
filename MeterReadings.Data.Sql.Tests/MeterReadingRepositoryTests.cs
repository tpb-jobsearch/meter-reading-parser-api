using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Moq;
using MeterReadingModel = MeterReadings.Core.Models.MeterReading;

namespace MeterReadings.Data.Sql.Tests;

public class MeterReadingRepositoryTests
{
    [Fact]
    public async Task SaveAsync_WithDataItem_SavesToContext()
    {
        using var dbConnection = new SqliteConnection("Filename=:memory:");
        dbConnection.Open();
        var contextOptionsBuilder = new DbContextOptionsBuilder<MeterReadingsDbContext>().UseSqlite(dbConnection);

        CreateDatabase(contextOptionsBuilder);
        InitializeAccounts(contextOptionsBuilder);

        var mockIDbContextFactory = new Mock<IDbContextFactory<MeterReadingsDbContext>>();
        mockIDbContextFactory.Setup(x => x.CreateDbContextAsync(It.IsAny<CancellationToken>()))
                             .ReturnsAsync(new MeterReadingsDbContext(contextOptionsBuilder.Options));

        var repository = new MeterReadingRepository(mockIDbContextFactory.Object);

        var data = new MeterReadingModel(0, 1, new DateTime(2020, 12, 13), 233);
        var cancellationToken = new CancellationTokenSource().Token;
        var result = await repository.SaveAsync(data, cancellationToken);

        using var dbContext = new MeterReadingsDbContext(contextOptionsBuilder.Options);
        var dbMeterReadingResult = dbContext.MeterReadings.FirstOrDefault();

        Assert.NotNull(dbMeterReadingResult);
        Assert.NotEqual(0, dbMeterReadingResult.Id);
        Assert.Equal(data.AccountId, dbMeterReadingResult.AccountId);
        Assert.Equal(data.ReadingDateTime, dbMeterReadingResult.ReadingDateTime);
        Assert.Equal(data.Value, dbMeterReadingResult.Value);
    }

    private void CreateDatabase(DbContextOptionsBuilder<MeterReadingsDbContext> contextOptionsBuilder)
    {
        using var dbContext = new MeterReadingsDbContext(contextOptionsBuilder.Options);

        dbContext.Database.EnsureCreated();
    }

    private void InitializeAccounts(DbContextOptionsBuilder<MeterReadingsDbContext> contextOptionsBuilder)
    {
        using var dbContext = new MeterReadingsDbContext(contextOptionsBuilder.Options);

        dbContext.Accounts.Add(new Entities.Account() { FirstName = "A", LastName = "B" });

        dbContext.SaveChanges();
    }
}
