/*--------------------------------------------------------------------------------------
* Title: 业务逻辑脚本自动生成工具
* Author: 铸梦xy
* Date:2024/3/3 12:13:55
* Description:业务逻辑层,主要负责游戏的业务逻辑处理
* Modify:
* 注意:以下文件为自动生成，强制再次生成将会覆盖
----------------------------------------------------------------------------------------*/
using System.Collections.Generic;
using UnityEngine;
using ZM.AssetFrameWork;
using FixIntPhysics;
using FixMath;
namespace ZMGC.Battle
{
    public class MonsterLogicCtrl : ILogicBehaviour
    {
        /// <summary>
        /// 场景中的怪物列表
        /// </summary>
        public List<MonsterLogic> monsterList = new List<MonsterLogic>();
        //怪物位置列表
        public List<Vector3> monsterPosList = new List<Vector3>() { 
            new Vector3(0,0,0),
            new Vector3(-2,0,0),
            new Vector3(-3,0,2),
        };
        //怪物生成id数组
        public int[] monsteridArr = new int[] {20001, 20005,30001};

        public void OnCreate()
        {
            UnityEngine.Debug.Log("MonsterLogicCtrl OnCreate");
        }
        /// <summary>
        /// 初始化场景中怪物
        /// </summary>
        public void InitMonster()
        {
            int index = 0;
            foreach (var id in monsteridArr)
            {
                FixIntVector3 initPos = new FixIntVector3(monsterPosList[index]);
                //生成怪物到场景中
                GameObject monsterObj = ZMAsset.Instantiate(AssetPathConfig.GAME_PREFABS_MONSTER + id, null);

                //处理怪物碰撞数据
                BoxColliderGizmo boxInfo= monsterObj.GetComponent<BoxColliderGizmo>();
                boxInfo.enabled = false;
                //创建定点数碰撞体
                FixIntBoxCollider monsterBox = new FixIntBoxCollider(boxInfo.mSize,boxInfo.mConter);
                monsterBox.SetBoxData(boxInfo.mConter, boxInfo.mSize);
                monsterBox.UpdateColliderInfo(initPos, new FixIntVector3(boxInfo.mSize));

                //创建怪物和英雄的逻辑层与渲染层
                MonsterRedner monsterRedner= monsterObj.GetComponent<MonsterRedner>();
                MonsterLogic monsterLogic = new MonsterLogic(id, monsterRedner, monsterBox, initPos);
                monsterRedner.SetLoigcObject(monsterLogic);

                monsterLogic.OnCreate();
                monsterRedner.OnCreate();

                monsterList.Add(monsterLogic);
                index++;
                monsterLogic.OnDeathCallBack += OnMonsterDeathCallback;
            }
  
        }
        /// <summary>
        /// 怪物死亡回调
        /// </summary>
        public void OnMonsterDeathCallback()
        {
            foreach (var monster in monsterList)
            {
                if (monster.ObjectState != LogicObjectState.Death)
                {
                    return;
                }
            }
            //所有的怪物已经死亡
            //向服务端发送战斗结束的消息
            BattleWorld.GetExitsLogicCtrl<BattleLogicCtrl>().LocalBattleEnd();
        }
        public void OnLogicFrameUpdate()
        {
            for (int i = 0; i < monsterList.Count; i++)
            {
                monsterList[i].OnLogicFrameUpdate();
            }
        }
        public void OnDestroy()
        {
            foreach (var monster in monsterList)
            {
                monster.OnDestroy();
            }
            monsterList.Clear();
        }

    }
}
