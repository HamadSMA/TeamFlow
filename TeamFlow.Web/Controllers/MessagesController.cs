using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TeamFlow.Web.Data;
using TeamFlow.Web.Models;

namespace TeamFlow.Web.Controllers;

[Authorize]
public class MessagesController : Controller
{
    private readonly ApplicationDbContext _db;
    private readonly SettingsService _settings;

    public MessagesController(ApplicationDbContext db, SettingsService settings)
    {
        _db = db;
        _settings = settings;
    }

    public async Task<IActionResult> Inbox()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrWhiteSpace(userId))
        {
            return Challenge();
        }

        var messages = await _db.Messages
            .Include(m => m.Sender)
            .Where(m => m.RecipientId == userId)
            .OrderByDescending(m => m.SentAt)
            .ToListAsync();

        ViewData["TimeZoneId"] = (await _settings.GetAsync()).TimeZoneId;
        return View(messages);
    }

    public async Task<IActionResult> Sent()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrWhiteSpace(userId))
        {
            return Challenge();
        }

        var messages = await _db.Messages
            .Include(m => m.Recipient)
            .Where(m => m.SenderId == userId)
            .OrderByDescending(m => m.SentAt)
            .ToListAsync();

        ViewData["TimeZoneId"] = (await _settings.GetAsync()).TimeZoneId;
        return View(messages);
    }

    public async Task<IActionResult> Compose(string? recipientId = null, string? subject = null)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrWhiteSpace(userId))
        {
            return Challenge();
        }

        var recipients = await _db.Users
            .Where(u => u.Id != userId)
            .OrderBy(u => u.Email)
            .ToListAsync();

        ViewData["Recipients"] = recipients;
        return View(new MessageComposeViewModel
        {
            RecipientId = recipientId,
            Subject = subject
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Compose(MessageComposeViewModel model)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrWhiteSpace(userId))
        {
            return Challenge();
        }

        var recipients = await _db.Users
            .Where(u => u.Id != userId)
            .OrderBy(u => u.Email)
            .ToListAsync();
        ViewData["Recipients"] = recipients;

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var message = new Message
        {
            SenderId = userId,
            RecipientId = model.RecipientId,
            Subject = model.Subject,
            Body = model.Body,
            SentAt = DateTime.UtcNow,
            IsRead = false
        };

        _db.Messages.Add(message);
        await _db.SaveChangesAsync();

        return RedirectToAction(nameof(Sent));
    }

    public async Task<IActionResult> View(int id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrWhiteSpace(userId))
        {
            return Challenge();
        }

        var message = await _db.Messages
            .Include(m => m.Sender)
            .Include(m => m.Recipient)
            .FirstOrDefaultAsync(m => m.Id == id);

        if (message == null)
        {
            return NotFound();
        }

        if (message.RecipientId != userId && message.SenderId != userId)
        {
            return Forbid();
        }

        if (!message.IsRead && message.RecipientId == userId)
        {
            message.IsRead = true;
            await _db.SaveChangesAsync();
        }

        ViewData["TimeZoneId"] = (await _settings.GetAsync()).TimeZoneId;
        return View(message);
    }

    [HttpGet]
    public async Task<IActionResult> HasNew(long? afterTicks)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrWhiteSpace(userId))
        {
            return Unauthorized();
        }

        if (afterTicks == null || afterTicks <= 0)
        {
            return Json(new { hasNew = false });
        }

        var latest = await _db.Messages
            .Where(m => m.RecipientId == userId)
            .OrderByDescending(m => m.SentAt)
            .Select(m => m.SentAt)
            .FirstOrDefaultAsync();

        var latestTicks = DateTime.SpecifyKind(latest, DateTimeKind.Utc).Ticks;
        return Json(new { hasNew = latestTicks > afterTicks });
    }
}
