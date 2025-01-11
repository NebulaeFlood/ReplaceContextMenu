using System;
using UnityEngine;
using Verse;

namespace NoCrowdedContextMenu
{
    internal class VanillaMenuItem : MenuItem
    {
        private readonly Action<Rect> _mouseoverGUI;

        public VanillaMenuItem(Window window, FloatMenu originMenu, FloatMenuOption option)
            : base(window, originMenu, option)
        {
            _mouseoverGUI = option.mouseoverGuiAction;

            option.mouseoverGuiAction = null;
        }

        protected override Rect DrawCore(Rect renderRect)
        {
            if (Option.DoGUI(renderRect, OriginMenu.givesColonistOrders, OriginMenu))
            {
                Window.Close();
            }

            if (_mouseoverGUI != null)
            {
                if (Mouse.IsOver(new Rect(
                    renderRect.x,
                    renderRect.y,
                    renderRect.width - Option.extraPartWidth,
                    renderRect.height)))
                {
                    Vector2 mousePos = Input.mousePosition;
                    mousePos.y = UI.screenHeight - mousePos.y;

                    _mouseoverGUI.Invoke(new Rect(
                        mousePos.x - Window.windowRect.x + InfoTooltipXOffset,
                        mousePos.y - Window.windowRect.y + InfoTooltipYOffset,
                        0f,
                        0f));
                }
            }

            return renderRect;
        }
    }
}
