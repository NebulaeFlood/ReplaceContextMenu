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
            _optionPanel = new WrapPanel
            {
                ItemWidth = 300f,
                ItemHeight = 48f
            };

            _searchBox = new SearchBox
            {
                Margin = 4f,
                HorizontalAlignment = HorizontalAlignment.Left,
                Height = 36f,
                Width = 0.6f,
                MinWidth = 340f
            };

            _optionPanel.Filter = FilterControl;
            _searchBox.Search += OnSearch;

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
                Background = BrushUtility.DarkGrey,
                BorderBrush = BrushUtility.Grey,
                BorderThickness = 1f,
                Content = scrollViewer,
                Margin = new Thickness(0f, 16f, 0f, 0f),
                Padding = new Thickness(4f, 4f, 0f, 0f)
            };

            return new Grid()
                .DefineRows(36f, Grid.Remain)
                .Set(
                    _searchBox,
                    border
                );
        }


        //------------------------------------------------------
        //
        //  Private Methods
        //
        //------------------------------------------------------

        #region Private Methods

        private bool FilterControl(Control control)
        {
            return _searchBox.Matches(control.Name);
        }

        private IEnumerable<MenuOptionView> ProcessOptions(FloatMenu menu, List<FloatMenuOption> options)
        {
            for (int i = options.Count - 1; i >= 0; i--)
            {
                var option = options[i];
                option.SetSizeMode(FloatMenuSizeMode.Normal);
                yield return new MenuOptionView(menu, option, i);
            }
        }

        private void OnSearch(SearchBox sender, EventArgs args)
        {
            _optionPanel.InvalidateFilter();
        }

        #endregion


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
