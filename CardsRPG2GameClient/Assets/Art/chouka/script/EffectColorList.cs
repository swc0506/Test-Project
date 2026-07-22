using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectColorList : MonoBehaviour
{
    public Color[] colorlist;

    public void SetColors(int index)
    {
        GetComponent<MeshRenderer>().material.color = colorlist[index];
    }
}
