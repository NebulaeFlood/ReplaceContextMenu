using NoCrowdedContextMenu.Utilities;
using Verse;

namespace NoCrowdedContextMenu.Patches
{
    internal static class WindowStack_Patch
    {
        internal static void AddPrefix(WindowStack __instance, ref Window window)
        {
            if (window is FloatMenu floatMenu)
            {
                window = MenuOptionUtility.ReplaceFloatMenu(__instance, floatMenu);
            }
        }
    }
}
