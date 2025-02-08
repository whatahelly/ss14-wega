using Content.Client.UserInterface.Controls;
using Content.Shared.Blood.Cult;
using Robust.Client.UserInterface.XAML;
using Robust.Shared.Player;

namespace Content.Client.Select.Construct.UI;

public sealed class BloodConstructMenu : RadialMenu
{
    [Dependency] private readonly IEntityManager _entityManager = default!;
    [Dependency] private readonly IEntityNetworkManager _entityNetworkManager = default!;
    [Dependency] private readonly ISharedPlayerManager _playerManager = default!;

    public event Action<string>? OnSelectConstruct;
    public bool IsDisposed { get; private set; }
    private NetEntity _constructUid;
    private NetEntity _mindUid;

    public BloodConstructMenu()
    {
        RobustXamlLoader.Load(this);
        IoCManager.InjectDependencies(this);

        InitializeButtons();
    }

    private void InitializeButtons()
    {
        var juggernautButton = FindControl<RadialMenuTextureButton>("BloodJuggernautButton");
        var wraithButton = FindControl<RadialMenuTextureButton>("BloodWraithButton");
        var artificerButton = FindControl<RadialMenuTextureButton>("BloodArtificerButton");
        var proteonButton = FindControl<RadialMenuTextureButton>("BloodProteonButton");

        juggernautButton.OnButtonUp += _ => HandleRitesSelection("MobConstructJuggernaut");
        wraithButton.OnButtonUp += _ => HandleRitesSelection("MobConstructWraith");
        artificerButton.OnButtonUp += _ => HandleRitesSelection("MobConstructArtificer");
        proteonButton.OnButtonUp += _ => HandleRitesSelection("MobConstructProteon");
    }

    public void SetData(NetEntity constructUid, NetEntity mindUid)
    {
        _constructUid = constructUid;
        _mindUid = mindUid;
    }

    private void HandleRitesSelection(string constructName)
    {
        OnSelectConstruct?.Invoke(constructName);
        var netEntity = _entityManager.GetNetEntity(_playerManager.LocalSession?.AttachedEntity ?? EntityUid.Invalid);
        _entityNetworkManager.SendSystemNetworkMessage(new BloodConstructMenuClosedEvent(netEntity, _constructUid, _mindUid, constructName));
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

