using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraRestriction : MonoBehaviour
{
    [SerializeField] private new CameraController camera; 
        
    [SerializeField] private bool fixX;
    [SerializeField] private bool fixY;
    [SerializeField] private float fixedX;
    [SerializeField] private float fixedY;
    
    void Update()
    {
        Vector3 targetPosition = camera.targetPosition;
        if (fixX) targetPosition.x = fixedX;
        if (fixY) targetPosition.y = fixedY;
        transform.position = targetPosition;
    }
}
