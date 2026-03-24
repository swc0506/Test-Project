using UnityEngine;

public class FogDepth_113 : PostEffectBase
{
    public Color fogColor = Color.gray;
    [Range(0, 3)]
    public float fogDensity = 1f;//雾密度
    public float fogStartDistance = 10f;//雾开始距离
    public float fogEndDistance = 2000f;//
    private Matrix4x4 rayMatrix;
    
    void Start()
    {
        Camera.main.depthTextureMode = DepthTextureMode.Depth;
    }

    protected override void UpdateProperties()
    {
        if (Material != null)
        {
            //得到摄像机 视口 夹角
            float fov = Camera.main.fieldOfView / 2f;
            //得到近裁剪面距离
            float near = Camera.main.nearClipPlane;
            //得到窗口比例
            float aspect = Camera.main.aspect;
            //计算高的一半
            float halfHeight = near * Mathf.Tan(fov * Mathf.Deg2Rad);
            //计算宽的一半
            float halfWidth = halfHeight * aspect;
            
            //得到偏移量
            Vector3 toTop = Camera.main.transform.up * halfHeight;
            Vector3 toRight = Camera.main.transform.right * halfWidth;
            
            //得到顶点
            Vector3 topLeft = Camera.main.transform.forward * near + toTop - toRight;
            Vector3 topRight = Camera.main.transform.forward * near + toTop + toRight;
            Vector3 bottomLeft = Camera.main.transform.forward * near - toTop - toRight;
            Vector3 bottomRight = Camera.main.transform.forward * near - toTop + toRight;
            //得到缩放比例 为了让深度值计算出来是两点间的距离 所以需要乘以一个缩放值
            float scale = topLeft.magnitude / near;
            //缩放
            topLeft = topLeft.normalized * scale;
            topRight = topRight.normalized * scale;
            bottomLeft = bottomLeft.normalized * scale;
            bottomRight = bottomRight.normalized * scale;
            
            //得到矩阵 逆时针
            rayMatrix.SetRow(0, bottomLeft);
            rayMatrix.SetRow(1, bottomRight);
            rayMatrix.SetRow(2, topRight);
            rayMatrix.SetRow(3, topLeft);
            
            //设置材质球相关属性
            Material.SetColor("_FogColor", fogColor);
            Material.SetFloat("_FogDensity", fogDensity);
            Material.SetFloat("_FogStart", fogStartDistance);
            Material.SetFloat("_FogEnd", fogEndDistance);
            Material.SetMatrix("_RayMatrix", rayMatrix);
        }
    }
}
