using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TeamFlow.Web.Data;
using TeamFlow.Web.Models;

namespace TeamFlow.Web.Controllers;

[Authorize(Policy = "AdminOnly")]
public class TeamsController : Controller
{
    private readonly ApplicationDbContext _db;

    public TeamsController(ApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<IActionResult> Index()
    {
        var teams = await _db.Teams
            .OrderBy(t => t.Name)
            .ToListAsync();
        return View(teams);
    }

    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Team team)
    {
        if (!ModelState.IsValid)
        {
            return View(team);
        }

        _db.Teams.Add(team);
        await _db.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int id)
    {
        var team = await _db.Teams.FindAsync(id);
        if (team == null)
        {
            return NotFound();
        }

        return View(team);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Team team)
    {
        if (id != team.Id)
        {
            return BadRequest();
        }

        if (!ModelState.IsValid)
        {
            return View(team);
        }

        _db.Entry(team).State = EntityState.Modified;
        await _db.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Delete(int id)
    {
        var team = await _db.Teams.FindAsync(id);
        if (team == null)
        {
            return NotFound();
        }

        return View(team);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var team = await _db.Teams.FindAsync(id);
        if (team == null)
        {
            return NotFound();
        }

        _db.Teams.Remove(team);
        await _db.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }
}
