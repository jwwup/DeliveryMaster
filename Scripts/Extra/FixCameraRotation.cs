using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FixCameraRotation : MonoBehaviour
{
    private float initialZRotation;
    
    void Awake()
    {
        initialZRotation = transform.rotation.eulerAngles.z;
    }

    void FixedUpdate()
    {
        // 현재 오브젝트의 회전값을 가져옴
        Quaternion currentRotation = transform.rotation;
        
        // X, Y 회전값은 그대로 두고 Z 회전값만 초기 값으로 고정
        currentRotation = Quaternion.Euler(currentRotation.eulerAngles.x, currentRotation.eulerAngles.y, initialZRotation);
        
        // 고정된 회전값을 다시 오브젝트에 적용
        transform.rotation = currentRotation;

    }
}
