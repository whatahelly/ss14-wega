using Content.Shared.Martial.Arts.Prototypes;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;

namespace Content.Shared.Martial.Arts.Components;

/// <summary>
/// The component responsible for assigning actions and processing the logic of martial arts.
/// </summary>
[RegisterComponent, NetworkedComponent, Access(typeof(SharedMartialArtsSystem))]
public sealed partial class MartialArtsComponent : Component
{
    [DataField]
    public ProtoId<MartialArtsPrototype> Style;

    [DataField]
    public List<EntityUid> AddedActions { get; private set; } = new();
}
