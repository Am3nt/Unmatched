using Unmatched.UI.Services;
using Unmatched.UI.Ui;

namespace Unmatched.UI.Screens;

public class CreateRoomScreen : ScreenBase
{
    private readonly IRoomService _rooms;

    private readonly TextBox _nameBox = Theme.Input("Your name");
    private readonly Button _createButton;
    private readonly Label _codeCaption = Theme.Muted("Room code");
    private readonly Label _codeLabel = Theme.RoomCodeLabel();
    private readonly Button _copyButton;
    private readonly Label _waitingLabel = Theme.Muted("Waiting for an opponent to join…");
    private readonly ProgressBar _waitingBar = Theme.WaitingBar();
    private readonly Label _status = Theme.Status();
    private readonly Button _backButton;

    public CreateRoomScreen(IRoomService rooms, Action onBack)
    {
        _rooms = rooms;

        _createButton = Theme.PrimaryButton("Create", () => _ = CreateAsync());
        _copyButton = Theme.SecondaryButton("Copy code", CopyCode);
        _backButton = Theme.SecondaryButton("Back", onBack);

        _nameBox.KeyDown += (_, e) =>
        {
            if (e.KeyCode != Keys.Enter) return;
            e.SuppressKeyPress = true;
            _ = CreateAsync();
        };

        SetRoomVisible(false);

        Content.Controls.AddRange(new Control[]
        {
            Theme.Heading("Create a room"),
            Theme.Muted("Choose a name, then share the room code with your opponent.", 48),
            _nameBox,
            _createButton,
            _codeCaption,
            _codeLabel,
            _copyButton,
            _waitingLabel,
            _waitingBar,
            _status,
            _backButton
        });
    }

    private async Task CreateAsync()
    {
        if (!_createButton.Enabled) return;

        var name = _nameBox.Text.Trim();
        if (name.Length == 0)
        {
            Theme.SetStatus(_status, "Please enter your name.", StatusKind.Error);
            _nameBox.Focus();
            return;
        }

        _nameBox.Enabled = false;
        _createButton.Enabled = false;
        Theme.SetStatus(_status, "Creating room…", StatusKind.Info);

        try
        {
            var room = await _rooms.CreateRoomAsync(name);
            if (IsDisposed) return;

            _codeLabel.Text = room.Code;
            Theme.SetStatus(_status, "", StatusKind.Info);
            SetRoomVisible(true);

            // TODO (server): when a second player joins, leave this screen for the game/lobby.
        }
        catch (Exception ex)
        {
            if (IsDisposed) return;

            Theme.SetStatus(_status, $"Could not create room: {ex.Message}", StatusKind.Error);
            _nameBox.Enabled = true;
            _createButton.Enabled = true;
        }
    }

    private void SetRoomVisible(bool created)
    {
        _nameBox.Visible = !created;
        _createButton.Visible = !created;

        _codeCaption.Visible = created;
        _codeLabel.Visible = created;
        _copyButton.Visible = created;
        _waitingLabel.Visible = created;
        _waitingBar.Visible = created;

        _backButton.Text = created ? "Leave room" : "Back";
    }

    private void CopyCode()
    {
        try
        {
            Clipboard.SetText(_codeLabel.Text);
            Theme.SetStatus(_status, "Code copied to clipboard.", StatusKind.Success);
        }
        catch (Exception)
        {
            Theme.SetStatus(_status, "Could not access the clipboard.", StatusKind.Error);
        }
    }
}