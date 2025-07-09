using Nebulae.RimWorld.UI.Core.Data.Utilities;
using NoCrowdedContextMenu.Coordinators;
using NoCrowdedContextMenu.Models;
using NoCrowdedContextMenu.Views;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace NoCrowdedContextMenu.Utilities
{
    public static class MenuOptionUtility
    {
        public static bool IsType(this MenuOptionType sourceType, MenuOptionType type)
        {
            return (sourceType & type) != 0;
        }


        internal static BuildingMenuOptionModel GetBuildingMenuItemInfo(ThingDef material, int index)
        {
            if (_isBuildingPicker
                && _buildings[index] is Designator_Place designator
                && designator.PlacingDef is BuildableDef building
                && (building.CostStuffCount > 0 || building.costList.Count == 1))
            {
                return new BuildingMenuOptionModel(designator.PlacingDef, material);
            }
            else if (_isMaterialPicker
                && _building != null
                && (_building.CostStuffCount > 0 || _building.costList.Count == 1))
            {
                return new BuildingMenuOptionModel(_building, material);
            }
            else
            {
                return BuildingMenuOptionModel.Empty;
            }
        }

        internal static void OnBuildingPickerCreated(List<Designator> buildings)
        {
            _buildings = buildings;
            _isBuildingPicker = true;
        }

        internal static void OnMaterialPickerCreated(BuildableDef def)
        {
            _building = def;
            _isMaterialPicker = true;
        }

        internal static void OnMenuReplaced()
        {
            _isBuildingPicker = false;
            _buildings = null;

            _isMaterialPicker = false;
            _building = null;
        }

        internal static Window ReplaceFloatMenu(WindowStack windowStack, FloatMenu menu)
        {
            var options = OptionsGetter(menu);
            var settings = NCCM.Settings;

            if (options.Count < settings.MinimumOptionCountCauseReplacement)
            {
                return menu;
            }

            if (!settings.AskBeforeReplace)
            {
                return ItemPickerCoordinator.Bind(menu, options);
            }

            if (!MenuSourceModel.TryCreate(out var model))
            {
                return settings.ReplaceUnknownSource ? ItemPickerCoordinator.Bind(menu, options) : menu;
            }

            if (settings.ProtectedMenuSources.Contains(model))
            {
                return menu;
            }

            if (settings.ReplacedMenuSources.Contains(model))
            {
                return ItemPickerCoordinator.Bind(menu, options);
            }

            void AcceptReplacement()
            {
                settings.ReplacedMenuSources.Add(model);
                settings.Mod.WriteSettings();
                windowStack.Add(ItemPickerCoordinator.Bind(menu, options));

                MenuManagerCoordinator.View.ReplacedSourcePanel.Append(new MenuSourceView(model));
            }

            void RejectReplacement()
            {
                settings.ProtectedMenuSources.Add(model);
                settings.Mod.WriteSettings();
                windowStack.Add(menu);

                MenuManagerCoordinator.View.ProtectedSourcePanel.Append(new MenuSourceView(model));
            }

            return new Dialog_MessageBox(
                "NCCN.ConfirmReplace.Text".Translate(),
                "NCCN.ConfirmReplace.Accept.Label".Translate(),
                AcceptReplacement,
                "NCCN.ConfirmReplace.Reject.Label".Translate(),
                RejectReplacement);
        }

        internal static void ResolveDesignator(Designator_Build designator)
        {
            if (designator.PlacingDef is BuildableDef def && def.MadeFromStuff)
            {
                _isMaterialPicker = true;
                _building = def;
            }
        }

        internal static void ResolveDesignator(Designator_Dropdown designator)
        {
            _isBuildingPicker = true;
            _buildings = designator.Elements.FindAll(x => x.Visible);
        }


        //------------------------------------------------------
        //
        //  Private Fields
        //
        //------------------------------------------------------

        #region Private Fields

        private static readonly FieldAccessor<FloatMenu, List<FloatMenuOption>> OptionsGetter = FieldUtility.CreateFieldAccessor<FloatMenu, List<FloatMenuOption>>("options");

        private static bool _isBuildingPicker;
        private static List<Designator> _buildings;

        private static bool _isMaterialPicker;
        private static BuildableDef _building;

        #endregion
    }
}
