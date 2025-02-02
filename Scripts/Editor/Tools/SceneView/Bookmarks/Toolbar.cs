using UnityEditor;
using UnityEditor.Overlays;
using UnityEditor.Toolbars;
using UnityEngine;

namespace SceneViewTools
{
    [Overlay(typeof(SceneView), "Scene View Bookmarks")]
    [Icon(SceneViewBookmarkManager.iconPath)]
    internal class SceneViewBookmarkToolbarOverlay : ToolbarOverlay
    {
        SceneViewBookmarkToolbarOverlay() : base(SceneViewBookmarkToolbarDropdown.id) {}
    }

    [EditorToolbarElement(id, typeof(SceneView))]
    internal class SceneViewBookmarkToolbarDropdown : EditorToolbarDropdown
    {
        public const string id = "SceneViewBookmarkToolbarDropdown";

        public SceneViewBookmarkToolbarDropdown()
        {
            text = "Bookmarks";
            clicked += ShowDropdown;
        }

        private static void HandleMoveToBookmark(object userData)
        {
            var slot = (int)userData;
            SceneViewBookmarkManager.MoveToBookmark(slot);
        }

        static void HandleSetBookmark(object userData)
        {
            var slot = (int)userData;
            SceneViewBookmarkManager.SetBookmark(slot);
        }

        private static void ShowDropdown()
        {
            var menu = new GenericMenu();

            for (var slot = 1; slot <= SceneViewBookmarkManager.maxBookmarkCount; slot++)
            {
                var content = new GUIContent($"Move to Bookmark {slot} %{slot}");

                if (SceneViewBookmarkManager.HasBookmark(slot))
                {
                    menu.AddItem(content, false, HandleMoveToBookmark, slot);
                }
                else
                {
                    menu.AddDisabledItem(content);
                }
            }

            menu.AddSeparator(string.Empty);

            var returnToPreviousViewContent = new GUIContent("Return to Previous View %0");

            if (SceneViewBookmarkManager.hasPreviousView)
            {
                menu.AddItem(returnToPreviousViewContent, false, SceneViewBookmarkManager.ReturnToPreviousView);
            }
            else
            {
                menu.AddDisabledItem(returnToPreviousViewContent);
            }

            menu.AddSeparator(string.Empty);

            for (var slot = 1; slot <= SceneViewBookmarkManager.maxBookmarkCount; slot++)
            {
                menu.AddItem(new GUIContent($"Set Bookmark {slot} %&{slot}"), false, HandleSetBookmark, slot);
            }

            menu.ShowAsContext();
        }
    }
}