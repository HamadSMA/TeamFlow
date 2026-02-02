using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TeamFlow.Web.Data;
using TeamFlow.Web.Models;

namespace TeamFlow.Web.Controllers;

[Authorize]
public class DashboardController : Controller
{
    private readonly ApplicationDbContext _db;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SettingsService _settings;

    public DashboardController(ApplicationDbContext db, UserManager<ApplicationUser> userManager, SettingsService settings)
    {
        _db = db;
        _userManager = userManager;
        _settings = settings;
    }

    public async Task<IActionResult> Index()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrWhiteSpace(userId))
        {
            return Challenge();
        }

        var user = await _db.Users
            .Include(u => u.Team)
            .FirstOrDefaultAsync(u => u.Id == userId);
        if (user == null)
        {
            return Challenge();
        }

        var isAdmin = await _userManager.IsInRoleAsync(user, "Admin");
        List<Team> teams;
        DashboardStatsViewModel stats;

        if (isAdmin)
        {
            teams = await _db.Teams
                .Include(t => t.Users)
                .OrderBy(t => t.Users.Count == 0)
                .ThenBy(t => t.Name)
                .ToListAsync();

            stats = new DashboardStatsViewModel
            {
                TeamMembers = await _db.Users.CountAsync(u => u.TeamId != null),
                ActiveUsers = await _db.Users.CountAsync(u => u.IsActive),
                AvailableUsers = await _db.Users.CountAsync(u => u.IsActive && u.CurrentStatus == Status.Available),
                BusyUsers = await _db.Users.CountAsync(u => u.IsActive && u.CurrentStatus == Status.Busy),
                AwayUsers = await _db.Users.CountAsync(u => u.IsActive && u.CurrentStatus == Status.InMeeting),
                TotalUsers = await _db.Users.CountAsync(),
                InactiveUsers = await _db.Users.CountAsync(u => !u.IsActive),
                UsersWithoutTeam = await _db.Users.CountAsync(u => u.TeamId == null),
                TotalTeams = await _db.Teams.CountAsync(),
                StatusUpdatesLast24h = await _db.StatusHistories.CountAsync(h => h.Timestamp >= DateTime.UtcNow.AddHours(-24))
            };
        }
        else if (user.TeamId.HasValue)
        {
            teams = await _db.Teams
                .Where(t => t.Id == user.TeamId.Value)
                .Include(t => t.Users)
                .ToListAsync();
            teams = teams
                .OrderBy(t => t.Users.Count == 0)
                .ThenBy(t => t.Name)
                .ToList();

            var teamId = user.TeamId.Value;
            stats = new DashboardStatsViewModel
            {
                TeamMembers = await _db.Users.CountAsync(u => u.TeamId == teamId),
                ActiveUsers = await _db.Users.CountAsync(u => u.TeamId == teamId && u.IsActive),
                AvailableUsers = await _db.Users.CountAsync(u => u.TeamId == teamId && u.IsActive && u.CurrentStatus == Status.Available),
                BusyUsers = await _db.Users.CountAsync(u => u.TeamId == teamId && u.IsActive && u.CurrentStatus == Status.Busy),
                AwayUsers = await _db.Users.CountAsync(u => u.TeamId == teamId && u.IsActive && u.CurrentStatus == Status.InMeeting)
            };
        }
        else
        {
            teams = new List<Team>();
            stats = new DashboardStatsViewModel();
        }

        var model = new DashboardViewModel
        {
            IsAdmin = isAdmin,
            Teams = teams,
            Stats = stats
        };

        ViewData["TimeZoneId"] = (await _settings.GetAsync()).TimeZoneId;
        return View(model);
    }
}
