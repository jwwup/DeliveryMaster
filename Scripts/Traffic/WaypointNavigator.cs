using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaypointNavigator : MonoBehaviour
{

    MovingObjectController controller;
    public Waypoint currentWaypoint;   // 웨이포인트를 참조할 변수

    private void Awake()
    {
        controller = GetComponent<MovingObjectController>();

    }

    // Start is called before the first frame update
    void Start()
    {
        controller.SetDestination(currentWaypoint.GetPosition()); 
    }

    // Update is called once per frame
    void Update()
    {
        if(controller.reachedDestination)
        {
            bool shouldBranch = false;

            if(currentWaypoint.branches != null && currentWaypoint.branches.Count>0) //브랜치가 존재할 경우
            {
                shouldBranch = Random.Range(0f,1f) <= currentWaypoint.branchRatio ? true : false; //랜덤으로 브랜치로 이동할지를 결정
            }

            if(shouldBranch)
            {
                currentWaypoint = currentWaypoint.branches[Random.Range(0,currentWaypoint.branches.Count-1)]; //브랜치의 웨이포인트 참조
            }
            else
            {
                currentWaypoint = currentWaypoint.nextWaypoint; //현재 웨이 포인트에서 지정된 다음 웨이포인트 참조
            }
            
            controller.SetDestination(currentWaypoint.GetPosition()); //새롭게 참조한 웨이포인트를 목적지로 설정
        }
    }
}
