using Nebulae.RimWorld.UI;
using Nebulae.RimWorld.UI.Automation.Attributes;
using NoCrowdedContextMenu.Models;
using System.Collections.Generic;
using Verse;

namespace NoCrowdedContextMenu
{
    [LayoutModel("NCCM.Settings")]
    public class NCCMSettings : NebulaeModSettings<NCCMSettings>
    {
        [BooleanEntry]
        public bool AskBeforeReplace = true;
        [BooleanEntry]
        public bool ReplaceUnknownSource = true;

        [BooleanEntry]
        public bool CloseOnClickOutSide = false;
        [BooleanEntry]
        public bool FocusSearchBar = false;
        [BooleanEntry]
        public bool HasMemory = false;
        [BooleanEntry]
        public bool IsDragable = true;
        [BooleanEntry]
        public bool IsResizable = true;
        [BooleanEntry]
        public bool PauseGame = false;

        [NumberEntry(2f, 50f)]
        public int MinimumOptionCountCauseReplacement = 10;

        [NumberEntry(200f, 600f)]
        public float OptionWidth = 330f;

        [NumberEntry(140f, 300f)]
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
            Scribe_Values.Look(ref OptionWidth, nameof(OptionWidth), 330f);
            Scribe_Values.Look(ref OptionDescriptionMaxHeight, nameof(OptionDescriptionMaxHeight), 160f);

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
