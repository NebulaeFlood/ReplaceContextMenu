using Nebulae.RimWorld.UI.Core.Data.Utilities;
using NoCrowdedContextMenu.Coordinators;
using NoCrowdedContextMenu.Models;
using NoCrowdedContextMenu.Views;
using RimWorld;
using System.Collections.Generic;
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

        internal static void OnMenuProcessed()
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
                OnMenuProcessed();
                return menu;
            }

            if (ReferenceEquals(_protectedMenu, menu))
            {
                OnMenuProcessed();
                _protectedMenu = null;
                return menu;
            }

            if (!settings.AskBeforeReplace)
            {
                return ItemPickerCoordinator.Bind(menu, options);
            }

            if (!MenuSourceModel.TryCreate(out var model))
            {
                if (settings.ReplaceUnknownSource)
                {
                    return ItemPickerCoordinator.Bind(menu, options);
                }
                else
                {
                    OnMenuProcessed();
                    return menu;
                }
            }

            if (settings.ProtectedMenuSources.Contains(model))
            {
                OnMenuProcessed();
                return menu;
            }

            if (settings.ReplacedMenuSources.Contains(model))
            {
                return ItemPickerCoordinator.Bind(menu, options);
            }

            if (_askMessageBox is null)
            {
                void AcceptReplacement()
                {
                    if (settings.ReplacedMenuSources.Add(model))
                    {
                        settings.Mod.WriteSettings();
                        MenuManagerCoordinator.View.ReplacedSourcePanel.Append(new MenuSourceView(model));
                    }

                    _askMessageBox = null;

                    windowStack.Add(ItemPickerCoordinator.Bind(menu, options));
                }

                void RejectReplacement()
                {
                    _protectedMenu = menu;

                    if (settings.ProtectedMenuSources.Add(model))
                    {
                        settings.Mod.WriteSettings();
                        MenuManagerCoordinator.View.ProtectedSourcePanel.Append(new MenuSourceView(model));
                    }

                    _askMessageBox = null;

                    windowStack.Add(menu);
                }

                _askMessageBox = new Dialog_MessageBox(
                    "NCCN.ConfirmReplace.Text".Translate(),
                    "NCCN.ConfirmReplace.Accept.Label".Translate(),
                    AcceptReplacement,
                    "NCCN.ConfirmReplace.Reject.Label".Translate(),
                    RejectReplacement);

                return _askMessageBox;
            }

            return null;
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

        private static Dialog_MessageBox _askMessageBox;
        private static FloatMenu _protectedMenu;

        #endregion
    }
}
