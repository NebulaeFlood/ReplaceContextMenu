using Nebulae.RimWorld.UI.Controls;
using Nebulae.RimWorld.UI.Controls.Basic;
using Nebulae.RimWorld.UI.Controls.Composites;
using Nebulae.RimWorld.UI.Controls.Panels;
using Nebulae.RimWorld.UI.Core.Data;
using NoCrowdedContextMenu.Coordinators;
using NoCrowdedContextMenu.Models;
using NoCrowdedContextMenu.Utilities;
using RimWorld;
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
    internal sealed class MenuSourceView : CompositeControl
    {
        public readonly MenuSourceModel Model;


        static MenuSourceView()
        {
            MarginProperty.OverrideMetadata(typeof(MenuSourceView),
                new ControlPropertyMetadata(new Thickness(4f), ControlRelation.Measure));
        }

        internal MenuSourceView(MenuSourceModel model)
        {
            Model = model;
            Initialize();
        }


        protected override Control CreateContent()
        {
            var label = new Label
            {
                Anchor = TextAnchor.MiddleLeft,
                Padding = new Thickness(12f, 0f, 12f, 0f),
                Text = Model.ToString(),
                Tooltip = "NCCM.MenuSource.Label.Tooltip".Translate()
            };

            var switchButton = new IconButton(ResourceUtility.SwitchButtonIcon)
            {
                ClickSound = SoundDefOf.Tick_Tiny,
                CursorEnterSound = SoundDefOf.Mouseover_Standard,
                Tooltip = "NCCM.MenuSource.Switch.Tooltip".Translate()
            };
            switchButton.Click += MenuManagerCoordinator.OnSourceSwitchType;

            var deleteButton = new IconButton(TexButton.Delete)
            {
                ClickSound = SoundDefOf.Click,
                CursorEnterSound = SoundDefOf.Mouseover_Standard,
                Tooltip = "NCCM.MenuSource.Delete.Tooltip".Translate()
            };
            deleteButton.Click += MenuManagerCoordinator.OnSourceDelete;

            var grid = new Grid()
                .DefineColumns(Grid.Remain, 24f, 24f)
                .DefineRows(34f)
                .Set(label, switchButton, deleteButton);

            return new Border
            {
                Background = TexUI.HighlightTex,
                Content = grid
            };
        }
    }
}
