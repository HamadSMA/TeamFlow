using System.ComponentModel.DataAnnotations;

namespace TeamFlow.Web.Models;

public class Message
{
    public int Id { get; set; }

    [Required]
    public string SenderId { get; set; } = string.Empty;
    public ApplicationUser? Sender { get; set; }

    [Required]
    public string RecipientId { get; set; } = string.Empty;
    public ApplicationUser? Recipient { get; set; }

    [Required]
    [StringLength(120)]
    public string Subject { get; set; } = string.Empty;

    [Required]
    public string Body { get; set; } = string.Empty;

    public DateTime SentAt { get; set; }

    public bool IsRead { get; set; }
}
