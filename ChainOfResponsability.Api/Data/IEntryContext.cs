using ChainOfResponsability.Api.Models;

namespace ChainOfResponsability.Api.Data;

public interface IEntryContext
{
    List<Entry> Entries { get; set; }

    Task SaveChangesAsync();
}