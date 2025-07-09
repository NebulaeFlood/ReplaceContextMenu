using Nebulae.RimWorld.UI.Controls;
using Nebulae.RimWorld.UI.Controls.Basic;
using Nebulae.RimWorld.UI.Controls.Composites;
using Nebulae.RimWorld.UI.Controls.Panels;
using Nebulae.RimWorld.UI.Utilities;
using NoCrowdedContextMenu.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;
using Grid = Nebulae.RimWorld.UI.Controls.Panels.Grid;

namespace NoCrowdedContextMenu.Views
{
    internal sealed class MenuManagerView : CompositeControl
    {
        //------------------------------------------------------
        //
        //  Public Fields
        //
        //------------------------------------------------------

        #region Public Fields

        public readonly StackPanel ProtectedSourcePanel = new StackPanel();
        public readonly StackPanel ReplacedSourcePanel = new StackPanel();

        #endregion


        static MenuManagerView()
        {
            MarginProperty.OverrideMetadata(typeof(MenuManagerView),
                new ControlPropertyMetadata(new Thickness(8f), ControlRelation.Measure));
        }

        internal MenuManagerView()
        {
            Initialize();
        }


        protected override Control CreateContent()
        {
            var settings = NCCM.Settings;

            MenuSourceView Convert(MenuSourceModel model)
            {
                return new MenuSourceView(model);
            }

            ProtectedSourcePanel.Set(settings.ProtectedMenuSources.Select(Convert));
            ReplacedSourcePanel.Set(settings.ReplacedMenuSources.Select(Convert));

            var protectedSourcesView = new Grid()
                .DefineRows(36f, Grid.Remain)
                .Set(
                    new Label
                    {
                        Anchor = TextAnchor.LowerLeft,
                        Text = "NCCM.Advanced.BlackList.Label".Translate(),
                        Tooltip = "NCCM.Advanced.BlackList.Tooltip".Translate()
                    },
                    new Border
                    {
                        Background = BrushUtility.WindowBackground,
                        BorderBrush = BrushUtility.Grey,
                        BorderThickness = 1f,
                        Content = new ScrollViewer
                        {
                            Content = ProtectedSourcePanel,
                            HorizontalScrollBarVisibility = ScrollBarVisibility.Hidden
                        }
                    }
                );

            var replacedSourceView = new Grid()
                .DefineRows(36f, Grid.Remain)
                .Set(
                    new Label
                    {
                        Anchor = TextAnchor.LowerLeft,
                        Text = "NCCM.Advanced.WhiteList.Label".Translate(),
                        Tooltip = "NCCM.Advanced.WhiteList.Tooltip".Translate()
                    },
                    new Border
                    {
                        Background = BrushUtility.WindowBackground,
                        BorderBrush = BrushUtility.Grey,
                        BorderThickness = 1f,
                        Content = new ScrollViewer
                        {
                            Content = ReplacedSourcePanel,
                            HorizontalScrollBarVisibility = ScrollBarVisibility.Hidden
                        }
                    }
                );

            var panel = new StackPanel
            {
                ItemHeight = 0.5f,
                Margin = new Thickness(4f, 0f, 4f, 4f)
            }.Set(protectedSourcesView, replacedSourceView);

            return panel;
        }
    }
}
