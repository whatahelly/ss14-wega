using Content.Client.Audio;
using Content.Shared.CCVar;
using Content.Shared.Speech.Synthesis;
using Robust.Client.Audio;
using Robust.Shared.Audio;
using Robust.Shared.Audio.Systems;
using Robust.Shared.Configuration;
using Robust.Shared.Timing;

namespace Content.Client.Speech.Synthesis.System;

/// <summary>
/// Система отвечающая за прогрыщ звука для каждого калиента
/// </summary>
public sealed class BarkSystem : EntitySystem
{
    [Dependency] private readonly IConfigurationManager _cfg = default!;
    [Dependency] private readonly AudioSystem _audio = default!;
    [Dependency] private readonly IEntityManager _entityManager = default!;

    private const float MinimalVolume = -10f;
    private const float WhisperFade = 4f;

    public override void Initialize()
    {
        SubscribeNetworkEvent<PlayBarkEvent>(OnPlayBark);
    }

    public void RequestPreviewBark(string barkVoiceId)
    {
        RaiseNetworkEvent(new RequestPreviewBarkEvent(barkVoiceId));
    }

    private void OnPlayBark(PlayBarkEvent ev)
    {
        var sourceEntity = _entityManager.GetEntity(ev.SourceUid);
        if (!_entityManager.EntityExists(sourceEntity) || _entityManager.Deleted(sourceEntity))
            return;

        var userVolume = _cfg.GetCVar(WegaCVars.BarksVolume);
        var baseVolume = SharedAudioSystem.GainToVolume(userVolume * ContentAudioSystem.BarksMultiplier);

        float volume = MinimalVolume + baseVolume;

        if (ev.Obfuscated)
            volume -= WhisperFade;

        var audioParams = new AudioParams
        {
            Volume = volume,
            Variation = 0.125f
        };

        int messageLength = ev.Message.Length;
        float totalDuration = messageLength * 0.05f;
        float soundInterval = 0.15f / ev.PlaybackSpeed;

        int soundCount = (int)(totalDuration / soundInterval);
        soundCount = Math.Max(soundCount, 1);

        for (int i = 0; i < soundCount; i++)
        {
            Timer.Spawn(TimeSpan.FromSeconds(i * soundInterval), () =>
            {
                _audio.PlayPvs(ev.SoundPath, sourceEntity, audioParams);
            });
        }
    }
}
