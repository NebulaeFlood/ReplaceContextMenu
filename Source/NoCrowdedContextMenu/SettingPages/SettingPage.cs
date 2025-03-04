using Nebulae.RimWorld.UI.Controls;
using Nebulae.RimWorld.UI.Utilities;

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
