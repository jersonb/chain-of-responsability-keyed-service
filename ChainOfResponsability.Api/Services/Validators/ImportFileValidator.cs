using ChainOfResponsability.Api.Data;

namespace ChainOfResponsability.Api.Services.Validators;

public class ImportFileValidator(IEnumerable<IImportFileValidator> fileValidators)
{
    public IEnumerable<string> Validate(ImportFile importFile)
    {
        foreach (var validator in fileValidators)
        {
            var validation = validator.Validate(importFile);

            if (validation.Any())
            {
                return validation;
            }
        }

        return [];
    }
}

public interface IImportFileValidator
{
    IEnumerable<string> Validate(ImportFile importFile);
}

internal sealed class AlreadyExistsEntry(IEntryContext context) : IImportFileValidator
{
    public IEnumerable<string> Validate(ImportFile importFile)
    {
        var ids = importFile.Worksheet
              .Column(4)
              .CellsUsed()
              .Skip(1)
              .Select(x => x.Value.GetNumber());

        var checkIdExists = context.Entries.Any(entry => ids.Contains(entry.Id));

        if (checkIdExists)
        {
            return ["Algum Id já esxiste no no banco"];
        }
        return [];
    }
}

internal sealed class HeaderValidator : IImportFileValidator
{
    public IEnumerable<string> Validate(ImportFile importFile)
    {
        var headers = importFile.Worksheet.RowsUsed().Skip(1).First();

        var checkHeaderDate = headers.Cell(1).Value.GetText() != "Data";
        var checkHeaderDescription = headers.Cell(2).Value.GetText() != "Descrição";
        var checkHeaderValue = headers.Cell(3).Value.GetText() != "Valor";

        if (checkHeaderDate || checkHeaderDescription || checkHeaderValue)
        {
            return ["Falha no cabeçalho as colunas devem se chamar Data, Descrição e Valor."];
        }
        return [];
    }
}

internal sealed class FirstColumnValidator : IImportFileValidator
{
    public IEnumerable<string> Validate(ImportFile importFile)
    {
        var firstColumn = importFile.Worksheet.FirstColumnUsed().CellsUsed().Skip(2);

        var checkAllValuesIsDateTime = firstColumn.All(x => x.Value.IsDateTime);

        if (checkAllValuesIsDateTime)
        {
            return [];
        }
        return ["Valores na primeira coluna devem ser do tipo data."];
    }
}
