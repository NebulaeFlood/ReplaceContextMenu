using Nebulae.RimWorld.UI;
using Nebulae.RimWorld.UI.Automation.Attributes;
using NoCrowdedContextMenu.Models;
using System.Collections.Generic;
using Verse;

namespace NoCrowdedContextMenu
{
    [Settings("NCCM.Settings")]
    public class NCCMSettings : NebulaeModSettings<NCCMSettings>
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

        [NumberSettingEntry(2f, 50f)]
        public int MinimumOptionCountCauseReplacement = 10;

        [NumberSettingEntry(200f, 600f)]
        public float OptionWidth = 300f;

        [NumberSettingEntry(140f, 300f)]
        public float OptionDescriptionMaxHeight = 160f;

        public HashSet<MenuSourceModel> ProtectedMenuSources;
        public HashSet<MenuSourceModel> ReplacedMenuSources;

        public override void ExposeData()
        {
            Scribe_Values.Look(ref AskBeforeReplace, nameof(AskBeforeReplace), true);
            Scribe_Values.Look(ref ReplaceUnknownSource, nameof(ReplaceUnknownSource), true);

            Scribe_Values.Look(ref CloseOnClickOutSide, nameof(CloseOnClickOutSide), false);
            Scribe_Values.Look(ref FocusSearchBar, nameof(FocusSearchBar), false);
            Scribe_Values.Look(ref HasMemory, nameof(HasMemory), false);
            Scribe_Values.Look(ref IsDragable, nameof(IsDragable), true);
            Scribe_Values.Look(ref IsResizable, nameof(IsResizable), true);
            Scribe_Values.Look(ref PauseGame, nameof(PauseGame), false);

            Scribe_Values.Look(ref MinimumOptionCountCauseReplacement, nameof(MinimumOptionCountCauseReplacement), 10);

            Scribe_Collections.Look(ref ProtectedMenuSources, nameof(ProtectedMenuSources), LookMode.Deep);
            Scribe_Collections.Look(ref ReplacedMenuSources, nameof(ReplacedMenuSources), LookMode.Deep);
        }

        public override void OnCheckIntegrity()
        {
            if (ProtectedMenuSources is null)
            {
                ProtectedMenuSources = new HashSet<MenuSourceModel>();
            }

            if (ReplacedMenuSources is null)
            {
                ReplacedMenuSources = new HashSet<MenuSourceModel>();
            }
        }
    }
}
