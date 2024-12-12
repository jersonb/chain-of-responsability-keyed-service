using ChainOfResponsability.Api.Models;

namespace ChainOfResponsability.Api.Data;

public class EntryContext(ILogger<EntryContext> logger) : IEntryContext
{
    public List<Entry> Entries { get; set; } = [];

    public Task SaveChangesAsync()
    {
        logger.LogInformation("Saved {@Count}", Entries.Count);
        return Task.CompletedTask;
    }
}