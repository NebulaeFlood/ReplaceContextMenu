using Nebulae.RimWorld.UI.Data.Utilities;
using System.Collections.Generic;
using UnityEngine;
using Verse;


namespace NoCrowdedContextMenu
{
    internal static class FieldAccessUtility
    {
        internal static readonly FieldAccesser<FloatMenu, List<FloatMenuOption>> OptionsGetter;

        internal static readonly FieldAccesser<FloatMenuOption, Thing> IconThingGetter;
        internal static readonly FieldAccesser<FloatMenuOption, Texture2D> ItemIconGetter;
        internal static readonly FieldAccesser<FloatMenuOption, bool> DrawPlaceHolderIconGetter;
        internal static readonly FieldAccesser<FloatMenuOption, ThingDef> ShownItemGetter;
        internal static readonly FieldAccesser<FloatMenuOption, ThingStyleDef> ThingStyleGetter;

        static FieldAccessUtility()
        {
            OptionsGetter = FieldUtility.CreateFieldAccesser<FloatMenu, List<FloatMenuOption>>("options");
            IconThingGetter = FieldUtility.CreateFieldAccesser<FloatMenuOption, Thing>("iconThing");
            ItemIconGetter = FieldUtility.CreateFieldAccesser<FloatMenuOption, Texture2D>("itemIcon");
            DrawPlaceHolderIconGetter = FieldUtility.CreateFieldAccesser<FloatMenuOption, bool>("drawPlaceHolderIcon");
            ShownItemGetter = FieldUtility.CreateFieldAccesser<FloatMenuOption, ThingDef>("shownItem");
            ThingStyleGetter = FieldUtility.CreateFieldAccesser<FloatMenuOption, ThingStyleDef>("thingStyle");
        }
    }
}
