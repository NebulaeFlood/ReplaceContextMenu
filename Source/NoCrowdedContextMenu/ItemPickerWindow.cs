using Nebulae.RimWorld.UI;
using Nebulae.RimWorld.UI.Controls;
using Nebulae.RimWorld.UI.Controls.Panels;
using Nebulae.RimWorld.UI.Data.Binding;
using Nebulae.RimWorld.UI.Windows;
using NoCrowdedContextMenu.CustomControls;
using RimWorld;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using Verse;
using Grid = Nebulae.RimWorld.UI.Controls.Panels.Grid;

namespace NoCrowdedContextMenu
{
    [StaticConstructorOnStartup]
    internal class ItemPickerWindow : ControlWindow
    {
        //------------------------------------------------------
        //
        //  Public Const
        //
        //------------------------------------------------------

        #region Public Const

        public const float OptionAreaMargin = 8f;
        public const float OptionItemHeight = 48f;
        public const float OptionItemWidth = 300f;
        public const float SearchBarHeight = 36f;
        public const float SearchBarWidth = 340f;
        public const float StandardItemMargin = 4f;

        #endregion


        internal static bool AnyItemHovered = false;
        internal static ItemPickerWindow PickerWindow = new ItemPickerWindow();


        //------------------------------------------------------
        //
        //  Private Fields
        //
        //------------------------------------------------------

        #region Private Fields

        private static Rect _previousWindowRect;

        private readonly QuickSearchFilter _filter;
        private TextBox _searchTextBox;
        private VirtualizingWrapPanel _slectionArea;

        #endregion

        public string SearchBarText
        {
            get => _filter.Text;
            set
            {
                _filter.Text = value;

                if (IsOpen)
                {
                    _searchTextBox.Text = value;
                    _slectionArea.InvalidateFilter();
                }
            }
        }


        internal ItemPickerWindow()
        {
            doCloseButton = false;
            drawInScreenshotMode = false;
            preventCameraMotion = false;
            soundAppear = SoundDefOf.FloatMenu_Open;

            _filter = new QuickSearchFilter();

            Content = new Grid()
                .SetSize(new float[] { 1f }, new float[] { SearchBarHeight, Grid.Remain })
                .Set(
                    CreateSearchBar(),
                    CreateSelectionArea()
                );
        }


        //------------------------------------------------------
        //
        //  Public Methods
        //
        //------------------------------------------------------

        #region Public Methods

        protected override void LateWindowOnGUI(Rect inRect)
        {
            if (AnyItemHovered)
            {
                AnyItemHovered = false;
                MaterialInfoWindow.Open();
            }
            else
            {
                MaterialInfoWindow.Hide();
            }
        }

        public override void PostClose()
        {
            if (NCCM.Settings.HasMemory)
            {
                _previousWindowRect = windowRect;
            }
            else
            {
                _previousWindowRect = Rect.zero;
            }

            SearchBarText = string.Empty;

            NCCMPatch.IsBuildPickerMenu = false;
            NCCMPatch.IsMaterialPickerMenu = false;
            NCCMPatch.BuildRequireMaterial = null;

            base.PostClose();
        }

        public override void PreOpen()
        {
            base.PreOpen();

            if (NCCM.Settings.FocusSearchBar
                || Current.ProgramState is ProgramState.Entry)
            {
                _searchTextBox.GetFocus();
            }
        }

        public void SetOptions(FloatMenu menu, List<FloatMenuOption> options)
        {
            Control[] menuItems = new Control[options.Count];

            if (NCCM.Settings.UseVanillaRenderMode)
            {
                layer = WindowLayer.Super;

                for (int i = 0; i < menuItems.Length; i++)
                {
                    options[i].SetSizeMode(FloatMenuSizeMode.Normal);
                    menuItems[i] = new VanillaMenuItem(this, menu, options[i])
                    {
                        Margin = StandardItemMargin
                    };
                }
            }
            else
            {
                layer = WindowLayer.Dialog;

                for (int i = 0; i < menuItems.Length; i++)
                {
                    options[i].SetSizeMode(FloatMenuSizeMode.Normal);
                    menuItems[i] = new CustomMenuItem(this, menu, options[i], i)
                    {
                        Margin = StandardItemMargin
                    };
                }
            }

            _slectionArea.Set(menuItems);

            closeOnClickedOutside = NCCM.Settings.CloseOnClickOutSide;
            draggable = NCCM.Settings.IsDragable;
            resizeable = NCCM.Settings.IsResizable;
            forcePause = NCCM.Settings.PauseGame;
        }

        protected override void SetInitialSizeAndPosition()
        {
            if (NCCM.Settings.HasMemory
                && _previousWindowRect != Rect.zero)
            {
                windowRect = _previousWindowRect;
            }
            else
            {
                base.SetInitialSizeAndPosition();
            }
        }

        #endregion


        private bool FilterOption(Control control)
        {
            return !_filter.Active || _filter.Matches(control.Name);
        }


        //------------------------------------------------------
        //
        //  Control Creater
        //
        //------------------------------------------------------

        #region Control Creater

        private StackPanel CreateSearchBar()
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

            _searchTextBox = textBox;

            BindingManager.Bind(
                textBox,
                TextBox.TextProperty,
                this,
                nameof(SearchBarText),
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

        private ScrollViewer CreateSelectionArea()
        {
            _slectionArea = new VirtualizingWrapPanel
            {
                ItemHeight = OptionItemHeight,
                ItemWidth = OptionItemWidth,
                Margin = new Thickness(OptionAreaMargin, OptionAreaMargin * 2f, OptionAreaMargin, OptionAreaMargin),
                Filter = FilterOption,
            };

            return new ScrollViewer
            {
                Content = _slectionArea,
                HorizontalScroll = true,
                HorizontalScrollBarVisibility = ScrollBarVisibility.Auto,
                VerticalScrollBarVisibility = ScrollBarVisibility.Hidden
            };
        }

        #endregion
    }
}
