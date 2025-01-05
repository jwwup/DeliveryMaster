using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SignalViolationCheck : MonoBehaviour
{
    private Collider triggerCollider; 

    private void Awake()
    {
        
        triggerCollider = GetComponent<Collider>();
        if (triggerCollider == null)
        {
            Debug.LogWarning("No collider found on this GameObject. Make sure it has a collider component.");
        }
    }

    void OnTriggerEnter(Collider collider) //빨간불인 경우에만 트리거를 활성화하여 플레이어를 감지하면 신호위반 상태로 설정
    {
        // 차량이 신호등의 트리거에 들어올 때
        if(collider.CompareTag("PlayerVehicle"))
        {
            VehicleControlable vehicle = collider.gameObject.GetComponent<VehicleControlable>();
            if (vehicle != null)
            {
                vehicle.signalViolation = true; // 신호 위반 상태 설정
            }
        }
    }

    void OnTriggerExit(Collider collider)
    {
        // 차량이 신호등의 트리거에서 나갈 때
        if(collider.CompareTag("PlayerVehicle"))
        {
            VehicleControlable vehicle = collider.gameObject.GetComponent<VehicleControlable>();
            if (vehicle != null)
            {
                vehicle.signalViolation = false; // 신호 위반 상태 해제
            }
        }
    }

    // 트리거 콜라이더를 활성화하는 함수
    public void EnableTrigger()
    {
        if (triggerCollider != null)
        {
            triggerCollider.enabled = true; // 콜라이더 활성화
        }
    }

    // 트리거 콜라이더를 비활성화하는 함수
    public void DisableTrigger()
    {
        if (triggerCollider != null)
        {
            triggerCollider.enabled = false; // 콜라이더 비활성화
        }
    }
}