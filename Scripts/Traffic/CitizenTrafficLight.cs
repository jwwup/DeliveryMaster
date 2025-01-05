using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CitizenTrafficLight : MonoBehaviour
{
    public enum SignalState //신호 상태
    {
        Green,
        Red
    }

    public SignalState currentState; //신호등의 현재 상태 

    [SerializeField]
    private float initialDelay = 0f; //초기 딜레이,신호등마다 다르게 설정

    //신호등의 상태를 나타내는 오브젝트들 
    [SerializeField]
    private GameObject greenMainSign;
    [SerializeField]
    private GameObject[] greenProgressSign; 
    [SerializeField]
    private GameObject redMainSign;
    [SerializeField]
    private GameObject[] redProgressSign;

    //각 신호의 활성화 시간 
    [SerializeField]
    private float redTime;
    [SerializeField]
    private float greenTime;
    //신호의 부분 시간(신호의 진행 상태를 나타낼때 필요)
    private float greenPartialTime;
    private float redPartialTime;

    private float timeElapsed; 

    // Start is called before the first frame update
    void Start()
    {
        currentState = SignalState.Red;
        redPartialTime = redTime / (float)redProgressSign.Length;
        greenPartialTime = greenTime / (float)greenProgressSign.Length;

        // 신호등 변경 시작
        StartCoroutine(ChangeSign());
    }

    private IEnumerator ChangeSign() //각 지정된 시간마다 신호를 바꾼다
    {
        yield return new WaitForSeconds(initialDelay); //초기 딜레이 값만큼 대기 했다가 실행
        currentState = SignalState.Green;
        UpdateLights();

        while (true)
        {
            timeElapsed = 0f; // 시간 초기화

            if (currentState == SignalState.Red)
            {
                yield return new WaitForSeconds(redTime);
                currentState = SignalState.Green;
            }
            else // green
            {
                while (timeElapsed < greenTime)
                {
                    timeElapsed += Time.deltaTime; // 경과 시간 업데이트

                    // 현재 진행 신호를 끄는 로직
                    int signalIndex = Mathf.FloorToInt(timeElapsed / greenPartialTime);
                    if (signalIndex < greenProgressSign.Length)
                    {
                        greenProgressSign[signalIndex].SetActive(false); // 하나씩 꺼지기
                    }

                    yield return null; 
                }
                currentState = SignalState.Red; 
            }

            UpdateLights();
        }
    }

    private void UpdateLights() //신호등의 상태를 나타내는 오브젝트를 상황에 맞게 활성화
    {
        if (currentState == SignalState.Green)
        {
            greenMainSign.SetActive(true);
            redMainSign.SetActive(false);
            for(int i=0; i<greenProgressSign.Length;i++)
            {
                greenProgressSign[i].SetActive(true);
            }

        }
        else if (currentState == SignalState.Red)
        {
            greenMainSign.SetActive(false);
            redMainSign.SetActive(true);
            for(int i=0; i<greenProgressSign.Length;i++)
            {
                greenProgressSign[i].SetActive(false);
            }
        }
    }
}