using Nebulae.RimWorld.UI.Controls;
using Nebulae.RimWorld.UI.Utilities;
using Nebulae.RimWorld.Utilities;
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

        internal CustomMenuItem(ItemPickerWindow window, FloatMenu originalMenu, FloatMenuOption option, int index)
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

            _optionInfo = new OptionInfo(option, index);
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
                Vector2 mousePos = Input.mousePosition / Prefs.UIScale;
                mousePos.y = UI.screenHeight - mousePos.y;

                if (ReferenceEquals(Find.WindowStack.GetWindowAt(mousePos), _owner))
                {
                    if (isEnabled)
                    {
                        background = isPressing
                            ? Button.DefaultPressedBackground
                            : Button.DefaultMouseOverBackground;
                    }

                    if (_optionInfo.ShowMaterialInfoWindow
                        && !ItemPickerWindow.AnyItemHovered)
                    {
                        ItemPickerWindow.AnyItemHovered = true;
                        MaterialInfoWindow.Initialize(_optionInfo,
                            new Rect(
                                _owner.windowRect.x - 220f,
                                _owner.windowRect.y,
                                220f,
                                320f));
                    }

                    _option.mouseoverGuiAction?.Invoke(new Rect(
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

            _owner.Close();
            MaterialInfoWindow.Hide();
            _originalMenu.PreOptionChosen(_option);
            _option.action.Invoke();
        }

        protected override Rect SegmentCore(Rect visiableRect)
        {
            visiableRect = _hitTestRectCache.IntersectWith(visiableRect);

            UpdateHitTestRect(visiableRect);

            return visiableRect;
        }
    }
}
