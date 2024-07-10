namespace Chameleon.Av.Fluent.Common.Models;

public class PageBaseModel
{
    public string? Header { get; init; }

    public string? Description { get; init; }

    public string? IconResourceKey { get; init; }

    public string? PageKey { get; init; }

    public string[]? SearchKeywords { get; init; }
}
