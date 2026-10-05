using EmployeeManagement.Desktop.Resources;

namespace EmployeeManagement.Desktop.Forms;

/// <summary>
/// Message dialogs in one consistent style. Task dialogs are used instead of message boxes
/// because their buttons can carry descriptive texts like "Delete" instead of "Yes".
/// </summary>
internal static class Dialogs
{
    public static void ShowError(IWin32Window? owner, string text) => Show(owner, text, TaskDialogIcon.Error);

    private static void Show(IWin32Window? owner, string text, TaskDialogIcon icon)
    {
        var page = new TaskDialogPage
        {
            Caption = Strings.AppTitle,
            Text = text,
            Icon = icon,
            Buttons = { TaskDialogButton.OK },
        };

        ShowPage(owner, page);
    }

    private static TaskDialogButton ShowPage(IWin32Window? owner, TaskDialogPage page) =>
        owner is null ? TaskDialog.ShowDialog(page) : TaskDialog.ShowDialog(owner, page);
}
