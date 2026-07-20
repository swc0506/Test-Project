using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectHeroWindow : MonoBehaviour
{
    public GameObject opHeroObjects;
    public Camera camera3D;
    public GameObject mCurSelectHero;
    public Transform[] seatTransArr;

    private bool mIsPress;
    
    public void OnEnable()
    {
        opHeroObjects.SetActive(true);
        for (int i = 0; i < seatTransArr.Length; i++)
        {
            GameObject heroObj = ResourcesManager.Instance.LoadObject(AssetPathConfig.HERO + (100 + i + 1), seatTransArr[i], true, true);
            heroObj.name = heroObj.name.Replace("(Clone)", "");
        }
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            mIsPress = true;
            //从鼠标位置发射射线
            Ray ray = camera3D.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                Debugger.Log("点击了英雄: " + hit.transform.name);
                // ==1 说明移动位置 ==2 交换位置
                if (mCurSelectHero == null && hit.collider != null && hit.collider.transform.parent.childCount >= 2)
                {
                    mCurSelectHero = hit.collider.transform.parent.GetChild(1).gameObject;
                }
            }
        }

        //如果鼠标一直处于按下状态
        if (Input.GetMouseButton(0))
        {
            if (mIsPress && mCurSelectHero != null)
            {
                Vector3 pos = camera3D.ScreenToWorldPoint(Input.mousePosition);
                pos.y = 1;
                pos.z += 12.6f;
                pos.z *= 3;
                mCurSelectHero.transform.position = pos;
            }
        }

        //交换位置
        if (Input.GetMouseButtonUp(0))
        {
            mIsPress = false;
            if (mCurSelectHero != null)
            {
                for (int i = 0; i < seatTransArr.Length; i++)
                {
                    if (Vector3.Distance(mCurSelectHero.transform.position, seatTransArr[i].position) <= 3)
                    {
                        Transform parent = seatTransArr[i];
                        // 如果位置上已经有英雄，就交换位置
                        if (parent.childCount >= 2)
                        {
                            Transform targetHero = parent.GetChild(1);
                            targetHero.transform.SetParent(mCurSelectHero.transform.parent);
                            targetHero.transform.localPosition = Vector3.zero;
                        }
                        mCurSelectHero.transform.SetParent(parent); 
                        break;
                    }
                }
                mCurSelectHero.transform.localPosition = Vector3.zero;
            }
            mCurSelectHero = null;
        }
    }
}
