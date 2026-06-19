namespace PeyzajApp.Core.Entities;

public class Service
{
    public int Id { get; set; }
    public string Title { get; set; } = null!;
    public string Description { get; set; } = null!;
    public string? IconClass { get; set; } // Örn: "fa-leaf" (FontAwesome ikonları için)
    public bool IsActive { get; set; } = true;
}