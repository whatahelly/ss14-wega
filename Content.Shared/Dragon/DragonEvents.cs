using Content.Shared.Actions;

namespace Content.Shared.Dragon;

public sealed partial class DragonDevourActionEvent : EntityTargetActionEvent
{
}

public sealed partial class DragonSpawnRiftActionEvent : InstantActionEvent
{
}

//Corvax-Wega-DragonPushSkill-start
public sealed partial class DragonPushActionEvent : InstantActionEvent
{
    public string Sound = "/Audio/Effects/Footsteps/largethud.ogg";
}
//Corvax-Wega-DragonPushSkill-end
