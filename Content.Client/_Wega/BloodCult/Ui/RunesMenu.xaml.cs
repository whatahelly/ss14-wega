using System.Numerics;
using Content.Client.UserInterface.Controls;
using Content.Shared.Blood.Cult;
using Content.Shared.Blood.Cult.Components;
using Robust.Client.UserInterface.Controls;
using Robust.Client.UserInterface.CustomControls;
using Robust.Client.UserInterface.XAML;
using Robust.Shared.Player;

namespace Content.Client.Runes.Panel.Ui;

public sealed partial class RunesPanelMenu : DefaultWindow
{
    [Dependency] private readonly IEntityManager _entityManager = default!;
    [Dependency] private readonly IEntityNetworkManager _entityNetworkManager = default!;
    [Dependency] private readonly ISharedPlayerManager _playerManager = default!;

    public event Action<string>? OnRuneSelected;
    public BoxContainer RunesContainer => this.FindControl<BoxContainer>("RunesContainer");

    public RunesPanelMenu()
    {
        RobustXamlLoader.Load(this);
        IoCManager.InjectDependencies(this);

        InitializeRunes();
    }

    private void InitializeRunes()
    {
        AddRuneButton(Loc.GetString("offering-rune"), "BloodRuneOffering");
        AddRuneButton(Loc.GetString("teleport-rune"), "BloodRuneTeleport");
        AddRuneButton(Loc.GetString("empowering-rune"), "BloodRuneEmpowering");
        AddRuneButton(Loc.GetString("revive-rune"), "BloodRuneRevive");
        AddRuneButton(Loc.GetString("barrier-rune"), "BloodRuneBarrier");
        AddRuneButton(Loc.GetString("summoning-rune"), "BloodRuneSummoning");
        AddRuneButton(Loc.GetString("bloodboil-rune"), "BloodRuneBloodBoil");
        AddRuneButton(Loc.GetString("spiritrealm-rune"), "BloodRuneSpiritealm");
        AddRuneButton(Loc.GetString("ritual-dimensional-rending-rune"), "BloodRuneRitualDimensionalRending");
    }

    private void AddRuneButton(string runeName, string protoId)
    {
        var button = new Button
        {
            Text = runeName,
            MinSize = new Vector2(300, 32),
            MaxSize = new Vector2(300, 32),
            HorizontalAlignment = HAlignment.Center,
            VerticalAlignment = VAlignment.Center,
        };

        button.OnPressed += _ => HandleRuneSelection(protoId);

        RunesContainer.AddChild(button);
    }

    private void HandleRuneSelection(string protoId)
    {
        OnRuneSelected?.Invoke(protoId);
        var netEntity = _entityManager.GetNetEntity(_playerManager.LocalSession?.AttachedEntity ?? EntityUid.Invalid);
        _entityNetworkManager.SendSystemNetworkMessage(new RuneSelectEvent(netEntity, protoId));
        Close();
    }

    public new void Close()
    {
        base.Close();
    }
}

public sealed class EmpoweringRuneMenu : RadialMenu
{
    [Dependency] private readonly IEntityManager _entityManager = default!;
    [Dependency] private readonly IEntityNetworkManager _entityNetworkManager = default!;
    [Dependency] private readonly ISharedPlayerManager _playerManager = default!;

    public event Action<string>? OnSelectSpell;
    public bool IsDisposed { get; private set; }

    public EmpoweringRuneMenu()
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
        _entityNetworkManager.SendSystemNetworkMessage(new EmpoweringRuneMenuClosedEvent(netEntity, spellName));
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

public sealed partial class SummoningRunePanelMenu : DefaultWindow
{
    [Dependency] private readonly IEntityManager _entityManager = default!;
    [Dependency] private readonly IEntityNetworkManager _entityNetworkManager = default!;
    [Dependency] private readonly ISharedPlayerManager _playerManager = default!;

    public BoxContainer CultistsContainer => this.FindControl<BoxContainer>("CultistsContainer");

    public SummoningRunePanelMenu()
    {
        RobustXamlLoader.Load(this);
        IoCManager.InjectDependencies(this);

        InitializeButtons();
    }

    private void InitializeButtons()
    {
        foreach (var cultist in _entityManager.EntityQuery<BloodCultistComponent>())
        {
            if (_entityManager.TryGetComponent<MetaDataComponent>(cultist.Owner, out var metaData))
            {
                var entityName = metaData.EntityName;
                AddCultistButton(entityName, cultist.Owner);
            }
        }
    }

    private void AddCultistButton(string cultistName, EntityUid cultistUid)
    {
        var button = new Button
        {
            Text = cultistName,
            HorizontalAlignment = HAlignment.Center,
            VerticalAlignment = VAlignment.Center,
            MinSize = new Vector2(300, 32),
            MaxSize = new Vector2(300, 32)
        };

        button.OnPressed += _ => HandleCultistSelection(cultistUid);

        CultistsContainer.AddChild(button);
    }

    private void HandleCultistSelection(EntityUid cultistUid)
    {
        var netTargerEntity = _entityManager.GetNetEntity(cultistUid);
        var netEntity = _entityManager.GetNetEntity(_playerManager.LocalSession?.AttachedEntity ?? EntityUid.Invalid);
        _entityNetworkManager.SendSystemNetworkMessage(new SummoningSelectedEvent(netEntity, netTargerEntity));
        Close();
    }
}
