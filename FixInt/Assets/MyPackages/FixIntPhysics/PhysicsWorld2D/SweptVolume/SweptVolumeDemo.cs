using System;
using System.Collections;
using System.Collections.Generic;
using My.Physics2D;
using UnityEngine;
using UnityEngine.InputSystem;
using ZM.FixIntMath;

public class SweptVolumeDemo : MonoBehaviour
{
    public Transform prefabParent;

    public GameObject boxPrefab;
    
    private FixIntBoxCollider2D mFixIntCollider2D;
    
    private void Update()
    {
        if (Keyboard.current.qKey.wasPressedThisFrame)
        {
            RectTransform rectTrans = Instantiate(boxPrefab, prefabParent).transform as RectTransform;
            //创建子弹
            Bullet bullet = rectTrans.GetComponent<Bullet>();
            bullet.Init(new FixIntVector2(UnityEngine.Random.Range(-900, 900), -450));
            rectTrans.gameObject.SetActive(true);
        }
    }
}
