using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    public void Awake()
    {
        Instance = this;
    }


    public  GameObject loginWindow;
    public  GameObject hallWindow;
    public  GameObject battleWindow;

}
