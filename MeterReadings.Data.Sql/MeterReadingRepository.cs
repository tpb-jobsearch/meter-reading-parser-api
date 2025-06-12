using MeterReadings.Core;
using MeterReadings.Core.Exceptions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using MeterReadingEntity = MeterReadings.Data.Sql.Entities.MeterReading;
using MeterReadingModel = MeterReadings.Core.Models.MeterReading;

namespace MeterReadings.Data.Sql;

public class MeterReadingRepository : IMeterReadingRepository
{
    private readonly IDbContextFactory<MeterReadingsDbContext> _dbContextFactory;

    public MeterReadingRepository(IDbContextFactory<MeterReadingsDbContext> dbContextFactory)
    {
        _dbContextFactory = dbContextFactory;
    }

    public async Task<MeterReadingModel> SaveAsync(MeterReadingModel meterReading, CancellationToken cancellationToken)
    {
        using var dbContext = await _dbContextFactory.CreateDbContextAsync();
        var entity = Map(meterReading);

        await dbContext.MeterReadings.AddAsync(entity);

        try
        {
            await dbContext.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateException exception)
        {
            throw new PersistanceException("Failed to save Meter Reading.", exception);
        }

        return Map(entity);
    }

    private MeterReadingEntity Map(MeterReadingModel model)
    {
        return new MeterReadingEntity()
        {
            AccountId = model.AccountId,
            ReadingDateTime = model.ReadingDateTime,
            Value = model.Value,
        };
    }

    private MeterReadingModel Map(MeterReadingEntity model)
    {
        return new MeterReadingModel(model.Id, model.AccountId, model.ReadingDateTime, model.Value);
    }
}
