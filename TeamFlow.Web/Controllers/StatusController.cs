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
    private readonly SettingsService _settings;

    public StatusController(ApplicationDbContext db, SettingsService settings)
    {
        _db = db;
        _settings = settings;
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

        var settings = await _settings.GetAsync();
        ViewData["StatusOptions"] = BuildStatusOptions(settings);
        ViewData["LastUpdated"] = user.StatusLastUpdatedAt;
        ViewData["TimeZoneId"] = settings.TimeZoneId;
        ViewData["RequireStatusNote"] = settings.RequireStatusNote;
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Update(StatusUpdateViewModel model)
    {
        var settings = await _settings.GetAsync();

        if (!settings.EnableInMeeting && model.CurrentStatus == Status.InMeeting)
        {
            ModelState.AddModelError(nameof(StatusUpdateViewModel.CurrentStatus), "InMeeting status is disabled.");
        }

        if (settings.RequireStatusNote && string.IsNullOrWhiteSpace(model.StatusNote))
        {
            ModelState.AddModelError(nameof(StatusUpdateViewModel.StatusNote), "Note is required.");
        }

        if (!ModelState.IsValid)
        {
            ViewData["StatusOptions"] = BuildStatusOptions(settings);
            ViewData["TimeZoneId"] = settings.TimeZoneId;
            ViewData["RequireStatusNote"] = settings.RequireStatusNote;
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

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> QuickUpdate(StatusUpdateViewModel model, string? returnUrl = null)
    {
        var settings = await _settings.GetAsync();
        if (!settings.EnableInMeeting && model.CurrentStatus == Status.InMeeting)
        {
            TempData["StatusError"] = "InMeeting status is disabled.";
            return Redirect(returnUrl ?? Url.Action(nameof(Index))!);
        }

        if (settings.RequireStatusNote && string.IsNullOrWhiteSpace(model.StatusNote))
        {
            TempData["StatusError"] = "Note is required.";
            return Redirect(returnUrl ?? Url.Action(nameof(Index))!);
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
        TempData["StatusSuccess"] = "Status updated.";
        return Redirect(returnUrl ?? Url.Action(nameof(Index))!);
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

        var timeZone = await _settings.GetTimeZoneAsync();
        return Json(new
        {
            status = user.CurrentStatus.ToString(),
            note = user.StatusNote,
            lastUpdatedAt = user.StatusLastUpdatedAt,
            lastUpdatedAtLocal = user.StatusLastUpdatedAt.HasValue
                ? TimeZoneInfo.ConvertTimeFromUtc(
                    DateTime.SpecifyKind(user.StatusLastUpdatedAt.Value, DateTimeKind.Utc),
                    timeZone)
                : (DateTime?)null
        });
    }

    private List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem> BuildStatusOptions(AppSettings settings)
    {
        var options = new List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem>
        {
            new("Available", Status.Available.ToString()),
            new("Busy", Status.Busy.ToString())
        };

        if (settings.EnableInMeeting)
        {
            options.Add(new("InMeeting", Status.InMeeting.ToString()));
        }

        options.Add(new("Offline", Status.Offline.ToString()));

        return options;
    }
}
