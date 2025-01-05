using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeatherManager : MonoBehaviour
{

    [SerializeField]
    private GameObject rainParticle; //비가 내리는 효과를 주는 파티클
    [SerializeField]
    private WheelCollider[] wheels;
    private AudioSource audioSource;

    void Awake(){
        audioSource=GetComponent<AudioSource>();
    }
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(ControlWeather());
    }

    // 비 오는 날과 안 오는 날에 따라 바퀴의 마찰을 설정하는 메서드
    private void SetWheelFriction(bool isRaining)
    {
        foreach (WheelCollider wheel in wheels)
        {
            WheelFrictionCurve forwardFriction = wheel.forwardFriction; //앞뒤 방향의 마찰력
            WheelFrictionCurve sidewaysFriction = wheel.sidewaysFriction; // 좌우 방향의 마찰력

            if (isRaining) //비 왔을때의 wheel의 설정 변경
            {
                forwardFriction.extremumSlip = 0.5f; // 미끄러지지 않고 최적의 마찰을 유지하는 지점을 낮춘다
                forwardFriction.extremumValue = 1.5f; //최대 마찰력 낮춘다
                forwardFriction.asymptoteSlip = 1.5f; //타이어가 미끄러져 마찰력이 감소하기 시작하는 지점을 낮춘다
                forwardFriction.asymptoteValue = 4f; //타이어가 미끄러지면서 마찰력이 급격히 감소할 때의 마찰력 크기를 정의
                forwardFriction.stiffness = 1f; //타이어가 얼마나 강하게 지면에 붙어있는지

                sidewaysFriction.extremumSlip = 0.5f;
                sidewaysFriction.extremumValue = 1.5f;
                sidewaysFriction.asymptoteSlip = 1.5f;
                sidewaysFriction.asymptoteValue = 4f;
                sidewaysFriction.stiffness = 1f;
            }
            else
            {
                forwardFriction.extremumSlip = 1f;
                forwardFriction.extremumValue = 2f;
                forwardFriction.asymptoteSlip = 2f;
                forwardFriction.asymptoteValue = 4f;
                forwardFriction.stiffness = 1f;

                sidewaysFriction.extremumSlip = 1f;
                sidewaysFriction.extremumValue = 2f;
                sidewaysFriction.asymptoteSlip = 2f;
                sidewaysFriction.asymptoteValue = 4f;
                sidewaysFriction.stiffness = 1f;
            }

            wheel.forwardFriction = forwardFriction;
            wheel.sidewaysFriction = sidewaysFriction;
        }
    }

    private IEnumerator ControlWeather()
    {
        while (true)
        {
            //랜덤으로 비를 내리게 할지 결정
            int randomNum = Random.Range(1, 11);
            bool isRaining = randomNum % 3 == 0;

            rainParticle.SetActive(isRaining);
            SetWheelFriction(isRaining); //비가 오는지에 따라 바퀴의 설정 볌경

            if (isRaining)
            {
                if (!audioSource.isPlaying)
                {
                    audioSource.Play(); // 빗소리 재생
                }
            }
            else
            {
                if (audioSource.isPlaying)
                {
                    audioSource.Stop(); // 빗소리 정지
                }
            }

            // 날씨 변경 주기 설정 
            yield return new WaitForSeconds(15f);
        }
    }

}
