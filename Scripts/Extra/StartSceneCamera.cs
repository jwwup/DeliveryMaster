using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.Playables;

public class StartSceneCamera : MonoBehaviour
{
    public Camera mainCamera;  // 기본 Unity Camera
    public CinemachineVirtualCamera virtualCameraToSwitch;  // 전환할 Cinemachine 가상 카메라
    public PlayableDirector timelineDirector;  // 타임라인 재생을 위한 PlayableDirector
    public PlayableDirector timelineDirector2;  // 두 번째 타임라인 

    private CinemachineBrain cameraBrain;  // 메인 카메라에서 CinemachineBrain 컴포넌트

    [SerializeField]
    private GameObject[] startSceneUI;


    void Start()
    {
        
        cameraBrain = mainCamera.GetComponent<CinemachineBrain>();

        // CinemachineBrain이 없다면 추가
        if (cameraBrain == null)
        {
            cameraBrain = mainCamera.gameObject.AddComponent<CinemachineBrain>();
        }


        if (timelineDirector != null)
        {
            timelineDirector.stopped += OnTimeline1Stopped; //타임라인이 끝났을때의 이벤트 등록
        }

        // 게임 시작 시 기본 카메라는 활성화되고, 가상 카메라는 비활성화
        mainCamera.gameObject.SetActive(true);
        virtualCameraToSwitch.gameObject.SetActive(false);
    }

    // 게임시작 버튼을 누르면 카메라를 전환하고 타임라인을 시작
    public void SwitchCameraAndPlayTimeline()
    {
        // 가상 카메라로 전환
        virtualCameraToSwitch.gameObject.SetActive(true);  // 가상 카메라 활성화

        // 타임라인 재생 시작
        if (timelineDirector != null)
        {
            timelineDirector.Play();  // 첫 번째 타임라인 시작
        }

        if (timelineDirector2 != null)
        {
            timelineDirector2.Play();  // 두 번째 타임라인 시작
        }

        foreach(GameObject g in startSceneUI){
            g.SetActive(false);
        }
    }

    private void OnTimeline1Stopped(PlayableDirector director)
    {
        
        LoadingSceneManager.Instance.LoadScene("PlayScene"); //타임라인이 끝난 경우 PlayScene으로 씬 전환
    }

}