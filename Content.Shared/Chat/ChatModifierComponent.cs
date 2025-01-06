namespace Content.Shared.Chat;
/// <summary>
/// Corvax-Wega-Resomi
/// </summary>
[RegisterComponent]
public sealed partial class ChatModifierComponent : Component
{
    [DataField("whisperListeningRange")]
    public int WhisperListeningRange = SharedChatSystem.WhisperClearRange;
}
