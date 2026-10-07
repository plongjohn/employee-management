using EmployeeManagement.Desktop.Resources;

namespace EmployeeManagement.Desktop.Forms;

/// <summary>
/// Message dialogs in one consistent style. Task dialogs are used instead of message boxes
/// because their buttons can carry descriptive texts like "Delete" instead of "Yes".
/// </summary>
internal static class Dialogs
{
    public static void ShowError(IWin32Window? owner, string text) => Show(owner, text, TaskDialogIcon.Error);

    public static void ShowWarning(IWin32Window owner, string text) => Show(owner, text, TaskDialogIcon.Warning);

    /// <summary>
    /// Asks the user to confirm an action. The cancel button is the default, so pressing Enter
    /// or Escape never triggers the action by accident.
    /// </summary>
    /// <returns>True if the user clicked the confirm button.</returns>
    public static bool Confirm(IWin32Window owner, string heading, string text, string confirmText, string cancelText)
    {
        var confirmButton = new TaskDialogButton(confirmText);
        var cancelButton = new TaskDialogButton(cancelText);
        var page = new TaskDialogPage
        {
            Caption = Strings.AppTitle,
            Heading = heading,
            Text = text,
            Icon = TaskDialogIcon.Warning,
            Buttons = { confirmButton, cancelButton },
            DefaultButton = cancelButton,
            AllowCancel = true,
        };

        return ShowPage(owner, page) == confirmButton;
    }

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
