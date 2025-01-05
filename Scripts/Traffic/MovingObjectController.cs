using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MovingObjectController : MonoBehaviour
{
    protected Vector3 destination; //목적지
    public bool reachedDestination; //목적지 도착 유무 확인하는 변수
    [SerializeField]
    protected float stopDistance; //목척지 앞에서 멈출 거리
    [SerializeField]
    protected float rotationSpeed = 5f;
    [SerializeField]
    protected float movementSpeed = 7f;
    [SerializeField]
    protected float currentSpeed;

    //다른 오브젝트와의 충돌을 피하기 위한 레이캐스트 설정
    [SerializeField]
    protected float rayDistance = 10f;
    [SerializeField]
    protected int rayCount = 5;
    [SerializeField]
    protected float raySpreadAngle = 30f;
    [SerializeField]
    protected Vector3 rayStartPoint = new Vector3(0, 1f, 0);
    [SerializeField]
    protected LayerMask layer; //감지할 레이어
    protected bool obstacleDetected;

    // 자식 클래스가 구현해야 할 메서드 (신호등 로직)
    protected abstract void AdjustSpeed();

    // Start is called before the first frame update
    protected virtual void Start()
    {
        currentSpeed = movementSpeed;
        StartCoroutine(CheckObstacles());
    }

    // Update is called once per frame
    protected virtual void Update() //목표 지점으로 이동하도록, WaypointNavigator클래스에서 도착하면 계속해서 새로운 목적지 할당
    {
        if (transform.position != destination)
        {
            Vector3 destinationDirection = destination - transform.position;
            destinationDirection.y = 0;

            float destinationDistance = destinationDirection.magnitude;

            // 신호등 상태에 따른 속도 조정
            AdjustSpeed();

            // 장애물 감지에 따른 속도 조정
            if (obstacleDetected)
            {
                currentSpeed = 0f;
            }
            else if (destinationDistance >= stopDistance)
            {
                reachedDestination = false;
                Quaternion targetRotation = Quaternion.LookRotation(destinationDirection);
                transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
                transform.Translate(Vector3.forward * currentSpeed * Time.deltaTime);
            }
            else
            {
                reachedDestination = true;
            }
        }
    }

    // 레이캐스트를 이용하여 장애물 감지하는 코루틴
    protected IEnumerator CheckObstacles()
    {
        while (true)
        {
            obstacleDetected = false;

            for (int i = 0; i < rayCount; i++)
            {
                 // Ray의 각도를 계산 (ray의 개수에 따라 일정한 간격으로 분산)
                float angle = (i / (float)(rayCount - 1)) * raySpreadAngle - raySpreadAngle / 2;
                Quaternion rotation = Quaternion.Euler(0, angle, 0); // 각도에 따라 Ray 방향을 회전
                Vector3 rayDirection = rotation * transform.forward; // 회전된 방향 벡터를 구함(정면 기준으로)
                Vector3 rayOrigin = transform.position + transform.TransformDirection(rayStartPoint); // Ray의 시작 지점을 설정

                if (Physics.Raycast(rayOrigin, rayDirection, out RaycastHit hit, rayDistance, layer))
                {
                    obstacleDetected = true;
                    break;
                }
            }
            yield return null;
        }
    }

    //목적지 설정
    public void SetDestination(Vector3 destination)
    {
        this.destination = destination;
        reachedDestination = false;
    }
    
    //씬뷰에서 ray를 확인할 수 있도록
    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        for (int i = 0; i < rayCount; i++)
        {
            float angle = (i / (float)(rayCount - 1)) * raySpreadAngle - raySpreadAngle / 2;
            Quaternion rotation = Quaternion.Euler(0, angle, 0);
            Vector3 rayDirection = rotation * transform.forward;
            Vector3 rayOrigin = transform.position + transform.TransformDirection(rayStartPoint);

            Gizmos.DrawLine(rayOrigin, rayOrigin + rayDirection * rayDistance);
        }
    }
}
