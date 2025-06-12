# Meter Readings Api

## Assumptions

I made the following assumptions in building this as I was unclear from the requirements that were given:

* Negative value readings are invalid and should not be submitted.
* That a reading of zero is invalid.
* The NNNNN for value in the requirements points towards less than 100000, rather than enforcing that the number must be five digits with leading zeroes, etc.
* That having mutiple readings a day is valid... but that the full record should not be repeated (regarding uniqueness of an entry).
* That a valid entry must have a valid date of reading.
* That there was no need to include APIs to import Accounts - since it was not requested.
* That the files supplied were representative (in terms of size) as to the real files the API would process.

## Notes

* Used `CsvHelper` nuget package for CSV Parsing (which also handles things like commas in strings so prevents issues).
* Abstracted parser such that it would be possible to have multiple parsers according to some criteria (i.e. if different companies have different formats).
* Implemented onion architecture such that the Persistance layer is kept separate from the logic and implementations.
* Added `Scalar` for ease of debugging / trial.
* Omitted Connection String from configuration because this should be set via variable injection in pipeline or from configured in AppConfiguration or KeyVault instances.
* As an improvement (which is not in the requirements so did not implement it), I would suggest persisting the raw file in some blob storage for historic referencing purposes.
* Integration tests should be added but did not have time in this implementation.
* Give this is a WebApi, not an MVC app, have disabled AntiForgery in favour that I would introduce auth tokens.
* Considered using a mediator library but all seemed overkill for the solution. Instead have opted for a handler object.
* Added basic Sqlite test for the database but this is always a challenging area to test.
* Adding basic logging for errors. But likely would extend to OpenTelemetry and more logging, warnings, etc as needed.

## Database Commands

### Create Database Migration

Navigate to root folder and run the following command (changing <MIGRATIONNAME> to the name of the migration).

```bat
dotnet ef migrations add "<MIGRATION__NAME>" --project "MeterReadings.Data.Sql" --startup-project "MeterReadings.Api"
```

### Update Database Command

Navigate to root folder and run the following command.

```bat
dotnet ef database update --project "MeterReadings.Data.Sql" --startup-project "MeterReadings.Api"
```
