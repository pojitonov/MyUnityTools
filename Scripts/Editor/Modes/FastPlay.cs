﻿#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace MyTools
{
    [InitializeOnLoad]
    internal static class FastPlay
    {
        private const string MENU_NAME = Menus.MY_TOOLS_MENU + "Fast Play Mode";
        private const int ITEM_INDEX = Menus.MODES_INDEX + 101;
        private static bool _enabled;

        static FastPlay()
        {
            _enabled = EditorPrefs.GetBool(MENU_NAME, true);

            EditorApplication.delayCall += () => { PerformAction(_enabled); };
        }

        [MenuItem(MENU_NAME, priority = ITEM_INDEX)]
        private static void ToggleAction()
        {
            PerformAction(!_enabled);
        }

        private static void PerformAction(bool enabled)
        {
            UnityEditor.Menu.SetChecked(MENU_NAME, enabled);
            EditorPrefs.SetBool(MENU_NAME, enabled);

            _enabled = enabled;

            ToggleFastPlayMode(_enabled);
        }

        private static void ToggleFastPlayMode(bool enabled)
        {
            EditorSettings.enterPlayModeOptionsEnabled = enabled;
            AssetDatabase.Refresh();
            bool playModeState = EditorSettings.enterPlayModeOptionsEnabled;
            if (playModeState)
            {
                Debug.Log($"MyTools: Fast Play Mode is Enabled");
            }
            else
            {
                Debug.Log($"MyTools: Fast Play Mode is Disabled");
            }
        }
    }
}
#endif