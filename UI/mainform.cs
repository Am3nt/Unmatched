using Unmatched.UI.Screens;
using Unmatched.UI.Services;
using Unmatched.UI.Ui;

namespace Unmatched.UI;

/// <summary>The one window. It just swaps which screen is shown.</summary>
public class MainForm : Form
{
    private readonly IRoomService _rooms;

    public MainForm() : this(new StubRoomService()) { }

    public MainForm(IRoomService rooms)
    {
        _rooms = rooms;

        Text = "Unmatched";
        ClientSize = new Size(960, 640);
        MinimumSize = new Size(640, 520);
        StartPosition = FormStartPosition.CenterScreen;
        BackColor = Theme.Background;

        SetScreen(BuildTitle());
    }

    // Screens are built lazily and swapped with BeginInvoke, so a button never disposes its own screen mid-click.
    private void GoTo(Func<ScreenBase> build) => BeginInvoke(new Action(() => SetScreen(build())));

    private ScreenBase BuildTitle() =>
        new TitleScreen(
            onCreateRoom: () => GoTo(BuildCreateRoom),
            onJoinRoom: () => GoTo(BuildJoinRoom),
            onQuit: Close);

    private ScreenBase BuildCreateRoom() => new CreateRoomScreen(_rooms, onBack: () => GoTo(BuildTitle));

    private ScreenBase BuildJoinRoom() => new JoinRoomScreen(_rooms, onBack: () => GoTo(BuildTitle));

    private void SetScreen(ScreenBase screen)
    {
        SuspendLayout();
        foreach (var old in Controls.Cast<Control>().ToList())
        {
            Controls.Remove(old);
            old.Dispose();
        }
        Controls.Add(screen);
        ResumeLayout();
    }
}