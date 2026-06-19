namespace BestApp.Core.Entities;

public class ContactMessage
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string Subject { get; set; } = null!;
    public string Message { get; set; } = null!;
    public DateTime SentDate { get; set; } = DateTime.UtcNow;
    public bool IsRead { get; set; } = false;
}