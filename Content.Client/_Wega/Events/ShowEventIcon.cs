using Content.Shared.Event.Components;
using Content.Shared.StatusIcon.Components;
using Robust.Shared.Prototypes;

namespace Content.Client.Blood.Cult
{
    public sealed class ShowEventIconSystem : EntitySystem
    {
        [Dependency] private readonly IPrototypeManager _prototype = default!;

        public override void Initialize()
        {
            base.Initialize();

            SubscribeLocalEvent<ShowEventIconComponent, GetStatusIconsEvent>(GetEventIcons);
        }

        private void GetEventIcons(Entity<ShowEventIconComponent> ent, ref GetStatusIconsEvent args)
        {
            var iconPrototype = _prototype.Index(ent.Comp.StatusIcon);
            args.StatusIcons.Add(iconPrototype);
        }
    }
}
