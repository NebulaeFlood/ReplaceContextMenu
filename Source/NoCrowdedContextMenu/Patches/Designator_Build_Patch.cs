using NoCrowdedContextMenu.Utilities;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace NoCrowdedContextMenu.Patches
{
    internal static class Designator_Build_Patch
    {
        internal static void ProcessInputPrefix(Designator_Build __instance)
        {
            if (__instance.PlacingDef is BuildableDef def && def.MadeFromStuff)
            {
                MenuOptionUtility.OnMaterialPickerCreated(def);
            }
        }
    }
}
