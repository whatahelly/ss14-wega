using Content.Shared.Speech.Synthesis;

namespace Content.Client.Speech.Synthesis.System;

/// <summary>
/// Заглушка для отправки ивента на сервер
/// </summary>
public sealed class BarkSystem : EntitySystem
{
    public void RequestPreviewBark(string barkVoiceId)
    {
        RaiseNetworkEvent(new RequestPreviewBarkEvent(barkVoiceId));
    }
}
