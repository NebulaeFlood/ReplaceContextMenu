using HarmonyLib;
using Nebulae.RimWorld.UI;
using Nebulae.RimWorld.UI.Controls;
using Nebulae.RimWorld.UI.Utilities;
using NoCrowdedContextMenu.SettingPages;
using RimWorld;
using Verse;

namespace NoCrowdedContextMenu
{
    public class NCCM : NebulaeMod<NCCMSettings>
    {
        internal const string DebugLabel = "ReplaceContextMenu";
        internal const string UniqueId = "Nebulae.NoCrowdedContextMenu";
        internal static readonly Harmony HarmonyInstance;


        public override string CategoryLabel => "NCCM.SettingsCategory.Label".Translate();


        static NCCM()
        {
            HarmonyInstance = new Harmony(UniqueId);

            HarmonyInstance.Patch(
                AccessTools.Method(typeof(Designator_Build), nameof(Designator_Build.ProcessInput)),
                prefix: new HarmonyMethod(typeof(NCCMPatch), nameof(NCCMPatch.Designator_Build_ProcessInputPrefix)));
            HarmonyInstance.Patch(
                AccessTools.Method(typeof(Designator_Dropdown), nameof(Designator_Dropdown.ProcessInput)),
                prefix: new HarmonyMethod(typeof(NCCMPatch), nameof(NCCMPatch.Designator_Dropdown_ProcessInputPrefix)));
            HarmonyInstance.Patch(
                AccessTools.Method(typeof(WindowStack), nameof(WindowStack.Add)),
                prefix: new HarmonyMethod(typeof(NCCMPatch), nameof(NCCMPatch.WindowStack_AddPrefix)));
        }

        public NCCM(ModContentPack content) : base(content)
        {
        }


        protected override Control CreateContent()
        {
            return new TabControl().Set(
                new TabItem
                {
                    Content = SettingPage.Instance,
                    Text = "NCCM.Settings.NormalSettings.Tab.Label".Translate(),
                    ShowTooltip = true
                },
                new TabItem
                {
                    Content = AdvancedSettingPage.Instance,
                    Text = "NCCM.Settings.AdvancedSettings.Tab.Label".Translate(),
                    Tooltip = "NCCM.Settings.AdvancedSettings.Tab.Tooltip".Translate(),
                    ShowTooltip = true
                });
        }

        public override void HandleUIEvent(UIEventType type)
        {
            base.HandleUIEvent(type);

            if (type is UIEventType.LanguageChanged)
            {
                ItemPickerWindow.PickerWindow.Unbind();
                ItemPickerWindow.PickerWindow = new ItemPickerWindow();
                MaterialInfoWindow.InfoWindow.Unbind();
                MaterialInfoWindow.InfoWindow = new MaterialInfoWindow();
            }
        }
    }
}
