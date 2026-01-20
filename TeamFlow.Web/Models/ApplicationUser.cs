using Microsoft.AspNetCore.Identity;

namespace TeamFlow.Web.Models;

public class ApplicationUser : IdentityUser
{
    // Soft-disable user (required feature)
    public bool IsActive { get; set; } = true;

    public int? TeamId { get; set; }
    public Team? Team { get; set; }

    public Status CurrentStatus { get; set; } = Status.Available;
    public string? StatusNote { get; set; }
    public DateTime? StatusLastUpdatedAt { get; set; }
}
