using System;
using System.Collections;
using UnityEngine;
using System.Collections.Generic;
public class ObjectDragger : MonoBehaviour
{
    private Camera mainCamera;
    private GameObject selectedObject;
    private Vector3 offset;
    private float mouseZCoord;
    
    [Header("是否锁定到Z轴平面")]
    public bool lockToZAxisPlane = true;           // 是否锁定到特定平面移动
    public float planeYPosition = 0f;         // 锁定平面的Y坐标
    [Header("拖动平滑度")]
    public float dragSmoothness = 20f;         // 拖动平滑度
    
    [Header("交换距离阈值")]
    public float swapDistanceThreshold = 1.5f;
    [Header("交换动画时长")]
    public float swapDuration = 0.3f;
    public AnimationCurve swapCurve;
    
    public List<GameObject> draggableObjects = new List<GameObject>();
    private Dictionary<int,Vector3> objectsInitPosDic = new Dictionary<int,Vector3>();
    private GameObject potentialSwapTarget;
    private Vector3 velocity = Vector3.zero;
    /// <summary>
    /// 交换位置回调  x座位与x座位进行交换
    /// </summary>
    private Action<int,int>  onSwapSeatCallBack;

    void Start()
    {
        mainCamera = Camera.main;
        UpdateHeroOriginPos();
        if (swapCurve == null) swapCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    }

  

    void Update()
    {
        HandleMouseInput();
    }
    /// <summary>
    /// 动态更新拖拽列表
    /// </summary>
    /// <param name="draggableObjectList"></param>
    public void UpdateDraggableList(List<GameObject> draggableObjectList,Action<int,int> swapSeatCallBack)
    {
        onSwapSeatCallBack = swapSeatCallBack;
        foreach (GameObject item in draggableObjectList)
        {
            BoxCollider collider = item.GetComponent<BoxCollider>();
            if (collider==null)
                item.AddComponent<BoxCollider>();
        }

        this.draggableObjects = draggableObjectList;
        UpdateHeroOriginPos();
    }

    void HandleMouseInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            
            if (Physics.Raycast(ray, out hit))
            {
                if (draggableObjects.Contains(hit.collider.gameObject))
                {
                    selectedObject = hit.collider.gameObject;
                    mouseZCoord = mainCamera.WorldToScreenPoint(selectedObject.transform.position).z;
                    
                    // 计算基于摄像机旋转的偏移量
                    if (lockToZAxisPlane)
                    {
                        offset = selectedObject.transform.position - GetMouseWorldPosOnPlane();
                    }
                    else
                    {
                        offset = selectedObject.transform.position - GetMouseWorldPos();
                    }
                    
                    HighlightObject(selectedObject, true);
                }
            }
        }

        if (selectedObject && Input.GetMouseButton(0))
        {
            // 平滑移动物体
            Vector3 targetPosition = lockToZAxisPlane ? 
                GetMouseWorldPosOnPlane() + offset : 
                GetMouseWorldPos() + offset;
                
            selectedObject.transform.position = Vector3.SmoothDamp(
                selectedObject.transform.position, 
                targetPosition, 
                ref velocity, 
                1f/dragSmoothness);
            
            CheckForSwapTarget();
        }

        if (selectedObject && Input.GetMouseButtonUp(0))
        {
            if (potentialSwapTarget != null)
            {
                StartCoroutine(SwapPositions(selectedObject, potentialSwapTarget));
            }
            else
            {
                selectedObject.transform.position = objectsInitPosDic[selectedObject.GetInstanceID()];
            }
            
            HighlightObject(selectedObject, false);
            selectedObject = null;
            potentialSwapTarget = null;
        }
    }

    Vector3 GetMouseWorldPosOnPlane()
    {
        Plane plane = new Plane(Vector3.up, planeYPosition);
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        float distance;
        if (plane.Raycast(ray, out distance))
        {
            return ray.GetPoint(distance);
        }
        return Vector3.zero;
    }

    Vector3 GetMouseWorldPos()
    {
        Vector3 mousePoint = Input.mousePosition;
        mousePoint.z = mouseZCoord;
        return mainCamera.ScreenToWorldPoint(mousePoint);
    }

    void CheckForSwapTarget()
    {
        potentialSwapTarget = null;
        float closestDistance = swapDistanceThreshold;
        
        foreach (GameObject obj in draggableObjects)
        {
            if (obj != selectedObject)
            {
                float distance = Vector3.Distance(selectedObject.transform.position, obj.transform.position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    potentialSwapTarget = obj;
                }
            }
        }
        
        // 高亮显示潜在交换目标
        HighlightPotentialTarget(potentialSwapTarget);
    }
    /// <summary>
    /// 交换角色位置
    /// </summary>
    /// <param name="objA"></param>
    /// <param name="objB"></param>
    /// <returns></returns>
    IEnumerator SwapPositions(GameObject objA, GameObject objB)
    {
        int seatA = int.Parse(objA.transform.parent.name);
        int seatB = int.Parse(objB.transform.parent.name);
        //触发交换完成回调 逻辑先行，表现后行
        onSwapSeatCallBack?.Invoke(seatA, seatB);
        
        Vector3 startPosA = objA.transform.position;
        Vector3 startPosB = objB.transform.position;
        //获取英雄初始化位置
        Vector3 targetPosA = objectsInitPosDic[objA.GetHashCode()];
        Vector3 targetPosB = objectsInitPosDic[objB.GetHashCode()];
        
        //交换父物体(座位)
        Transform objAParent= objA.transform.parent;
        objA.transform.SetParent(objB.transform.parent);
        objB.transform.SetParent(objAParent);
        
        float elapsedTime = 0f;
        
        while (elapsedTime < swapDuration)
        {
            float t = swapCurve.Evaluate(elapsedTime / swapDuration);
            //执行交换动画
            objA.transform.position = Vector3.Lerp(startPosA, targetPosB, t);
            objB.transform.position = Vector3.Lerp(startPosB, targetPosA, t);
    
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        
        // 确保最终位置准确
        objA.transform.position = targetPosB;
        objB.transform.position = targetPosA;
        //更新英雄原始位置
        UpdateHeroOriginPos();
        //交换完成，重置所有对象的选中状态
        HighlightPotentialTarget(null);
        
    }

    void HighlightObject(GameObject obj, bool highlight)
    {
        if (obj == null) return;
        
        Renderer renderer = obj.GetComponent<Renderer>();
        if (renderer != null)
        {
            if (highlight)
                renderer.material.color = Color.yellow;
            else
                renderer.material.color = Color.white;
        }
    }

    void HighlightPotentialTarget(GameObject obj)
    {
        // 重置所有对象的高亮
        foreach (GameObject draggable in draggableObjects)
        {
            if (draggable != selectedObject)
            {
                Renderer r = draggable.GetComponent<Renderer>();
                if (r != null) r.material.color = Color.white;
            }
        }
        
        // 高亮潜在目标
        if (obj != null)
        {
            Renderer renderer = obj.GetComponent<Renderer>();
            if (renderer != null) renderer.material.color = Color.green;
        }
    }
    void UpdateHeroOriginPos()
    {
        objectsInitPosDic.Clear();
        //更新英雄原始位置
        foreach (GameObject obj in draggableObjects)
        {
            objectsInitPosDic.Add(obj.GetHashCode(),obj.transform.position);
        }
    }
}