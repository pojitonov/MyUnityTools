#if UNITY_EDITOR
using MyTools;
using UnityEditor;

public static class SwitchViews
{
    [MenuItem(Menus.EDITOR_MENU + "Switch SceneView _TAB", priority = Menus.EDITOR_INDEX + 200)]
    private static void SwitchView()
    {
        EditorWindow focusedWindow = EditorWindow.focusedWindow;

        if (focusedWindow != null)
        {
            string windowType = focusedWindow.GetType().Name;

            if (windowType == "SceneView")
            {
                EditorApplication.ExecuteMenuItem("Window/General/Game");
            }
            else if (windowType == "GameView")
            {
                EditorApplication.ExecuteMenuItem("Window/General/Scene");
            }
        }
    }
}
#endif