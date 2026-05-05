using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace RedDot.System
{
    public class RedDotItem : MonoBehaviour
    {
        public RedDotDefine redKey;
        public GameObject redDotObj;
        public Text countText;
        
        void Start()
        {
            RedDotSystem.Instance.RegisterRedDotChangeEvent(redKey, OnRedDotChangeEvent);
            RedDotSystem.Instance.UpdateRedDotState(redKey);
        }
        
        void OnEnable()
        {
            RedDotSystem.Instance.UpdateRedDotState(redKey);
        }
        
        void OnRedDotChangeEvent(RedDotType type, bool active, int count)
        {
            redDotObj.SetActive(active);
            if (type != RedDotType.Normal)
            {
                countText.text = count.ToString();
            }
            countText.gameObject.SetActive(type != RedDotType.Normal);
        }
        
        void OnDestroy()
        {
            RedDotSystem.Instance.UnRegisterRedDotChangeEvent(redKey, OnRedDotChangeEvent);
        }
    }
}