using Nebulae.RimWorld.UI.Controls;
using Nebulae.RimWorld.UI.Utilities;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.Sound;
using Button = Nebulae.RimWorld.UI.Controls.Button;
using GameText = Verse.Text;

namespace NoCrowdedContextMenu.CustomControls
{
    internal class CustomMenuItem : ButtonBase
    {
        //------------------------------------------------------
        //
        //  Internal Const
        //
        //------------------------------------------------------

        #region Internal Const

        internal const float IconSize = 27f;

        internal const float InfoTooltipXOffset = 4f;
        internal const float InfoTooltipYOffset = 16f;

        #endregion


        private readonly OptionInfo _optionInfo;
        private readonly FloatMenu _originalMenu;
        private readonly FloatMenuOption _option;
        private readonly ItemPickerWindow _owner;

        private Rect _hitTestRectCache;
        private Rect _iconRectCache;
        private Rect _infoButtonRectCache;
        private Rect _labelRectCache;


        static CustomMenuItem()
        {
            HorizontalAlignmentProperty.OverrideMetadata(typeof(CustomMenuItem),
                new ControlPropertyMetadata(HorizontalAlignment.Stretch, ControlRelation.Measure));

            VerticalAlignmentProperty.OverrideMetadata(typeof(CustomMenuItem),
                new ControlPropertyMetadata(VerticalAlignment.Stretch, ControlRelation.Measure));
        }

        internal CustomMenuItem(ItemPickerWindow window, FloatMenu originalMenu, FloatMenuOption option)
        {
            IsEnabled = option.action != null;
            ClickSound = IsEnabled
                ? SoundDefOf.ColonistOrdered
                : SoundDefOf.ClickReject;

            Name = option.Label;
            Text = option.Label;

            _originalMenu = originalMenu;
            _option = option;
            _owner = window;

            if (option.tooltip.HasValue)
            {
                Tooltip = option.tooltip.Value;
                ShowTooltip = true;
            }

            _optionInfo = new OptionInfo(option);
        }


        protected override Rect ArrangeCore(Rect availableRect)
        {
            Rect renderRect = base.ArrangeCore(availableRect);
            Rect contentRect = new Rect(
                renderRect.x + 5f,
                renderRect.y,
                renderRect.width - 5f,
                renderRect.height);

            if (_optionInfo.IconStatus is IconStatus.None)
            {
                _iconRectCache = new Rect(contentRect.x, contentRect.y, 0f, 0f);
            }
            else
            {
                _iconRectCache = new Size(IconSize).AlignToArea(contentRect,
                    HorizontalAlignment.Left, VerticalAlignment.Center);
            }

            _hitTestRectCache = renderRect;

            if (_optionInfo.HasInfoCard)
            {
                _infoButtonRectCache = new Size(_option.extraPartWidth, _option.RequiredHeight)
                    .AlignToArea(contentRect,
                        HorizontalAlignment.Right, VerticalAlignment.Center);

                _hitTestRectCache.xMax = _infoButtonRectCache.x;
            }
            else
            {
                _infoButtonRectCache = new Rect(
                    contentRect.xMax,
                    contentRect.y,
                    0f,
                    0f);
            }

            if (!string.IsNullOrEmpty(Text))
            {
                _labelRectCache = new Rect(
                    _iconRectCache.xMax + 5f,
                    contentRect.y,
                    contentRect.width - _iconRectCache.width - 5f,
                    contentRect.height);
            }
            else
            {
                _labelRectCache = new Rect(
                    _iconRectCache.xMax + 5f,
                    contentRect.y,
                    0f,
                    0f);
            }

            return renderRect;
        }


        protected override void DrawButton(Rect renderRect, bool isEnabled, bool isCursorOver, bool isPressing)
        {
            Color currentColor = GUI.color;
            GUI.color = isEnabled
                ? currentColor
                : currentColor * Widgets.InactiveColor;

            Texture2D background = Button.DefaultNormalBackground;

            if (isCursorOver)
            {
                if (isEnabled)
                {
                    background = isPressing
                        ? Button.DefaultPressedBackground
                        : Button.DefaultMouseOverBackground;
                }

                Vector2 mousePos = Input.mousePosition / Prefs.UIScale;
                mousePos.y = UI.screenHeight - mousePos.y;

                if (_option.mouseoverGuiAction != null
                    && ReferenceEquals(Find.WindowStack.GetWindowAt(mousePos), _owner))
                {
                    _option.mouseoverGuiAction.Invoke(new Rect(
                        mousePos.x - _owner.windowRect.x,
                        mousePos.y - _owner.windowRect.y + InfoTooltipYOffset,
                        InfoTooltipXOffset,
                        0f));
                }
            }

            Widgets.DrawAtlas(_hitTestRectCache, background);

            if (_optionInfo.IconStatus != IconStatus.None)
            {
                _optionInfo.DrawIcon(_iconRectCache);
            }

            if (!string.IsNullOrEmpty(Text))
            {
                TextAnchor anchor = GameText.Anchor;
                GameFont font = GameText.Font;
                GameText.Anchor = TextAnchor.MiddleLeft;
                GameText.Font = GameFont.Small;

                Widgets.Label(_labelRectCache, Text);

                GameText.Font = font;
                GameText.Anchor = anchor;
            }

            GUI.color = currentColor;

            if (_optionInfo.HasInfoCard)
            {
                _option.extraPartOnGUI.Invoke(_infoButtonRectCache);
            }

            if (!string.IsNullOrEmpty(_option.tutorTag))
            {
                UIHighlighter.HighlightOpportunity(_labelRectCache, _option.tutorTag);
            }
        }

        protected override void OnClick()
        {
            if (!string.IsNullOrEmpty(_option.tutorTag)
                && TutorSystem.AllowAction(_option.tutorTag))
            {
                TutorSystem.Notify_Event(_option.tutorTag);
            }

            ClickSound?.PlayOneShotOnCamera();

            _originalMenu.PreOptionChosen(_option);
            _option.action.Invoke();
            _owner.Close();
        }

        protected override Rect SegmentCore(Rect visiableRect)
        {
            visiableRect = _hitTestRectCache.IntersectWith(visiableRect);

            UpdateHitTestRect(visiableRect);

            return visiableRect;
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

            private readonly Color _compositionColor;
            private readonly Color _iconColor;
            private readonly Rect _iconTexCoords;

            private readonly bool _drawLayer;

            #endregion


            internal readonly bool HasInfoCard;
            internal readonly IconStatus IconStatus;


            internal OptionInfo(FloatMenuOption option)
            {
                _drawLayer = FieldAccessUtility.DrawPlaceHolderIconGetter(option);
                _iconThing = FieldAccessUtility.IconThingGetter(option);
                _itemIcon = FieldAccessUtility.ItemIconGetter(option);
                _shownItem = FieldAccessUtility.ShownItemGetter(option);

                _compositionColor = Color.white;
                _iconColor = option.iconColor;
                _iconTexCoords = option.iconTexCoords;
                _thingStyle = null;

                HasInfoCard = option.extraPartOnGUI != null;

                if (_shownItem != null)
                {
                    IconStatus = IconStatus.FromDef;

                    if (option.forceBasicStyle)
                    {
                        _thingStyle = null;
                    }
                    else
                    {
                        _thingStyle = FieldAccessUtility.ThingStyleGetter(option)
                            ?? Faction.OfPlayer.ideos?.PrimaryIdeo.GetStyleFor(_shownItem);
                    }

                    if (option.forceThingColor.HasValue)
                    {
                        _compositionColor = option.forceThingColor.Value;
                    }
                    else
                    {
                        _compositionColor = _shownItem.MadeFromStuff
                             ? _shownItem.GetColorForStuff(GenStuff.DefaultStuffFor(_shownItem))
                             : _shownItem.uiIconColor;
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

                if (IconStatus is IconStatus.None
                    || IconStatus is IconStatus.LayerOnly)
                {
                    return;
                }

                Color color = GUI.color;
                GUI.color = new Color(
                    _iconColor.r,
                    _iconColor.g,
                    _iconColor.b,
                    _iconColor.a * color.a);

                if (IconStatus is IconStatus.FromDef)
                {
                    Widgets.DefIcon(iconRect, _shownItem, thingStyleDef: _thingStyle, color: new Color(
                        _compositionColor.r,
                        _compositionColor.g,
                        _compositionColor.b,
                        _compositionColor.a * color.a));
                }
                else if (IconStatus is IconStatus.FromTexture)
                {
                    Widgets.DrawTextureFitted(iconRect, _itemIcon, 1f, new Vector2(1f, 1f), _iconTexCoords);
                }
                else
                {
                    Widgets.ThingIcon(iconRect, _iconThing);
                }

                GUI.color = color;
            }
        }
    }
}
