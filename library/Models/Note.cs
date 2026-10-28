using System.ComponentModel.DataAnnotations;

namespace library.Models;

public class Note
{
    public long NoteId { get; set; }

    [Required]
    [StringLength(100)]
    public string Title { get; set; } = "";

    public string? Body { get; set; }

    public DateOnly CreatedOn { get; set; }
}