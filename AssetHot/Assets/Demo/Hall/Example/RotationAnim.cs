using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotationAnim : MonoBehaviour
{
    public float rotationSpeed = 10f;
    
    void Start()
    {
        
    }

    void Update()
    {
        Vector3 angle = transform.localEulerAngles;
        angle.z += rotationSpeed * Time.deltaTime;
        transform.localEulerAngles = angle;
    }
}
