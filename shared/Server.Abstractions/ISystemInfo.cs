namespace Server.Abstractions;

public interface ISystemInfo
{
    DateTime BuildDateTime { get; }

    DateTime StartDateTime { get; }

    bool IsDevelopment { get; }
}
