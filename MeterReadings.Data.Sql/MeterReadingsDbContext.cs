using MeterReadings.Data.Sql.Entities;
using Microsoft.EntityFrameworkCore;

namespace MeterReadings.Data.Sql;

public class MeterReadingsDbContext(DbContextOptions<MeterReadingsDbContext> options) : DbContext(options)
{
    public DbSet<Account> Accounts { get; set; }
    public DbSet<MeterReading> MeterReadings { get; set; }
}
