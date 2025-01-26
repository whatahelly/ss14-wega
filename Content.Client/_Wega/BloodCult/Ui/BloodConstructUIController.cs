using Content.Shared.Blood.Cult;
using Robust.Client.Player;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controllers;
using Timer = Robust.Shared.Timing.Timer;

namespace Content.Client.Select.Construct.UI
{
    public sealed class BloodConstructMenuUIController : UIController
    {
        [Dependency] private readonly IUserInterfaceManager _uiManager = default!;
        [Dependency] private readonly IEntityManager _entityManager = default!;

        private BloodConstructMenu? _menu;
        private bool _menuDisposed = false;

        public override void Initialize()
        {
            base.Initialize();
            SubscribeNetworkEvent<OpenConstructMenuEvent>(OnConstructMenuReceived);
        }

        private void OnConstructMenuReceived(OpenConstructMenuEvent args, EntitySessionEventArgs eventArgs)
        {
            var session = IoCManager.Resolve<IPlayerManager>().LocalSession;
            var userEntity = _entityManager.GetEntity(args.Uid);

            if (session?.AttachedEntity.HasValue == true && session.AttachedEntity.Value == userEntity)
            {
                if (_menu is null || _menu.IsDisposed)
                {
                    _menu = _uiManager.CreateWindow<BloodConstructMenu>();
                    _menu.OnClose += OnMenuClosed;

                    _menu.SetData(args.ConstructUid, args.Mind);

                    _menu.OpenCentered();
                }
                else
                {
                    _menu.OpenCentered();
                }

                Timer.Spawn(30000, () =>
                {
                    if (_menu != null && !_menuDisposed)
                    {
                        _menu.Close();
                    }
                });
            }
        }

        private void OnMenuClosed()
        {
            _menuDisposed = true;
            _menu = null;
        }
    }
}
