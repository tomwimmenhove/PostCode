namespace PostCode.Dto;

public class SearchResultItemDto
{
    public string Id { get; set; } = default!;
    public string Type { get; set; } = default!;
    public string Text { get; set; } = default!;
    public string Highlight { get; set; } = default!;
    public string Description { get; set; } = default!;
}

public class SearchResultDto
{
    public SearchResultItemDto[] Items { get; set; } = default!;
}
