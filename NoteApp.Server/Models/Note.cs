using System;
using System.Text.Json.Serialization;

namespace NoteApp.Server.Models
{
    public class Note
    {
        public int Id { get; set; }
        public string? Title { get; set; }
        public string? EncryptedContent { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public int UserId { get; set; }
        public User User { get; set; } = null!;
    }

}
