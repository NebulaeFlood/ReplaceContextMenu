using Nebulae.RimWorld.UI.Controls;
using Nebulae.RimWorld.UI.Controls.Basic;
using Nebulae.RimWorld.UI.Controls.Composites;
using Nebulae.RimWorld.UI.Core.Data;
using Nebulae.RimWorld.UI.Utilities;
using NoCrowdedContextMenu.Models;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;
using Grid = Nebulae.RimWorld.UI.Controls.Panels.Grid;
using TextBlock = Nebulae.RimWorld.UI.Controls.Basic.TextBlock;

namespace NoCrowdedContextMenu.Views
{
    internal sealed class BuildingMenuOptionInfoView : CompositeControl
    {
        public bool IsEmpty => _isEmpty;


        static BuildingMenuOptionInfoView()
        {
            VerticalAlignmentProperty.OverrideMetadata(typeof(BuildingMenuOptionInfoView),
                new PropertyMetadata(VerticalAlignment.Top));
        }

        internal BuildingMenuOptionInfoView()
        {
            Initialize();
        }


        //------------------------------------------------------
        //
        //  Public Methods
        //
        //------------------------------------------------------

        #region Public Methods

        public void Bind(MenuOptionView option)
        {
            if (!option.Model.Building.IsValid)
            {
                return;
            }

            _icon.Model = option.Model;
            _resourceName.Text = option.Name;

            int cost = option.Model.Building.Cost;
            int count = option.Model.Building.ResourceCount;

            _costLabel.Text = "NCCM.InfoWindow.CostResource.CostCount".Translate(cost);

            if (count > -1)
            {
                _countLabel.Text = "NCCM.InfoWindow.CostResource.CurrentCount".Translate(count).Colorize(cost > count ? ColorLibrary.RedReadable : ColorLibrary.Green);
                _countLabel.Visibility = Visibility.Visible;
            }
            else
            {
                _countLabel.Visibility = Visibility.Collapsed;
            }

            _description.MaxHeight = NCCM.Settings.OptionDescriptionMaxHeight;
            _description.Text = option.Model.Building.Material.description;
            _isEmpty = false;
        }

        public void Clear()
        {
            _isEmpty = true;
        }

        #endregion


        protected override Control CreateContent()
        {
            var title = new Label
            {
                Anchor = TextAnchor.UpperLeft,
                FontSize = GameFont.Medium,
                Text = "NCCM.InfoWindow.CostResource.Title".Translate().Colorize(ColorLibrary.Yellow)
            };

            var resourceInfo = new Grid()
                .DefineColumns(27f, 4f, Grid.Remain)
                .DefineRows(27f)
                .Set(_icon, null, _resourceName);

            var grid = new Grid()
                .DefineRows(36f, 30f, 24f, Grid.Auto, Grid.Auto)
                .Set(title, resourceInfo, _costLabel, _countLabel, _description);

            return new Border
            {
                Background = BrushUtility.WindowBackground,
                BorderBrush = BrushUtility.WindowBorder,
                BorderThickness = 1f,
                Content = grid,
                Padding = 16f
            };
        }


        //------------------------------------------------------
        //
        //  Private Fields
        //
        //------------------------------------------------------

        #region Private Fields

        private readonly Icon _icon = new Icon();
        private readonly Label _resourceName = new Label { Anchor = TextAnchor.MiddleLeft };

        private readonly Label _costLabel = new Label { Anchor = TextAnchor.LowerLeft };
        private readonly Label _countLabel = new Label { Anchor = TextAnchor.UpperLeft, Height = 24f };

        private readonly TextBlock _description = new TextBlock { MaxHeight = 140f };

        private bool _isEmpty = true;

        #endregion


        private sealed class Icon : Control
        {
            public MenuOptionModel Model
            {
                get => _model;
                set => _model = value;
            }


            //------------------------------------------------------
            //
            //  Protected Methods
            //
            //------------------------------------------------------

            #region Protected Methods

            protected override Size MeasureCore(Size availableSize) => new Size(27f);

            protected override void DrawCore(ControlState states)
            {
                _model.DrawIcon(DesiredRect);
            }

            #endregion


            private MenuOptionModel _model;
        }
    }
}
