using EmployeeManagement.Desktop.Forms;

namespace EmployeeManagement.Desktop;

internal static class Program
{
    [STAThread]
    private static void Main()
    {
        ApplicationConfiguration.Initialize();
        Application.Run(new MainForm());
    }
}
