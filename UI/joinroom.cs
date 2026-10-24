using Unmatched.UI.Services;
using Unmatched.UI.Ui;

namespace Unmatched.UI.Screens;

public class JoinRoomScreen : ScreenBase
{
    private readonly IRoomService _rooms;

    private readonly TextBox _nameBox = Theme.Input("Your name");
    private readonly TextBox _codeBox = Theme.Input("Room code", RoomCode.Length);
    private readonly Button _joinButton;
    private readonly Label _status = Theme.Status();

    public JoinRoomScreen(IRoomService rooms, Action onBack)
    {
        _rooms = rooms;

        _codeBox.CharacterCasing = CharacterCasing.Upper;
        _joinButton = Theme.PrimaryButton("Join", () => _ = JoinAsync());

        void EnterToJoin(object? sender, KeyEventArgs e)
        {
            if (e.KeyCode != Keys.Enter) return;
            e.SuppressKeyPress = true;
            _ = JoinAsync();
        }
        _nameBox.KeyDown += EnterToJoin;
        _codeBox.KeyDown += EnterToJoin;

        Content.Controls.AddRange(new Control[]
        {
            Theme.Heading("Join a room"),
            Theme.Muted("Enter your name and the room code your opponent shared.", 48),
            _nameBox,
            _codeBox,
            _joinButton,
            _status,
            Theme.SecondaryButton("Back", onBack)
        });
    }

    private async Task JoinAsync()
    {
        if (!_joinButton.Enabled) return;

        var name = _nameBox.Text.Trim();
        var code = _codeBox.Text.Trim().ToUpperInvariant();

        if (name.Length == 0)
        {
            Theme.SetStatus(_status, "Please enter your name.", StatusKind.Error);
            _nameBox.Focus();
            return;
        }

        if (!RoomCode.IsValid(code))
        {
            Theme.SetStatus(_status, $"Room codes are {RoomCode.Length} characters (letters and digits).", StatusKind.Error);
            _codeBox.Focus();
            return;
        }

        SetInputsEnabled(false);
        Theme.SetStatus(_status, "Joining room…", StatusKind.Info);

        try
        {
            var room = await _rooms.JoinRoomAsync(code, name);
            if (IsDisposed) return;

            // TODO (server): leave this screen for the game/lobby once the join is confirmed.
            Theme.SetStatus(_status, $"Joined room {room.Code}. (Lobby not implemented yet.)", StatusKind.Success);
        }
        catch (Exception ex)
        {
            if (IsDisposed) return;

            Theme.SetStatus(_status, $"Could not join: {ex.Message}", StatusKind.Error);
            SetInputsEnabled(true);
        }
    }

    private void SetInputsEnabled(bool enabled)
    {
        _nameBox.Enabled = enabled;
        _codeBox.Enabled = enabled;
        _joinButton.Enabled = enabled;
    }
}