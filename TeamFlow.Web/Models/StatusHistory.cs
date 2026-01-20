using System.ComponentModel.DataAnnotations;

namespace TeamFlow.Web.Models;

public class StatusHistory
{
    public int Id { get; set; }

    [Required]
    public string UserId { get; set; } = string.Empty;
    public ApplicationUser? User { get; set; }

    public Status OldStatus { get; set; }
    public Status NewStatus { get; set; }

    public string? Note { get; set; }

    public DateTime Timestamp { get; set; }
}
