using Content.Shared.Blood.Cult;
using Robust.Client.Player;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controllers;
using Timer = Robust.Shared.Timing.Timer;

namespace Content.Client.Runes.Panel.Ui
{
    public sealed class RunesMenuUIController : UIController
    {
        [Dependency] private readonly IUserInterfaceManager _uiManager = default!;
        [Dependency] private readonly IEntityManager _entityManager = default!;

        private RunesPanelMenu? _panel;
        private bool _panelDisposed = false;

        public override void Initialize()
        {
            base.Initialize();

            SubscribeNetworkEvent<RunesMenuOpenedEvent>(OnRunesMenuReceived);
        }

        private void OnRunesMenuReceived(RunesMenuOpenedEvent args, EntitySessionEventArgs eventArgs)
        {
            var session = IoCManager.Resolve<IPlayerManager>().LocalSession;
            var userEntity = _entityManager.GetEntity(args.Uid);
            if (session?.AttachedEntity.HasValue == true && session.AttachedEntity.Value == userEntity)
            {
                if (_panel is null)
                {
                    _panel = _uiManager.CreateWindow<RunesPanelMenu>();
                    _panel.OnClose += OnMenuClosed;
                    _panel.OpenCentered();
                }
                else
                {
                    _panel.OpenCentered();
                }
            }
        }

        private void OnMenuClosed()
        {
            _panelDisposed = true;
            _panel = null;
        }
    }

    public sealed class EmpoweringRuneMenuUIController : UIController
    {
        [Dependency] private readonly IUserInterfaceManager _uiManager = default!;
        [Dependency] private readonly IEntityNetworkManager _entityNetworkManager = default!;
        [Dependency] private readonly IEntityManager _entityManager = default!;

        private EmpoweringRuneMenu? _panel;
        private bool _panelDisposed = false;

        public override void Initialize()
        {
            base.Initialize();

            SubscribeNetworkEvent<EmpoweringRuneMenuOpenedEvent>(OnRuneMenuReceived);
        }

        private void OnRuneMenuReceived(EmpoweringRuneMenuOpenedEvent args, EntitySessionEventArgs eventArgs)
        {
            var session = IoCManager.Resolve<IPlayerManager>().LocalSession;
            var userEntity = _entityManager.GetEntity(args.Uid);
            if (session?.AttachedEntity.HasValue == true && session.AttachedEntity.Value == userEntity)
            {
                if (_panel is null)
                {
                    _panel = _uiManager.CreateWindow<EmpoweringRuneMenu>();
                    _panel.OnClose += OnMenuClosed;
                    _panel.OpenCentered();
                }
                else
                {
                    _panel.OpenCentered();
                }

                Timer.Spawn(30000, () =>
                {
                    if (_panel != null && !_panelDisposed)
                    {
                        _panel.Close();
                    }
                });
            }
        }

        private void OnMenuClosed()
        {
            _panelDisposed = true;
            _panel = null;
        }
    }

    public sealed class SummoningRuneMenuUIController : UIController
    {
        [Dependency] private readonly IUserInterfaceManager _uiManager = default!;
        [Dependency] private readonly IEntityNetworkManager _entityNetworkManager = default!;
        [Dependency] private readonly IEntityManager _entityManager = default!;

        private SummoningRunePanelMenu? _panel;
        private bool _panelDisposed = false;

        public override void Initialize()
        {
            base.Initialize();

            SubscribeNetworkEvent<SummoningRuneMenuOpenedEvent>(OnRuneMenuReceived);
        }

        private void OnRuneMenuReceived(SummoningRuneMenuOpenedEvent args, EntitySessionEventArgs eventArgs)
        {
            var session = IoCManager.Resolve<IPlayerManager>().LocalSession;
            var userEntity = _entityManager.GetEntity(args.Uid);
            if (session?.AttachedEntity.HasValue == true && session.AttachedEntity.Value == userEntity)
            {
                if (_panel is null)
                {
                    _panel = _uiManager.CreateWindow<SummoningRunePanelMenu>();
                    _panel.OnClose += OnMenuClosed;
                    _panel.OpenCentered();
                }
                else
                {
                    _panel.OpenCentered();
                }

                Timer.Spawn(30000, () =>
                {
                    if (_panel != null && !_panelDisposed)
                    {
                        _panel.Close();
                    }
                });
            }
        }

        private void OnMenuClosed()
        {
            _panelDisposed = true;
            _panel = null;
        }
    }
}
