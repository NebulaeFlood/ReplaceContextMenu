using NoCrowdedContextMenu.Utilities;
using RimWorld;
using Verse;

namespace NoCrowdedContextMenu.Patches
{
    internal static class Designator_Dropdown_Patch
    {
        internal static void ProcessInputPrefix(Designator_Dropdown __instance)
        {
            bool IsVisible(Designator d)
            {
                return d.Visible;
            }

            MenuOptionUtility.OnBuildingPickerCreated(__instance.Elements.FindAll(IsVisible));
        }
    }
}
