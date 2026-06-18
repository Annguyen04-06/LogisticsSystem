namespace Logistics.Application.Common;

public static class VietnamTime
{
    private static readonly TimeZoneInfo TimeZone = ResolveTimeZone();

    public static DateTime Now => ToVietnamTime(DateTime.UtcNow);

    public static DateTime Today => Now.Date;

    public static DateTime ToVietnamTime(DateTime value)
    {
        var utcValue = value.Kind switch
        {
            DateTimeKind.Utc => value,
            DateTimeKind.Local => value.ToUniversalTime(),
            _ => DateTime.SpecifyKind(value, DateTimeKind.Utc)
        };

        return TimeZoneInfo.ConvertTimeFromUtc(utcValue, TimeZone);
    }

    public static DateTime? ToVietnamTime(DateTime? value)
    {
        return value.HasValue ? ToVietnamTime(value.Value) : null;
    }

    public static DateTime ToUtc(DateTime vietnamTime)
    {
        if (vietnamTime.Kind == DateTimeKind.Utc)
        {
            return vietnamTime;
        }

        var localValue = DateTime.SpecifyKind(vietnamTime, DateTimeKind.Unspecified);
        return TimeZoneInfo.ConvertTimeToUtc(localValue, TimeZone);
    }

    private static TimeZoneInfo ResolveTimeZone()
    {
        foreach (var id in new[] { "SE Asia Standard Time", "Asia/Ho_Chi_Minh" })
        {
            try
            {
                return TimeZoneInfo.FindSystemTimeZoneById(id);
            }
            catch (TimeZoneNotFoundException)
            {
            }
            catch (InvalidTimeZoneException)
            {
            }
        }

        return TimeZoneInfo.CreateCustomTimeZone(
            "Vietnam Standard Time",
            TimeSpan.FromHours(7),
            "Vietnam Standard Time",
            "Vietnam Standard Time");
    }
}
