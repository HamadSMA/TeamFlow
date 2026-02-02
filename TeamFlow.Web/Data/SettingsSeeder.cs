using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using TeamFlow.Web.Models;

namespace TeamFlow.Web.Data;

public static class SettingsSeeder
{
    public static async Task SeedAsync(ApplicationDbContext db)
    {
        try
        {
            if (await db.AppSettings.AnyAsync())
            {
                return;
            }

            db.AppSettings.Add(new AppSettings());
            await db.SaveChangesAsync();
        }
        catch (SqliteException)
        {
            // Database is likely not migrated yet.
        }
    }
}
