using Nebulae.RimWorld.UI;
using Nebulae.RimWorld.UI.Controls;
using Nebulae.RimWorld.UI.Controls.Panels;
using Nebulae.RimWorld.UI.Converters;
using Nebulae.RimWorld.UI.Data.Binding;
using Nebulae.RimWorld.UI.Utilities;
using UnityEngine;
using Verse;
using static Mono.Security.X509.X520;
using Grid = Nebulae.RimWorld.UI.Controls.Panels.Grid;

namespace NoCrowdedContextMenu.SettingPages
{
    internal class SettingPage : UserControl
    {
        internal static readonly SettingPage Instance = new SettingPage();

        internal const float RowHeight = 40f;
        internal const float NumberBoxWidth = 100f;

        private SettingPage()
        {
            Initialize();
        }


        protected override Control CreateContent()
        {
            return new StackPanel { Margin = 10f }.Set
                (
                    CreateEntry(nameof(NCCM.Settings.AskBeforeReplace)),
                    CreateEntry(nameof(NCCM.Settings.ReplaceUnknownSource)),
                    CreateEntry(nameof(NCCM.Settings.FocusSearchBar)),
                    CreateEntry(nameof(NCCM.Settings.HasMemory)),
                    CreateEntry(nameof(NCCM.Settings.IsDragable)),
                    CreateEntry(nameof(NCCM.Settings.IsResizable)),
                    CreateEntry(nameof(NCCM.Settings.PauseGame)),
                    CreateEntry(nameof(NCCM.Settings.UseVanillaRenderMode)),
                    ModSettingLayoutUtility.CreateNumberEntry(
                        NCCM.Settings,
                        nameof(NCCM.Settings.MinimumOptionCountCauseReplacement),
                        "NCCM.Settings.MinimumOptionCountCauseReplacement.Label".Translate(),
                        "NCCM.Settings.MinimumOptionCountCauseReplacement.Tooltip".Translate(),
                        8f,
                        200f,
                        0,
                        1f)
                );
        }


        private static CheckBox CreateEntry(string key)
        {
            return ModSettingLayoutUtility.CreateBooleanEntry(
                NCCM.Settings,
                key,
                ("NCCM.Settings." + key + ".Label").Translate(),
                ("NCCM.Settings." + key + ".Tooltip").Translate());
        }
    }
}
