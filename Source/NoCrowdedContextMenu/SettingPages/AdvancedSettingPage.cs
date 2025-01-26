﻿using Nebulae.RimWorld.UI.Controls;
using Nebulae.RimWorld.UI.Controls.Geomerties;
using Nebulae.RimWorld.UI.Controls.Panels;
using Nebulae.RimWorld.UI.Utilities;
using NoCrowdedContextMenu;
using NoCrowdedContextMenu.CustomControls;
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
    internal class AdvancedSettingPage : FrameworkControl
    {
        private static Control _content;

        private static StackPanel _protectedRecordPanel;
        private static StackPanel _replaceRecordPanel;

        private static Rect _protectedRecordPanelRect;
        private static Rect _replaceRecordPanelRect;


        static AdvancedSettingPage()
        {
            HorizontalAlignmentProperty.OverrideMetadata(typeof(AdvancedSettingPage),
                new ControlPropertyMetadata(HorizontalAlignment.Stretch, ControlRelation.Measure));

            VerticalAlignmentProperty.OverrideMetadata(typeof(AdvancedSettingPage),
                new ControlPropertyMetadata(VerticalAlignment.Stretch, ControlRelation.Measure));
        }

        internal AdvancedSettingPage()
        {
            if (NCCM.Settings.ProtectedMenuKeys is null)
            {
                NCCM.Settings.ProtectedMenuKeys = new HashSet<FloatMenuKey>();
            }    

            if (NCCM.Settings.ReplacedMenuKeys is null)
            {
                NCCM.Settings.ReplacedMenuKeys = new HashSet<FloatMenuKey>();
            }

            _protectedRecordPanel = new StackPanel { Margin = 4f }
                .Set(NCCM.Settings.ProtectedMenuKeys
                    .Select(key => new ReplaceRecordEntry(key, true)).ToArray());

            _replaceRecordPanel = new StackPanel { Margin = 4f }
                .Set(NCCM.Settings.ReplacedMenuKeys
                    .Select(key => new ReplaceRecordEntry(key, false)).ToArray());

            _content = new StackPanel
            {
                ChildMaxHeight = 0.5f,
                Margin = 4f
            }
                .Set
                (
                    new Grid { Margin = 4f }
                        .SetSize(new float[] { 1f }, new float[] { 36f, Grid.Remain })
                        .Set
                        (
                            new Label
                            {
                                Anchor = TextAnchor.MiddleLeft,
                                Text = "NCCM.AdvancedSettings.Category.BlackList.Label".Translate(),
                                Tooltip = "NCCM.AdvancedSettings.Category.BlackList.Tooltip".Translate(),
                                ShowTooltip = true
                            },
                            new ScrollViewer
                            {
                                Content = _protectedRecordPanel,
                                HorizontalScrollBarVisibility = ScrollBarVisibility.Hidden
                            }
                        ),
                    new Grid { Margin = 4f }
                        .SetSize(new float[] { 1f }, new float[] { 36f, Grid.Remain })
                        .Set
                        (
                            new Label
                            {
                                Anchor = TextAnchor.MiddleLeft,
                                Text = "NCCM.AdvancedSettings.Category.WhiteList.Label".Translate(),
                                Tooltip = "NCCM.AdvancedSettings.Category.WhiteList.Tooltip".Translate(),
                                ShowTooltip = true
                            },
                            new ScrollViewer
                            {
                                Content = _replaceRecordPanel,
                                HorizontalScrollBarVisibility = ScrollBarVisibility.Hidden
                            }
                        )
                );

            _content.SetParent(this);
        }


        internal static void AddRecord(FloatMenuKey key, bool isProtected)
        {
            if (isProtected)
            {
                _protectedRecordPanel.Append(new ReplaceRecordEntry(key, true));

                NCCM.Settings.ProtectedMenuKeys.Add(key);
            }
            else
            {
                _replaceRecordPanel.Append(new ReplaceRecordEntry(key, false));

                NCCM.Settings.ReplacedMenuKeys.Add(key);
            }

            NCCM.Settings.Write();
        }


        //------------------------------------------------------
        //
        //  Protected Methods
        //
        //------------------------------------------------------

        #region Protected Methods

        protected override Rect ArrangeCore(Rect availableRect)
        {
            availableRect = _content.Arrange(base.ArrangeCore(availableRect));

            _protectedRecordPanelRect = _protectedRecordPanel.GetParent().DesiredRect;
            _replaceRecordPanelRect = _replaceRecordPanel.GetParent().DesiredRect;

            return availableRect;
        }

        protected override void DrawCore()
        {
            UIUtility.DrawRectangle(_protectedRecordPanelRect, Widgets.WindowBGFillColor, 1f, Color.gray);
            UIUtility.DrawRectangle(_replaceRecordPanelRect, Widgets.WindowBGFillColor, 1f, Color.gray);

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


        //------------------------------------------------------
        //
        //  Event Handlers
        //
        //------------------------------------------------------

        #region Event Handlers

        internal static void RemoveSavedKey(ButtonBase button, EventArgs e)
        {
            if (button.GetParent()?.GetParent() is ReplaceRecordEntry entry)
            {
                if (entry.IsProtectedKey)
                {
                    _protectedRecordPanel.Remove(entry);
                    NCCM.Settings.ProtectedMenuKeys.Remove(entry.Key);
                }
                else
                {
                    _replaceRecordPanel.Remove(entry);
                    NCCM.Settings.ReplacedMenuKeys.Remove(entry.Key);
                }

                NCCM.Settings.Write();
            }
        }

        internal static void SwitchSavedKey(ButtonBase button, EventArgs e)
        {
            if (button.GetParent()?.GetParent() is ReplaceRecordEntry entry)
            {
                if (entry.IsProtectedKey)
                {
                    _protectedRecordPanel.Remove(entry);
                    NCCM.Settings.ProtectedMenuKeys.Remove(entry.Key);

                    entry.IsProtectedKey = false;

                    _replaceRecordPanel.Append(entry);
                    NCCM.Settings.ReplacedMenuKeys.Add(entry.Key);
                }
                else
                {
                    _replaceRecordPanel.Remove(entry);
                    NCCM.Settings.ReplacedMenuKeys.Remove(entry.Key);

                    entry.IsProtectedKey = true;

                    _protectedRecordPanel.Append(entry);
                    NCCM.Settings.ProtectedMenuKeys.Add(entry.Key);
                }

                NCCM.Settings.Write();
            }
        }

        #endregion
    }
}
