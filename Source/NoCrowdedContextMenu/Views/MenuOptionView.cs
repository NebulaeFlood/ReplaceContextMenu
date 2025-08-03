using Nebulae.RimWorld.UI.Controls;
using Nebulae.RimWorld.UI.Controls.Basic;
using Nebulae.RimWorld.UI.Controls.Resources;
using Nebulae.RimWorld.UI.Core.Data;
using Nebulae.RimWorld.UI.Core.Events;
using Nebulae.RimWorld.UI.Utilities;
using Nebulae.RimWorld.Utilities;
using NoCrowdedContextMenu.Models;
using NoCrowdedContextMenu.Utilities;
using NoCrowdedContextMenu.Windows;
using RimWorld;
using System;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace NoCrowdedContextMenu.Views
{
    internal sealed class MenuOptionView : ButtonBase
    {
        public readonly MenuOptionModel Model;


        static MenuOptionView()
        {
            MarginProperty.OverrideMetadata(typeof(MenuOptionView),
                new ControlPropertyMetadata(new Thickness(0f, 0f, 4f, 4f), ControlRelation.Measure));

            HorizontalAlignmentProperty.OverrideMetadata(typeof(MenuOptionView),
                new PropertyMetadata(HorizontalAlignment.Stretch));
            VerticalAlignmentProperty.OverrideMetadata(typeof(MenuOptionView),
                new PropertyMetadata(VerticalAlignment.Stretch));

            ClickSoundProperty.OverrideMetadata(typeof(MenuOptionView),
                new PropertyMetadata(SoundDefOf.ColonistOrdered));
            CursorEnterSoundProperty.OverrideMetadata(typeof(MenuOptionView),
                new PropertyMetadata(SoundDefOf.Mouseover_Standard));
        }

        internal MenuOptionView(FloatMenu sourceMenu, FloatMenuOption option, int index)
        {
            Name = option.Label;

            if (option.action is null)
            {
                IsEnabled = false;
            }

            if (option.tooltip.HasValue)
            {
                Tooltip = option.tooltip.Value;
            }

            Model = new MenuOptionModel(option, index);

            _hasLabel = !string.IsNullOrEmpty(option.Label);
            _istutor = !string.IsNullOrEmpty(option.tutorTag);

            _sourceMenu = sourceMenu;
            _sourceOption = option;
        }


        //------------------------------------------------------
        //
        //  Protected Methods
        //
        //------------------------------------------------------

        #region Protected Methods

        protected override Rect ArrangeOverride(Rect availableRect)
        {
            float x = availableRect.x;
            float y = availableRect.y;

            if (Model.OptionType.IsType(MenuOptionType.WithIcon))
            {
                _iconRect = new Size(27f).AlignToArea(new Rect(x + RenderSize.Height * 0.2f, y, RenderSize.Width, RenderSize.Height),
                    HorizontalAlignment.Left, VerticalAlignment.Center);
            }
            else
            {
                _iconRect = new Rect(x + RenderSize.Height * 0.2f, y, 0f, 0f);
            }

            _backgroundRect = availableRect;

            if (Model.OptionType.IsType(MenuOptionType.WithInfoCard))
            {
                _infoButtonRect = new Size(_sourceOption.extraPartWidth, _sourceOption.RequiredHeight)
                    .AlignToArea(availableRect,
                        HorizontalAlignment.Right, VerticalAlignment.Center);

                _backgroundRect.xMax = _infoButtonRect.x;
            }
            else
            {
                _infoButtonRect = new Rect(x, y, 0f, 0f);
            }

            if (_hasLabel)
            {
                _labelRect = new Rect(_iconRect.xMax + 5f, y, RenderSize.Width - (_iconRect.width + 5f), RenderSize.Height);
            }
            else
            {
                _labelRect = new Rect(x, y, 0f, 0f);
            }

            return availableRect;
        }

        protected override SegmentResult SegmentCore(Rect visiableRect)
        {
            return _backgroundRect.IntersectWith(visiableRect);
        }

        protected override void DrawCore(ControlState states)
        {
            var color = GUI.color;

            if (states.HasState(ControlState.Disabled))
            {
                GUI.color *= Widgets.InactiveColor;
            }

            if (states.HasState(ControlState.CursorDirectlyOver) && Model.OptionType.IsType(MenuOptionType.WithGUITooltip))
            {
                var cursorPos = InputUtility.CursorPosition;
                var ownerRect = LayoutManager.Owner.windowRect;

                _sourceOption.mouseoverGuiAction(new Rect(cursorPos.x - ownerRect.x, cursorPos.y - ownerRect.y + 16f, 4f, 0f));
            }

            Widgets.DrawAtlas(_backgroundRect, _background);

            if (Model.OptionType.IsType(MenuOptionType.WithIcon))
            {
                Model.DrawIcon(_iconRect);
            }

            if (_hasLabel)
            {
                Name.DrawLabel(_labelRect, TextAnchor.MiddleLeft);
            }

            GUI.color = color;

            if (Model.OptionType.IsType(MenuOptionType.WithInfoCard))
            {
                _sourceOption.extraPartOnGUI(_infoButtonRect);
            }

            if (_istutor)
            {
                UIHighlighter.HighlightOpportunity(_backgroundRect, _sourceOption.tutorTag);
            }
        }

        protected override void OnClick(RoutedEventArgs e)
        {
            if (_istutor && TutorSystem.AllowAction(_sourceOption.tutorTag))
            {
                TutorSystem.Notify_Event(_sourceOption.tutorTag);
            }

            LayoutManager.Owner.Close();

            var action = _sourceOption.action;

            try
            {
                _sourceMenu.PreOptionChosen(_sourceOption);
                action.Invoke();
            }
            catch (Exception ex)
            {
                NCCM.DebugLabel.Error($"Faild to invoke a float menu option's action from '{action.Method.DeclaringType}'.\n---> {ex}");
            }

            ClickSound.PlayOneShotOnCamera();
            e.Handled = true;
        }

        protected override void OnMouseEnter(RoutedEventArgs e)
        {
            if (LayoutManager.Owner is ItemPickerWindow window)
            {
                window.InfoView.Bind(this);
            }

            base.OnMouseEnter(e);
        }

        protected override void OnMouseLeave(RoutedEventArgs e)
        {
            if (LayoutManager?.Owner is ItemPickerWindow window)
            {
                window.InfoView.Clear();
            }

            e.Handled = true;
        }

        protected override void OnStatesChanged(ControlState states)
        {
            if (states.HasState(ControlState.Disabled))
            {
                _background = ButtonResources.NormalBackground;
                return;
            }

            if (states.HasState(ControlState.Pressing))
            {
                _background = ButtonResources.PressedBackground;
            }
            else if (states.HasState(ControlState.CursorDirectlyOver))
            {
                _background = ButtonResources.CursorOverBackground;
            }
            else
            {
                _background = ButtonResources.NormalBackground;
            }
        }

        #endregion


        //------------------------------------------------------
        //
        //  Private Fields
        //
        //------------------------------------------------------

        #region Private Fields

        private readonly bool _hasLabel;
        private readonly bool _istutor;

        private readonly FloatMenu _sourceMenu;
        private readonly FloatMenuOption _sourceOption;

        private Texture2D _background = ButtonResources.NormalBackground;

        private Rect _backgroundRect;
        private Rect _iconRect;
        private Rect _infoButtonRect;
        private Rect _labelRect;

        #endregion
    }
}
