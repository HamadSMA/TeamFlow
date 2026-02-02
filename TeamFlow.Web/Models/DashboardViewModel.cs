namespace TeamFlow.Web.Models;

public class DashboardViewModel
{
    public bool IsAdmin { get; set; }
    public List<Team> Teams { get; set; } = new();
    public DashboardStatsViewModel Stats { get; set; } = new();
}
