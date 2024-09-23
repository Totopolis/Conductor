namespace Server.Abstractions;

public interface IMaintenanceBot
{
    Task BroadcastMessage(string message);
}
