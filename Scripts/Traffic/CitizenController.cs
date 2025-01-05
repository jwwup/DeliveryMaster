using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CitizenController : MovingObjectController
{
     private CitizenTrafficLight trafficLight; //감지한 신호등 참조할 변수

    protected override void AdjustSpeed() //신호등의 신호상황에 따라 속도 조절
    {
        if (trafficLight == null || trafficLight.currentState == CitizenTrafficLight.SignalState.Green)
        {
            currentSpeed = movementSpeed;
        }
        else if (trafficLight.currentState == CitizenTrafficLight.SignalState.Red)
        {
            // 빨간 불일 경우 속도 0
            currentSpeed = 0f;
        }
    }

    void OnTriggerEnter(Collider collider) //신호등의 트리거 감지
    {
        if (collider.CompareTag("CitizenSignal"))
        {
            trafficLight = collider.gameObject.GetComponent<CitizenTrafficLight>();
        }
    }

    void OnTriggerExit(Collider collider)
    {
        if (collider.CompareTag("CitizenSignal"))
        {
            trafficLight = null;
        }
    }
}
