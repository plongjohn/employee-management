namespace EmployeeManagement.Desktop.Styling;

/// <summary>
/// Colours and fonts of the application in one place: black header, white background, red accent.
/// </summary>
internal static class Theme
{
    public static readonly Color Header = Color.Black;
    public static readonly Color HeaderText = Color.White;
    public static readonly Color Background = Color.White;
    public static readonly Color Text = FromRgb(0x1A1A1A);

    // About 6:1 on white – still readable for small secondary text (WCAG AA).
    public static readonly Color MutedText = FromRgb(0x5F6368);

    // White text on this red reaches 4.9:1 (WCAG AA). Darker shades give hover and pressed feedback.
    public static readonly Color Accent = FromRgb(0xDA291C);
    public static readonly Color AccentHover = FromRgb(0xB82216);
    public static readonly Color AccentPressed = FromRgb(0x9C1C12);

    public static readonly Color InputBackground = FromRgb(0xF2F4F5);
    public static readonly Color HoverBackground = FromRgb(0xF5F6F7);
    public static readonly Color SelectionBackground = FromRgb(0xE9ECEF);
    public static readonly Color GridLine = FromRgb(0xE3E5E8);
    public static readonly Color DisabledBackground = FromRgb(0xD5D8DC);
    public static readonly Color AvatarBackground = FromRgb(0xE3E5E8);

    // Muted colours without red, so a department never looks like an error or the accent colour.
    // Eight entries cover the departments in setup.sql; further ids reuse colours.
    private static readonly (Color Background, Color Text)[] BadgePalette =
    [
        (FromRgb(0xE3EEFA), FromRgb(0x1D4E89)),
        (FromRgb(0xE3F4EA), FromRgb(0x1E6B3A)),
        (FromRgb(0xFDF1DC), FromRgb(0x8A5A00)),
        (FromRgb(0xEEE8FA), FromRgb(0x4B2E83)),
        (FromRgb(0xE0F4F4), FromRgb(0x145C5C)),
        (FromRgb(0xECEEF1), FromRgb(0x3D4752)),
        (FromRgb(0xF0F2DC), FromRgb(0x5A6014)),
        (FromRgb(0xF3ECE6), FromRgb(0x6B4426)),
    ];

    public static readonly Font BaseFont = new("Segoe UI", 10F);
    public static readonly Font SmallFont = new("Segoe UI", 9F);
    public static readonly Font SemiboldFont = new("Segoe UI Semibold", 10F);
    public static readonly Font SmallSemiboldFont = new("Segoe UI Semibold", 9F);
    public static readonly Font TitleFont = new("Segoe UI Semibold", 14F);
    public static readonly Font HeadingFont = new("Segoe UI Semibold", 16F);

    // Ships with Windows 10 and 11 and stays sharp at any scaling, unlike bitmap icons.
    public static readonly Font IconFont = new("Segoe MDL2 Assets", 11F);

    public static (Color Background, Color Text) BadgeColorsFor(int departmentId) =>
        BadgePalette[departmentId % BadgePalette.Length];

    public static void ApplyPrimary(Button button)
    {
        ApplyFlat(button, Accent, AccentHover, AccentPressed);
        button.ForeColor = Color.White;
        button.FlatAppearance.BorderSize = 0;

        // A flat button keeps its colour when disabled; grey makes the state visible.
        button.EnabledChanged += (_, _) => button.BackColor = button.Enabled ? Accent : DisabledBackground;
        button.BackColor = button.Enabled ? Accent : DisabledBackground;
    }

    public static void ApplySecondary(Button button)
    {
        ApplyFlat(button, Background, HoverBackground, SelectionBackground);
        button.ForeColor = Text;
        button.FlatAppearance.BorderColor = Text;
        button.FlatAppearance.BorderSize = 1;
    }

    public static void ApplyIcon(Button button)
    {
        ApplySecondary(button);
        button.Font = IconFont;
        button.FlatAppearance.BorderColor = DisabledBackground;
    }

    public static void ApplyInput(TextBox textBox)
    {
        textBox.BorderStyle = BorderStyle.FixedSingle;
        textBox.BackColor = InputBackground;
        textBox.ForeColor = Text;
    }

    public static void ApplyInput(ComboBox comboBox)
    {
        comboBox.FlatStyle = FlatStyle.Flat;
        comboBox.BackColor = InputBackground;
        comboBox.ForeColor = Text;
    }

    private static void ApplyFlat(Button button, Color background, Color hover, Color pressed)
    {
        button.FlatStyle = FlatStyle.Flat;
        button.BackColor = background;
        button.FlatAppearance.MouseOverBackColor = hover;
        button.FlatAppearance.MouseDownBackColor = pressed;
        button.Font = SemiboldFont;
        button.Cursor = Cursors.Hand;
        button.UseVisualStyleBackColor = false;
    }

    private static Color FromRgb(int rgb) => Color.FromArgb(255, Color.FromArgb(rgb));
}
