using System;
using System.Reflection;
using UnityEditor;

namespace MyTools
{
    internal static class ClearConsole
    {
        [MenuItem(Menus.EDITOR_MENU + "Clear Console %l", priority = Menus.EDITOR_INDEX + 300)] // Ctrl+L
        static void Clear()
        {
            Functions.ClearConsole();
        }
    }
}