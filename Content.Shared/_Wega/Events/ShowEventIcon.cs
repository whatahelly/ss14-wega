using Content.Shared.StatusIcon;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;

namespace Content.Shared.Event.Components
{
    [NetworkedComponent, RegisterComponent]
    public sealed partial class ShowEventIconComponent : Component
    {
        [DataField("eventStatusIcon")]
        public ProtoId<FactionIconPrototype> StatusIcon { get; set; } = "EventFaction";
    }
}
