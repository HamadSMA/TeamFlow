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

    public DashboardController(ApplicationDbContext db, UserManager<ApplicationUser> userManager)
    {
        _db = db;
        _userManager = userManager;
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

        if (isAdmin)
        {
            teams = await _db.Teams
                .Include(t => t.Users)
                .OrderBy(t => t.Name)
                .ToListAsync();
        }
        else if (user.TeamId.HasValue)
        {
            teams = await _db.Teams
                .Where(t => t.Id == user.TeamId.Value)
                .Include(t => t.Users)
                .ToListAsync();
        }
        else
        {
            teams = new List<Team>();
        }

        var model = new DashboardViewModel
        {
            IsAdmin = isAdmin,
            Teams = teams
        };

        return View(model);
    }
}
