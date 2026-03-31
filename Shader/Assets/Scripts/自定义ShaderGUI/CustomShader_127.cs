using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class CustomShader_127 : ShaderGUI
{
    private bool isShow;

    public override void OnGUI(MaterialEditor materialEditor, MaterialProperty[] properties)
    {
        //base.OnGUI(materialEditor, properties);

        Material material = materialEditor.target as Material;
        if (GUILayout.Button(isShow ? "隐藏" : "显示"))
        {
            isShow = !isShow;
        }

        if (GUILayout.Button("重置"))
        {
            material.SetTexture("_MainTex", null);
            material.SetFloat("_TestFloat1", 0);
            material.SetFloat("_TestFloat2", 0);
            material.SetFloat("_TestFloat3", 0);
            material.SetFloat("_TestFloat4", 0);
        }
        

        if (isShow)
        {
            //获取当前材质球
            foreach (var item in properties)
            {
                if (item.displayName.Equals("TestFloat1"))
                {
                    //自定义一个拖动条去设置TestFloat属性
                    item.floatValue = EditorGUILayout.Slider(item.displayName, item.floatValue, -1, 1);
                }
                else
                {
                    //利用Unity自带窗口UI显示方式
                    materialEditor.ShaderProperty(item, item.displayName);
                }
            } 
            MaterialProperty property = FindProperty("_TestFloat2", properties);
            property.floatValue = EditorGUILayout.Slider(property.displayName, property.floatValue, -1, 1);
        }
    }
}