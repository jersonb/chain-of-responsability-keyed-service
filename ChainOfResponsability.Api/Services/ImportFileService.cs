using ChainOfResponsability.Api.Data;
using ChainOfResponsability.Api.Models;
using ChainOfResponsability.Api.Services.Validators;
using ClosedXML.Excel;

namespace ChainOfResponsability.Api.Services;

public class ImportFileService(ImportFileValidator validator, IEntryContext context)
{
    public async Task<IEnumerable<string>> Import(ImportFile importFile)
    {
        var errors = validator.Validate(importFile);

        if (errors.Any())
        {
            return errors;
        }

        var entries = GetEntries(importFile);
        context.Entries.AddRange(entries);
        await context.SaveChangesAsync();

        return [];
    }

    private static IEnumerable<Entry> GetEntries(ImportFile importFile)
    {
        var rows = importFile.Worksheet.RowsUsed().Skip(2);

        foreach (var row in rows)
        {
            var date = row.Cell(1).Value.GetDateTime();
            var description = row.Cell(2).Value.GetText();
            var value = row.Cell(3).Value.GetNumber();

            yield return new Entry
            {
                Date = date,
                Description = description,
                Value = (decimal)value
            };
        }
    }
}

public class ImportFile(Stream stream)
{
    public IXLWorksheet Worksheet => new XLWorkbook(stream).Worksheet(1);

    public static implicit operator ImportFile(Stream stream) => new(stream);
}