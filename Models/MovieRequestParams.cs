namespace MovieForge.Api.Models;

public class MovieRequestParams
{
    public long Seed { get; set; }
    public string Locale { get; set; } = "en-US";
    public int Page { get; set; } = 0;
    public int PageSize { get; set; } = 10;
}