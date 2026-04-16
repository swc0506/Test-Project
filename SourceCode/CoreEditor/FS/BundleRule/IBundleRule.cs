using System.Collections.Generic;
using UnityEngine;

namespace CoreEditor.FS
{
    public interface IBundleRule
    {
        /// <summary>
        /// 分析资源的bundle规则
        /// </summary>
        /// <param name="assets">资源对象</param>
        /// <param name="groupName">资源组名</param>
        /// <returns>资源path,abName</returns>
        Dictionary<string, string> Anasyle(IEnumerable<Object> assets, string groupName);
    }
}