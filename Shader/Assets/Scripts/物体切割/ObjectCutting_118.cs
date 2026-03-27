using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class ObjectCutting_118 : MonoBehaviour
{
    //材质
    private Material material;
    //切割对象 用于确定切割位置
    public GameObject cutObj;
    
    void Start()
    {
        material = GetComponent<Renderer>().sharedMaterial;
    }

    void Update()
    {
        if (material != null && cutObj != null)
        {
            material.SetVector("_CuttingPos", cutObj.transform.position);
        }
    }
}
