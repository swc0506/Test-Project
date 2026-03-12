using UnityEngine;

public class Lesson_99 : MonoBehaviour
{
    public Color color = Color.white;
    [Range(0,1)]
    public float fresnelScale;
    
    void Start()
    { 
        //获取对象的渲染器
        Renderer renderer = GetComponent<Renderer>();
        if (renderer != null)
        {
            Material material = renderer.sharedMaterial;//material;
            //得到所有的材质球
            //Material[] materials = renderer.sharedMaterials;
            material.color = color;
            //修改主纹理
            //material.mainTexture = Resources.Load<Texture2D>("路径");
            if (material.HasColor("_Color"))
            {
                material.SetColor("_Color", color);
                print(material.GetColor("_Color"));
            }
            if (material.HasFloat("_FresnelScale"))
                material.SetFloat("_FresnelScale", fresnelScale);
            
            //material.renderQueue = 2000;//设置渲染队列
            //material.SetTextureOffset("_MainTex", new Vector2(0, 0));
            //material.SetTextureScale("_MainTex", new Vector2(1, 1));
            //material.shader = Shader.Find("Unlit/Fresnel_80");
        }
    }

    void Update()
    {
        
    }
}
