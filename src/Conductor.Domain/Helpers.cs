using NodaTime;
using System.Text.Json;

namespace Conductor.Domain;

public static class Helpers
{
    /*public static DateTime RoundToMicroseconds(this DateTime dt)
    {
        var originalTicks = dt.Ticks;
        var ticksInMillisecond = TimeSpan.TicksPerMicrosecond;
        var roundedTicks = (long)Math.Round((double)originalTicks / ticksInMillisecond) * ticksInMillisecond;
        return new DateTime(roundedTicks, dt.Kind);
    }*/

    private static Instant RoundToMicroseconds(this Instant instant)
    {
        const long TicksPerMicrosecond = NodaConstants.TicksPerMillisecond / 1000;
        long ticks = instant.ToUnixTimeTicks();

        long roundedTicks = (long)Math.Round((double)ticks / TicksPerMicrosecond) * TicksPerMicrosecond;

        return Instant.FromUnixTimeTicks(roundedTicks);
    }

    public static Instant GetInstantNow(this TimeProvider timeProvider)
    {
        var result = Instant
            .FromDateTimeUtc(timeProvider.GetUtcNow().UtcDateTime)
            .RoundToMicroseconds();

        return result;
    }

    public static JsonElement EmptyJsonElement()
    {
        var json = "{}";
        using var doc = JsonDocument.Parse(json);
        var emptyObject = doc.RootElement;

        return emptyObject;
    }
}
