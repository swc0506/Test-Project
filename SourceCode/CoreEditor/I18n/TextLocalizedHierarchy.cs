#if I18N
using Core.I18n;
using CoreEditor.I18N;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace CoreEditor
{
    [InitializeOnLoad]
    static class TextLocalizedHierarchy
    {
        static TextLocalizedHierarchy()
        {
            EditorApplication.hierarchyChanged += OnMonitorText;
        }

        private static void OnMonitorText()
        {
            // I18NSettingsObject config = I18NSettingsObject.Get();
            // if (!config.useTextLocalized)
            // {
            //     return;
            // }
            //
            // Text[] texts = GameObject.FindObjectsOfType<Text>();
            // foreach (var item in texts)
            // {
            //     TextLocalized content = item.GetComponent<TextLocalized>();
            //     if (null == content)
            //     {
            //         content = item.gameObject.AddComponent<TextLocalized>();
            //     }
            // }
        }
    }
}
#endif