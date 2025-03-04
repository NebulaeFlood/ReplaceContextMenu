using Nebulae.RimWorld.UI.Controls;
using Nebulae.RimWorld.UI.Controls.Panels;
using Nebulae.RimWorld.UI.Utilities;
using Nebulae.RimWorld.UI.Windows;
using Nebulae.RimWorld.Utilities;
using UnityEngine;
using Verse;
using Grid = Nebulae.RimWorld.UI.Controls.Panels.Grid;
using TextBlock = Nebulae.RimWorld.UI.Controls.TextBlock;

namespace NoCrowdedContextMenu.Windows
{
    internal sealed class MaterialInfoWindow : ControlWindow
    {
        internal static MaterialInfoWindow InfoWindow = new MaterialInfoWindow();

        private static Label _label;

        private static ImageView _thingIcon;

        private static NumberLabel _costCountLabel;
        private static NumberLabel _currentCountLabel;

        private static TextBlock _description;

        public MaterialInfoWindow()
        {
            closeOnAccept = false;
            closeOnCancel = false;
            doCloseButton = false;
            doCloseX = false;
            focusWhenOpened = false;
            preventCameraMotion = false;
            soundAppear = null;
            soundClose = null;


            _label = new Label
            {
                Anchor = TextAnchor.MiddleLeft,
                Margin = new Thickness(2f, 0f, 0f, 0f)
            };

            _thingIcon = new ImageView
            {
                Width = 24f,
                Height = 24f
            };

            _costCountLabel = new NumberLabel
            {
                IsCostNumber = true,
                Margin = new Thickness(0f, 4f, 0f, 0f)
            };
            _currentCountLabel = new NumberLabel
            {
                IsCostNumber = false,
                Margin = new Thickness(0f, 0f, 0f, 4f)
            };

            _description = new TextBlock { HorizontalAlignment = HorizontalAlignment.Left };

            Content = new StackPanel()
                .Set(
                    new Label
                    {
                        Anchor = TextAnchor.UpperLeft,
                        FontSize = GameFont.Medium,
                        Text = "NCCM.InfoWindow.CostResource.Title".Translate().Colorize(ColorLibrary.Yellow)
                    },
                    new Grid()
                        .SetSize(new float[] { 24f, Grid.Remain }, new float[] { 24f })
                        .Set(_thingIcon, _label),
                    _costCountLabel,
                    _currentCountLabel,
                    _description);
            LayoutManager.DebugDrawButtons = false;
        }


        public static void Hide()
        {
            if (InfoWindow.IsOpen)
            {
                _label.Text = string.Empty;
                _thingIcon.ImageSource = Widgets.PlaceholderIconTex;
                _costCountLabel.Number = -1;
                _costCountLabel.Number = -1;
                _description.Text = string.Empty;

                ItemPickerWindow.AnyItemHovered = false;
                InfoWindow.Close(false);
            }
        }

        public static void Initialize(OptionInfo info, Rect windowRect)
        {
            _label.Text = info.Label;
            _thingIcon.CompositionColor = info.Composition;
            _thingIcon.ImageSource = info.Icon;
            _costCountLabel.Number = info.CostCount;
            _currentCountLabel.Number = info.CurrentMapCount;
            _description.Text = info.Description;

            windowRect.height = Mathf.Max(
                windowRect.height,
                InfoWindow.Content.Measure(windowRect - InfoWindow.Padding).Height + InfoWindow.Padding.Top + InfoWindow.Padding.Bottom);
            InfoWindow.windowRect = windowRect;
        }

        public static void Open()
        {
            if (!InfoWindow.IsOpen)
            {
                InfoWindow.Show();
            }
        }


        protected override void SetInitialSizeAndPosition() { }


        private sealed class NumberLabel : Control
        {
            private int _number = -1;
            private string _label;

            public bool IsCostNumber { get; set; }


            public int Number
            {
                get => _number;
                set
                {
                    if (_number != value)
                    {
                        _number = value;

                        if (IsCostNumber)
                        {
                            _label = "NCCM.InfoWindow.CostResource.CostCount".Translate(value);
                        }
                        else
                        {
                            _label = "NCCM.InfoWindow.CostResource.CurrentCount".Translate(value);

                            if (value < _costCountLabel.Number)
                            {
                                _label = _label.Colorize(ColorLibrary.RedReadable);
                            }
                            else
                            {
                                _label = _label.Colorize(ColorLibrary.Green);
                            }
                        }
                    }
                }
            }


            protected override Rect ArrangeCore(Rect availableRect)
            {
                return RenderSize.AlignToArea(availableRect,
                    HorizontalAlignment.Stretch, VerticalAlignment.Top);
            }

            protected override void DrawCore()
            {
                var anchor = Text.Anchor;
                var font = Text.Font;
                Text.Anchor = TextAnchor.MiddleLeft;
                Text.Font = GameFont.Small;

                GUI.Label(RenderRect, _label);

                Text.Anchor = anchor;
                Text.Font = font;
            }

            protected override Size MeasureCore(Size availableSize)
            {
                return new Size(availableSize.Width,
                    _number > -1
                    ? GameFont.Small.GetHeight()
                    : 0f);
            }
        }
    }
}
