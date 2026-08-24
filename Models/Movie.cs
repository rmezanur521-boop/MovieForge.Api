namespace MovieForge.Api.Models;

public class Movie
{
    public int Index { get; set; }
    public string Title { get; set; } = string.Empty;
    public List<string> Actors { get; set; } = new();
    public int Year { get; set; }
    public string Genre { get; set; } = string.Empty;
}