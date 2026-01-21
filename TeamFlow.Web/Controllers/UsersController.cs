using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TeamFlow.Web.Data;
using TeamFlow.Web.Models;

namespace TeamFlow.Web.Controllers;

[Authorize(Policy = "AdminOnly")]
public class UsersController : Controller
{
    private readonly ApplicationDbContext _db;

    public UsersController(ApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<IActionResult> Index()
    {
        var users = await _db.Users
            .Include(u => u.Team)
            .OrderBy(u => u.Email)
            .ToListAsync();

        return View(users);
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

        user.IsActive = !user.IsActive;
        await _db.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }
}
