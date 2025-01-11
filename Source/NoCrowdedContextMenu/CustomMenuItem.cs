using Nebulae.RimWorld.UI;
using Nebulae.RimWorld.UI.Controls;
using RimWorld;
using UnityEngine;
using Verse;

namespace NoCrowdedContextMenu
{
    internal class CustomMenuItem : MenuItem
    {
        //------------------------------------------------------
        //
        //  Internal Const
        //
        //------------------------------------------------------

        #region Internal Const

        internal const float IconSize = 27f;
        internal const float InfoButtonHeight = 34f;
        internal const float Padding = 5f;

        #endregion


        private readonly OptionInfo _optionInfo;


        static CustomMenuItem()
        {
            MarginProperty.OverrideMetadata(typeof(CustomMenuItem),
                new ControlPropertyMetadata(new Thickness(Padding), ControlRelation.Measure));

            HorizontalAlignmentProperty.OverrideMetadata(typeof(CustomMenuItem),
                new ControlPropertyMetadata(HorizontalAlignment.Stretch, ControlRelation.Measure));

            VerticalAlignmentProperty.OverrideMetadata(typeof(CustomMenuItem),
                new ControlPropertyMetadata(VerticalAlignment.Stretch, ControlRelation.Measure));
        }

        public CustomMenuItem(Window window ,FloatMenu originMenu, FloatMenuOption option)
            : base(window,originMenu, option)
        {
            _optionInfo = new OptionInfo(option);
        }


        protected override Rect DrawCore(Rect renderRect)
        {
            Rect contentRect = new Rect(renderRect.x + Padding,
                renderRect.y,
                renderRect.width - Padding,
                renderRect.height);

            Rect iconRect = new Size(IconSize).AlignRectToArea(contentRect,
                HorizontalAlignment.Left, VerticalAlignment.Center);

            bool noIcon = _optionInfo.IconStatus is IconStatus.None;

            if (noIcon)
            {
                iconRect = new Rect(iconRect.x, iconRect.y, 0f, 0f);
            }

            Rect infoButtonRect = new Rect(
                contentRect.xMax - Option.extraPartWidth,
                contentRect.y,
                0f,
                0f);

            Rect hitTestRect = new Rect(
                renderRect.x,
                renderRect.y,
                0f,
                renderRect.height);

            if (_optionInfo.HasInfoCard)
            {
                infoButtonRect.width = Option.extraPartWidth;
                infoButtonRect.height = InfoButtonHeight;

                hitTestRect.width = renderRect.width + Padding - Option.extraPartWidth;
            }
            else
            {
                hitTestRect.width = renderRect.width;
            }

            bool isCursorOver = Mouse.IsOver(hitTestRect);

            Color currentColor = GUI.color;
            GUI.color = IsDisabled
                ? currentColor * Widgets.InactiveColor
                : currentColor;

            Texture2D background = Button.DefaultNormalBackground;
            if (!IsDisabled && isCursorOver)
            {
                background = Button.DefaultMouseOverBackground;

                Vector2 mousePos = Input.mousePosition;
                mousePos.y = UI.screenHeight - mousePos.y;

                Option.mouseoverGuiAction?.Invoke(new Rect(
                    mousePos.x - Window.windowRect.x + InfoTooltipXOffset,
                    mousePos.y - Window.windowRect.y + InfoTooltipYOffset,
                    0f,
                    0f));

                if (Input.GetMouseButton(0))
                {
                    background = Button.DefaultPressedBackground;
                }
            }

            Widgets.DrawAtlas(renderRect, background);

            if (!noIcon)
            {
                _optionInfo.DrawIcon(iconRect);
            }

            Rect labelRect = new Rect(
                iconRect.xMax + Padding,
                renderRect.y,
                hitTestRect.width - iconRect.width,
                renderRect.height);

            TextAnchor anchor = Text.Anchor;
            GameFont font = Text.Font;
            Text.Anchor = TextAnchor.MiddleLeft;
            Text.Font = GameFont.Small;

            Widgets.Label(
                labelRect,
                Name);


            if (!IsDisabled && isCursorOver
                && Event.current.type is EventType.MouseUp)
            {
                OnSelected();
            }

            GUI.color = currentColor;

            if (_optionInfo.HasInfoCard
                && Option.extraPartOnGUI.Invoke(infoButtonRect))
            {
                Window.Close();
            }

            if (Option.tutorTag != null)
            {
                UIHighlighter.HighlightOpportunity(labelRect, Option.tutorTag);
                TutorSystem.Notify_Event(Option.tutorTag);
            }

            Text.Font = font;
            Text.Anchor = anchor;

            return renderRect;
        }


        private enum IconStatus
        {
            None,
            FromDef,
            FromTexture,
            FromThing,
            LayerOnly
        }


        private readonly struct OptionInfo
        {
            //------------------------------------------------------
            //
            //  Private Fields
            //
            //------------------------------------------------------

            #region Private Fields

            private readonly Thing _iconThing;
            private readonly Texture2D _itemIcon;
            private readonly ThingDef _shownItem;
            private readonly ThingStyleDef _thingStyle;

            private readonly Rect _iconTexCoords;

            private readonly bool _drawLayer;

            #endregion


            internal readonly bool HasInfoCard;
            internal readonly IconStatus IconStatus;


            internal OptionInfo(FloatMenuOption option)
            {
                _drawLayer = MenuOptionUtility.DrawPlaceHolderIconGetter(option);
                _iconThing = MenuOptionUtility.IconThingGetter(option);
                _itemIcon = MenuOptionUtility.ItemIconGetter(option);
                _shownItem = MenuOptionUtility.ShownItemGetter(option);

                _iconTexCoords = option.iconTexCoords;
                _thingStyle = null;

                HasInfoCard = false;

                if (_shownItem != null)
                {
                    HasInfoCard = option.extraPartOnGUI != null;
                    IconStatus = IconStatus.FromDef;

                    if (option.forceBasicStyle)
                    {
                        _thingStyle = null;
                    }
                    else
                    {
                        _thingStyle = MenuOptionUtility.ThingStyleGetter(option)
                            ?? Faction.OfPlayer.ideos?.PrimaryIdeo.GetStyleFor(_shownItem);
                    }
                }
                else if (_itemIcon != null)
                {
                    IconStatus = IconStatus.FromTexture;
                }
                else if (_iconThing != null)
                {
                    IconStatus = IconStatus.FromThing;
                }
                else
                {
                    IconStatus = _drawLayer
                        ? IconStatus.LayerOnly
                        : IconStatus.None;
                }
            }


            internal void DrawIcon(Rect iconRect)
            {
                if (_drawLayer)
                {
                    Widgets.DrawTextureFitted(iconRect, Widgets.PlaceholderIconTex, 1f);
                }

                switch (IconStatus)
                {
                    case IconStatus.FromDef:
                        Widgets.DefIcon(iconRect, _shownItem, thingStyleDef: _thingStyle);
                        break;
                    case IconStatus.FromTexture:
                        Widgets.DrawTextureFitted(iconRect, _itemIcon, 1f, new Vector2(1f, 1f), _iconTexCoords);
                        break;
                    case IconStatus.FromThing:
                        Widgets.ThingIcon(iconRect, _iconThing);
                        break;
                    default:    // None, LayerOnly
                        break;
                }
            }
        }
    }
}
