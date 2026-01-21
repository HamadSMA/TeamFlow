using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using TeamFlow.Web.Data;
using TeamFlow.Web.Models;

namespace TeamFlow.Web.Controllers;

[Authorize]
public class StatusController : Controller
{
    private readonly ApplicationDbContext _db;

    public StatusController(ApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<IActionResult> Index()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrWhiteSpace(userId))
        {
            return Challenge();
        }

        var user = await _db.Users.FirstOrDefaultAsync(u => u.Id == userId);
        if (user == null)
        {
            return Challenge();
        }

        var model = new StatusUpdateViewModel
        {
            CurrentStatus = user.CurrentStatus,
            StatusNote = user.StatusNote
        };

        ViewData["LastUpdated"] = user.StatusLastUpdatedAt;
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Update(StatusUpdateViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View("Index", model);
        }

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrWhiteSpace(userId))
        {
            return Challenge();
        }

        var user = await _db.Users.FirstOrDefaultAsync(u => u.Id == userId);
        if (user == null)
        {
            return Challenge();
        }

        var oldStatus = user.CurrentStatus;
        user.CurrentStatus = model.CurrentStatus;
        user.StatusNote = model.StatusNote;
        user.StatusLastUpdatedAt = DateTime.UtcNow;

        _db.StatusHistories.Add(new StatusHistory
        {
            UserId = user.Id,
            OldStatus = oldStatus,
            NewStatus = model.CurrentStatus,
            Note = model.StatusNote,
            Timestamp = user.StatusLastUpdatedAt.Value
        });

        await _db.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Current()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrWhiteSpace(userId))
        {
            return Challenge();
        }

        var user = await _db.Users.FirstOrDefaultAsync(u => u.Id == userId);
        if (user == null)
        {
            return Challenge();
        }

        return Json(new
        {
            status = user.CurrentStatus.ToString(),
            note = user.StatusNote,
            lastUpdatedAt = user.StatusLastUpdatedAt,
            lastUpdatedAtRiyadh = user.StatusLastUpdatedAt.HasValue
                ? TimeZoneInfo.ConvertTimeFromUtc(
                    DateTime.SpecifyKind(user.StatusLastUpdatedAt.Value, DateTimeKind.Utc),
                    TimeZoneInfo.FindSystemTimeZoneById("Asia/Riyadh"))
                : (DateTime?)null
        });
    }
}
