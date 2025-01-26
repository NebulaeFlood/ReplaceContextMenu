using HarmonyLib;
using Nebulae.RimWorld.UI;
using NoCrowdedContextMenu.SettingPages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace NoCrowdedContextMenu
{
    [StaticConstructorOnStartup]
    internal static class NCCMPatch
    {
        private static ItemPickerWindow _itemPickerWindow;
        private static FloatMenu _replacingMenu;


        static NCCMPatch()
        {
            _itemPickerWindow = new ItemPickerWindow();
        }


        [HarmonyPrefix]
        internal static bool WindowStack_AddPrefix(WindowStack __instance, ref Window window)
        {
            if (ReferenceEquals(_replacingMenu, window))
            {
                _replacingMenu = null;

                return true;
            }
            else if (window is FloatMenu menu)
            {
                var options = FieldAccessUtility.OptionsGetter(menu);

                if (options.Count < NCCM.Settings.MinimumOptionCountCauseReplacement)
                {
                    return true;
                }

                if (NCCM.Settings.AskBeforeReplace)
                {
                    if (!FloatMenuKey.TryCreateKey(out var key))
                    {
                        if (NCCM.Settings.ReplaceUnknownSource)
                        {
                            _itemPickerWindow.SetOptions(menu, options);
                            window = _itemPickerWindow;
                        }

                        return true;
                    }

                    if (NCCM.Settings.ReplacedMenuKeys.Contains(key))
                    {
                        _itemPickerWindow.SetOptions(menu, options);
                        window = _itemPickerWindow;

                        return true;
                    }

                    if (NCCM.Settings.ProtectedMenuKeys.Contains(key))
                    {
                        return true;
                    }

                    __instance.Add(new Dialog_MessageBox(
                        "NCCN.ConfirmReplaceFloatMenu.Text".Translate(),
                        "NCCN.ConfirmReplaceFloatMenu.AcceptButton.Label".Translate(),
                        () =>
                        {
                            AdvancedSettingPage.AddRecord(key, false);

                            _itemPickerWindow.SetOptions(menu, options);
                            _itemPickerWindow.Show();
                        },
                        "NCCN.ConfirmReplaceFloatMenu.RejectButton.Label".Translate(),
                        () =>
                        {
                            AdvancedSettingPage.AddRecord(key, true);

                            _replacingMenu = menu;
                            Find.WindowStack.Add(menu);
                        },
                        buttonADestructive: true));

                    return false;
                }
                else
                {
                    _itemPickerWindow.SetOptions(menu, options);
                    window = _itemPickerWindow;
                }
            }

            return true;
        }
    }
}
