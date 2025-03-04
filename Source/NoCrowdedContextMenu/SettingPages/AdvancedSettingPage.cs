using Nebulae.RimWorld.UI.Controls;
using Nebulae.RimWorld.UI.Controls.Panels;
using Nebulae.RimWorld.UI.Utilities;
using NoCrowdedContextMenu.CustomControls;
using RimWorld;
using System;
using System.Linq;
using UnityEngine;
using Verse;
using Grid = Nebulae.RimWorld.UI.Controls.Panels.Grid;

namespace NoCrowdedContextMenu.SettingPages
{
    internal class AdvancedSettingPage : UserControl
    {
        internal static readonly AdvancedSettingPage Instance = new AdvancedSettingPage();

        private static VirtualizingStackPanel _protectedRecordPanel;
        private static VirtualizingStackPanel _replaceRecordPanel;


        private AdvancedSettingPage()
        {
            Initialize();
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


        protected override Control CreateContent()
        {
            _protectedRecordPanel = new VirtualizingStackPanel()
                .Set(NCCM.Settings.ProtectedMenuKeys
                    .Select(key => new ReplaceRecordEntry(key, true)).ToArray());

            _replaceRecordPanel = new VirtualizingStackPanel()
                .Set(NCCM.Settings.ReplacedMenuKeys
                    .Select(key => new ReplaceRecordEntry(key, false)).ToArray());

            var background = Widgets.WindowBGFillColor.ToBrush();

            return new StackPanel
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
                            new Border
                            {
                                Background = background,
                                BorderBrush = BrushUtility.Grey,
                                BorderThickness = 1,
                                Content = new ScrollViewer
                                {
                                    Content = _protectedRecordPanel,
                                    Margin = 4f,
                                    HorizontalScrollBarVisibility = ScrollBarVisibility.Hidden
                                }
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
                            new Border
                            {
                                Background = background,
                                BorderBrush = BrushUtility.Grey,
                                BorderThickness = 1,
                                Content = new ScrollViewer
                                {
                                    Content = _replaceRecordPanel,
                                    Margin = 4f,
                                    HorizontalScrollBarVisibility = ScrollBarVisibility.Hidden
                                }
                            }
                        )
                );
        }


        //------------------------------------------------------
        //
        //  Event Handlers
        //
        //------------------------------------------------------

        #region Event Handlers

        internal static void RemoveSavedKey(ButtonBase button, EventArgs e)
        {
            if (button.GetParent(2) is ReplaceRecordEntry entry)
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
            if (button.GetParent(2) is ReplaceRecordEntry entry)
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
