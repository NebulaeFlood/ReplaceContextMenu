using Nebulae.RimWorld.UI;
using Nebulae.RimWorld.UI.Controls;
using System;
using UnityEngine;
using Verse;

namespace NoCrowdedContextMenu.CustomControls
{
    internal class VanillaMenuItem : FrameworkControl
    {
        private readonly Action<Rect> _mouseoverGUI;
        private readonly FloatMenu _originMenu;
        private readonly FloatMenuOption _option;
        private readonly ItemPickerWindow _owner;

        private readonly bool _hasInfoCard;

        internal VanillaMenuItem(ItemPickerWindow window, FloatMenu originMenu, FloatMenuOption option)
        {
            _hasInfoCard = option.extraPartOnGUI != null;
            _originMenu = originMenu;
            _option = option;
            _owner = window;

            if (option.tooltip.HasValue)
            {
                Tooltip = option.tooltip.Value;
                ShowTooltip = true;
            }

            Name = option.Label;

            _mouseoverGUI = option.mouseoverGuiAction;

            option.mouseoverGuiAction = null;
        }


        protected override void DrawCore()
        {
            if (_option.DoGUI(RenderRect, _originMenu.givesColonistOrders, _originMenu))
            {
                _owner.Close();
            }

            if (_mouseoverGUI != null)
            {
                Vector2 mousePos = Input.mousePosition / Prefs.UIScale;
                mousePos.y = UI.screenHeight - mousePos.y;

                if (_hasInfoCard
                    && ReferenceEquals(Find.WindowStack.GetWindowAt(mousePos), _owner)
                    && new Rect(
                        RenderRect.x,
                        RenderRect.y,
                        RenderRect.width - _option.extraPartWidth,
                        RenderRect.height).Contains(Event.current.mousePosition))
                {
                    _mouseoverGUI.Invoke(new Rect(
                        mousePos.x - _owner.windowRect.x,
                        mousePos.y - _owner.windowRect.y + CustomMenuItem.InfoTooltipYOffset,
                        CustomMenuItem.InfoTooltipXOffset,
                        0f));
                }
            }
        }
    }
}
