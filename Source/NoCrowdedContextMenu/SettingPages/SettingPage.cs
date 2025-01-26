using Nebulae.RimWorld.UI;
using Nebulae.RimWorld.UI.Controls;
using Nebulae.RimWorld.UI.Converters;
using Nebulae.RimWorld.UI.Data.Binding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;
using Grid = Nebulae.RimWorld.UI.Controls.Panels.Grid;

namespace NoCrowdedContextMenu.SettingPages
{
    internal class SettingPage : FrameworkControl
    {
        internal const float RowHeight = 40f;
        internal const float NumberBoxWidth = 100f;


        private static Control _content;


        static SettingPage()
        {
            HorizontalAlignmentProperty.OverrideMetadata(typeof(SettingPage),
                new ControlPropertyMetadata(HorizontalAlignment.Stretch, ControlRelation.Measure));

            VerticalAlignmentProperty.OverrideMetadata(typeof(SettingPage),
                new ControlPropertyMetadata(VerticalAlignment.Top, ControlRelation.Measure));
        }

        internal SettingPage()
        {
            CheckBox[] checkBoxes = new CheckBox[8];
            for (int i = 0; i < 8; i++)
            {
                checkBoxes[i] = new CheckBox
                {
                    Padding = 0f,
                    HorizontalAlignment = HorizontalAlignment.Left,
                    ShowTooltip = true
                };
            }

            Initialize(checkBoxes[0], NCCM.Settings.AskBeforeReplace, nameof(NCCM.Settings.AskBeforeReplace));
            Initialize(checkBoxes[1], NCCM.Settings.ReplaceUnknownSource, nameof(NCCM.Settings.ReplaceUnknownSource));
            Initialize(checkBoxes[2], NCCM.Settings.FocusSearchBar, nameof(NCCM.Settings.FocusSearchBar));
            Initialize(checkBoxes[3], NCCM.Settings.HasMemory, nameof(NCCM.Settings.HasMemory));
            Initialize(checkBoxes[4], NCCM.Settings.IsDragable, nameof(NCCM.Settings.IsDragable));
            Initialize(checkBoxes[5], NCCM.Settings.IsResizable, nameof(NCCM.Settings.IsResizable));
            Initialize(checkBoxes[6], NCCM.Settings.PauseGame, nameof(NCCM.Settings.PauseGame));
            Initialize(checkBoxes[7], NCCM.Settings.UseVanillaRenderMode, nameof(NCCM.Settings.UseVanillaRenderMode));

            var numberBox = new NumberBox
            {
                DecimalPartDigit = 0,
                Margin = new Thickness(0f, 8f, 0f, 8f),
                Maximum = 200f,
                Minimum = 10f,
                Value = NCCM.Settings.MinimumOptionCountCauseReplacement,
                Tooltip = "NCCM.Settings.MinimumOptionCountCauseReplacement.Tooltip".Translate(),
                ShowTooltip = true
            };

            var slider = new Slider
            {
                Maximum = 200f,
                Minimum = 10f,
                Step = 1f,
                Value = NCCM.Settings.MinimumOptionCountCauseReplacement,
                Tooltip = "NCCM.Settings.MinimumOptionCountCauseReplacement.Tooltip".Translate(),
                ShowTooltip = true
            };

            BindingManager.Bind(
                slider,
                Slider.ValueProperty,
                numberBox,
                NumberBox.ValueProperty,
                BindingMode.TwoWay);

            BindingManager.Bind(
                slider,
                Slider.ValueProperty,
                NCCM.Settings,
                nameof(NCCM.Settings.MinimumOptionCountCauseReplacement),
                BindingMode.OneWay);

            var label = new Label
            {
                Anchor = TextAnchor.MiddleLeft,
                Text = "NCCM.Settings.MinimumOptionCountCauseReplacement.Label".Translate(),
                Tooltip = "NCCM.Settings.MinimumOptionCountCauseReplacement.Tooltip".Translate(),
                ShowTooltip = true
            };

            _content = new Grid { Margin = 10f }
                .SetSize(
                    new float[] { Grid.Remain, NumberBoxWidth },
                    new float[]
                    {
                        RowHeight,
                        RowHeight,
                        RowHeight,
                        RowHeight,
                        RowHeight,
                        RowHeight,
                        RowHeight,
                        RowHeight,
                        RowHeight,
                        RowHeight
                    })
                .Set(
                        checkBoxes[0], checkBoxes[0],
                        checkBoxes[1], checkBoxes[1],
                        checkBoxes[2], checkBoxes[2],
                        checkBoxes[3], checkBoxes[3],
                        checkBoxes[4], checkBoxes[4],
                        checkBoxes[5], checkBoxes[5],
                        checkBoxes[6], checkBoxes[6],
                        checkBoxes[7], checkBoxes[7],
                        label, numberBox,
                        slider, slider
                    );
        }


        //------------------------------------------------------
        //
        //  Protected Methods
        //
        //------------------------------------------------------

        #region Protected Methods

        protected override Rect ArrangeCore(Rect availableRect)
        {
            return _content.Arrange(base.ArrangeCore(availableRect));
        }

        protected override void DrawCore()
        {
            _content.Draw();
        }

        protected override Size MeasureCore(Size availableSize)
        {
            return _content.Measure(base.MeasureCore(availableSize));
        }

        protected override Rect SegmentCore(Rect visiableRect)
        {
            return _content.Segment(base.SegmentCore(visiableRect));
        }

        #endregion


        private static void Initialize(CheckBox checkBox, bool value, string name)
        {
            checkBox.Status = value ? ToggleStatus.Checked : ToggleStatus.Unchecked;
            checkBox.Text = ("NCCM.Settings." + name + ".Label").Translate();
            checkBox.Tooltip = ("NCCM.Settings." + name + ".Tooltip").Translate();

            BindingManager.Bind(
                checkBox,
                ToggleButton.StatusProperty,
                NCCM.Settings,
                name,
                ToggleStatusConverter.Instance,
                BindingMode.OneWay);
        }
    }
}
