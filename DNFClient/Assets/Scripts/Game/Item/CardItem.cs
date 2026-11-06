/*----------------------------------------------------------------------------
* Title: #Title#
*
* Author: 铸梦
*
* Date: #CreateTime#
*
* Description:
*
* Remarks: QQ:975659933 邮箱：zhumengxyedu@163.com
*
* 教学网站：
----------------------------------------------------------------------------*/
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEngine;
using ZMGC.Battle;

public class CardItem : MonoBehaviour
{
    public GameObject cradRwardObj;
    private int mindex;
    
    public void Initlizate(int index)
    {
        mindex = index;
        UIEventControl.AddEvent(UIEventEnum.ReceiveCardReward, OnReceiveReward);
    }
    public void OnReceiveReward(object data)
    {
        if (mindex == (int)data)
        {
            transform.DOLocalRotate(new Vector3(0, 90, 0), 0.3f).OnComplete(() => {
            cradRwardObj.gameObject.SetActive(true);
            transform.DOLocalRotate(new Vector3(0, 180, 0), 0.3f);
            });
        }
    }

    public void OnButtonClick()
    {
        if (UIModule.Instance.GetWindow<DungeonRewardWindow>().isReceiveReward == false)
        {
            //向服务端发送翻牌子的消息
            BattleWorld.GetExitsMsgMgr<BattleMsgMgr>().SendReceiveEndRewardMessage(mindex
                ,BattleWorld.UserData.AccountId,BattleWorld.GetExitsDataMgr<BattleDataMgr>().battleId);
            UIModule.Instance.GetWindow<DungeonRewardWindow>().isReceiveReward = true;
        }
  
    }

    public void OnRelease()
    {
        UIEventControl.RemoveEvent(UIEventEnum.ReceiveCardReward, OnReceiveReward);
    }
}
