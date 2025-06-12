using CsvHelper;
using CsvHelper.Configuration;
using MeterReadings.Core.Models;
using Microsoft.VisualBasic.FileIO;
using System.Globalization;

namespace MeterReadings.UploadAccounts.Console;

public class AccountCsvParser
{
    public IEnumerable<Account> ParseMany(string filePath)
    {
        using var streamReader = new StreamReader(filePath);
        using var csvReader = new CsvReader(streamReader, CultureInfo.InvariantCulture);

        csvReader.Context.RegisterClassMap<NewAccountClassMap>();

        return csvReader.GetRecords<NewAccount>().Select(Map).ToArray();
    }

    private Account Map(NewAccount newAccount)
    {
        return new Account(newAccount.Id, newAccount.FirstName, newAccount.LastName);
    }

    private sealed class NewAccountClassMap : ClassMap<NewAccount>
    {
        public NewAccountClassMap()
        {
            Map(x => x.Id).Name("AccountId");
            Map(x => x.FirstName).Name("FirstName");
            Map(x => x.LastName).Name("LastName");
        }
    }

    public class NewAccount 
    { 
        public int Id { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
    }

}
