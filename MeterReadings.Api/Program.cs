using MeterReadings.Api;
using MeterReadings.Api.Handlers;
using MeterReadings.Api.Parsers;
using MeterReadings.Api.Requests;
using MeterReadings.Core;
using MeterReadings.Data.Sql;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;
using MeterReadingContract = MeterReadings.Api.Contracts.MeterReading;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddAntiforgery();

builder.Logging.AddConsole();

builder.Services.AddDbContextFactory<MeterReadingsDbContext>(db => db.UseSqlServer(builder.Configuration.GetConnectionString("MeterReadingsDatabase")));

builder.Services.AddScoped<UploadMeterReadingFileRequestHandler>();

builder.Services.AddScoped<IMeterReadingRepository, MeterReadingRepository>();

builder.Services.AddScoped<IParser<MeterReadingContract>, MeterReadingCsvParser>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseHttpsRedirection();
app.UseAntiforgery();

app.MapPost("/meter-reading-uploads", (IFormFile file, UploadMeterReadingFileRequestHandler handler, CancellationToken cancellationToken) => handler.HandleAsync(new UploadMeterReadingFileRequest(file), cancellationToken))
   .DisableAntiforgery(); 

app.Run();
