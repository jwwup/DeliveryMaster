using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarController : MovingObjectController
{
    private VehicleTrafficLight trafficLight; //감지한 신호등 참조할 변수

    protected override void AdjustSpeed() //신호등의 신호상황에 따라 속도 조절
    {
        if (trafficLight == null || trafficLight.currentState == VehicleTrafficLight.SignalState.Green)
        {
            currentSpeed = movementSpeed;
        }
        else if (trafficLight.currentState == VehicleTrafficLight.SignalState.Red || trafficLight.currentState == VehicleTrafficLight.SignalState.Yellow)
        {
            //빨간불이거나 노란불이면 속도 0으로
            currentSpeed = 0f;
        }
    }

    void OnTriggerEnter(Collider collider) //신호등의 트리거 감지
    {
        if (collider.CompareTag("TrafficSignal"))
        {
            trafficLight = collider.gameObject.GetComponent<VehicleTrafficLight>();
        }
    }

    void OnTriggerExit(Collider collider)
    {
        if (collider.CompareTag("TrafficSignal"))
        {
            trafficLight = null;
        }
    }
}