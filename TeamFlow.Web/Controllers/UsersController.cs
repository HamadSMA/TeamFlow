using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TeamFlow.Web.Data;
using TeamFlow.Web.Models;

namespace TeamFlow.Web.Controllers;

[Authorize(Policy = "AdminOnly")]
public class UsersController : Controller
{
    private readonly ApplicationDbContext _db;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SettingsService _settings;

    public UsersController(ApplicationDbContext db, UserManager<ApplicationUser> userManager, SettingsService settings)
    {
        _db = db;
        _userManager = userManager;
        _settings = settings;
    }

    public async Task<IActionResult> Index()
    {
        var users = await _db.Users
            .Include(u => u.Team)
            .OrderBy(u => u.Email)
            .ToListAsync();

        var adminUsers = await _userManager.GetUsersInRoleAsync("Admin");
        ViewData["AdminIds"] = new HashSet<string>(adminUsers.Select(u => u.Id));

        return View(users);
    }

    public async Task<IActionResult> Create()
    {
        ViewData["Teams"] = await _db.Teams.OrderBy(t => t.Name).ToListAsync();
        return View(new AdminCreateUserViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(AdminCreateUserViewModel model)
    {
        ViewData["Teams"] = await _db.Teams.OrderBy(t => t.Name).ToListAsync();
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var emailBase = BuildEmailBase(model.FirstName, model.LastName);
        var email = await EnsureUniqueEmailAsync(emailBase);

        var settings = await _settings.GetAsync();
        var user = new ApplicationUser
        {
            FirstName = model.FirstName,
            LastName = model.LastName,
            UserName = email,
            Email = email,
            EmailConfirmed = true,
            IsActive = model.IsActive,
            TeamId = model.TeamId,
            CurrentStatus = settings.DefaultStatus
        };

        var result = await _userManager.CreateAsync(user, "Aa123!");
        if (!result.Succeeded)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
            return View(model);
        }

        await _userManager.AddToRoleAsync(user, "Employee");
        ViewData["GeneratedEmail"] = email;
        ViewData["GeneratedPassword"] = "Aa123!";
        return View(model);
    }

    public async Task<IActionResult> Edit(string id)
    {
        if (string.IsNullOrWhiteSpace(id))
        {
            return BadRequest();
        }

        var user = await _db.Users
            .Include(u => u.Team)
            .FirstOrDefaultAsync(u => u.Id == id);

        if (user == null)
        {
            return NotFound();
        }

        var teams = await _db.Teams
            .OrderBy(t => t.Name)
            .ToListAsync();

        var viewModel = new UserTeamEditViewModel
        {
            UserId = user.Id,
            Email = user.Email ?? user.UserName ?? string.Empty,
            TeamId = user.TeamId,
            Teams = teams
        };

        return View(viewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(UserTeamEditViewModel model)
    {
        if (!ModelState.IsValid)
        {
            model.Teams = await _db.Teams
                .OrderBy(t => t.Name)
                .ToListAsync();
            return View(model);
        }

        var user = await _db.Users.FirstOrDefaultAsync(u => u.Id == model.UserId);
        if (user == null)
        {
            return NotFound();
        }

        user.TeamId = model.TeamId;
        await _db.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ToggleActive(string id)
    {
        if (string.IsNullOrWhiteSpace(id))
        {
            return BadRequest();
        }

        var user = await _db.Users.FirstOrDefaultAsync(u => u.Id == id);
        if (user == null)
        {
            return NotFound();
        }

        var isAdmin = await _userManager.IsInRoleAsync(user, "Admin");
        if (isAdmin && user.IsActive)
        {
            return RedirectToAction(nameof(Index));
        }

        user.IsActive = !user.IsActive;
        await _db.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }

    private static string BuildEmailBase(string firstName, string lastName)
    {
        var first = new string(firstName.Where(char.IsLetter).ToArray()).ToLowerInvariant();
        var last = new string(lastName.Where(char.IsLetter).ToArray()).ToLowerInvariant();

        if (string.IsNullOrWhiteSpace(first) || string.IsNullOrWhiteSpace(last))
        {
            return "user";
        }

        var firstPart = first.Length >= 1 ? first.Substring(0, 1) : first;
        var lastPart = last.Length >= 6 ? last.Substring(0, 6) : last;
        return $"{firstPart}{lastPart}";
    }

    private async Task<string> EnsureUniqueEmailAsync(string baseLocalPart)
    {
        var local = baseLocalPart;
        var counter = 1;
        while (await _db.Users.AnyAsync(u => u.Email == $"{local}@teamflow.com"))
        {
            local = $"{baseLocalPart}{counter}";
            counter++;
        }

        return $"{local}@teamflow.com";
    }
}
