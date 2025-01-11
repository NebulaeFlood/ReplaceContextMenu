using HarmonyLib;
using Nebulae.RimWorld.UI;
using Nebulae.RimWorld.UI.Controls;
using Nebulae.RimWorld.UI.Controls.Panels;
using Nebulae.RimWorld.UI.Converters;
using Nebulae.RimWorld.UI.Data.Binding;
using Verse;

namespace NoCrowdedContextMenu
{
    public class NCCM : NebulaeMod<NCCMSettings>
    {
        internal const string UniqueId = "Nebulae.NoCrowdedContextMenu";

        internal static readonly Harmony HarmonyInstance;

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
            var checkBox = new CheckBox
            {
                HorizontalAlignment = HorizontalAlignment.Left,
                Status = Settings.useVanillaRenderMode
                    ? ToggleStatus.Checked
                    : ToggleStatus.Unchecked,
                Text = "NCCM.Settings.UseVanillaRenderMode.Label".Translate(),
                Tooltip = "NCCM.Settings.UseVanillaRenderMode.Tooltip".Translate(),
                ShowTooltip = true
            };

            BindingManager.Bind(
                checkBox,
                ToggleButton.StatusProperty,
                Settings,
                nameof(Settings.useVanillaRenderMode),
                new ToggleStatusConverter(),
                BindingMode.OneWay);

            var numberBox = new NumberBox
            {
                DecimalPartDigit = 0,
                Margin = 4f,
                Maximum = 100,
                Minimum = 8,
                Value = Settings.minimumOptionCountCauseReplacement,
                Tooltip = "NCCM.Settings.MinimumOptionCountCauseReplacement.Tooltip".Translate(),
                ShowTooltip = true
            };

            var slider = new Slider
            {
                Maximum = 100f,
                Minimum = 8f,
                Step = 1f,
                Value = Settings.minimumOptionCountCauseReplacement,
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
                nameof(Settings.minimumOptionCountCauseReplacement),
                BindingMode.OneWay);

            var label = new Label
            {
                HorizontalAlignment = HorizontalAlignment.Left,
                Text = "NCCM.Settings.MinimumOptionCountCauseReplacement.Label".Translate(),
                Tooltip = "NCCM.Settings.MinimumOptionCountCauseReplacement.Tooltip".Translate(),
                ShowTooltip = true
            };

            return new Grid()
            .SetSize(new float[] { Grid.Remain, 100f }, new float[] { 40f, 40f, 40f })
            .Set(new Control[]
            {
                checkBox,   checkBox,
                label,      numberBox,
                slider,     slider
            });
        }

        [HarmonyPrefix]
        private static bool WindowStack_AddPrefix(WindowStack __instance, ref Window window)
        {
            if (window is FloatMenu menu)
            {
                var options = MenuOptionUtility.OptionsGetter(menu);

                if (options.Count < Settings.minimumOptionCountCauseReplacement)
                {
                    return true;
                }

                window = new ItemPickerWindow(menu, options);
            }
            return true;
        }
    }

    public class NCCMSettings : ModSettings
    {
        public int minimumOptionCountCauseReplacement = 10;
        public bool useVanillaRenderMode = false;

        public override void ExposeData()
        {
            Scribe_Values.Look(ref minimumOptionCountCauseReplacement, nameof(minimumOptionCountCauseReplacement), 10);
            Scribe_Values.Look(ref useVanillaRenderMode, nameof(useVanillaRenderMode), false);
        }
    }
}
