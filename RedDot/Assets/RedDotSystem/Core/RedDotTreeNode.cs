using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RedDot.System
{
    public class RedDotTreeNode
    {
        public RedDotType type;
        public RedDotDefine parent;
        public RedDotDefine node;
        
        public bool redDotActive;
        public int redDotCount;

        public Action<RedDotTreeNode> logicHander;
        
        public Action<RedDotType, bool, int> onRedDotChange;
        
        /// <summary>
        /// 刷新红点状态
        /// </summary>
        /// <returns></returns>
        public virtual bool RefreshActive()
        {
            redDotCount = 0;
            if (type == RedDotType.NodeNum)
            {
                //获取子节点红点数量
                redDotCount = RedDotSystem.Instance.GetChildNodeRedDotCount(node);
                redDotActive = redDotCount > 0;
            }
            else
            {
                redDotCount = RefreshCount();
            }
            
            logicHander?.Invoke(this);
            
            if (type == RedDotType.DataNum)
                redDotActive = redDotCount > 0;
            
            onRedDotChange?.Invoke(type, redDotActive, redDotCount);
            return redDotActive;
        }
        
        /// <summary>
        /// 刷新红点数量
        /// </summary>
        /// <returns></returns>
        public virtual int RefreshCount()
        {
            return 1;
        }
    }
}

