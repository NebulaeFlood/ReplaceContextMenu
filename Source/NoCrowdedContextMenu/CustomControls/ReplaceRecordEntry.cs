﻿using Nebulae.RimWorld.UI.Controls;
using Nebulae.RimWorld.UI.Utilities;
using NoCrowdedContextMenu.SettingPages;
using RimWorld;
using UnityEngine;
using Verse;
using Grid = Nebulae.RimWorld.UI.Controls.Panels.Grid;

namespace NoCrowdedContextMenu.CustomControls
{
    [StaticConstructorOnStartup]
    public class ReplaceRecordEntry : UserControl
    {
        private static readonly Texture2D RemoveRecord = TexButton.Delete;
        private static readonly Texture2D SwitchType = ContentFinder<Texture2D>.Get("UI/Icons/SwitchFaction");

        public readonly FloatMenuKey Key;
        public bool IsProtectedKey;


        static ReplaceRecordEntry()
        {
            MarginProperty.OverrideMetadata(typeof(ReplaceRecordEntry),
                new ControlPropertyMetadata(new Thickness(4f), ControlRelation.Measure));
        }

        public ReplaceRecordEntry(FloatMenuKey key, bool isProtectedKey)
        {
            Key = key;
            IsProtectedKey = isProtectedKey;
            Initialize();
        }


        //------------------------------------------------------
        //
        //  Protected Methods
        //
        //------------------------------------------------------

        #region Protected Methods

        protected override Control CreateContent()
        {
            var infoLabel = new Label
            {
                Anchor = TextAnchor.MiddleLeft,
                Margin = new Thickness(4f, 0f, 4f, 0f),
                Text = Key.ToString(),
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
            switchButton.Clicked += AdvancedSettingPage.SwitchSavedKey;

            var deleteButton = new IconButton
            {
                Icon = RemoveRecord,
                Tooltip = "NCCM.AdvancedSettings.Entry.DeleteButton.Tooltip".Translate(),
                ShowTooltip = true
            };
            deleteButton.Clicked += AdvancedSettingPage.RemoveSavedKey;

            return new Grid()
                .SetSize(new float[] { Grid.Remain, 24f, 24f }, new float[] { 1f })
                .Set(infoLabel, switchButton, deleteButton);
        }

        protected override void DrawCore()
        {
            GUI.DrawTexture(RenderRect, Widgets.LightHighlight);
            base.DrawCore();
        }

        #endregion
    }
}
