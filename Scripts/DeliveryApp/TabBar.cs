using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TabBar : MonoBehaviour
{
    //상단 화면 전환 버튼들
    [SerializeField]
    private GameObject[] screens;

    // 화면을 맨 앞으로 이동시키는 메서드
    private void ShowScreen(int screenIndex)
    {
        // 지정된 화면이 범위 내에 있는지 확인
        if (screenIndex >= 0 && screenIndex < screens.Length)
        {
            // 화면을 맨 앞으로 이동시킵니다.
            screens[screenIndex].transform.SetAsLastSibling();
        }
    }

    // 각 버튼 클릭 이벤트 메서드
    public void OnRegisterButtonClicked()
    {
        ShowScreen(0);
    }

    public void OnProgressButtonClicked()
    {
        ShowScreen(1);
    }

    public void OnCompletedButtonClicked()
    {
        ShowScreen(2);
    }

    public void OnMenuButtonClicked()
    {
        ShowScreen(3);
    }
    
}