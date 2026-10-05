using EmployeeManagement.Desktop.Styling;

namespace EmployeeManagement.Desktop.Controls;

/// <summary>A field label followed by a red asterisk that marks the field as required.</summary>
internal sealed class RequiredFieldLabel : Label
{
    private const string RequiredMarker = "*";
    private const TextFormatFlags TextFlags = TextFormatFlags.NoPadding | TextFormatFlags.SingleLine;

    public RequiredFieldLabel()
    {
        AutoSize = true;
    }

    public override Size GetPreferredSize(Size proposedSize)
    {
        var textSize = TextRenderer.MeasureText(Text + RequiredMarker, Font, Size.Empty, TextFlags);
        return new Size(textSize.Width + Padding.Horizontal, textSize.Height + Padding.Vertical);
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        var origin = new Point(Padding.Left, Padding.Top);
        var textWidth = TextRenderer.MeasureText(e.Graphics, Text, Font, Size.Empty, TextFlags).Width;

        TextRenderer.DrawText(e.Graphics, Text, Font, origin, ForeColor, TextFlags);
        TextRenderer.DrawText(e.Graphics, RequiredMarker, Font, origin with { X = origin.X + textWidth }, Theme.Accent, TextFlags);
    }
}
