namespace BestApp.Core.Entities;

public class About
{
    public int Id { get; set; }
    public string Title { get; set; } = null!;
    public string Content { get; set; } = null!;
    public string? ImageUrl { get; set; }
}