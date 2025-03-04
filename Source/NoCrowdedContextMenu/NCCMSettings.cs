using Nebulae.RimWorld.UI.Automation;
using System.Collections.Generic;
using Verse;

namespace NoCrowdedContextMenu
{
    [StandardTranslationKey("NCCM.Settings")]
    public class NCCMSettings : ModSettings
    {
        [BooleanSettingEntry]
        public bool AskBeforeReplace = true;
        [BooleanSettingEntry]
        public bool ReplaceUnknownSource = true;

        [BooleanSettingEntry]
        public bool CloseOnClickOutSide = false;
        [BooleanSettingEntry]
        public bool FocusSearchBar = false;
        [BooleanSettingEntry]
        public bool HasMemory = false;
        [BooleanSettingEntry]
        public bool IsDragable = true;
        [BooleanSettingEntry]
        public bool IsResizable = true;
        [BooleanSettingEntry]
        public bool PauseGame = false;
        [BooleanSettingEntry]
        public bool UseVanillaRenderMode = false;

        [NumberSettingEntry(1f, 200f)]
        public int MinimumOptionCountCauseReplacement = 10;

        public HashSet<FloatMenuKey> ProtectedMenuKeys = new HashSet<FloatMenuKey>();
        public HashSet<FloatMenuKey> ReplacedMenuKeys = new HashSet<FloatMenuKey>();

        public override void ExposeData()
        {
            Scribe_Values.Look(ref AskBeforeReplace, nameof(FocusSearchBar), true);
            Scribe_Values.Look(ref ReplaceUnknownSource, nameof(FocusSearchBar), true);

            Scribe_Values.Look(ref CloseOnClickOutSide, nameof(CloseOnClickOutSide), false);
            Scribe_Values.Look(ref FocusSearchBar, nameof(FocusSearchBar), false);
            Scribe_Values.Look(ref HasMemory, nameof(HasMemory), false);
            Scribe_Values.Look(ref IsDragable, nameof(IsDragable), true);
            Scribe_Values.Look(ref IsResizable, nameof(IsResizable), true);
            Scribe_Values.Look(ref PauseGame, nameof(PauseGame), false);
            Scribe_Values.Look(ref UseVanillaRenderMode, nameof(UseVanillaRenderMode), false);

            Scribe_Values.Look(ref MinimumOptionCountCauseReplacement, nameof(MinimumOptionCountCauseReplacement), 10);

            Scribe_Collections.Look(ref ProtectedMenuKeys, nameof(ProtectedMenuKeys), LookMode.Deep);
            Scribe_Collections.Look(ref ReplacedMenuKeys, nameof(ReplacedMenuKeys), LookMode.Deep);

            if (Scribe.mode is LoadSaveMode.PostLoadInit)
            {
                if (ProtectedMenuKeys is null)
                {
                    ProtectedMenuKeys = new HashSet<FloatMenuKey>();
                }

                if (ReplacedMenuKeys is null)
                {
                    ReplacedMenuKeys = new HashSet<FloatMenuKey>();
                }
            }
        }
    }
}
