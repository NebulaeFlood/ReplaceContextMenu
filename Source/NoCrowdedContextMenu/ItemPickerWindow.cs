using Nebulae.RimWorld.UI;
using Nebulae.RimWorld.UI.Controls;
using Nebulae.RimWorld.UI.Controls.Panels;
using Nebulae.RimWorld.UI.Data;
using Nebulae.RimWorld.UI.Data.Binding;
using Nebulae.RimWorld.UI.Windows;
using Nebulae.RimWorld.Utilities;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using Verse;
using Grid = Nebulae.RimWorld.UI.Controls.Panels.Grid;

namespace NoCrowdedContextMenu
{
    public partial class ItemPickerWindow : ControlWindow
    {
        //------------------------------------------------------
        //
        //  Public Const
        //
        //------------------------------------------------------

        #region Public Const

        public const float OptionAreaMargin = 8f;
        public const float OptionItemHeight = 44f;
        public const float OptionItemWidth = 240f;
        public const float SearchBarHeight = 38f;
        public const float SearchBarWidth = 280f;
        public const float StandardItemMargin = 4f;

        #endregion


        private static Rect _previousWindowRect = Rect.zero;

        private QuickSearchFilter _filter;
        private Binding _searchTextBinding;
        private TextBox _searchTextBox;
        private Panel _slectionArea;

        public override Vector2 InitialSize
        {
            get
            {
                return NCCM.Settings.HasMemory && _previousWindowRect != Rect.zero
                    ? new Size(_previousWindowRect.width, _previousWindowRect.height)
                    : new Size(900f, 700f);
            }
        }


        public ItemPickerWindow()
        {
            absorbInputAroundWindow = true;
            closeOnClickedOutside = true;
            doCloseButton = false;
            drawInScreenshotMode = false;
            layer = WindowLayer.Super;
            preventCameraMotion = false;
            soundAppear = SoundDefOf.FloatMenu_Open;

            _filter = new QuickSearchFilter();
        }


        //------------------------------------------------------
        //
        //  Public Methods
        //
        //------------------------------------------------------

        #region Public Methods

        public override void PostClose()
        {
            base.PostClose();

            if (NCCM.Settings.HasMemory)
            {
                _previousWindowRect = windowRect;
            }
            else
            {
                _previousWindowRect = Rect.zero;
            }

            _searchTextBinding.Unbind();
            
            Content = null;
        }

        public override void PostOpen()
        {
            base.PostOpen();
            
            if (NCCM.Settings.FocusSearchBar)
            {
                _searchTextBox.GetFocus();
            }
        }

        public override void PreOpen()
        {
            base.PreOpen();

            if (NCCM.Settings.HasMemory
                && _previousWindowRect != Rect.zero)
            {
                windowRect = _previousWindowRect;
            }
        }

        public void SetOptions(FloatMenu menu, List<FloatMenuOption> options)
        {
            Content = new Grid()
                .SetSize(new float[] { 1f }, new float[] { SearchBarHeight, Grid.Remain })
                .Set(new Control[]
                {
                                CreateSearchBar(this),
                                CreateSelectionArea(this, menu, options)
                });

            TextBox.TextProperty.GetMetadata(typeof(TextBox))
                .PropertyChanged += ForceFilter;

            _filter.Text = string.Empty;

            draggable = NCCM.Settings.IsDragable;
            resizeable = NCCM.Settings.IsResizable;
            forcePause = NCCM.Settings.PauseGame;
        }

        #endregion


        private bool FilterOption(Control control)
        {
            return !_filter.Active || _filter.Matches(control.Name);
        }

        private void ForceFilter(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            _slectionArea.InvalidateFilter();
        }


        //------------------------------------------------------
        //
        //  Control Creater
        //
        //------------------------------------------------------

        #region Control Creater

        private static StackPanel CreateSearchBar(ItemPickerWindow window)
        {
            TextBox textBox = new TextBox
            {
                Margin = StandardItemMargin,
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Stretch,
                Width = SearchBarWidth,
                Name = "SearchBar",
                WrapText = false
            };

            window._searchTextBox = textBox;

            window._searchTextBinding = BindingManager.Bind(
                textBox,
                TextBox.TextProperty,
                window._filter,
                nameof(QuickSearchFilter.Text),
                BindingMode.OneWay,
                BindingFlags.Instance | BindingFlags.Public);

            return new StackPanel
            {
                ChildMaxHeight = SearchBarHeight,
                ChildMaxWidth = float.PositiveInfinity,
                HorizontalAlignment = HorizontalAlignment.Left,
                Orientation = Orientation.Horizontal
            }.Set(
                new ImageView
                {
                    ImageSource = TexButton.Search,
                    Margin = StandardItemMargin + 2f
                },
                textBox);
        }

        private static ScrollViewer CreateSelectionArea(
            ItemPickerWindow window,
            FloatMenu menu,
            List<FloatMenuOption> options)
        {
            MenuItem[] menuItems = new MenuItem[options.Count];

            if (NCCM.Settings.UseVanillaRenderMode)
            {
                for (int i = 0; i < menuItems.Length; i++)
                {
                    options[i].SetSizeMode(FloatMenuSizeMode.Normal);
                    menuItems[i] = new VanillaMenuItem(window, menu, options[i])
                    {
                        Margin = StandardItemMargin
                    };
                }
            }
            else
            {
                for (int i = 0; i < menuItems.Length; i++)
                {
                    options[i].SetSizeMode(FloatMenuSizeMode.Normal);
                    menuItems[i] = new CustomMenuItem(window, menu, options[i])
                    {
                        Margin = StandardItemMargin
                    };
                }
            }

            window._slectionArea = new VirtualizingWrapPanel
            {
                ChildMaxHeight = OptionItemHeight,
                ChildMaxWidth = OptionItemWidth,
                Margin = new Thickness(OptionAreaMargin, OptionAreaMargin * 2f, OptionAreaMargin, OptionAreaMargin),
                Filter = window.FilterOption,
            }.Set(menuItems);

            return new ScrollViewer
            {
                Content = window._slectionArea,
                HorizontalScroll = true,
                HorizontalScrollBarVisibility = ScrollBarVisibility.Auto,
                VerticalScrollBarVisibility = ScrollBarVisibility.Hidden
            };
        }

        #endregion
    }
}
