using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEngine;
using Verse;

namespace NoCrowdedContextMenu
{
    internal static class FieldAccessUtility
    {
        internal static readonly Func<FloatMenu, List<FloatMenuOption>> OptionsGetter;

        internal static readonly Func<FloatMenuOption, Thing> IconThingGetter;
        internal static readonly Func<FloatMenuOption, Texture2D> ItemIconGetter;
        internal static readonly Func<FloatMenuOption, bool> DrawPlaceHolderIconGetter;
        internal static readonly Func<FloatMenuOption, ThingDef> ShownItemGetter;
        internal static readonly Func<FloatMenuOption, ThingStyleDef> ThingStyleGetter;

        static FieldAccessUtility()
        {
            OptionsGetter = CreateFieldGetter<Func<FloatMenu, List<FloatMenuOption>>>(
                typeof(FloatMenu),
                "options");

            Type optionType = typeof(FloatMenuOption);

            IconThingGetter = CreateFieldGetter<Func<FloatMenuOption, Thing>>(
                optionType,
                "iconThing");

            ItemIconGetter = CreateFieldGetter<Func<FloatMenuOption, Texture2D>>(
                optionType,
                "itemIcon");

            DrawPlaceHolderIconGetter = CreateFieldGetter<Func<FloatMenuOption, bool>>(
                optionType,
                "drawPlaceHolderIcon");

            ShownItemGetter = CreateFieldGetter<Func<FloatMenuOption, ThingDef>>(
                optionType,
                "shownItem");

            ThingStyleGetter = CreateFieldGetter<Func<FloatMenuOption, ThingStyleDef>>(
                optionType,
                "thingStyle");
        }


        public static T CreateFieldGetter<T>(Type type, string name)
        {
            ParameterExpression targetExp = Expression.Parameter(type, "target");
            MemberExpression fieldExp = Expression.Field(
                targetExp,
                AccessTools.Field(type, name));

            return Expression.Lambda<T>(
                fieldExp,
                targetExp).Compile();
        }
    }
}
