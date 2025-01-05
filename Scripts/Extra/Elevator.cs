
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Elevator : MonoBehaviour
{
    public enum Floor  //이동 가능한 층들
    {
        first,  
        second, 
        third,  
        fourth, 
        fifth   
    }

    public float speed = 2.0f;
    private bool isMoving = false;
    private Vector3 targetLocalPosition; //이동할 목표 위치
    private Rigidbody rb;

    [SerializeField]
    private GameObject leftDoor;
    [SerializeField]
    private GameObject rightDoor;

    private Vector3 leftDoorOpenPosition;  //문이 열려 있을때의 위치
    private Vector3 rightDoorOpenPosition;
    private Vector3 doorClosedPosition = Vector3.zero;

    public float doorMoveDuration = 1.0f; // 문이 열리거나 닫히는 시간
    public float doorOpenTime = 3.0f; // 문이 열려있는 시간

    private Coroutine doorCoroutine = null;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Start()
    {
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody>();
        }
        rb.isKinematic = true; 

        // 문이 열릴 때의 위치 설정 (로컬 위치 기준)
        leftDoorOpenPosition = new Vector3(0, -0.015f, 0);
        rightDoorOpenPosition = new Vector3(0, 0.015f, 0);
    }

    void FixedUpdate()
    {
        if (isMoving)
        {
            // 타겟 위치를 월드 좌표계로 변환
            Vector3 targetWorldPosition = transform.parent.TransformPoint(targetLocalPosition);

            // 목표 위치까지 일정 속도로 이동
            Vector3 direction = (targetWorldPosition - rb.position).normalized;
            Vector3 movement = direction * speed * Time.fixedDeltaTime;

            // 목표 위치와의 남은 거리
            float distanceRemaining = Vector3.Distance(rb.position, targetWorldPosition);

            // 너무 작은 움직임 방지
            if (distanceRemaining < movement.magnitude)
            {
                rb.MovePosition(targetWorldPosition); // 목표 위치로 직접 이동
                isMoving = false; // 이동 종료

                if (doorCoroutine != null)
                {
                    StopCoroutine(doorCoroutine);
                }
                doorCoroutine = StartCoroutine(OpenAndCloseDoors()); // 도착 시 문을 열고 닫는 코루틴 시작

                
            }
            else
            {
                rb.MovePosition(rb.position + movement); //지정한 위치로 이동
            }
        }
    }

    public void MoveFloor(Floor targetFloor) // 목표 층수의 y값으로 목표 위치 지정
    {
        closeDoor();

        float targetY = transform.localPosition.y;  

        switch (targetFloor)
        {
            case Floor.first:
                targetY = -5.36f;
                break;
            case Floor.second:
                targetY = 0.07f;
                break;
            case Floor.third:
                targetY = 3.12f;
                break;
            case Floor.fourth:
                targetY = 6.17f;
                break;
            case Floor.fifth:
                targetY = 9.22f;
                break;
        }

        targetLocalPosition = new Vector3(transform.localPosition.x, targetY, transform.localPosition.z);
        isMoving = true;
    }
    
    public void openDoor()  //엘리베이터의 열기 버튼 눌렀을 경우 호출
    {
        if (!isMoving)
        {
            // 문을 열 때 현재 문을 닫는 코루틴이 실행 중이면 중지
            if (doorCoroutine != null)
            {
                StopCoroutine(doorCoroutine);
            }
            // 문을 열기 위한 코루틴 실행
            doorCoroutine = StartCoroutine(MoveDoors(leftDoorOpenPosition, rightDoorOpenPosition));
        }
    }

    public void closeDoor() //엘리베이터의 닫기 버튼 눌렀을 경우 호출
    {
        if (!isMoving)
        {
            // 문을 닫을 때 현재 문을 여는 코루틴이 실행 중이면 중지
            if (doorCoroutine != null)
            {
                StopCoroutine(doorCoroutine);
            }
            // 문을 닫기 위한 코루틴 실행
            doorCoroutine = StartCoroutine(MoveDoors(doorClosedPosition, doorClosedPosition));
        }
    }

    public void UpDown() //엘리베이터의 위, 아래 버튼 눌렀을 경우 호출
    {
        if (doorCoroutine != null)
        {
            StopCoroutine(doorCoroutine); //실행 중인 코루틴 종료 
        }
        doorCoroutine = StartCoroutine(OpenAndCloseDoors()); 
    }

    private IEnumerator MoveDoors(Vector3 leftDoorTargetPosition, Vector3 rightDoorTargetPosition) //오른쪽, 왼쪽 문의 목표 위치를 받아서 이동
    {
        Vector3 leftInitialPosition = leftDoor.transform.localPosition;
        Vector3 rightInitialPosition = rightDoor.transform.localPosition;
        float elapsedTime = 0f;

        while (elapsedTime < doorMoveDuration) //목표 위치로 이동시키기
        {
            leftDoor.transform.localPosition = Vector3.Lerp(leftInitialPosition, leftDoorTargetPosition, elapsedTime / doorMoveDuration);
            rightDoor.transform.localPosition = Vector3.Lerp(rightInitialPosition, rightDoorTargetPosition, elapsedTime / doorMoveDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // 최종 위치를 목표위치로 설정
        leftDoor.transform.localPosition = leftDoorTargetPosition;
        rightDoor.transform.localPosition = rightDoorTargetPosition;

        
        if (doorCoroutine != null)
        {
            doorCoroutine = null;
        }
    }

    private IEnumerator OpenAndCloseDoors()
    {
        // 문 열기
        yield return StartCoroutine(MoveDoors(leftDoorOpenPosition, rightDoorOpenPosition));

        // 일정 시간 대기
        yield return new WaitForSeconds(doorOpenTime);

        // 문 닫기
        yield return StartCoroutine(MoveDoors(doorClosedPosition, doorClosedPosition));
    }

}