#nullable enable
using UnityEditor;
using UnityEngine;

namespace Unity2Dx.Editor
{
    public static class ConversionMenus
    {
        [MenuItem("Window/2Dx/Convert All to 3D")]
        private static void ConvertAllTo3D()
        {
            Conversion.ConvertAll(false);
        }

        [MenuItem("Window/2Dx/Convert All to 2D")]
        private static void ConvertAllTo2D()
        {
            Conversion.ConvertAll(true);
        }

        private const string debugConvertibleCopiesMenuPath = "Window/2Dx/" + CopyConvertible<Component, Component>.debugConvertibleCopiesPrefsKey;
        [MenuItem(debugConvertibleCopiesMenuPath)]
        private static void DebugConvertibleCopies()
        {
            var key = CopyConvertible<Component, Component>.debugConvertibleCopiesPrefsKey;
            EditorPrefs.SetBool(key, !EditorPrefs.GetBool(key, false));
            SetDebugConvertibleCopiesChecked();
        }

        [InitializeOnLoadMethod]
        private static void SetDebugConvertibleCopiesChecked()
        {
            var key = CopyConvertible<Component, Component>.debugConvertibleCopiesPrefsKey;
            Menu.SetChecked(debugConvertibleCopiesMenuPath, EditorPrefs.GetBool(key, false));
        }

        [MenuItem("CONTEXT/Component/Convert Game Object to 3D")]
        private static void ConvertTo3D(MenuCommand command)
        {
            var component = (Component)command.context;
            Conversion.Convert(false, component.gameObject);
        }

        [MenuItem("CONTEXT/Component/Convert Game Object to 2D")]
        private static void ConvertTo2D(MenuCommand command)
        {
            var component = (Component)command.context;
            Conversion.Convert(true, component.gameObject);
        }
    }
}
