using NoCrowdedContextMenu.Views;
using NoCrowdedContextMenu.Windows;
using System.Collections.Generic;
using Verse;

namespace NoCrowdedContextMenu.Coordinators
{
    internal static class ItemPickerCoordinator
    {
        public static Window Bind(FloatMenu menu, List<FloatMenuOption> options)
        {
            var settings = NCCM.Settings;

            View.SetOptions(menu, options);

            Window.absorbInputAroundWindow = settings.CloseOnClickOutSide;
            Window.closeOnClickedOutside = settings.CloseOnClickOutSide;
            Window.draggable = settings.IsDragable;
            Window.resizeable = settings.IsResizable;
            Window.forcePause = settings.PauseGame;

            return Window;
        }


        //------------------------------------------------------
        //
        //  Private Static Fields
        //
        //------------------------------------------------------

        #region Private Static Fields

        private static readonly ItemPickerView View = new ItemPickerView();
        private static readonly ItemPickerWindow Window = new ItemPickerWindow(View);

        #endregion
    }
}
