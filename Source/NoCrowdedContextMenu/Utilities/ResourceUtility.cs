using UnityEngine;
using Verse;

namespace NoCrowdedContextMenu.Utilities
{
    [StaticConstructorOnStartup]
    internal static class ResourceUtility
    {
        internal static readonly Texture2D SwitchButtonIcon = ContentFinder<Texture2D>.Get("UI/Icons/SwitchFaction");
    }
}
