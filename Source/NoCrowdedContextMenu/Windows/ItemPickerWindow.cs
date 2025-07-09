using Nebulae.RimWorld.UI.Controls;
using Nebulae.RimWorld.UI.Windows;
using NoCrowdedContextMenu.Views;
using RimWorld;
using UnityEngine;

namespace NoCrowdedContextMenu.Windows
{
    internal sealed class ItemPickerWindow : ControlWindow
    {
        public readonly BuildingMenuOptionInfoView InfoView;


        internal ItemPickerWindow(ItemPickerView view)
        {
            doCloseX = true;
            soundAppear = SoundDefOf.FloatMenu_Open;

            Content = view;
            InfoView = new BuildingMenuOptionInfoView();

            _infoManager = new LayoutManager
            {
                DrawDebugButtons = false,
                IsHitTestVisible = false,
                Root = InfoView
            };
        }


        //------------------------------------------------------
        //
        //  Public Methods
        //
        //------------------------------------------------------

        #region Public Methods

        public override void ExtraOnGUI()
        {
            if (!InfoView.IsEmpty)
            {
                _infoManager.Draw(new Rect(windowRect.x - 219f, windowRect.y, 220f, 220f));
            }
        }

        public override void PostClose()
        {
            base.PostClose();
            InfoView.Clear();
            ((ItemPickerView)Content).Clear();

            if (NCCM.Settings.HasMemory)
            {
                _savedWindowRect = windowRect;
            }
        }

        public override void PreOpen()
        {
            base.PreOpen();

            if (NCCM.Settings.HasMemory && _savedWindowRect.size.sqrMagnitude > 0f)
            {
                windowRect = _savedWindowRect;
            }
        }

        #endregion


        private static Rect _savedWindowRect;


        private readonly LayoutManager _infoManager;
    }
}
