namespace EcoMonitor.Contracts.Contracts.BlockUser;

public record BlockUserRequest(int Days, string Reason)
{
    public BlockUserRequest() : this(1, "не указана") { }
    public BlockUserRequest(int days) : this(days, "не указана") { }
    public BlockUserRequest(string reason) : this(1, reason) { }
    
    public TimeSpan ToTimeSpan() => TimeSpan.FromDays(Days);
}   