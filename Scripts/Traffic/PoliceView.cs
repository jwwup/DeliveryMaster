using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoliceView : MonoBehaviour
{
    [Range(0, 30)]
    [SerializeField] private float viewRange = 24f; // 시야 범위
    [Range(0, 360)]
    [SerializeField] private float viewAngle = 90f; // 시야 각도

    [SerializeField] private LayerMask targetMask; // 탐색 대상
    [SerializeField] private LayerMask obstacleMask; // 장애물 대상
    [SerializeField] private GameObject policeComment; //경찰 ui요소
    [SerializeField] private AudioClip whistleSound;

    private VehicleControlable detectedPlayer;
    private bool penaltyApplied = false;  //패널티를 짧은 시간내 계속 주지 않도록 체크하는 변수

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(CheckTarget());
    }

    IEnumerator CheckTarget()
    {
        

        while (true)
        {
            Collider[] cols = Physics.OverlapSphere(transform.position, viewRange, targetMask); //구 영역내의 콜라이더 감지

            foreach (var col in cols)
            {
                Vector3 direction = (col.transform.position - transform.position).normalized; //방향 계산

                // 시야 각도 내에 있는지 체크
                if (Vector3.Angle(transform.forward, direction) < viewAngle * 0.5f)
                {
                    float distance = Vector3.Distance(transform.position, col.transform.position);

                    // 장애물이 없는지 확인
                    if (!Physics.Raycast(transform.position, direction, distance, obstacleMask))
                    {
                        detectedPlayer = col.GetComponent<VehicleControlable>();

                        if (detectedPlayer != null)
                        {
                            // 패널티를 주기 전에 체크
                            if (!penaltyApplied)
                            {
                                CheckViolations();
                                penaltyApplied = true;
                                yield return new WaitForSeconds(4.7f); // 패널티를 반복해서 주지 않도록 대기
                                penaltyApplied = false;
                            }
                        }
                    }
                }
            }
            yield return new WaitForSeconds(0.1f);
        }
    }

    private void CheckViolations()  // 감지한 플레이어의 신호위반 체크
    {
        if (detectedPlayer.speeding || detectedPlayer.signalViolation || detectedPlayer.reverseLane)
        {
            detectedPlayer.driveCharacter.violationDetected();
            StartCoroutine(DisableSpeedingCommentAfterDelay());
            SoundManager.Instance.PlaySound(whistleSound);
        }
    }

    private IEnumerator DisableSpeedingCommentAfterDelay() // 신호위반 감지시 경찰 ui 활성화
    {
        policeComment.SetActive(true);
        yield return new WaitForSeconds(2.0f);
        policeComment.SetActive(false); 
    }

    // 씬뷰에서 확인하기 위하여
    void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Vector3 leftBoundary = Quaternion.Euler(0, -viewAngle * 0.5f, 0) * transform.forward;
        Vector3 rightBoundary = Quaternion.Euler(0, viewAngle * 0.5f, 0) * transform.forward;

        Gizmos.DrawLine(transform.position, transform.position + leftBoundary * viewRange);
        Gizmos.DrawLine(transform.position, transform.position + rightBoundary * viewRange);
        Gizmos.DrawLine(transform.position, transform.position + transform.forward * viewRange);
    }
}