using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public static class TransformAgent  
{
    public static void ReSetParent(this Transform self,Transform parent)
    {
        self.SetParent(parent);
        self.transform.localPosition = Vector3.zero;
        self.transform.localRotation = Quaternion.identity;
        self.transform.localScale = Vector3.one;
    }
}
