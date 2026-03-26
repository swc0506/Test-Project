using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerObject : MonoBehaviour
{
    void Awake()
    {
        InputMgr.GetInstance().StartOrEndCheck(true);
        AddListener();
    }

    void Start()
    {
        
    }

    void Update()
    {
        
    }
    
    void OnDestroy()
    {
        RemoveListener();
    }
    
    private void CheckX(float value)
    {
    }
    
    private void CheckY(float value)
    {
        
    }
    
    private void CheckKeyDown(KeyCode key)
    {
        switch (key)
        {
            case KeyCode.J:
                break;
            case KeyCode.K:
                break;
            case KeyCode.L:
                break;
            case KeyCode.Space:
                break;
        }
    }
    
    private void AddListener()
    {
        EventCenter.GetInstance().AddEventListener<float>("Horizontal", CheckX);
        EventCenter.GetInstance().AddEventListener<float>("Vertical", CheckY);
        EventCenter.GetInstance().AddEventListener<KeyCode>("SomeKeyDown", CheckKeyDown);
    }
    
    private void RemoveListener()
    {
        EventCenter.GetInstance().RemoveEventListener<float>("Horizontal", CheckX);
        EventCenter.GetInstance().RemoveEventListener<float>("Vertical", CheckY);
        EventCenter.GetInstance().RemoveEventListener<KeyCode>("SomeKeyDown", CheckKeyDown);
    }
}
