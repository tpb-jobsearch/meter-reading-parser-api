namespace MeterReadings.Core.Exceptions;

public class PersistanceException : Exception
{
    public PersistanceException() { }

    public PersistanceException(string? message) : base(message) { }

    public PersistanceException(string? message, Exception? innerException) : base(message, innerException) { }
}
