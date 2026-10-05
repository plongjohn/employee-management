using System.Drawing.Drawing2D;
using EmployeeManagement.Core.Models;

namespace EmployeeManagement.Desktop.Styling;

/// <summary>
/// Custom drawing for the employee grid: initials avatar, department badge and the hint for an
/// empty list. Sizes are logical pixels and scaled to the grid's DPI.
/// </summary>
internal sealed class EmployeeGridRenderer(DataGridView grid)
{
    private const int CellPadding = 8;
    private const int AvatarDiameter = 32;
    private const int AvatarTextGap = 12;
    private const int BadgeHeight = 24;
    private const int BadgeHorizontalPadding = 10;

    private const TextFormatFlags CenteredText =
        TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.SingleLine |
        TextFormatFlags.NoPadding | TextFormatFlags.NoPrefix;

    private const TextFormatFlags CellText =
        TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.SingleLine |
        TextFormatFlags.EndEllipsis | TextFormatFlags.NoPrefix;

    public void PaintName(DataGridViewCellPaintingEventArgs e, Employee employee)
    {
        if (e.Graphics is not { } graphics)
        {
            return;
        }

        e.PaintBackground(e.ClipBounds, cellsPaintSelectionBackground: true);

        var cell = e.CellBounds;
        var diameter = Scale(AvatarDiameter);
        var avatar = new Rectangle(
            cell.Left + Scale(CellPadding), cell.Top + (cell.Height - diameter) / 2, diameter, diameter);

        using (var brush = new SolidBrush(Theme.AvatarBackground))
        {
            FillSmooth(graphics, () => graphics.FillEllipse(brush, avatar));
        }

        TextRenderer.DrawText(graphics, Initials(employee.FirstName, employee.LastName),
            Theme.SmallSemiboldFont, avatar, Theme.Text, CenteredText);

        var textBounds = Rectangle.FromLTRB(
            avatar.Right + Scale(AvatarTextGap), cell.Top, cell.Right - Scale(CellPadding), cell.Bottom);
        TextRenderer.DrawText(graphics, e.FormattedValue as string, Theme.BaseFont, textBounds, Theme.Text, CellText);

        e.Handled = true;
    }

    public void PaintDepartmentBadge(DataGridViewCellPaintingEventArgs e, Employee employee)
    {
        if (e.Graphics is not { } graphics)
        {
            return;
        }

        e.PaintBackground(e.ClipBounds, cellsPaintSelectionBackground: true);

        var cell = e.CellBounds;
        var (background, text) = Theme.BadgeColorsFor(employee.DepartmentId);
        var textWidth = TextRenderer.MeasureText(
            graphics, employee.DepartmentName, Theme.SmallSemiboldFont, Size.Empty, CenteredText).Width;

        var height = Scale(BadgeHeight);
        var width = Math.Min(textWidth + 2 * Scale(BadgeHorizontalPadding), cell.Width - 2 * Scale(CellPadding));
        var badge = new Rectangle(cell.Left + Scale(CellPadding), cell.Top + (cell.Height - height) / 2, width, height);

        using (var brush = new SolidBrush(background))
        {
            var cornerRadius = new Size(height / 2, height / 2);
            FillSmooth(graphics, () => graphics.FillRoundedRectangle(brush, badge, cornerRadius));
        }

        TextRenderer.DrawText(graphics, employee.DepartmentName, Theme.SmallSemiboldFont, badge, text,
            CenteredText | TextFormatFlags.EndEllipsis);

        e.Handled = true;
    }

    public void PaintEmptyHint(Graphics graphics, string text)
    {
        var belowHeaders = Rectangle.FromLTRB(
            0, grid.ColumnHeadersHeight, grid.ClientSize.Width, grid.ClientSize.Height);
        TextRenderer.DrawText(graphics, text, Theme.BaseFont, belowHeaders, Theme.MutedText, CenteredText);
    }

    private int Scale(int logicalPixels) => grid.LogicalToDeviceUnits(logicalPixels);

    private static string Initials(string firstName, string lastName) =>
        string.Concat(FirstLetter(firstName), FirstLetter(lastName));

    private static string FirstLetter(string name) =>
        name.Length == 0 ? string.Empty : char.ToUpperInvariant(name[0]).ToString();

    private static void FillSmooth(Graphics graphics, Action fill)
    {
        var previousMode = graphics.SmoothingMode;
        graphics.SmoothingMode = SmoothingMode.AntiAlias;
        try
        {
            fill();
        }
        finally
        {
            graphics.SmoothingMode = previousMode;
        }
    }
}
