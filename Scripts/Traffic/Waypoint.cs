using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Waypoint : MonoBehaviour
{
    public Waypoint previousWaypoint; //이전 waypoint
    public Waypoint nextWaypoint; // 다음 waypoint

    [Range(0f,5f)]
    public float width = 2.7f;

    public List<Waypoint> branches = new List<Waypoint>(); //여러 갈래의 길을 위한 브랜치

    [Range(0f,1f)]
    public float branchRatio = 0.5f; //브랜치 이동 여부를 결정하는 변수

    public Vector3 GetPosition() //근처의 랜덤한 위치값 반환
    {
        Vector3 minBound = transform.position + transform.right*width/2f;
        Vector3 maxBound = transform.position - transform.right*width/2f;

        return Vector3.Lerp(minBound,maxBound,Random.Range(0f,1f));
    }


}
