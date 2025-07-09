using Nebulae.RimWorld.UI.Controls.Basic;
using Nebulae.RimWorld.UI.Core.Events;
using Nebulae.RimWorld.UI.Utilities;
using NoCrowdedContextMenu.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NoCrowdedContextMenu.Coordinators
{
    internal static class MenuManagerCoordinator
    {
        public static readonly MenuManagerView View = new MenuManagerView();


        internal static void OnSourceDelete(object sender, RoutedEventArgs e)
        {
            if (sender is Control control && control.TryFindPartent<MenuSourceView>(out var sourceView))
            {
                var settings = NCCM.Settings;

                if (settings.ProtectedMenuSources.Remove(sourceView.Model))
                {
                    View.ProtectedSourcePanel.Remove(sourceView);
                }
                else if (settings.ReplacedMenuSources.Remove(sourceView.Model))
                {
                    View.ReplacedSourcePanel.Remove(sourceView);
                }
            }
        }

        internal static void OnSourceSwitchType(object sender, RoutedEventArgs e)
        {
            if (sender is Control control && control.TryFindPartent<MenuSourceView>(out var sourceView))
            {
                var settings = NCCM.Settings;

                if (settings.ProtectedMenuSources.Remove(sourceView.Model))
                {
                    View.ProtectedSourcePanel.Remove(sourceView);
                    View.ReplacedSourcePanel.Append(sourceView);
                    settings.ReplacedMenuSources.Add(sourceView.Model);
                }
                else if (settings.ReplacedMenuSources.Remove(sourceView.Model))
                {
                    View.ReplacedSourcePanel.Remove(sourceView);
                    View.ProtectedSourcePanel.Append(sourceView);
                    settings.ProtectedMenuSources.Add(sourceView.Model);
                }
            }
        }
    }
}
