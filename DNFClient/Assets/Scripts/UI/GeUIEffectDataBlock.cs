using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;
using System;

namespace UnityEngine.UI
{
    [System.Serializable]
    public class GeUIEffectDataBlock
    {
        public GeUIEffectDataBlock(string name,string value)
        {
            m_Name = name;
            m_Value = value;
        }

        public string m_Name;
        public string m_Value;
    }

    public class GeUIEffectDataBlockSerializer
    {
        static public string[] ToString(GeUIEffectDataBlock[] data)
        {
            string[] res = new string[data.Length];
            for(int i = 0; i < data.Length; ++ i)
                res[i] = string.Format("{0}|{1}", data[i].m_Name, data[i].m_Value);

            return res;
        }

        static public GeUIEffectDataBlock[] FromString(string[] str)
        {
            GeUIEffectDataBlock[] res = new GeUIEffectDataBlock[str.Length];
            for (int i = 0; i < str.Length; ++i)
            {
                string[] data = str[i].Split('|');
                if(data.Length == 2)
                    res[i] = new GeUIEffectDataBlock(data[0],data[1]);
            }
            return res;
        }
    }
}
