using System.ComponentModel.DataAnnotations;

namespace TeamFlow.Web.Models;

public class Team
{
    public int Id { get; set; }

    [Required]
    public string Name { get; set; } = string.Empty;

    public List<ApplicationUser> Users { get; set; } = new();
}
