using Content.Shared.CCVar;
using Content.Server.Chat.Systems;
using Content.Shared.Speech.Synthesis;
using Content.Shared.Speech.Synthesis.Components;
using Robust.Server.Audio;
using Robust.Shared.Audio;
using Robust.Shared.Configuration;
using Robust.Shared.Prototypes;
using Robust.Shared.Timing;

namespace Content.Server.Speech.Synthesis.System;

/// <summary>
/// Обрабатывает барки для сущностей.
/// </summary>
public sealed class BarkSystem : EntitySystem
{
    [Dependency] private readonly AudioSystem _audio = default!;
    [Dependency] private readonly IPrototypeManager _prototypeManager = default!;
    [Dependency] private readonly IConfigurationManager _configurationManager = default!;

    public override void Initialize()
    {
        SubscribeLocalEvent<SpeechSynthesisComponent, EntitySpokeEvent>(OnEntitySpoke);

        SubscribeNetworkEvent<RequestPreviewBarkEvent>(OnRequestPreviewBark);
    }

    private void OnEntitySpoke(EntityUid uid, SpeechSynthesisComponent comp, EntitySpokeEvent args)
    {
        if (comp.VoicePrototypeId is null || !_prototypeManager.TryIndex<BarkPrototype>(comp.VoicePrototypeId, out var barkProto)
            || !_configurationManager.GetCVar(WegaCVars.BarksEnabled))
            return;

        var soundPath = barkProto.SoundFiles[new Random().Next(barkProto.SoundFiles.Count)];
        var soundSpecifier = new SoundPathSpecifier(soundPath);

        float volume = -2f;
        if (args.ObfuscatedMessage != null)
            volume = -8f;
        else if (args.Message.EndsWith("!"))
            volume = 4f;

        var audioParams = new AudioParams
        {
            Pitch = comp.Pitch,
            Volume = volume,
            Variation = 0.125f
        };

        int messageLength = args.Message.Length;
        float totalDuration = messageLength * 0.05f;
        float soundInterval = 0.15f / comp.PlaybackSpeed;

        int soundCount = (int)(totalDuration / soundInterval);
        soundCount = Math.Max(soundCount, 1);

        for (int i = 0; i < soundCount; i++)
        {
            Timer.Spawn(TimeSpan.FromSeconds(i * soundInterval), () =>
            {
                _audio.PlayPvs(soundSpecifier, uid, audioParams);
            });
        }
    }

    private async void OnRequestPreviewBark(RequestPreviewBarkEvent ev, EntitySessionEventArgs args)
    {
        if (string.IsNullOrEmpty(ev.BarkVoiceId) || !_prototypeManager.TryIndex<BarkPrototype>(ev.BarkVoiceId, out var barkProto)
            || !_configurationManager.GetCVar(WegaCVars.BarksEnabled))
            return;

        var soundPath = barkProto.SoundFiles[new Random().Next(barkProto.SoundFiles.Count)];
        var soundSpecifier = new SoundPathSpecifier(soundPath);

        var audioParams = new AudioParams
        {
            Pitch = 1.0f,
            Volume = 4f,
            Variation = 0.125f
        };

        _audio.PlayGlobal(soundSpecifier, args.SenderSession, audioParams);
    }
}
