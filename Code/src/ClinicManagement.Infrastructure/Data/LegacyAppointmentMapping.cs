namespace ClinicManagement.Infrastructure.Data;

internal static class LegacyAppointmentMapping
{
    public const int StatusApproved = 1;
    public const int StatusPending = 2;
    public const int StatusCompleted = 3;
    public const int StatusCancelled = 4;

    public static string StatusToString(int status) => status switch
    {
        StatusApproved => "Approved",
        StatusPending => "Pending",
        StatusCompleted => "Completed",
        StatusCancelled => "Cancelled",
        _ => "Pending"
    };

    public static int StatusFromString(string status) => status switch
    {
        "Approved" => StatusApproved,
        "Pending" => StatusPending,
        "Completed" => StatusCompleted,
        "Cancelled" => StatusCancelled,
        _ => StatusPending
    };

    /// <summary>UI slot 1–10 maps to appointment hours 9–18 on the current day.</summary>
    public static int SlotToHour(int slot) => slot + 8;

    public static int HourToSlot(int hour) => hour - 8;

    public static DateTime BuildAppointmentTimestamp(int freeSlot)
    {
        var hour = SlotToHour(freeSlot);
        return DateTime.Today.AddHours(hour);
    }

    public static int GetFreeSlotFromDate(DateTime appointmentDate) =>
        HourToSlot(appointmentDate.Hour);
}
