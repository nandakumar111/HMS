namespace HotelManagementSystem.Models;

public class Collections
{
    public string Room { get; set; }
    public string Booking { get; set; }
}

public interface IDatabaseSettings
{
    public string ConnectionString { get; set; }
    public string Database { get; set; }
    public bool HaveReplicaSet { get; set; }
    public Collections Collections { get; set; }
}

public class DatabaseSettings : IDatabaseSettings
{
    public string ConnectionString { get; set; }
    public string Database { get; set; }
    public bool HaveReplicaSet { get; set; }
    public Collections Collections { get; set; }
}
