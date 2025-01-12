using Nebulae.RimWorld;
using Nebulae.RimWorld.UI.Controls;
using RimWorld;
using System;
using Verse;
using Verse.Sound;

namespace NoCrowdedContextMenu
{
    internal abstract class MenuItem : Control
    {
        internal const float InfoTooltipXOffset = 8f;
        internal const float InfoTooltipYOffset = -16f;


        protected readonly bool IsDisabled;
        protected readonly FloatMenu OriginMenu;
        protected readonly FloatMenuOption Option;
        protected readonly Window Window;


        protected MenuItem(Window window ,FloatMenu originMenu, FloatMenuOption option)
        {
            IsDisabled = option.action is null;
            Name = option.Label;
            OriginMenu = originMenu;
            Option = option;
            Window = window;

            if (option.tooltip.HasValue)
            {
                Tooltip = option.tooltip.Value;
                ShowTooltip = true;
            }
        }

        protected void OnSelected()
        {
            OriginMenu.PreOptionChosen(Option);

            if (IsDisabled)
            {
                SoundDefOf.ClickReject.PlayOneShotOnCamera();
            }
            else
            {
                Option.action.Invoke();
                if (OriginMenu.givesColonistOrders)
                {
                    SoundDefOf.ColonistOrdered.PlayOneShotOnCamera();
                }
            }

            Window.Close();
        }
    }
}
