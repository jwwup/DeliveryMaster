using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VehicleTrafficLight : MonoBehaviour
{
    public enum SignalState //신호 상태 
    {
        Green,
        Yellow,
        Red
    }

    public SignalState currentState; //현재 신호 상태를 나타내는 변수

    //신호를 표시하는 오브젝트
    [SerializeField]
    private GameObject greenSign;
    [SerializeField]
    private GameObject yellowSign;
    [SerializeField]
    private GameObject redSign;

    [SerializeField]
    private float initialDelay = 0f; //초기 딜레이
    //각 신호별 시간
    [SerializeField]
    private float redTime;
    [SerializeField]
    private float greenTime;
    [SerializeField]
    private float yellowTime;

    [SerializeField]
    private SignalViolationCheck violationCheck; // 신호위반 체크하는 오브젝트

    void Start()
    {
        currentState = SignalState.Red;
        UpdateLights();
        StartCoroutine(ChangeSign());
    }

    private IEnumerator ChangeSign()  // 일정시간마다 신호를 바꾸는 코루틴
    {
        yield return new WaitForSeconds(initialDelay); //초기 딜레이 이후 실행(각 신호등 마다 다름)

        if (currentState == SignalState.Red)
        {
            currentState = SignalState.Green;
        }

        UpdateLights();
        //지정된 시간이 지나면 다음 신호를 바꿈
        while (true)
        {
            if (currentState == SignalState.Red)
            {
                yield return new WaitForSeconds(redTime);
                currentState = SignalState.Green;
            }
            else if (currentState == SignalState.Yellow)
            {
                yield return new WaitForSeconds(yellowTime);
                currentState = SignalState.Red;
            }
            else // green
            {
                yield return new WaitForSeconds(greenTime);
                currentState = SignalState.Yellow;
            }

            UpdateLights();
        }
    }

    private void UpdateLights() //신호 상태를 나타내는 오브젝트 상황에 맞게 활성화
    {
        // 신호등 상태에 따라 violationCheck 트리거 활성화/비활성화
        if (currentState == SignalState.Green)
        {
            greenSign.SetActive(true);
            yellowSign.SetActive(false);
            redSign.SetActive(false);
            violationCheck.DisableTrigger(); // 초록일 때 트리거 비활성화
        }
        else if (currentState == SignalState.Red)
        {
            greenSign.SetActive(false);
            yellowSign.SetActive(false);
            redSign.SetActive(true);
            violationCheck.EnableTrigger(); // 빨간색일 때 트리거 활성화
        }
        else // 노란색
        {
            greenSign.SetActive(false);
            yellowSign.SetActive(true);
            redSign.SetActive(false);
            violationCheck.DisableTrigger(); // 노란색일 때 트리거 비활성화
        }
    }
}