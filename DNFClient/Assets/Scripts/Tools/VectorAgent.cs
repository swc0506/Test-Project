using Fantasy;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.AI;

public static class VectorAgent 
{
    public static CSVector3 ToCSVector3(this Vector3 ve3)
    {
        CSVector3 cs3 =new CSVector3(); 
        cs3.x = ve3.x;  
        cs3.y = ve3.y;
        cs3.z = ve3.z;
        return cs3;
    }

    public static Vector3 ToVector3(this CSVector3 ve3)
    {
        Vector3 cs3 = new Vector3(ve3.x,ve3.y,ve3.z);
        return cs3;
    }
}
