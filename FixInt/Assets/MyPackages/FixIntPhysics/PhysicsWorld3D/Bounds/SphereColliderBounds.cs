#if UNITY_EDITOR
using UnityEngine;
using ZM.FixIntMath;

namespace My.FixIntPhysics3D
{
    [ExecuteInEditMode]
    public class SphereColliderBounds : MonoBehaviour
    {
        private Vector3 mCenter;
        private float mRadius;
        public Color color = new Color(1, 0, 0.1f);
        private static Material _material;

        public void SyncRenderData(FixIntVector3 logicPos, FixInt radius, FixIntVector3 center)
        {
            mRadius = radius.RawFloat;
            transform.position = logicPos.ToVector3();
            transform.localScale = (Vector3.one * (mRadius * 2));
        }

        void OnRenderObject()
        {
            GL.PushMatrix();
            if (_material == null)
            {
                _material = new Material(Shader.Find("Hidden/Internal-Colored"));
                _material.color = color;
            }

            _material.SetPass(0);
            GL.MultMatrix(transform.localToWorldMatrix);
            int segments = 20;
            float thetaSegment = Mathf.PI / segments;
            float phiSegment = 2 * Mathf.PI / segments;

            for (int i = 0; i <= segments; i++)
            {
                float theta = i * thetaSegment;
                for (int j = 0; j <= segments; j++)
                {
                    float phi = j * phiSegment;
                    GL.Begin(GL.LINES);
                    Vector3 start = new Vector3(Mathf.Sin(theta) * Mathf.Cos(phi), Mathf.Sin(theta) * Mathf.Sin(phi),
                        Mathf.Cos(theta)) * 0.5f + mCenter;
                    if (i < segments)
                    {
                        Vector3 end = new Vector3(Mathf.Sin(theta + thetaSegment) * Mathf.Cos(phi),
                                          Mathf.Sin(theta + thetaSegment) * Mathf.Sin(phi),
                                          Mathf.Cos(theta + thetaSegment)) * 0.5f +
                                      mCenter;
                        GL.Vertex(start);
                        GL.Vertex(end);
                    }

                    GL.End();
                }
            }

            GL.PopMatrix();
        }

        public void OnRelease()
        {
            GameObject.DestroyImmediate(gameObject);
        }
    }
}
#endif