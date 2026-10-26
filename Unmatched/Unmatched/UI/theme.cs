namespace Unmatched.UI.Ui;

public enum StatusKind { Info, Success, Error }

/// <summary>Colors, fonts and small control factories so every screen looks the same.</summary>
public static class Theme
{
    public const int ContentWidth = 420;

    public static readonly Color Background = Color.FromArgb(22, 24, 31);
    public static readonly Color Surface = Color.FromArgb(38, 41, 52);
    public static readonly Color SurfaceHover = Color.FromArgb(54, 58, 72);
    public static readonly Color Accent = Color.FromArgb(200, 60, 60);
    public static readonly Color AccentHover = Color.FromArgb(222, 84, 84);
    public static readonly Color TextPrimary = Color.FromArgb(240, 240, 245);
    public static readonly Color TextMuted = Color.FromArgb(150, 156, 172);
    public static readonly Color Success = Color.FromArgb(90, 200, 120);
    public static readonly Color Error = Color.FromArgb(240, 100, 100);

    // ---- labels ----
    public static Label Title(string text) => MakeLabel(text, 32f, FontStyle.Bold, TextPrimary, 84);
    public static Label Heading(string text) => MakeLabel(text, 22f, FontStyle.Bold, TextPrimary, 56);
    public static Label Muted(string text, int height = 32) => MakeLabel(text, 11f, FontStyle.Regular, TextMuted, height);
    public static Label Status() => MakeLabel("", 10f, FontStyle.Regular, TextMuted, 48);

    public static Label RoomCodeLabel() =>
        MakeLabel("", 44f, FontStyle.Bold, Accent, 80, "Consolas");

    public static void SetStatus(Label label, string text, StatusKind kind)
    {
        label.Text = text;
        label.ForeColor = kind switch
        {
            StatusKind.Success => Success,
            StatusKind.Error => Error,
            _ => TextMuted
        };
    }

    // ---- buttons ----
    public static Button PrimaryButton(string text, Action onClick) => MakeButton(text, onClick, Accent, AccentHover);
    public static Button SecondaryButton(string text, Action onClick) => MakeButton(text, onClick, Surface, SurfaceHover);

    // ---- inputs and misc ----
    public static TextBox Input(string placeholder, int maxLength = 24) => new()
    {
        PlaceholderText = placeholder,
        MaxLength = maxLength,
        Width = ContentWidth,
        Font = new Font("Segoe UI", 13f),
        BackColor = Surface,
        ForeColor = TextPrimary,
        BorderStyle = BorderStyle.FixedSingle,
        Margin = new Padding(0, 8, 0, 8)
    };

    public static ProgressBar WaitingBar() => new()
    {
        Style = ProgressBarStyle.Marquee,
        MarqueeAnimationSpeed = 30,
        Size = new Size(ContentWidth, 8),
        Margin = new Padding(0, 8, 0, 8)
    };

    public static Panel Spacer(int height) => new()
    {
        Size = new Size(ContentWidth, height),
        BackColor = Color.Transparent,
        Margin = Padding.Empty
    };

    // ---- internals ----
    private static Label MakeLabel(string text, float size, FontStyle style, Color color, int height, string font = "Segoe UI") => new()
    {
        Text = text,
        AutoSize = false,
        Size = new Size(ContentWidth, height),
        Font = new Font(font, size, style),
        ForeColor = color,
        BackColor = Color.Transparent,
        TextAlign = ContentAlignment.MiddleCenter,
        Margin = new Padding(0, 4, 0, 4)
    };

    private static Button MakeButton(string text, Action onClick, Color back, Color hover)
    {
        var button = new Button
        {
            Text = text,
            Size = new Size(ContentWidth, 48),
            FlatStyle = FlatStyle.Flat,
            BackColor = back,
            ForeColor = TextPrimary,
            Font = new Font("Segoe UI", 12f, FontStyle.Bold),
            Cursor = Cursors.Hand,
            Margin = new Padding(0, 8, 0, 8),
            UseVisualStyleBackColor = false
        };
        button.FlatAppearance.BorderSize = 0;
        button.FlatAppearance.MouseOverBackColor = hover;
        button.FlatAppearance.MouseDownBackColor = hover;
        button.Click += (_, _) => onClick();
        return button;
    }
}