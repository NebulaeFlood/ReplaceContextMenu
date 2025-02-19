using HarmonyLib;
using Nebulae.RimWorld.UI.Data;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEngine;
using Verse;
using static Nebulae.RimWorld.UI.Data.FieldUtility;

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
            OptionsGetter = CreateFieldAccesser<FloatMenu, List<FloatMenuOption>>("options");
            IconThingGetter = CreateFieldAccesser<FloatMenuOption, Thing>("iconThing");
            ItemIconGetter = CreateFieldAccesser<FloatMenuOption, Texture2D>("itemIcon");
            DrawPlaceHolderIconGetter = CreateFieldAccesser<FloatMenuOption, bool>("drawPlaceHolderIcon");
            ShownItemGetter = CreateFieldAccesser<FloatMenuOption, ThingDef>("shownItem");
            ThingStyleGetter = CreateFieldAccesser<FloatMenuOption, ThingStyleDef>("thingStyle");
        }
    }
}
