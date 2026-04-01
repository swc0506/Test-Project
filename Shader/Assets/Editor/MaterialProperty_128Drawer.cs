using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class MaterialProperty_128Drawer : MaterialPropertyDrawer
{
    private float min;
    private float max;

    public MaterialProperty_128Drawer(float min, float max)
    {
        this.min = min;
        this.max = max;
    }

    public override void OnGUI(Rect position, MaterialProperty prop, string label, MaterialEditor editor)
    {
        //base.OnGUI(position, prop, label, editor);

        if (prop.type != MaterialProperty.PropType.Float)
        {
            EditorGUILayout.LabelField(label, "请使用float或者数值 不然无法使用该控件");
            return;
        }

        prop.floatValue = EditorGUILayout.Slider(label, prop.floatValue, min, max);
    }
}
