using Robust.Shared.Serialization;

namespace Content.Shared.Speech.Synthesis;

[Serializable, NetSerializable]
public sealed class RequestPreviewBarkEvent(string barkVoiceId) : EntityEventArgs
{
    public string BarkVoiceId { get; } = barkVoiceId;
}
