using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReverseLaneCheck : MonoBehaviour
{

    [SerializeField]
    private float targetRotation; // 지정할 기준 회전 값 (degrees)
    
    private bool isPlayerInTrigger = false; // 플레이어가 트리거 안에 있는지 여부
    private VehicleControlable player; // 플레이어 오브젝트의 참조

    private void Update()
    {
        // 플레이어가 트리거 안에 있을 때만 체크
        if (isPlayerInTrigger && player != null)
        {
            // 플레이어의 Y축 회전 값을 가져와서 역주행 체크
            float currentYRotation = player.gameObject.transform.eulerAngles.y; // 플레이어의 회전 값
            if (IsReversing(currentYRotation))
            {
                player.reverseLane=true;
            }
            else
            {
                player.reverseLane=false;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // 플레이어가 트리거에 들어갔을 때
        if (other.CompareTag("PlayerVehicle") && other.GetType() == typeof(BoxCollider)) // 탈것이 여러개의 콜라이더 중 박스 콜라이더로만 확인
        {
            isPlayerInTrigger = true;
            player = other.gameObject.GetComponent<VehicleControlable>(); 
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // 플레이어가 트리거에서 나갔을 때 참조 해제, 감지한 값들 초기화
        if (other.CompareTag("PlayerVehicle") && other.GetType() == typeof(BoxCollider))
        {
            if(player!=null)
            {
                player.reverseLane=false;
                isPlayerInTrigger = false;
                player = null; 
            }
            
        }
    }

    public bool IsReversing(float currentYRotation) //지정한 값보다 특정 각도 이상으로 넘어가면 역주행 감지
    {
        // 현재 회전 값과 지정한 회전 값의 차이를 계산
        float angleDifference = Mathf.DeltaAngle(currentYRotation, targetRotation);

        // 왼쪽 또는 오른쪽으로 90도 이상 넘어갔는지 확인
        return Mathf.Abs(angleDifference) >= 90f;
    }
}
