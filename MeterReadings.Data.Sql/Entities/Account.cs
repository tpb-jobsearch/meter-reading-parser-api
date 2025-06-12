namespace MeterReadings.Data.Sql.Entities;
public class Account
{
    public int Id { get; set; }

    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;

    public ICollection<MeterReading> MeterReadings { get; set; } = null!;
}
