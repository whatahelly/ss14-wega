using Content.Client.UserInterface.Controls;
using Content.Shared.Blood.Cult;
using Robust.Client.UserInterface.XAML;
using Robust.Shared.Player;

namespace Content.Client.Blood.Rites.UI;

public sealed class BloodRitesMenu : RadialMenu
{
    [Dependency] private readonly IEntityManager _entityManager = default!;
    [Dependency] private readonly IEntityNetworkManager _entityNetworkManager = default!;
    [Dependency] private readonly ISharedPlayerManager _playerManager = default!;

    public event Action<string>? OnSelectRites;
    public bool IsDisposed { get; private set; }

    public BloodRitesMenu()
    {
        RobustXamlLoader.Load(this);
        IoCManager.InjectDependencies(this);

        InitializeButtons();
    }

    private void InitializeButtons()
    {
        var bloodOrbButton = FindControl<RadialMenuTextureButton>("BloodOrbButton");
        var bloodRechargeButton = FindControl<RadialMenuTextureButton>("BloodRechargeButton");
        var bloodSpearButton = FindControl<RadialMenuTextureButton>("BloodSpearButton");
        var bloodBoltBarrageButton = FindControl<RadialMenuTextureButton>("BloodBoltBarrageButton");

        bloodOrbButton.OnButtonUp += _ => HandleRitesSelection("ActionBloodCultOrb");
        bloodRechargeButton.OnButtonUp += _ => HandleRitesSelection("ActionBloodCultRecharge");
        bloodSpearButton.OnButtonUp += _ => HandleRitesSelection("ActionBloodCultSpear");
        bloodBoltBarrageButton.OnButtonUp += _ => HandleRitesSelection("ActionBloodCultBoltBarrage");
    }

    private void HandleRitesSelection(string ritesName)
    {
        OnSelectRites?.Invoke(ritesName);
        var netEntity = _entityManager.GetNetEntity(_playerManager.LocalSession?.AttachedEntity ?? EntityUid.Invalid);
        _entityNetworkManager.SendSystemNetworkMessage(new BloodRitesMenuClosedEvent(netEntity, ritesName));
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

