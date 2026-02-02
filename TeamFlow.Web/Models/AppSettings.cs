using System.ComponentModel.DataAnnotations;

namespace TeamFlow.Web.Models;

public class AppSettings
{
    public int Id { get; set; }

    [Required]
    public string OrganizationName { get; set; } = "TeamFlow";

    public Status DefaultStatus { get; set; } = Status.Available;

    public bool EnableInMeeting { get; set; } = true;

    public bool RequireStatusNote { get; set; }

    public int? AutoOfflineHours { get; set; }

    [Required]
    public string TimeZoneId { get; set; } = "Asia/Riyadh";

    [Required]
    public string AdminEmail { get; set; } = "admin@teamflow.local";

    public int SessionTimeoutMinutes { get; set; } = 60;

    public bool AllowSelfRegistration { get; set; } = true;
}
