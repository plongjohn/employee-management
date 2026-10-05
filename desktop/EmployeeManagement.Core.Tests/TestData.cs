using EmployeeManagement.Core.Models;
using Microsoft.Extensions.Time.Testing;

namespace EmployeeManagement.Core.Tests;

internal static class TestData
{
    public static EmployeeInput ValidInput() =>
        new("Anna", "Müller", "anna.mueller@example.com", DepartmentId: 1, new DateOnly(2020, 1, 15));

    public static FakeTimeProvider ClockAt(DateOnly today)
    {
        var clock = new FakeTimeProvider(new DateTimeOffset(today, new TimeOnly(12, 0), TimeSpan.Zero));
        clock.SetLocalTimeZone(TimeZoneInfo.Utc);
        return clock;
    }
}
