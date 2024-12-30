using Content.Client.UserInterface.Controls;
using Content.Shared.Vampire;
using Robust.Client.UserInterface.XAML;
using Robust.Shared.Player;

namespace Content.Client.Select.Class.UI;

public sealed class SelectClassMenu : RadialMenu
{
    [Dependency] private readonly IEntityManager _entityManager = default!;
    [Dependency] private readonly IEntityNetworkManager _entityNetworkManager = default!;
    [Dependency] private readonly ISharedPlayerManager _playerManager = default!;

    public event Action<string>? OnSelectClass;
    public bool IsDisposed { get; private set; }

    public SelectClassMenu()
    {
        RobustXamlLoader.Load(this);
        IoCManager.InjectDependencies(this);

        InitializeButtons();
    }

    private void InitializeButtons()
    {
        var hemomancerButton = FindControl<RadialMenuTextureButton>("HemomancerButton");
        var umbraeButton = FindControl<RadialMenuTextureButton>("UmbraeButton");
        var gargantuaButton = FindControl<RadialMenuTextureButton>("GargantuaButton");
        var dantalionButton = FindControl<RadialMenuTextureButton>("DantalionButton");
        //var bestiaButton = FindControl<RadialMenuTextureButton>("BestiaButton");

        hemomancerButton.OnButtonUp += _ => HandleClassSelection("Hemomancer");
        umbraeButton.OnButtonUp += _ => HandleClassSelection("Umbrae");
        gargantuaButton.OnButtonUp += _ => HandleClassSelection("Gargantua");
        dantalionButton.OnButtonUp += _ => HandleClassSelection("Dantalion");
        //bestiaButton.OnButtonUp += _ => HandleClassSelection("Bestia");
    }

    private void HandleClassSelection(string className)
    {
        OnSelectClass?.Invoke(className);
        var netEntity = _entityManager.GetNetEntity(_playerManager.LocalSession?.AttachedEntity ?? EntityUid.Invalid);
        _entityNetworkManager.SendSystemNetworkMessage(new VampireSelectClassMenuClosedEvent(netEntity, className));
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

