using Content.Client.UserInterface.Controls;
using Content.Shared.Blood.Cult;
using Robust.Client.UserInterface.XAML;
using Robust.Shared.Player;

namespace Content.Client.Blood.Magic.UI;

public sealed class BloodMagicMenu : RadialMenu
{
    [Dependency] private readonly IEntityManager _entityManager = default!;
    [Dependency] private readonly IEntityNetworkManager _entityNetworkManager = default!;
    [Dependency] private readonly ISharedPlayerManager _playerManager = default!;

    public event Action<string>? OnSelectSpell;
    public bool IsDisposed { get; private set; }

    public BloodMagicMenu()
    {
        RobustXamlLoader.Load(this);
        IoCManager.InjectDependencies(this);

        InitializeButtons();
    }

    private void InitializeButtons()
    {
        var stunButton = FindControl<RadialMenuTextureButton>("StunButton");
        var teleportButton = FindControl<RadialMenuTextureButton>("TeleportButton");
        var electromagneticPulseButton = FindControl<RadialMenuTextureButton>("ElectromagneticPulseButton");
        var shadowShacklesButton = FindControl<RadialMenuTextureButton>("ShadowShacklesButton");
        var twistedConstructionButton = FindControl<RadialMenuTextureButton>("TwistedConstructionButton");
        var summonEquipmentButton = FindControl<RadialMenuTextureButton>("SummonEquipmentButton");
        var summonDaggerButton = FindControl<RadialMenuTextureButton>("SummonDaggerButton");
        var hallucinationsButton = FindControl<RadialMenuTextureButton>("HallucinationsButton");
        var concealPresenceButton = FindControl<RadialMenuTextureButton>("ConcealPresenceButton");
        var bloodRitesButton = FindControl<RadialMenuTextureButton>("BloodRitesButton");

        stunButton.OnButtonUp += _ => HandleSpellSelection("ActionBloodCultStun");
        teleportButton.OnButtonUp += _ => HandleSpellSelection("ActionBloodCultTeleport");
        electromagneticPulseButton.OnButtonUp += _ => HandleSpellSelection("ActionBloodCultElectromagneticPulse");
        shadowShacklesButton.OnButtonUp += _ => HandleSpellSelection("ActionBloodCultShadowShackles");
        twistedConstructionButton.OnButtonUp += _ => HandleSpellSelection("ActionBloodCultTwistedConstruction");
        summonEquipmentButton.OnButtonUp += _ => HandleSpellSelection("ActionBloodCultSummonEquipment");
        summonDaggerButton.OnButtonUp += _ => HandleSpellSelection("ActionBloodCultSummonDagger");
        hallucinationsButton.OnButtonUp += _ => HandleSpellSelection("ActionBloodCultHallucinations");
        concealPresenceButton.OnButtonUp += _ => HandleSpellSelection("ActionBloodCultConcealPresence");
        bloodRitesButton.OnButtonUp += _ => HandleSpellSelection("ActionBloodCultBloodRites");
    }

    private void HandleSpellSelection(string spellName)
    {
        OnSelectSpell?.Invoke(spellName);
        var netEntity = _entityManager.GetNetEntity(_playerManager.LocalSession?.AttachedEntity ?? EntityUid.Invalid);
        _entityNetworkManager.SendSystemNetworkMessage(new BloodMagicMenuClosedEvent(netEntity, spellName));
        Close();
    }

    public new void Close()
    {
        if (!IsDisposed)
        {
            IsDisposed = true;
            Dispose();
        }
    }
}

