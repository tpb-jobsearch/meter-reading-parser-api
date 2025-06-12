using Microsoft.EntityFrameworkCore;

namespace MeterReadings.Data.Sql.Entities;

[Index(nameof(AccountId), nameof(ReadingDateTime), nameof(Value), IsUnique = true)]
public class MeterReading
{
    public int Id { get; set; } 
    public int AccountId { get; set; }
    public Account Account { get; set; } = null!;
    public DateTime ReadingDateTime { get; set; }
    public int Value { get; set; }
}
