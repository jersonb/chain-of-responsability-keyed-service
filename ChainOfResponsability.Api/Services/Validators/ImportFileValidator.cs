using ChainOfResponsability.Api.Data;

namespace ChainOfResponsability.Api.Services.Validators;

public class ImportFileValidator(IEntryContext context)
{
    public IEnumerable<string> Validate(ImportFile importFile)
    {
        var headerValidator = new HeaderValidator(importFile);
        var firstColumnValidator = new FirstColumnValidator(importFile);
        var alreadyExistsEntry = new AlreadyExistsEntry(importFile, context);
        firstColumnValidator.SetNextValidator(alreadyExistsEntry);
        headerValidator.SetNextValidator(firstColumnValidator);
        return headerValidator.Validate();
    }
}

internal sealed class AlreadyExistsEntry(ImportFile importFile, IEntryContext context) : ImportFileValidatorBase(importFile)
{
    public override IEnumerable<string> DescribeValidation()
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

internal sealed class HeaderValidator(ImportFile importFile) : ImportFileValidatorBase(importFile)
{
    public override IEnumerable<string> DescribeValidation()
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

internal sealed class FirstColumnValidator(ImportFile importFile) : ImportFileValidatorBase(importFile)
{
    private readonly ImportFile importFile = importFile;

    public override IEnumerable<string> DescribeValidation()
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

internal abstract class ImportFileValidatorBase(ImportFile importFile)
{
    public IEnumerable<string> Validations { get; private set; } = [];

    private ImportFileValidatorBase? importFileValidatorBase;

    public void SetNextValidator(ImportFileValidatorBase nextValidator)
        => importFileValidatorBase = nextValidator;

    public abstract IEnumerable<string> DescribeValidation();

    public IEnumerable<string> Validate()
    {
        if (importFile == null)
        {
            return ["ImportFile cannot be null"];
        }

        Validations = DescribeValidation();

        if (Validations.Any())
        {
            return Validations;
        }

        if (importFileValidatorBase == null)
        {
            return [];
        }

        return importFileValidatorBase.Validate();
    }
}