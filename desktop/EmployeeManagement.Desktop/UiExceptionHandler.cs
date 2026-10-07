using System.Data.Common;
using EmployeeManagement.Desktop.Forms;
using EmployeeManagement.Desktop.Resources;
using Microsoft.Extensions.Logging;

namespace EmployeeManagement.Desktop;

/// <summary>
/// Reports errors from the user interface: the error is logged and shown, and the application
/// keeps running so the user can try again, e.g. once the database is reachable again.
/// </summary>
internal sealed class UiExceptionHandler(ILogger<UiExceptionHandler> logger)
{
    /// <summary>Runs an operation started by a UI event and reports any failure instead of throwing.</summary>
    public async Task RunAsync(IWin32Window owner, Func<Task> operation)
    {
        try
        {
            await operation();
        }
        catch (Exception ex)
        {
            Report(owner, ex);
        }
    }

    /// <summary>Last line of defence for exceptions that escape an event handler anyway.</summary>
    public void Handle(object sender, ThreadExceptionEventArgs e) => Report(Form.ActiveForm, e.Exception);

    private void Report(IWin32Window? owner, Exception exception)
    {
        if (exception is DbException)
        {
            logger.LogError(exception, "Database access failed");
            Dialogs.ShowError(owner, Strings.ErrorDatabaseUnavailable);
            return;
        }

        logger.LogError(exception, "Unexpected error in the user interface");
        Dialogs.ShowError(owner, Strings.ErrorUnexpected);
    }
}
