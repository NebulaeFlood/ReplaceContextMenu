using HarmonyLib;
using Nebulae.RimWorld.UI;
using NoCrowdedContextMenu.SettingPages;
using Verse;
using static NoCrowdedContextMenu.ItemPickerWindow;

namespace NoCrowdedContextMenu
{
    [StaticConstructorOnStartup]
    internal static class NCCMPatch
    {
        private static Window _replacingMenu;


        static NCCMPatch()
        {
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
                            PickerWindow.SetOptions(menu, options);
                            window = PickerWindow;
                        }

                        return true;
                    }

                    if (NCCM.Settings.ReplacedMenuKeys.Contains(key))
                    {
                        PickerWindow.SetOptions(menu, options);
                        window = PickerWindow;

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

                            _replacingMenu = PickerWindow;

                            PickerWindow.SetOptions(menu, options);
                            PickerWindow.Show();
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
                    PickerWindow.SetOptions(menu, options);
                    window = PickerWindow;
                }
            }

            return true;
        }
    }
}
