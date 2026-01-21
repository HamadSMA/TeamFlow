using System.ComponentModel.DataAnnotations;

namespace TeamFlow.Web.Models;

public class StatusUpdateViewModel
{
    [Required]
    public Status CurrentStatus { get; set; } = Status.Available;

    public string? StatusNote { get; set; }
}
