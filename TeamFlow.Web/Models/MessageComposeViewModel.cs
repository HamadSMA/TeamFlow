using System.ComponentModel.DataAnnotations;

namespace TeamFlow.Web.Models;

public class MessageComposeViewModel
{
    [Required]
    public string RecipientId { get; set; } = string.Empty;

    [Required]
    [StringLength(120)]
    public string Subject { get; set; } = string.Empty;

    [Required]
    public string Body { get; set; } = string.Empty;
}
