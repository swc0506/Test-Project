using UnityEngine;
using System.Collections.Generic;


namespace UnityEngine.UI
{
    public class GeUIEffectGeo
    {
        public GeUIEffectGeo(int vertices)
        {
            Debug.Assert(vertices >= 4);
            if (vertices < 4)
                vertices = 4;

            VertexNum = vertices;

            Pos = new Vector2[vertices];
            UV = new Vector2[vertices];
            Color = Color.white;
            UnderUsed = false;
        }

        public Vector2[] Pos;
        public Vector2[] UV;
        public Color Color;
        public int VertexNum;
        public bool UnderUsed;
    }
    

    public class GeUIEffectGeoPool
    {
        List<GeUIEffectGeo> m_UIEffectGeoList = new List<GeUIEffectGeo>();
        int m_GeoVertCnt = 0;

        public void Init(int vertices)
        {
            m_GeoVertCnt = vertices;
        }

        public void Deinit()
        {
            Clear();
        }

        public void Clear()
        {
            m_UIEffectGeoList.Clear();
        }

        public GeUIEffectGeo AllocGeometry()
        {
            for(int i = 0; i < m_UIEffectGeoList.Count; ++ i)
            {
                if (false == m_UIEffectGeoList[i].UnderUsed)
                    return m_UIEffectGeoList[i];
            }

            return _Expand();
        }

        protected GeUIEffectGeo _Expand()
        {
            Debug.Assert(m_GeoVertCnt > 0);
            GeUIEffectGeo newGeo = new GeUIEffectGeo(m_GeoVertCnt);

            m_UIEffectGeoList.Add(newGeo);
            return newGeo;
        }
    }
}



