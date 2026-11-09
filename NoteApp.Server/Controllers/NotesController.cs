using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using NoteApp.Server.Data;
using NoteApp.Server.Models;


[ApiController]
[Route("api/notes")]
[Authorize]
public class NotesController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly ILogger<NotesController> _logger;


    public NotesController(AppDbContext context, ILogger<NotesController> logger)
    {
        _context = context;
        _logger = logger;
    }


    [HttpPost]
    public async Task<ActionResult<NoteResponse>> CreateNote([FromBody] CreateNoteDto dto)
    {
        try
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdStr, out var userId))
            {
                return Unauthorized("Invalid user ID");
            }




            var note = new Note
            {
                Title = dto.Title,
                EncryptedContent = dto.EncryptedContent,
                UserId = userId,
                CreatedAt = DateTime.UtcNow
            };


            _context.Notes.Add(note);
            await _context.SaveChangesAsync();


            return Ok(new NoteResponse
            {
                Id = note.Id,
                Title = note.Title,
                EncryptedContent = note.EncryptedContent,
                CreatedAt = note.CreatedAt
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating note");
            return StatusCode(500, "Error creating note");
        }
    }


    [HttpGet]
    public async Task<ActionResult<IEnumerable<NoteResponse>>> GetUserNotes()
    {
        var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!int.TryParse(userIdStr, out var userId))
        {
            return Unauthorized("Invalid user ID");
        }




        var notes = await _context.Notes
            .Where(n => n.UserId == userId)
            .OrderByDescending(n => n.CreatedAt)
            .Select(n => new NoteResponse
            {
                Id = n.Id,
                Title = n.Title,
                EncryptedContent = n.EncryptedContent,
                CreatedAt = n.CreatedAt
            })
            .ToListAsync();


        return Ok(notes);
    }


    [HttpGet("{id}")]
    public async Task<ActionResult<NoteResponse>> GetNote(int id)
    {
        var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!int.TryParse(userIdStr, out var userId))
        {
            return Unauthorized("Invalid user ID");
        }




        var note = await _context.Notes
            .Where(n => n.Id == id && n.UserId == userId)
            .Select(n => new NoteResponse
            {
                Id = n.Id,
                Title = n.Title,
                EncryptedContent = n.EncryptedContent,
                CreatedAt = n.CreatedAt
            })
            .FirstOrDefaultAsync();


        return note != null ? Ok(note) : NotFound();
    }
        [HttpPut("{id}")]
    public async Task<ActionResult<NoteResponse>> UpdateNote(int id, [FromBody] CreateNoteDto dto)
    {
        var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!int.TryParse(userIdStr, out var userId))
        {
            return Unauthorized("Invalid user ID");
        }

        var note = await _context.Notes
            .FirstOrDefaultAsync(n => n.Id == id && n.UserId == userId);

        if (note == null)
        {
            return NotFound();
        }

        note.Title = dto.Title;
        note.EncryptedContent = dto.EncryptedContent;
        note.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return Ok(new NoteResponse
        {
            Id = note.Id,
            Title = note.Title,
            EncryptedContent = note.EncryptedContent,
            CreatedAt = note.CreatedAt
        });
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteNote(int id)
    {
        try
        {
            _logger.LogInformation($"Attempting to delete note with ID: {id}");

            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdStr, out var userId))
            {
                _logger.LogWarning("Invalid user ID format");
                return Unauthorized("Invalid user ID");
            }

            _logger.LogInformation($"User ID: {userId} attempting to delete note {id}");

            var note = await _context.Notes
                .FirstOrDefaultAsync(n => n.Id == id && n.UserId == userId);

            if (note == null)
            {
                _logger.LogWarning($"Note not found or doesn't belong to user. Note ID: {id}, User ID: {userId}");
                return NotFound();
            }

            _context.Notes.Remove(note);
            await _context.SaveChangesAsync();

            _logger.LogInformation($"Successfully deleted note ID: {id}");
            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error deleting note ID: {id}");
            return StatusCode(500, "Error deleting note");
        }
    }


}

    public class NoteResponse
{
    public int Id { get; set; }
    public string? Title { get; set; }
    public string? EncryptedContent { get; set; }
    public DateTime CreatedAt { get; set; }
}



