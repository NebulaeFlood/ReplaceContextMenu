using HarmonyLib;
using Nebulae.RimWorld.UI;
using Nebulae.RimWorld.UI.Controls;
using NoCrowdedContextMenu.SettingPages;
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
                    Content = new SettingPage(),
                    Text = "NCCM.Settings.NormalSettings.Tab.Label".Translate(),
                    ShowTooltip = true
                },
                new TabItem
                {
                    Content = new AdvancedSettingPage(),
                    Text = "NCCM.Settings.AdvancedSettings.Tab.Label".Translate(),
                    Tooltip = "NCCM.Settings.AdvancedSettings.Tab.Tooltip".Translate(),
                    ShowTooltip = true
                });
        }
    }
}
