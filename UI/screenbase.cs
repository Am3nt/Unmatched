using Unmatched.UI.Ui;

namespace Unmatched.UI.Screens;

/// <summary>A full-window screen whose content column stays centered. Add controls to Content.</summary>
public abstract class ScreenBase : UserControl
{
    protected FlowLayoutPanel Content { get; } = new()
    {
        FlowDirection = FlowDirection.TopDown,
        WrapContents = false,
        AutoSize = true,
        AutoSizeMode = AutoSizeMode.GrowAndShrink,
        BackColor = Color.Transparent
    };

    protected ScreenBase()
    {
        Dock = DockStyle.Fill;
        BackColor = Theme.Background;
        Controls.Add(Content);

        Content.SizeChanged += (_, _) => CenterContent();
    }

    protected override void OnResize(EventArgs e)
    {
        base.OnResize(e);
        CenterContent();
    }

    private void CenterContent()
    {
        Content.Location = new Point(
            Math.Max(0, (ClientSize.Width - Content.Width) / 2),
            Math.Max(0, (ClientSize.Height - Content.Height) / 2));
    }
}