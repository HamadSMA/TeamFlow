using System.ComponentModel.DataAnnotations;

namespace TeamFlow.Web.Models;

public class UserTeamEditViewModel
{
    [Required]
    public string UserId { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public int? TeamId { get; set; }

    public List<Team> Teams { get; set; } = new();
}
