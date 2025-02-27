using System.IO;
using System.Numerics;
using System.Text.RegularExpressions;
using Content.Client.UserInterface.Systems.Interaction;
using Content.Shared.Chat.Prototypes;
using Content.Shared.Interaction.Panel;
using Content.Shared.Popups;
using Robust.Client.Player;
using Robust.Client.ResourceManagement;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Client.UserInterface.CustomControls;
using Robust.Client.UserInterface.XAML;
using Robust.Shared.Audio;

namespace Content.Client.Interaction.Panel.Ui
{
    public sealed partial class InteractionConstructorMenu : DefaultWindow
    {
        [Dependency] private readonly IFileDialogManager _dialogManager = default!;
        [Dependency] private readonly EntityManager _entManager = default!;
        [Dependency] private readonly InteractionPanelManager _sharedInteraction = default!;
        [Dependency] private readonly IPlayerManager _playerManager = default!;
        private readonly InteractionUIController _interactionPanelController;
        private SharedPopupSystem _popup;

        public BoxContainer PreviewContainer => this.FindControl<BoxContainer>("PreviewContainer");
        public LineEdit IdLine => this.FindControl<LineEdit>("IdLine");
        public LineEdit NameLine => this.FindControl<LineEdit>("NameLine");
        public LineEdit MessageLine => this.FindControl<LineEdit>("MessageLine");
        public LineEdit SpriteLine => this.FindControl<LineEdit>("SpriteLine");
        public CheckBox ErpCheckbox => this.FindControl<CheckBox>("ErpCheckbox");
        public OptionButton SexButton => this.FindControl<OptionButton>("SexButton");
        public OptionButton SpeciesButton => this.FindControl<OptionButton>("SpeciesButton");
        public CheckBox BlackCheckbox => this.FindControl<CheckBox>("BlackCheckbox");
        public OptionButton BlackListButton => this.FindControl<OptionButton>("BlackListButton");
        public OptionButton TargetSexButton => this.FindControl<OptionButton>("TargetSexButton");
        public OptionButton TargetSpeciesButton => this.FindControl<OptionButton>("TargetSpeciesButton");
        public LineEdit PathLine => this.FindControl<LineEdit>("PathLine");
        public CheckBox SoundCheckbox => this.FindControl<CheckBox>("SoundCheckbox");
        public CheckBox PathCheckbox => this.FindControl<CheckBox>("PathCheckbox");
        public OptionButton CollectionButton => this.FindControl<OptionButton>("CollectionButton");
        public CheckBox CollectionCheckbox => this.FindControl<CheckBox>("CollectionCheckbox");
        public Button ExportButton => this.FindControl<Button>("ExportButton");
        public Button AddButton => this.FindControl<Button>("AddButton");
        private ErrorLevel _errorLevel = ErrorLevel.None;
        private ISawmill _sawmill = default!;
        private bool _exporting;

        public InteractionConstructorMenu()
        {
            RobustXamlLoader.Load(this);
            IoCManager.InjectDependencies(this);

            _interactionPanelController = UserInterfaceManager.GetUIController<InteractionUIController>();

            _popup = _entManager.System<SharedPopupSystem>();

            UpdatePreview();

            IdLine.OnTextChanged += OnIdLineChanged;
            NameLine.OnTextChanged += OnNameLineChanged;
            MessageLine.OnTextChanged += OnMessageLineChanged;

            NameLine.OnTextChanged += _ => UpdatePreview();
            SpriteLine.OnTextChanged += _ => UpdatePreview();

            _errorLevel |= ErrorLevel.IdLine;
            _errorLevel |= ErrorLevel.NameLine;
            _errorLevel |= ErrorLevel.MessageLine;
            IdLine.ModulateSelfOverride = Color.Red;
            NameLine.ModulateSelfOverride = Color.Red;
            MessageLine.ModulateSelfOverride = Color.Red;

            InitializedItems();

            PathCheckbox.OnToggled += OnPathCheckboxToggled;
            CollectionCheckbox.OnToggled += OnCollectionCheckboxToggled;

            ExportButton.OnPressed += OnExportButtonPressed;
            AddButton.OnPressed += OnAddButtonPressed;
        }

        private void InitializedItems()
        {
            SexButton.AddItem(Loc.GetString("interaction-constructor-none"), (int)Sex.None);
            SexButton.AddItem(Loc.GetString("interaction-constructor-male"), (int)Sex.Male);
            SexButton.AddItem(Loc.GetString("interaction-constructor-female"), (int)Sex.Female);
            SexButton.AddItem(Loc.GetString("interaction-constructor-unsexed"), (int)Sex.Unsexed);

            SexButton.OnItemSelected += args =>
            {
                SexButton.SelectId(args.Id);
            };

            SpeciesButton.AddItem(Loc.GetString("interaction-constructor-none"), (int)Species.None);
            SpeciesButton.AddItem(Loc.GetString("interaction-constructor-human"), (int)Species.Human);
            SpeciesButton.AddItem(Loc.GetString("interaction-constructor-dwarf"), (int)Species.Dwarf);
            SpeciesButton.AddItem(Loc.GetString("interaction-constructor-felinid"), (int)Species.Felinid);
            SpeciesButton.AddItem(Loc.GetString("interaction-constructor-moth"), (int)Species.Moth);
            SpeciesButton.AddItem(Loc.GetString("interaction-constructor-reptilian"), (int)Species.Reptilian);
            SpeciesButton.AddItem(Loc.GetString("interaction-constructor-slimeperson"), (int)Species.SlimePerson);
            SpeciesButton.AddItem(Loc.GetString("interaction-constructor-vulpkanin"), (int)Species.Vulpkanin);
            SpeciesButton.AddItem(Loc.GetString("interaction-constructor-skrell"), (int)Species.Skrell);
            SpeciesButton.AddItem(Loc.GetString("interaction-constructor-resomi"), (int)Species.Resomi);

            SpeciesButton.OnItemSelected += args =>
            {
                SpeciesButton.SelectId(args.Id);
            };

            BlackListButton.AddItem(Loc.GetString("interaction-constructor-none"), (int)Species.None);
            BlackListButton.AddItem(Loc.GetString("interaction-constructor-human"), (int)Species.Human);
            BlackListButton.AddItem(Loc.GetString("interaction-constructor-dwarf"), (int)Species.Dwarf);
            BlackListButton.AddItem(Loc.GetString("interaction-constructor-felinid"), (int)Species.Felinid);
            BlackListButton.AddItem(Loc.GetString("interaction-constructor-moth"), (int)Species.Moth);
            BlackListButton.AddItem(Loc.GetString("interaction-constructor-reptilian"), (int)Species.Reptilian);
            BlackListButton.AddItem(Loc.GetString("interaction-constructor-slimeperson"), (int)Species.SlimePerson);
            BlackListButton.AddItem(Loc.GetString("interaction-constructor-vulpkanin"), (int)Species.Vulpkanin);
            BlackListButton.AddItem(Loc.GetString("interaction-constructor-skrell"), (int)Species.Skrell);
            BlackListButton.AddItem(Loc.GetString("interaction-constructor-resomi"), (int)Species.Resomi);

            BlackListButton.OnItemSelected += args =>
            {
                BlackListButton.SelectId(args.Id);
            };

            TargetSexButton.AddItem(Loc.GetString("interaction-constructor-none"), (int)Sex.None);
            TargetSexButton.AddItem(Loc.GetString("interaction-constructor-male"), (int)Sex.Male);
            TargetSexButton.AddItem(Loc.GetString("interaction-constructor-female"), (int)Sex.Female);
            TargetSexButton.AddItem(Loc.GetString("interaction-constructor-unsexed"), (int)Sex.Unsexed);

            TargetSexButton.OnItemSelected += args =>
            {
                TargetSexButton.SelectId(args.Id);
            };

            TargetSpeciesButton.AddItem(Loc.GetString("interaction-constructor-none"), (int)Species.None);
            TargetSpeciesButton.AddItem(Loc.GetString("interaction-constructor-human"), (int)Species.Human);
            TargetSpeciesButton.AddItem(Loc.GetString("interaction-constructor-dwarf"), (int)Species.Dwarf);
            TargetSpeciesButton.AddItem(Loc.GetString("interaction-constructor-felinid"), (int)Species.Felinid);
            TargetSpeciesButton.AddItem(Loc.GetString("interaction-constructor-moth"), (int)Species.Moth);
            TargetSpeciesButton.AddItem(Loc.GetString("interaction-constructor-reptilian"), (int)Species.Reptilian);
            TargetSpeciesButton.AddItem(Loc.GetString("interaction-constructor-slimeperson"), (int)Species.SlimePerson);
            TargetSpeciesButton.AddItem(Loc.GetString("interaction-constructor-vulpkanin"), (int)Species.Vulpkanin);
            TargetSpeciesButton.AddItem(Loc.GetString("interaction-constructor-skrell"), (int)Species.Skrell);
            TargetSpeciesButton.AddItem(Loc.GetString("interaction-constructor-resomi"), (int)Species.Resomi);

            TargetSpeciesButton.OnItemSelected += args =>
            {
                TargetSpeciesButton.SelectId(args.Id);
            };

            CollectionButton.AddItem(Loc.GetString("interaction-constructor-kisses"), (int)Collection.Kisses);

            CollectionButton.OnItemSelected += args =>
            {
                CollectionButton.SelectId(args.Id);
            };
        }

        private void UpdatePreview()
        {
            PreviewContainer.RemoveAllChildren();

            var buttonText = NameLine.Text;
            if (string.IsNullOrWhiteSpace(buttonText))
            {
                buttonText = Loc.GetString("interaction-constructor-unnamed");
            }

            var button = new Button
            {
                Text = buttonText,
                MinWidth = 420,
                MinHeight = 32
            };

            var spritePath = SpriteLine.Text;
            if (!string.IsNullOrWhiteSpace(spritePath))
            {
                if (!spritePath.StartsWith("/"))
                {
                    spritePath = "/Textures" + spritePath;
                }

                var textureResource = IoCManager.Resolve<IResourceCache>().GetResource<TextureResource>(spritePath);
                var iconButton = new TextureButton
                {
                    TextureNormal = textureResource.Texture,
                    Margin = new Thickness(4),
                    Scale = new Vector2(1f, 1f)
                };

                PreviewContainer.AddChild(iconButton);
            }

            PreviewContainer.AddChild(button);
        }

        private void OnIdLineChanged(LineEdit.LineEditEventArgs args)
        {
            if (string.IsNullOrWhiteSpace(args.Text))
            {
                _errorLevel |= ErrorLevel.IdLine;
                IdLine.ModulateSelfOverride = Color.Red;
            }
            else
            {
                _errorLevel &= ~ErrorLevel.IdLine;
                IdLine.ModulateSelfOverride = null;
            }

            UpdateAddButtonState();
        }

        private void OnNameLineChanged(LineEdit.LineEditEventArgs args)
        {
            if (string.IsNullOrWhiteSpace(args.Text))
            {
                _errorLevel |= ErrorLevel.NameLine;
                NameLine.ModulateSelfOverride = Color.Red;
            }
            else
            {
                _errorLevel &= ~ErrorLevel.NameLine;
                NameLine.ModulateSelfOverride = null;
            }

            UpdateAddButtonState();
        }

        private void OnMessageLineChanged(LineEdit.LineEditEventArgs args)
        {
            if (string.IsNullOrWhiteSpace(args.Text))
            {
                _errorLevel |= ErrorLevel.MessageLine;
                MessageLine.ModulateSelfOverride = Color.Red;
            }
            else
            {
                _errorLevel &= ~ErrorLevel.MessageLine;
                MessageLine.ModulateSelfOverride = null;
            }

            UpdateAddButtonState();
        }

        private void OnPathCheckboxToggled(BaseButton.ButtonEventArgs args)
        {
            if (args.Button.Pressed)
            {
                CollectionCheckbox.Pressed = false;
            }
        }

        private void OnCollectionCheckboxToggled(BaseButton.ButtonEventArgs args)
        {
            if (args.Button.Pressed)
            {
                PathCheckbox.Pressed = false;
            }
        }

        private void UpdateAddButtonState()
        {
            ExportButton.Disabled = _errorLevel != ErrorLevel.None;
            AddButton.Disabled = _errorLevel != ErrorLevel.None;
        }

        private async void OnExportButtonPressed(BaseButton.ButtonEventArgs args)
        {
            if (_errorLevel != ErrorLevel.None || _exporting)
                return;

            _exporting = true;

            var id = IdLine.Text;
            if (!IsValidId(id))
            {
                var session = _playerManager.LocalSession;
                if (session?.AttachedEntity.HasValue == true)
                {
                    _popup.PopupCursor(Loc.GetString("interaction-constructor-invalid-id"), session.AttachedEntity.Value);
                    _exporting = false;
                }
                return;
            }
            var name = NameLine.Text;
            var message = MessageLine.Text;
            var spritePath = SpriteLine.Text;
            if (string.IsNullOrWhiteSpace(spritePath))
            {
                spritePath = "/Textures/_Wega/Interface/InteractionPanel/heart.png";
            }
            var erp = ErpCheckbox.Pressed;

            var sexId = GetSelectedId(SexButton) ?? 0;
            var speciesId = GetSelectedId(SpeciesButton) ?? 0;
            var targetSexId = GetSelectedId(TargetSexButton) ?? 0;
            var targetSpeciesId = GetSelectedId(TargetSpeciesButton) ?? 0;
            var blackListSpeciesId = GetSelectedId(BlackListButton) ?? 0;
            var soundCollectionId = GetSelectedId(CollectionButton) ?? 0;

            var sex = GetSexString(sexId);
            var species = GetSpeciesString(speciesId);
            var targetSex = GetSexString(targetSexId);
            var targetSpecies = GetSpeciesString(targetSpeciesId);
            var blackListSpecies = GetSpeciesString(blackListSpeciesId);

            var soundPerceived = SoundCheckbox.Pressed;
            var soundCollection = GetCollectionString(soundCollectionId);
            var soundPath = PathLine.Text;

            var file = await _dialogManager.SaveFile(new FileDialogFilters(new FileDialogFilters.Group("yml")));
            if (file == null)
            {
                _exporting = false;
                return;
            }

            try
            {
                var interactionPrototype = new InteractionPrototype
                {
                    ID = id,
                    Name = name,
                    UserMessages = new List<string> { message },
                    Icon = spritePath,
                    ERP = erp,
                    AllowedGenders = new List<string> { sex },
                    AllowedSpecies = new List<string> { species },
                    NearestAllowedGenders = new List<string> { targetSex },
                    NearestAllowedSpecies = new List<string> { targetSpecies },
                    BlackListSpecies = null,
                    SoundPerceivedByOthers = soundPerceived,
                    InteractSound = null
                };
                if (BlackCheckbox.Pressed)
                {
                    interactionPrototype.BlackListSpecies = new List<string> { blackListSpecies };
                }
                if (PathCheckbox.Pressed && !string.IsNullOrWhiteSpace(PathLine.Text))
                {
                    interactionPrototype.InteractSound = new SoundPathSpecifier(soundPath);
                }
                else if (CollectionCheckbox.Pressed)
                {
                    interactionPrototype.InteractSound = new SoundCollectionSpecifier(soundCollection);
                }

                var dataNode = _sharedInteraction.ToDataNode(interactionPrototype);
                await using var writer = new StreamWriter(file.Value.fileStream);
                dataNode.Write(writer);

                _exporting = false;
            }
            catch (Exception exc)
            {
                _sawmill = Logger.GetSawmill("interaction_export");
                _sawmill.Error($"Error when exporting\n{exc.StackTrace}");
                _exporting = false;
            }
        }

        private void OnAddButtonPressed(BaseButton.ButtonEventArgs args)
        {
            if (_errorLevel != ErrorLevel.None)
                return;

            var id = IdLine.Text;
            if (!IsValidId(id))
            {
                var session = _playerManager.LocalSession;
                if (session?.AttachedEntity.HasValue == true)
                    _popup.PopupCursor(Loc.GetString("interaction-constructor-invalid-id"), session.AttachedEntity.Value);
                return;
            }
            var name = NameLine.Text;
            var message = MessageLine.Text;
            var spritePath = SpriteLine.Text;
            if (string.IsNullOrWhiteSpace(spritePath))
            {
                spritePath = "/Textures/_Wega/Interface/InteractionPanel/heart.png";
            }
            var erp = ErpCheckbox.Pressed;

            var sexId = GetSelectedId(SexButton) ?? 0;
            var speciesId = GetSelectedId(SpeciesButton) ?? 0;
            var targetSexId = GetSelectedId(TargetSexButton) ?? 0;
            var targetSpeciesId = GetSelectedId(TargetSpeciesButton) ?? 0;
            var blackListSpeciesId = GetSelectedId(BlackListButton) ?? 0;
            var soundCollectionId = GetSelectedId(CollectionButton) ?? 0;

            var sex = GetSexString(sexId);
            var species = GetSpeciesString(speciesId);
            var targetSex = GetSexString(targetSexId);
            var targetSpecies = GetSpeciesString(targetSpeciesId);
            var blackListSpecies = GetSpeciesString(blackListSpeciesId);

            var soundPerceived = SoundCheckbox.Pressed;
            var soundCollection = GetCollectionString(soundCollectionId);
            var soundPath = PathLine.Text;

            var interactionPrototype = new InteractionPrototype
            {
                ID = id,
                Name = name,
                UserMessages = new List<string> { message },
                Icon = spritePath,
                ERP = erp,
                AllowedGenders = new List<string> { sex },
                AllowedSpecies = new List<string> { species },
                NearestAllowedGenders = new List<string> { targetSex },
                NearestAllowedSpecies = new List<string> { targetSpecies },
                BlackListSpecies = null,
                SoundPerceivedByOthers = soundPerceived,
                InteractSound = null
            };
            if (BlackCheckbox.Pressed)
            {
                interactionPrototype.BlackListSpecies = new List<string> { blackListSpecies };
            }
            if (PathCheckbox.Pressed && !string.IsNullOrWhiteSpace(PathLine.Text))
            {
                interactionPrototype.InteractSound = new SoundPathSpecifier(soundPath);
            }
            else if (CollectionCheckbox.Pressed)
            {
                interactionPrototype.InteractSound = new SoundCollectionSpecifier(soundCollection);
            }

            _interactionPanelController.AddConstructor(interactionPrototype);
        }

        public bool IsValidId(string id)
        {
            var regex = new Regex(@"^[a-zA-Z0-9]+$");
            return regex.IsMatch(id);
        }

        private int? GetSelectedId(OptionButton optionButton)
        {
            if (optionButton.SelectedId == -1)
                return null;

            return optionButton.SelectedId;
        }

        private string GetSexString(int sexId)
        {
            return sexId switch
            {
                0 => "all",
                1 => "Male",
                2 => "Female",
                3 => "Unsexed",
                _ => throw new ArgumentOutOfRangeException(nameof(sexId), sexId, "Unknown sex ID")
            };
        }

        private string GetSpeciesString(int speciesId)
        {
            return speciesId switch
            {
                0 => "all",
                1 => "Human",
                2 => "Dwarf",
                3 => "Felinid",
                4 => "Moth",
                5 => "Reptilian",
                6 => "Slimeperson",
                7 => "Vulpkanin",
                8 => "Skrell",
                9 => "Resomi",
                _ => throw new ArgumentOutOfRangeException(nameof(speciesId), speciesId, "Unknown species ID")
            };
        }

        private string GetCollectionString(int collectionId)
        {
            return collectionId switch
            {
                0 => "Kisses", // You like kissing boys don't you
                _ => throw new ArgumentOutOfRangeException(nameof(collectionId), collectionId, "Unknown collection ID")
            };
        }

        [Flags]
        private enum ErrorLevel : byte
        {
            None = 0,
            IdLine = 1 << 0,
            NameLine = 1 << 1,
            MessageLine = 1 << 2,
        }

        private enum Sex : byte
        {
            None,
            Male,
            Female,
            Unsexed,
        }

        private enum Species : byte
        {
            None,
            Human,
            Dwarf,
            Felinid,
            Moth,
            Reptilian,
            SlimePerson,
            Vulpkanin,
            Skrell,
            Resomi,
        }

        private enum Collection : byte
        {
            Kisses,
        }
    }
}
