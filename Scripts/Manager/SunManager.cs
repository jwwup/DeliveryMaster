using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SunManager : MonoBehaviour
{
    [SerializeField]
    private float day = 1.3f; //낮의 속도 
    [SerializeField]
    private float night = 6f; //밤의 속도

    [SerializeField]
    private CompletedScrollViewController completeScrollview;

    private bool isDay = true; //현재 낮인지 밤인지 체크하는 변수
  
    // Update is called once per frame
    void Update()
    {
        float speed = night;
        //태양이 일정 높이 이상일 경우 낮으로 설정
        if (transform.position.y >= -1.4)
        {
            speed = day;
            if (!isDay)
            {
                isDay = true;
                completeScrollview.deleteAllMessages(); //하루가 지날때마다 스크롤뷰의 메세지 비우기
            }
        } 
        else
        {
            isDay = false;
        }

        //태양이 원점을 중심으로 바라보며 회전하도록 설정
        transform.RotateAround(Vector3.zero, Vector3.right, speed * Time.deltaTime);
        transform.LookAt(Vector3.zero);
    }

    //낮인지 확인하는 메서드
    public bool IsDay()
    {
        return transform.position.y >= -1.4;
    }
}