namespace TeamFlow.Web.Models;

public class DashboardStatsViewModel
{
    public int TeamMembers { get; set; }
    public int ActiveUsers { get; set; }
    public int AvailableUsers { get; set; }
    public int BusyUsers { get; set; }
    public int AwayUsers { get; set; }

    public int TotalUsers { get; set; }
    public int InactiveUsers { get; set; }
    public int UsersWithoutTeam { get; set; }
    public int TotalTeams { get; set; }
    public int StatusUpdatesLast24h { get; set; }
}
