using Nebulae.RimWorld.UI.Controls;
using Nebulae.RimWorld.UI.Controls.Basic;
using Nebulae.RimWorld.UI.Controls.Composites;
using Nebulae.RimWorld.UI.Controls.Panels;
using Nebulae.RimWorld.UI.Utilities;
using System;
using System.Collections.Generic;
using Verse;

namespace NoCrowdedContextMenu.Views
{
    internal sealed class ItemPickerView : CompositeControl
    {
        internal ItemPickerView()
        {
            _searchBox = new SearchBox
            {
                Margin = 4f,
                HorizontalAlignment = HorizontalAlignment.Left,
                Height = 36f,
                Width = 0.6f,
                MinWidth = 340f
            };

            _optionPanel = new WrapPanel
            {
                ItemWidth = 300f,
                ItemHeight = 48f
            };
            _optionPanel.Bind(_searchBox);

            Initialize();
        }


        //------------------------------------------------------
        //
        //  Public Methods
        //
        //------------------------------------------------------

        #region Public Methods

        public void Clear()
        {
            _optionPanel.Clear();
            _searchBox.Text = string.Empty;
        }

        public void SetOptions(FloatMenu menu, List<FloatMenuOption> options)
        {
            _optionPanel.Set(ProcessOptions(menu, options));
            _optionPanel.ItemWidth = NCCM.Settings.OptionWidth;

            if (NCCM.Settings.FocusSearchBar)
            {
                _searchBox.Focus();
            }
        }

        #endregion


        protected override Control CreateContent()
        {
            var scrollViewer = new ScrollViewer
            {
                Content = _optionPanel,
                HorizontalScroll = true,
                VerticalScrollBarVisibility = ScrollBarVisibility.Hidden
            };

            var border = new Border
            {
                Background = BrushUtility.DarkerGrey,
                BorderBrush = BrushUtility.LightGrey,
                BorderThickness = 1f,
                Content = scrollViewer,
                Margin = (0f, 16f, 0f, 0f),
                Padding = 4f
            };

            return new Grid()
                .DefineRows(36f, Grid.Remain)
                .Set(_searchBox, border
                );
        }


        private IEnumerable<MenuOptionView> ProcessOptions(FloatMenu menu, List<FloatMenuOption> options)
        {
            int count = options.Count;

            for (int i = 0; i < count; i++)
            {
                var option = options[i];
                option.SetSizeMode(FloatMenuSizeMode.Normal);
                yield return new MenuOptionView(menu, option, i);
            }
        }


        //------------------------------------------------------
        //
        //  Private Fields
        //
        //------------------------------------------------------

        #region Private Fields

        private readonly WrapPanel _optionPanel;
        private readonly SearchBox _searchBox;

        #endregion
    }
}
