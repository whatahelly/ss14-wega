using Content.Shared.Body.Prototypes;
using Content.Shared.Damage;
using Content.Shared.FixedPoint;
using Content.Shared.StatusIcon;
using Content.Shared.Store;
using Robust.Shared.Audio;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;

namespace Content.Shared.Vampire.Components;

[RegisterComponent, NetworkedComponent]
public sealed partial class VampireComponent : Component
{
    [ValidatePrototypeId<CurrencyPrototype>]
    public static readonly string CurrencyProto = "BloodEssence";

    [ValidatePrototypeId<MetabolizerTypePrototype>]
    public static readonly string MetabolizerVampire = "Vampire";

    public static readonly DamageSpecifier HolyDamage = new()
    {
        DamageDict = new Dictionary<string, FixedPoint2>() { { "Heat", 10 } }
    };

    public static readonly DamageSpecifier SpaceDamage = new()
    {
        DamageDict = new Dictionary<string, FixedPoint2>() { { "Heat", 2.5 } }
    };

    [ValidatePrototypeId<EntityPrototype>]
    public static readonly string DrinkActionPrototype = "ActionDrinkBlood";

    [ValidatePrototypeId<EntityPrototype>]
    public static readonly string StoreActionPrototype = "ActionVampireShop";

    [ValidatePrototypeId<EntityPrototype>]
    public static readonly string SelectClassActionPrototype = "ActionVampireSelectClass";

    [ValidatePrototypeId<EntityPrototype>]
    public static readonly string RejuvenateActionPrototype = "ActionVampireRejuvenate";

    [ValidatePrototypeId<EntityPrototype>]
    public static readonly string GlareActionPrototype = "ActionVampireGlare";

    public readonly SoundSpecifier BloodDrainSound = new SoundPathSpecifier(
        "/Audio/Items/drink.ogg",
        new AudioParams() { Volume = -3f, MaxDistance = 3f }
    );

    [DataField("vampireStatusIcon")]
    public ProtoId<FactionIconPrototype> StatusIcon { get; set; } = "VampireFaction";

    public float TotalBloodDrank = 0;

    public float NextSpaceDamageTick { get; set; }

    public float NextNullDamageTick { get; set; }

    public bool IsTruePowerActive = false;

    public bool IsDamageSharingActive = false;

    [DataField]
    public FixedPoint2 NullDamage = 0;

    [DataField]
    public float MouthVolume = 5;

    [DataField]
    public int ThrallCount = 0;

    [DataField]
    public int MaxThrallCount = 1;

    [DataField]
    public List<EntityUid> ThrallOwned = new();
}

[RegisterComponent, NetworkedComponent]
public sealed partial class ThrallComponent : Component
{
    [DataField]
    public EntityUid? VampireOwner = null;

    [DataField]
    public ProtoId<FactionIconPrototype> StatusIcon = "ThrallFaction";
}

/// <summary>
/// Marks an entity as taking damage when hit by a bible, rather than being healed
/// </summary>
[RegisterComponent]
public sealed partial class UnholyComponent : Component { }

[RegisterComponent]
public sealed partial class BeaconSoulComponent : Component
{
    [DataField]
    public EntityUid VampireOwner = EntityUid.Invalid;
}
