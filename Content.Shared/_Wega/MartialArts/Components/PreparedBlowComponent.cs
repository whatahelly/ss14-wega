namespace Content.Shared.Martial.Arts.Components;

/// <summary>
/// The component needed to process the logic of a combat strike.
/// </summary>
[RegisterComponent, Access(typeof(SharedMartialArtsSystem))]
public sealed partial class PreparedBlowComponent : Component
{
    [DataField]
    public string SelectedType;
}
