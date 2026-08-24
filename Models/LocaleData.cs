namespace MovieForge.Api.Models;

public class LocaleData
{
    public List<string> TitleAdjectives { get; set; } = new();
    public List<string> TitleNouns { get; set; } = new();
    public List<string> FirstNames { get; set; } = new();
    public List<string> LastNames { get; set; } = new();
    public List<string> Genres { get; set; } = new();
    public List<string> ReviewPhrases { get; set; } = new();
}