using HarmonyLib;
using Nebulae.RimWorld.UI;
using Nebulae.RimWorld.UI.Controls;
using Nebulae.RimWorld.UI.Controls.Panels;
using Nebulae.RimWorld.UI.Converters;
using Nebulae.RimWorld.UI.Data.Binding;
using UnityEngine;
using Verse;
using Grid = Nebulae.RimWorld.UI.Controls.Panels.Grid;

namespace NoCrowdedContextMenu
{
    public class NCCM : NebulaeMod<NCCMSettings>
    {
        #region Public Const

        public const float CheckBoxWidth = 280f;
        public const float RowHeight = 40f;
        public const float NumberBoxWidth = 100f;

        #endregion

        internal const string UniqueId = "Nebulae.NoCrowdedContextMenu";

        internal static readonly Harmony HarmonyInstance;

        private static bool _isWindowInitialized = false;
        private static ItemPickerWindow _itemPickerWindow;
        private static readonly ToggleStatusConverter _statusConverter = new ToggleStatusConverter();


        public override string CategoryLabel => "NCCM.SettingsCategory.Label".Translate();


        static NCCM()
        {
            HarmonyInstance = new Harmony(UniqueId);
            HarmonyInstance.Patch(
                AccessTools.Method(typeof(WindowStack), nameof(WindowStack.Add)),
                prefix: new HarmonyMethod(typeof(NCCM), nameof(WindowStack_AddPrefix)));
        }

        public NCCM(ModContentPack content) : base(content)
        {
        }

        protected override Control CreateContent()
        {
            var booleanSettings = CreateCheckList();

            var numberBox = new NumberBox
            {
                DecimalPartDigit = 0,
                Margin = 4f,
                Maximum = 200f,
                Minimum = 10f,
                Value = Settings.MinimumOptionCountCauseReplacement,
                Tooltip = "NCCM.Settings.MinimumOptionCountCauseReplacement.Tooltip".Translate(),
                ShowTooltip = true
            };

            var slider = new Slider
            {
                Maximum = 200f,
                Minimum = 10f,
                Step = 1f,
                Value = Settings.MinimumOptionCountCauseReplacement,
                Tooltip = "NCCM.Settings.MinimumOptionCountCauseReplacement.Tooltip".Translate(),
                ShowTooltip = true
            };

            BindingManager.Bind(
                slider,
                Slider.ValueProperty,
                numberBox,
                NumberBox.ValueProperty,
                BindingMode.TwoWay);

            BindingManager.Bind(
                slider,
                Slider.ValueProperty,
                Settings,
                nameof(Settings.MinimumOptionCountCauseReplacement),
                BindingMode.OneWay);

            var label = new Label
            {
                HorizontalAlignment = HorizontalAlignment.Left,
                Text = "NCCM.Settings.MinimumOptionCountCauseReplacement.Label".Translate(),
                Tooltip = "NCCM.Settings.MinimumOptionCountCauseReplacement.Tooltip".Translate(),
                ShowTooltip = true
            };

            return new Grid()
            .SetSize(new float[] { CheckBoxWidth, NumberBoxWidth },
            new float[]
            {
                RowHeight,
                RowHeight,
                RowHeight,
                RowHeight,
                RowHeight,
                RowHeight,
                RowHeight,
                RowHeight
            })
            .Set(new Control[]
            {
                booleanSettings[0], null,
                booleanSettings[1], null,
                booleanSettings[2], null,
                booleanSettings[3], null,
                booleanSettings[4], null,
                booleanSettings[5], null,
                label,              null,
                slider,             numberBox
            });
        }


        private static CheckBox[] CreateCheckList()
        {
            CheckBox[] checkBoxes = new CheckBox[6];
            for (int i = 0; i < 6; i++)
            {
                checkBoxes[i] = new CheckBox
                {
                    Padding = 0f,
                    Width = CheckBoxWidth,
                    HorizontalAlignment = HorizontalAlignment.Left,
                    ShowTooltip = true
                };
            }

            Initialize(checkBoxes[0], Settings.FocusSearchBar, nameof(Settings.FocusSearchBar));
            Initialize(checkBoxes[1], Settings.HasMemory, nameof(Settings.HasMemory));
            Initialize(checkBoxes[2], Settings.IsDragable, nameof(Settings.IsDragable));
            Initialize(checkBoxes[3], Settings.IsResizable, nameof(Settings.IsResizable));
            Initialize(checkBoxes[4], Settings.PauseGame, nameof(Settings.PauseGame));
            Initialize(checkBoxes[5], Settings.UseVanillaRenderMode, nameof(Settings.UseVanillaRenderMode));

            return checkBoxes;
        }

        private static void Initialize(CheckBox checkBox, bool value, string name)
        {
            checkBox.Status = value.ToStatus();
            checkBox.Status = value.ToStatus();
            checkBox.Text = ("NCCM.Settings." + name + ".Label").Translate();
            checkBox.Tooltip = ("NCCM.Settings." + name + ".Tooltip").Translate();

            BindingManager.Bind(
                checkBox,
                ToggleButton.StatusProperty,
                Settings,
                name,
                _statusConverter,
                BindingMode.OneWay);
        }


        [HarmonyPrefix]
        private static bool WindowStack_AddPrefix(WindowStack __instance, ref Window window)
        {
            if (window is FloatMenu menu)
            {
                var options = MenuOptionUtility.OptionsGetter(menu);

                if (options.Count < Settings.MinimumOptionCountCauseReplacement)
                {
                    return true;
                }

                if (!_isWindowInitialized)
                {
                    _itemPickerWindow = new ItemPickerWindow();
                    _isWindowInitialized = true;
                }

                _itemPickerWindow.SetOptions(menu, options);
                window = _itemPickerWindow;
            }
            return true;
        }
    }

    public class NCCMSettings : ModSettings
    {
        public bool FocusSearchBar = false;
        public bool HasMemory = false;
        public bool IsDragable = true;
        public bool IsResizable = true;
        public bool PauseGame = false;
        public bool UseVanillaRenderMode = false;

        public int MinimumOptionCountCauseReplacement = 10;

        public override void ExposeData()
        {
            Scribe_Values.Look(ref FocusSearchBar, nameof(FocusSearchBar), false);
            Scribe_Values.Look(ref HasMemory, nameof(HasMemory), false);
            Scribe_Values.Look(ref IsDragable, nameof(IsDragable), true);
            Scribe_Values.Look(ref IsResizable, nameof(IsResizable), true);
            Scribe_Values.Look(ref PauseGame, nameof(PauseGame), false);
            Scribe_Values.Look(ref UseVanillaRenderMode, nameof(UseVanillaRenderMode), false);

            Scribe_Values.Look(ref MinimumOptionCountCauseReplacement, nameof(MinimumOptionCountCauseReplacement), 10);
        }
    }
}
