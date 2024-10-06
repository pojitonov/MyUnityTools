using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using System.Reflection;
using UnityEngine;
using Object = System.Object;

namespace MyTools
{
    static class Shortcuts
    {
        // Grid
        [MenuItem("My Tools/Toggle Grid %&#g", priority = 11)] // Ctrl+Alt+Shift+G
        private static void ToggleGridVisibility()
        {
            // Iterate through all open SceneViews
            foreach (var sceneView in SceneView.sceneViews)
            {
                if (sceneView is SceneView view)
                {
                    // Toggle the grid visibility based on its current state
                    view.showGrid = !view.showGrid;
                }
            }
        }

        [MenuItem("My Tools/Toggle Grid Snapping &j", priority = 11)] // Alt+J
        public static void ToggleGridSnapping()
        {
            EditorSnapSettings.snapEnabled = !EditorSnapSettings.snapEnabled;
        }

        // Panels
        [MenuItem("My Tools/Toggle Lock %&l", priority = 12)] // Ctrl+Alt+L
        static void ToggleWindowLock()
        {
            // "EditorWindow.focusedWindow" can be used instead
            EditorWindow windowToBeLocked = EditorWindow.mouseOverWindow;

            if (windowToBeLocked != null && windowToBeLocked.GetType().Name == "InspectorWindow")
            {
                Type type = Assembly.GetAssembly(typeof(Editor)).GetType("UnityEditor.InspectorWindow");
                PropertyInfo propertyInfo = type.GetProperty("isLocked");
                bool value = (bool)propertyInfo.GetValue(windowToBeLocked, null);
                propertyInfo.SetValue(windowToBeLocked, !value, null);
                windowToBeLocked.Repaint();
            }
            else if (windowToBeLocked != null && windowToBeLocked.GetType().Name == "ProjectBrowser")
            {
                Type type = Assembly.GetAssembly(typeof(Editor)).GetType("UnityEditor.ProjectBrowser");
                PropertyInfo propertyInfo = type.GetProperty("isLocked",
                    BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);

                bool value = (bool)propertyInfo.GetValue(windowToBeLocked, null);
                propertyInfo.SetValue(windowToBeLocked, !value, null);
                windowToBeLocked.Repaint();
            }
            else if (windowToBeLocked != null && windowToBeLocked.GetType().Name == "SceneHierarchyWindow")
            {
                Type type = Assembly.GetAssembly(typeof(Editor))
                    .GetType("UnityEditor.SceneHierarchyWindow");

                FieldInfo fieldInfo = type.GetField("m_SceneHierarchy",
                    BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
                PropertyInfo propertyInfo = fieldInfo.FieldType.GetProperty("isLocked",
                    BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
                object value = fieldInfo.GetValue(windowToBeLocked);
                bool value2 = (bool)propertyInfo.GetValue(value);
                propertyInfo.SetValue(value, !value2, null);
                windowToBeLocked.Repaint();
            }
        }

        // Console
        [MenuItem("My Tools/Clear Console &c", priority = 13)] // Alt+C
        static void ClearConsole()
        {
            MyTools.ClearConsole();
        }

        // View
        [MenuItem("My Tools/Maximize %b", priority = 14)] // Ctrl+B
        static void Maximize()
        {
            MyTools.ActivateWindowUnderCursor();
            EditorWindow window = EditorWindow.focusedWindow;
            // Assume the game view is focused.
            if (window)
            {
                window.maximized = !window.maximized;
            }
        }

        // Tabs
        [MenuItem("My Tools/Close Tab &w", priority = 15)] // Alt+W
        static void CloseTab()
        {
            MyTools.ActivateWindowUnderCursor();
            EditorWindow window = EditorWindow.focusedWindow;
            // Assume the game view is focused.
            if (window)
            {
                window.Close();
            }
        }

        // Assets
        [MenuItem("My Tools/Force Refresh Assets #r", priority = 16)] // Shift+R
        private static void ForceRefreshSelectedAsset()
        {
            // Get the selected assets in the Project Window
            var selectedObjects = Selection.objects;

            if (selectedObjects == null || selectedObjects.Length == 0)
            {
                // If no assets are selected, refresh all assets
                AssetDatabase.Refresh();
                Debug.Log("MyTools: All assets have been refreshed.");
            }
            else
            {
                foreach (var obj in selectedObjects)
                {
                    // Get the path of the selected asset
                    string assetPath = AssetDatabase.GetAssetPath(obj);

                    if (!string.IsNullOrEmpty(assetPath))
                    {
                        // Force refresh the specific asset
                        AssetDatabase.ImportAsset(assetPath, ImportAssetOptions.ForceUpdate);
                        Debug.Log($"MyTools: {assetPath} has been refreshed.");
                    }
                }
            }
        }


        // Prefab Overrides
        [MenuItem("My Tools/Apply Prefab Overrides #a", priority = 17)] // Shift+A
        // private static void ApplyOverrides()
        // {
        //     // Get the currently selected GameObject in the hierarchy
        //     GameObject selectedObject = Selection.activeGameObject;
        //
        //     // Check if the selected object is a valid prefab instance
        //     if (selectedObject == null)
        //     {
        //         return;
        //     }
        //
        //     PrefabInstanceStatus prefabStatus = PrefabUtility.GetPrefabInstanceStatus(selectedObject);
        //
        //     if (prefabStatus != PrefabInstanceStatus.Connected)
        //     {
        //         return;
        //     }
        //
        //     // Apply all overrides to the prefab asset
        //     GameObject prefabRoot = PrefabUtility.GetOutermostPrefabInstanceRoot(selectedObject);
        //     if (prefabRoot != null)
        //     {
        //         PrefabUtility.ApplyPrefabInstance(prefabRoot, InteractionMode.UserAction);
        //         Debug.Log("My Tools: Prefab overrides applied successfully to " + prefabRoot.name);
        //     }
        //     else
        //     {
        //         Debug.LogError("My Tools: Could not find the prefab root.");
        //     }
        // }
        public static void ApplySelectedPrefabOverrides()
        {
            GameObject[] selectedObjects = Selection.gameObjects;

            if (selectedObjects.Length == 0)
            {
                Debug.LogWarning("My Tools: No GameObjects selected.");
                return;
            }

            foreach (var obj in selectedObjects)
            {
                // Get the corresponding prefab root object
                GameObject prefabRoot = PrefabUtility.GetCorrespondingObjectFromSource(obj);

                if (prefabRoot != null)
                {
                    // Apply all overrides to the prefab
                    PrefabUtility.ApplyPrefabInstance(obj, InteractionMode.UserAction);
                    Debug.Log($"My Tools: Applied overrides to {prefabRoot.name}");
                }
                else
                {
                    Debug.LogWarning($"My Tools: No prefab found for {obj.name}");
                }
            }

            // Deselect and reselect the GameObjects to refresh the Inspector
            DeselectAndReselect(selectedObjects);
        }

        // Deselect and then reselect the given GameObjects to refresh the Inspector
        private static void DeselectAndReselect(GameObject[] selectedObjects)
        {
            // Save current selection
            var currentSelection = selectedObjects;

            // Clear the selection
            Selection.activeGameObject = null;

            // Reselect the objects after a small delay to ensure the inspector refreshes
            EditorApplication.delayCall += () => Selection.objects = currentSelection;
        }


        // Create Prefab
        [MenuItem("My Tools/Create Prefab from Selection", false, 18)]
        private static void CreatePrefabFromSelectedFBX()
        {
            // Get selected objects in the Project window
            Object[] selectedObjects = Selection.objects;

            foreach (var selectedObject in selectedObjects)
            {
                // Check if the selected object is an FBX file
                string path = AssetDatabase.GetAssetPath((UnityEngine.Object)selectedObject);
                if (Path.GetExtension(path).ToLower() == ".fbx")
                {
                    // Load the FBX model
                    GameObject fbxModel = AssetDatabase.LoadAssetAtPath<GameObject>(path);
                    if (fbxModel == null)
                    {
                        Debug.LogError("Could not load FBX model at path: " + path);
                        return;
                    }

                    // Create a prefab from the loaded model
                    string prefabPath = Path.ChangeExtension(path, ".prefab");
                    PrefabUtility.SaveAsPrefabAsset(fbxModel, prefabPath);

                    Debug.Log("Prefab created at: " + prefabPath);
                }
                else
                {
                    Debug.LogWarning("Selected object is not an FBX file: " + path);
                }
            }
        }


        // Toggle Isolation on Selection
        private static GameObject lastSelectedObject;
        private static bool toggleState;
        private static HashSet<GameObject> hiddenObjects = new();

        [MenuItem("My Tools/Toggle Isolation on Selection #\\", false, 19)]
        private static void ToggleObjectVisibility()
        {
            GameObject selectedObject = Selection.activeGameObject;

            if (selectedObject == null)
            {
                RestoreVisibility();
                toggleState = false;
                lastSelectedObject = null;
                return;
            }

            if (selectedObject != lastSelectedObject && toggleState)
            {
                RestoreVisibility();
                toggleState = false;
            }

            if (selectedObject == lastSelectedObject && toggleState)
            {
                RestoreVisibility();
            }
            else
            {
                HideAllExceptSelected(selectedObject);
            }

            toggleState = !toggleState;
            lastSelectedObject = selectedObject;
        }

        private static void HideAllExceptSelected(GameObject selectedObject)
        {
            GameObject[] rootObjects = selectedObject.scene.GetRootGameObjects();

            foreach (GameObject obj in rootObjects)
            {
                if (obj != selectedObject)
                {
                    SetSceneVisibility(obj, false); // Hide the object
                }
            }

            SetSceneVisibility(selectedObject, true);
        }

        private static void SetSceneVisibility(GameObject obj, bool visible)
        {
            if (visible)
            {
                SceneVisibilityManager.instance.Show(obj, true);
                hiddenObjects.Remove(obj);
            }
            else
            {
                SceneVisibilityManager.instance.Hide(obj, true);
                hiddenObjects.Add(obj);
            }

            foreach (Transform child in obj.transform)
            {
                SetSceneVisibility(child.gameObject, visible);
            }
        }

        private static void RestoreVisibility()
        {
            // Show all previously hidden objects
            foreach (GameObject obj in hiddenObjects)
            {
                SceneVisibilityManager.instance.Show(obj, true);
            }

            hiddenObjects.Clear();
        }
    }
}