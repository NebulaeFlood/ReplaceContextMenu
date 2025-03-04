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


        private SettingPage()
        {
            Initialize();
        }


        protected override Control CreateContent()
        {
            var panel = NCCM.Settings.CreateEntries(false);
            panel.Margin = 10f;
            return panel;
        }
    }
}
