using System.Collections.Generic;
using Verse;

namespace NoCrowdedContextMenu
{
    public class NCCMSettings : ModSettings
    {
        public bool AskBeforeReplace = true;
        public bool ReplaceUnknownSource = true;

        public bool FocusSearchBar = false;
        public bool HasMemory = false;
        public bool IsDragable = true;
        public bool IsResizable = true;
        public bool PauseGame = false;
        public bool UseVanillaRenderMode = false;

        public int MinimumOptionCountCauseReplacement = 10;

        public HashSet<FloatMenuKey> ProtectedMenuKeys = new HashSet<FloatMenuKey>();
        public HashSet<FloatMenuKey> ReplacedMenuKeys = new HashSet<FloatMenuKey>();

        public override void ExposeData()
        {
            Scribe_Values.Look(ref AskBeforeReplace, nameof(FocusSearchBar), true);
            Scribe_Values.Look(ref ReplaceUnknownSource, nameof(FocusSearchBar), true);

            Scribe_Values.Look(ref FocusSearchBar, nameof(FocusSearchBar), false);
            Scribe_Values.Look(ref HasMemory, nameof(HasMemory), false);
            Scribe_Values.Look(ref IsDragable, nameof(IsDragable), true);
            Scribe_Values.Look(ref IsResizable, nameof(IsResizable), true);
            Scribe_Values.Look(ref PauseGame, nameof(PauseGame), false);
            Scribe_Values.Look(ref UseVanillaRenderMode, nameof(UseVanillaRenderMode), false);

            Scribe_Values.Look(ref MinimumOptionCountCauseReplacement, nameof(MinimumOptionCountCauseReplacement), 10);

            Scribe_Collections.Look(ref ProtectedMenuKeys, nameof(ProtectedMenuKeys), LookMode.Deep);
            Scribe_Collections.Look(ref ReplacedMenuKeys, nameof(ReplacedMenuKeys), LookMode.Deep);
        }
    }
}
