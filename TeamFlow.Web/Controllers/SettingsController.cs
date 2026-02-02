using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TeamFlow.Web.Data;
using TeamFlow.Web.Models;

namespace TeamFlow.Web.Controllers;

[Authorize(Policy = "AdminOnly")]
public class SettingsController : Controller
{
    private readonly ApplicationDbContext _db;

    public SettingsController(ApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<IActionResult> Index()
    {
        var settings = await _db.AppSettings.FirstOrDefaultAsync() ?? new AppSettings();
        return View(settings);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Index(AppSettings model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var existing = await _db.AppSettings.FirstOrDefaultAsync();
        if (existing == null)
        {
            _db.AppSettings.Add(model);
        }
        else
        {
            existing.OrganizationName = model.OrganizationName;
            existing.DefaultStatus = model.DefaultStatus;
            existing.EnableInMeeting = model.EnableInMeeting;
            existing.RequireStatusNote = model.RequireStatusNote;
            existing.AutoOfflineHours = model.AutoOfflineHours;
            existing.TimeZoneId = model.TimeZoneId;
            existing.AdminEmail = model.AdminEmail;
            existing.SessionTimeoutMinutes = model.SessionTimeoutMinutes;
            existing.AllowSelfRegistration = model.AllowSelfRegistration;
        }

        await _db.SaveChangesAsync();
        ViewData["StatusMessage"] = "Settings saved.";
        return View(model);
    }
}
