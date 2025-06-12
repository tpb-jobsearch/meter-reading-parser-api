// See https://aka.ms/new-console-template for more information
using MeterReadings.UploadAccounts.Console;
using System.Text;

Console.WriteLine("Load Accounts Tool");

Console.WriteLine("Enter Test Accounts File Path");

var filePath = Console.ReadLine();

if(string.IsNullOrEmpty(filePath) && File.Exists(filePath))
{
    throw new ArgumentNullException("Invalid path.");
}

var parser = new AccountCsvParser();

var accounts = parser.ParseMany(filePath);

var builder = new StringBuilder();

foreach(var account in accounts)
{
    builder.AppendLine($"INSERT INTO Accounts (Id, FirstName, LastName) VALUES({account.Id}, '{account.FirstName}', '{account.LastName}')");
}

Console.WriteLine("SQL OUTPUT");
Console.WriteLine();

var sqlFilePath = Path.Combine(Path.GetDirectoryName(filePath), $"{Path.GetFileNameWithoutExtension(filePath)}.sql");
File.WriteAllText(sqlFilePath, builder.ToString());

Console.WriteLine($"Data written to file [{sqlFilePath}]");