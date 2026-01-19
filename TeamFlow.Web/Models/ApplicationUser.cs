using Microsoft.AspNetCore.Identity;

namespace TeamFlow.Web.Models;

public class ApplicationUser : IdentityUser
{
    // Soft-disable user (required feature)
    public bool IsActive { get; set; } = true;

    // Team relationship will be added later
    public int? TeamId { get; set; }
}
