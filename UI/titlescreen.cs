using Unmatched.UI.Ui;

namespace Unmatched.UI.Screens;

public class TitleScreen : ScreenBase
{
    public TitleScreen(Action onCreateRoom, Action onJoinRoom, Action onQuit)
    {
        Content.Controls.Add(Theme.Title("UNMATCHED"));
        Content.Controls.Add(Theme.Muted("Digital Edition  •  Prototype"));
        Content.Controls.Add(Theme.Spacer(40));
        Content.Controls.Add(Theme.PrimaryButton("Create Room", onCreateRoom));
        Content.Controls.Add(Theme.SecondaryButton("Join Room", onJoinRoom));
        Content.Controls.Add(Theme.SecondaryButton("Quit", onQuit));
    }
}