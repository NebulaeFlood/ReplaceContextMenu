using HarmonyLib;
using Nebulae.RimWorld.UI;
using Nebulae.RimWorld.UI.Automation;
using Nebulae.RimWorld.UI.Controls.Basic;
using Nebulae.RimWorld.UI.Utilities;
using NoCrowdedContextMenu.Coordinators;
using NoCrowdedContextMenu.Patches;
using RimWorld;
using Verse;

namespace NoCrowdedContextMenu
{
    public class NCCM : NebulaeMod<NCCMSettings>
    {
        internal const string DebugLabel = "ReplaceContextMenu";
        internal const string UniqueId = "Nebulae.NoCrowdedContextMenu";
        internal static readonly Harmony HarmonyInstance;



        static NCCM()
        {
            HarmonyInstance = new Harmony(UniqueId);

            HarmonyInstance.Patch(
                AccessTools.Method(typeof(Designator_Build), nameof(Designator_Build.ProcessInput)),
                prefix: new HarmonyMethod(typeof(Designator_Build_Patch), nameof(Designator_Build_Patch.ProcessInputPrefix)));
            HarmonyInstance.Patch(
                AccessTools.Method(typeof(Designator_Dropdown), nameof(Designator_Dropdown.ProcessInput)),
                prefix: new HarmonyMethod(typeof(Designator_Dropdown_Patch), nameof(Designator_Dropdown_Patch.ProcessInputPrefix)));
            HarmonyInstance.Patch(
                AccessTools.Method(typeof(WindowStack), nameof(WindowStack.Add)),
                prefix: new HarmonyMethod(typeof(WindowStack_Patch), nameof(WindowStack_Patch.AddPrefix)));
        }

        public NCCM(ModContentPack content) : base(content) { }


        public override string SettingsCategory()
        {
            return "NCCM.SettingsCategory.Label".Translate();
        }


        protected override Control CreateContent()
        {
            return new TabControl().Set(
                new TabItem
                {
                    Content = Settings.GenerateLayout(),
                    Header = "NCCM.Settings.Basic.Tab.Label".Translate()
                },
                new TabItem
                {
                    Content = MenuManagerCoordinator.View,
                    Header = "NCCM.Settings.Advanced.Tab.Label".Translate(),
                    Tooltip = "NCCM.Settings.Advanced.Tab.Tooltip".Translate()
                });
        }
    }
}
