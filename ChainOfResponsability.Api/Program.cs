using ChainOfResponsability.Api.Data;
using ChainOfResponsability.Api.Services;
using ChainOfResponsability.Api.Services.Validators;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<IEntryContext, EntryContext>();

builder.Services.AddScoped<IImportFileValidator, HeaderValidator>();
builder.Services.AddScoped<IImportFileValidator, FirstColumnValidator>();
builder.Services.AddScoped<IImportFileValidator, AlreadyExistsEntry>();

builder.Services.AddScoped<ImportFileValidator>();
builder.Services.AddScoped<ImportFileService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

await app.RunAsync();