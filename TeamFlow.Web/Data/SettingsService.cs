using Microsoft.EntityFrameworkCore;
using TeamFlow.Web.Models;

namespace TeamFlow.Web.Data;

public class SettingsService
{
    private readonly ApplicationDbContext _db;

    public SettingsService(ApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<AppSettings> GetAsync()
    {
        var settings = await _db.AppSettings.AsNoTracking().FirstOrDefaultAsync();
        return settings ?? new AppSettings();
    }

    public async Task<TimeZoneInfo> GetTimeZoneAsync()
    {
        var settings = await GetAsync();
        try
        {
            return TimeZoneInfo.FindSystemTimeZoneById(settings.TimeZoneId);
        }
        catch (TimeZoneNotFoundException)
        {
            return TimeZoneInfo.Utc;
        }
    }
}
