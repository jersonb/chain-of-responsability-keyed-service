using System.ComponentModel.DataAnnotations;

namespace ChainOfResponsability.Api.Models;

public class Entry
{
    [Key]
    public int Id { get; init; }

    public DateTime Date { get; init; }
    public string Description { get; init; } = string.Empty;
    public decimal Value { get; init; }
}