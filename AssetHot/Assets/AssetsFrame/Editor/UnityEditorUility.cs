using System;
using UnityEngine;

public class UnityEditorUility
{
    public static GUIStyle GetGUIStyle(string styleName)
    {
        GUIStyle guiStyle = null;
        foreach (var style in GUI.skin.customStyles)
        {
            if (string.Equals(style.name, styleName, StringComparison.OrdinalIgnoreCase))
            {
                guiStyle = style;
                break;
            }
        }
        return guiStyle;
    }
}
