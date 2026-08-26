namespace MovieForge.Api.Models;

public class ReviewItem
{
    public string ReviewerName { get; set; } = string.Empty;
    public int Rating { get; set; }
    public string Text { get; set; } = string.Empty;
    public string Date { get; set; } = string.Empty;
}