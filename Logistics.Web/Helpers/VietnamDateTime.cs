namespace Logistics.Web.Helpers;

public static class VietnamDateTime
{
    private const string DateTimeFormat = "dd/MM/yyyy HH:mm";
    private const string DateFormat = "dd/MM/yyyy";
    private static readonly TimeZoneInfo TimeZone = ResolveTimeZone();

    public static DateTime Now => ToLocal(DateTime.UtcNow);

    public static DateTime Today => Now.Date;

    public static DateTime ToLocal(DateTime value)
    {
        var utcValue = value.Kind switch
        {
            DateTimeKind.Utc => value,
            DateTimeKind.Local => value.ToUniversalTime(),
            _ => DateTime.SpecifyKind(value, DateTimeKind.Utc)
        };

        return TimeZoneInfo.ConvertTimeFromUtc(utcValue, TimeZone);
    }

    public static string Format(DateTime value)
    {
        return ToLocal(value).ToString(DateTimeFormat);
    }

    public static string Format(DateTime? value, string fallback = "-")
    {
        return value.HasValue ? Format(value.Value) : fallback;
    }

    public static string FormatDate(DateTime value)
    {
        return ToLocal(value).ToString(DateFormat);
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
