using System.ComponentModel.DataAnnotations;

namespace TeamFlow.Web.Models;

public class AdminCreateUserViewModel
{
    [Required]
    public string FirstName { get; set; } = string.Empty;

    [Required]
    public string LastName { get; set; } = string.Empty;

    public int? TeamId { get; set; }

    public bool IsActive { get; set; } = true;
}
