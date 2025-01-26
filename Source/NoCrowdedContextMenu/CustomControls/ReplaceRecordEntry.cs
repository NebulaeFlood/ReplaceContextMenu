using Nebulae.RimWorld.UI.Controls;
using Nebulae.RimWorld.UI.Utilities;
using NoCrowdedContextMenu.SettingPages;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;
using Grid = Nebulae.RimWorld.UI.Controls.Panels.Grid;

namespace NoCrowdedContextMenu.CustomControls
{
    [StaticConstructorOnStartup]
    public class ReplaceRecordEntry : FrameworkControl
    {
        private static readonly Texture2D RemoveRecord = TexButton.Delete;
        private static readonly Texture2D SwitchType = ContentFinder<Texture2D>.Get("UI/Icons/SwitchFaction");

        public readonly FloatMenuKey Key;
        public bool IsProtectedKey;

        private readonly Grid _content;


        static ReplaceRecordEntry()
        {
            MarginProperty.OverrideMetadata(typeof(ReplaceRecordEntry),
                new ControlPropertyMetadata(new Thickness(4f), ControlRelation.Measure));

            HorizontalAlignmentProperty.OverrideMetadata(typeof(ReplaceRecordEntry),
                new ControlPropertyMetadata(HorizontalAlignment.Stretch, ControlRelation.Measure));

            VerticalAlignmentProperty.OverrideMetadata(typeof(ReplaceRecordEntry),
                new ControlPropertyMetadata(VerticalAlignment.Stretch, ControlRelation.Measure));
        }

        public ReplaceRecordEntry(FloatMenuKey key, bool isProtectedKey)
        {
            Key = key;
            IsProtectedKey = isProtectedKey;

            var infoLabel = new Label
            {
                Anchor = TextAnchor.MiddleLeft,
                Margin = new Thickness(4f, 0f, 4f, 0f),
                Text = key.Namespace + "." + key.DeclaringTypeName + "." + key.MethodName,
                Tooltip = "NCCM.ReplaceSettings.Entry.Tooltip".Translate(),
                ShowTooltip = true
            };

            var switchButton = new IconButton
            {
                ClickSound = SoundDefOf.Tick_Tiny,
                Icon = SwitchType,
                Tooltip = "NCCM.AdvancedSettings.Entry.SwitchButton.Tooltip".Translate(),
                ShowTooltip = true
            };
            switchButton.Click += AdvancedSettingPage.SwitchSavedKey;
            switchButton.SetParent(this);

            var deleteButton = new IconButton
            {
                Icon = RemoveRecord,
                Tooltip = "NCCM.AdvancedSettings.Entry.DeleteButton.Tooltip".Translate(),
                ShowTooltip = true
            };
            deleteButton.Click += AdvancedSettingPage.RemoveSavedKey;
            deleteButton.SetParent(this);

            _content = new Grid()
                .SetSize(new float[] { Grid.Remain, 24f, 24f }, new float[] { 1f })
                .Set(infoLabel, switchButton, deleteButton);

            _content.SetParent(this);
        }


        //------------------------------------------------------
        //
        //  Protected Methods
        //
        //------------------------------------------------------

        #region Protected Methods

        protected override Rect ArrangeCore(Rect availableRect)
        {
            return _content.Arrange(base.ArrangeCore(availableRect));
        }

        protected override void DrawCore()
        {
            GUI.DrawTexture(RenderRect, Widgets.LightHighlight);

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
    }
}
