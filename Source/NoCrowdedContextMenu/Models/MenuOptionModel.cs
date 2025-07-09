using Nebulae.RimWorld.UI.Core.Data.Utilities;
using NoCrowdedContextMenu.Utilities;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace NoCrowdedContextMenu.Models
{
    [Flags]
    public enum MenuOptionType
    {
        Normal = 0b000,
        WithIcon = 0b001,
        WithInfoCard = 0b010,
        WithGUITooltip = 0b100
    }


    public readonly struct MenuOptionModel
    {
        public static readonly Color WhiteColor = new Color(1f, 1f, 1f, 1f);


        //------------------------------------------------------
        //
        //  Public Fields
        //
        //------------------------------------------------------

        #region Public Fields

        public readonly MenuOptionType OptionType;

        public readonly BuildingMenuOptionModel Building;

        public readonly Texture2D Icon;
        public readonly Thing Thing;
        public readonly ThingDef ThingDef;
        public readonly ThingStyleDef StyleDef;

        public readonly FloatMenuOption Source;

        #endregion


        static MenuOptionModel()
        {
            IconThingGetter = FieldUtility.CreateFieldAccessor<FloatMenuOption, Thing>("iconThing");
            ItemIconGetter = FieldUtility.CreateFieldAccessor<FloatMenuOption, Texture2D>("iconTex");
            DrawPlaceHolderIconGetter = FieldUtility.CreateFieldAccessor<FloatMenuOption, bool>("drawPlaceHolderIcon");
            ShownItemGetter = FieldUtility.CreateFieldAccessor<FloatMenuOption, ThingDef>("shownItem");
            ThingStyleGetter = FieldUtility.CreateFieldAccessor<FloatMenuOption, ThingStyleDef>("thingStyle");
        }

        internal MenuOptionModel(FloatMenuOption option, int index)
        {
            Source = option;

            _drawLayer = DrawPlaceHolderIconGetter(option);

            Thing = IconThingGetter(option);
            ThingDef = ShownItemGetter(option);

            Icon = ItemIconGetter(option);

            OptionType = option.extraPartOnGUI is null ? MenuOptionType.Normal : MenuOptionType.WithInfoCard;

            if (option.mouseoverGuiAction != null)
            {
                OptionType |= MenuOptionType.WithGUITooltip;
            }

            _iconComposition = WhiteColor;
            StyleDef = null;  

            if (ThingDef != null)
            {
                _iconStatus = IconStatus.FromDef;
                OptionType |= MenuOptionType.WithIcon;

                _iconComposition = option.forceThingColor ?? (ThingDef.MadeFromStuff ? ThingDef.GetColorForStuff(GenStuff.DefaultStuffFor(ThingDef)) : ThingDef.uiIconColor);
                StyleDef = option.forceBasicStyle ? null : (ThingStyleGetter(option) ?? Faction.OfPlayer.ideos?.PrimaryIdeo.GetStyleFor(ThingDef));
            }
            else if (Icon != null)
            {
                _iconStatus = IconStatus.FromTexture;
                OptionType |= MenuOptionType.WithIcon;
            }
            else if (Thing != null)
            {
                _iconStatus = IconStatus.FromThing;
                OptionType |= MenuOptionType.WithIcon;
                ThingDef = Thing.def;
            }
            else
            {
                _iconStatus = IconStatus.None;

                if (_drawLayer)
                {
                    OptionType |= MenuOptionType.WithIcon;
                }
            }

            Building = ThingDef is null ? BuildingMenuOptionModel.Empty : MenuOptionUtility.GetBuildingMenuItemInfo(ThingDef, index);
        }


        public void DrawIcon(Rect renderRect)
        {
            if (_drawLayer)
            {
                Widgets.DrawTextureFitted(renderRect, Widgets.PlaceholderIconTex, 1f);
            }

            if (_iconStatus is IconStatus.None)
            {
                return;
            }

            Color color = GUI.color;

            if (_iconStatus is IconStatus.FromDef)
            {
                Widgets.DefIcon(
                    renderRect,
                    ThingDef,
                    thingStyleDef: StyleDef,
                    color: new Color(_iconComposition.r, _iconComposition.g, _iconComposition.b, _iconComposition.a * color.a));
            }
            else
            {
                GUI.color = new Color(
                    Source.iconColor.r,
                    Source.iconColor.g,
                    Source.iconColor.b,
                    Source.iconColor.a * color.a);

                if (_iconStatus is IconStatus.FromTexture)
                {
                    Widgets.DrawTextureFitted(renderRect, Icon, 1f, new Vector2(1f, 1f), Source.iconTexCoords);
                }
                else
                {
                    Widgets.ThingIcon(renderRect, Thing);
                }

                GUI.color = color;
            }
        }


        //------------------------------------------------------
        //
        //  Private Static Fields
        //
        //------------------------------------------------------

        #region Private Static Fields

        private static readonly FieldAccessor<FloatMenuOption, Thing> IconThingGetter;
        private static readonly FieldAccessor<FloatMenuOption, Texture2D> ItemIconGetter;
        private static readonly FieldAccessor<FloatMenuOption, bool> DrawPlaceHolderIconGetter;
        private static readonly FieldAccessor<FloatMenuOption, ThingDef> ShownItemGetter;
        private static readonly FieldAccessor<FloatMenuOption, ThingStyleDef> ThingStyleGetter;

        #endregion


        //------------------------------------------------------
        //
        //  Private Fields
        //
        //------------------------------------------------------

        #region Private Fields

        private readonly IconStatus _iconStatus;

        private readonly bool _drawLayer;

        private readonly Color _iconComposition;

        #endregion


        private enum IconStatus
        {
            None,
            LayerOnly,
            FromTexture,
            FromDef,
            FromThing
        }
    }
}
