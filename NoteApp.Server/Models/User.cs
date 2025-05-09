using System.Text.Json.Serialization;
using NoteApp.Server.Models;

public class User
{
    public int Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public byte[] PasswordHash { get; set; } = Array.Empty<byte>();
    public byte[] PasswordSalt { get; set; } = Array.Empty<byte>();
    public string? RefreshToken { get; set; }
    public DateTime? RefreshTokenExpiry { get; set; }

    [JsonIgnore]
    public List<Note> Notes { get; set; } = new();

    public User() { }

    public User(string username, byte[] passwordHash, byte[] passwordSalt)
    {
        Username = username;
        PasswordHash = passwordHash;
        PasswordSalt = passwordSalt;
    }
}