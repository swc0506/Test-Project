using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshRenderOrderModify : MonoBehaviour
{
    public int sortingOrder;
    void Start()
    {
        GetComponent<MeshRenderer>().sortingOrder= sortingOrder;
    }

    
}
