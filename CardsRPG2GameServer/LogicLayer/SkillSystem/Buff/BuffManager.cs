using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuffManager : Singleton<BuffManager>, ILogicBehaviour
{
    private List<BuffLogic> buffList = new List<BuffLogic>();
    
    public void OnCreate()
    {
        
    }

    /// <summary>
    /// 创建一个buff
    /// </summary>
    /// <param name="buffId"></param>
    /// <param name="owner"></param>
    /// <param name="attacker"></param>
    /// <returns></returns>
    public BuffLogic CreateBuff(int buffId, LogicObject owner, LogicObject attacker)
    {
        Debugger.Log("创建一个buff id： " + buffId);
        BuffLogic buff = new BuffLogic(buffId, owner, attacker);
        buff.OnCreate();
        buffList.Add(buff);
        return buff; 
    }

    public void OnLogicFrameUpdate()
    {
        for (int i = 0; i < buffList.Count; i++)
        {
            buffList[i].OnLogicFrameUpdate();
        }

        for (int i = buffList.Count - 1; i >= 0; i--)
        {
            var buff = buffList[i];
            if (buff.objectState == LogicObjectState.Dead)
            {
                buff.OnDestroy();
                buffList.Remove(buff);
            }
        }
    }
    
    public void RemoveBuff(BuffLogic buff)
    {
        if (buffList.Contains(buff))
        {
            buffList.Remove(buff);
        }
    }

    public void DestroyBuff(BuffLogic buff)
    {
        buff.targetHero.RemoveBuff(buff);
    } 
        
    public void OnDestroy()
    {
        for (int i = 0; i < buffList.Count; i++)
        {
            buffList[i].OnDestroy();
        }
        buffList.Clear();
    }
}
