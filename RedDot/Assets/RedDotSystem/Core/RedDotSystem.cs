using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RedDot.System
{
    public enum RedDotType
    {
        Normal,//普通红点
        NodeNum,//节点红点
        DataNum,//数据红点
    }
    
    public class RedDotSystem
    {
        private static RedDotSystem _instance;
        public static RedDotSystem Instance
        {
            get
            {
                _instance ??= new RedDotSystem();
                return _instance;
            }
        }
        
        private Dictionary<RedDotDefine, RedDotTreeNode> _redDotLogicDic = new Dictionary<RedDotDefine, RedDotTreeNode>();
        
        /// <summary>
        /// 初始化红点树
        /// </summary>
        /// <param name="nodeList"></param>
        public void InitRedDotTree(List<RedDotTreeNode> nodeList)
        {
            foreach (var node in nodeList)
            {
                _redDotLogicDic.Add(node.node, node);
            }
        }
        
        public void UpdateRedDotState(RedDotDefine redKey)
        {
            if (redKey == RedDotDefine.None)
                return;
            if (_redDotLogicDic.TryGetValue(redKey, out RedDotTreeNode redDotNode))
            {
                redDotNode.RefreshActive();
                UpdateRedDotState(redDotNode.parent);
            }
        }

        /// <summary>
        /// 注册红点变化事件
        /// </summary>
        /// <param name="node"></param>
        /// <param name="onRedDotChange"></param>
        public void RegisterRedDotChangeEvent(RedDotDefine node, Action<RedDotType, bool, int> onRedDotChange)
        {
            if (_redDotLogicDic.TryGetValue(node, out RedDotTreeNode redDotNode))
            {
                redDotNode.onRedDotChange += onRedDotChange;
            }
            else
            {
                Debug.LogError($"红点节点{node}未注册");
            }
        }
        
        /// <summary>
        /// 取消红点变化事件
        /// </summary>
        /// <param name="node"></param>
        /// <param name="onRedDotChange"></param>
        public void UnRegisterRedDotChangeEvent(RedDotDefine node, Action<RedDotType, bool, int> onRedDotChange)
        {
            if (_redDotLogicDic.TryGetValue(node, out RedDotTreeNode redDotNode))
            {
                redDotNode.onRedDotChange -= onRedDotChange;
            }
            else
            {
                Debug.LogError($"红点节点{node}未注册");
            }
        }
        
        public int GetChildNodeRedDotCount(RedDotDefine redKey)
        {
            int childRedDotCount = 0;
            ComputeRedDotCount(redKey, ref childRedDotCount);
            return childRedDotCount;
        }
        
        public void ComputeRedDotCount(RedDotDefine redKey, ref int childRedDotCount)
        {
            foreach (var item in _redDotLogicDic.Values)
            {
                if (item.parent == redKey)
                {
                    item.RefreshActive();
                    if (item.redDotActive)
                    {
                        childRedDotCount += item.redDotCount;
                        //处理节点数量类型的嵌套树结构。跳过其所有子节点,读取总结点个数
                        if (item.type == RedDotType.NodeNum)
                        {
                            continue;
                        }
                        //处理红点数据个数类型的嵌套数结构。跳过其所有子节点,读取总结点个数
                        if (item.type!= RedDotType.DataNum)
                        {
                            ComputeRedDotCount(item.node,ref childRedDotCount);
                        }
                    }
                }
            }
        }
    }
}