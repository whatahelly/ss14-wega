namespace Content.Server.GameTicking.Rules.Components;

/// <summary>
/// Stores data for <see cref="BloodCultRuleSystem"/>.
/// </summary>
[RegisterComponent, Access(typeof(BloodCultRuleSystem))]
public sealed partial class BloodCultRuleComponent : Component
{
    [DataField]
    public string? SelectedGod;

    [DataField]
    public BloodCultWinType WinType = BloodCultWinType.Neutral;

    [DataField]
    public List<BloodCultWinType> BloodCultWinCondition = new();
}

public enum BloodCultWinType : byte
{
    GodCalled,
    RitualConducted,
    Neutral,
    CultLose
}
