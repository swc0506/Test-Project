using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class GameObjectSettingSerialize : MonoBehaviour
{
    public int          lightmapIndex = -1;
    public Vector4      lightmapScaleOffset; 
    
    void Awake()
    {
        if(lightmapIndex != -1)
        {
            Renderer renderer = gameObject.GetComponent<Renderer>();
            renderer.lightmapIndex = lightmapIndex;
            renderer.lightmapScaleOffset = lightmapScaleOffset;
        }
    }

    public void SnapSetting()
    {
        Renderer renderer = gameObject.GetComponent<Renderer>();

        if(renderer.lightmapIndex == -1)
        {
            DestroyImmediate(this);
        }
        else
        {
            lightmapIndex = renderer.lightmapIndex;
            lightmapScaleOffset = renderer.lightmapScaleOffset;
        }
    }
}
