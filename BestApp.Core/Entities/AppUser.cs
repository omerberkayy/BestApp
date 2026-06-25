namespace BestApp.Core.Entities;

public class AppUser
{
    public int Id { get; set; }
    public string Username { get; set; } = null!;
    public string PasswordHash { get; set; } = null!; // Şifrenin SHA256 hali
    public string Role { get; set; } = "Admin";
    public bool IsActive { get; set; } = true;
}