using Robust.Shared.Prototypes;

namespace Content.Shared.Speech.Synthesis;

/// <summary>
/// Прототип для доступных барков.
/// </summary>
[Prototype("bark")]
public sealed class BarkPrototype : IPrototype
{
    [IdDataField]
    public string ID { get; } = default!;

    /// <summary>
    /// Название голоса.
    /// </summary>
    [DataField("name")]
    public string Name { get; } = string.Empty;

    /// <summary>
    /// Набор звуков, используемых для речи.
    /// </summary>
    [DataField("soundFiles", required: true)]
    public List<string> SoundFiles { get; } = new();

    /// <summary>
    /// Доступен ли на старте раунда.
    /// </summary>
    [DataField("roundStart")]
    public bool RoundStart { get; } = true;
}
