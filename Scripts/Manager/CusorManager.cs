using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorManager : MonoBehaviour
{
    public Texture2D cursorTexture;  
    private bool isPhoneUp = false;  
    private bool isBoxOpen = false;
    [SerializeField]
    private CharacterControlable player;

    void Start()
    {
        // 커서를 화면 중앙에 고정하고 숨김
        Cursor.lockState = CursorLockMode.Locked;  
        Cursor.visible = false;  // 커서 숨김
    }

    // 전화 UI 상태에 따라 커서 상태 변경
    public void SetPhoneUpState(bool isPhoneUp)
    {
        this.isPhoneUp = isPhoneUp;  
        player.isPhoneUp=isPhoneUp;

        if (isPhoneUp) 
        {
           unlockCursor();
        }
        else
        {
            lockCursor();
        }
    }

    // 배달 박스의 상태에 따라 커서 상태 변경
    public void SetBoxOpenState(bool isBoxOpen)
    {
        this.isBoxOpen = isBoxOpen;

        if(isBoxOpen)
        {
            unlockCursor();
        }
        else
        {
            lockCursor();
        }

    }

    //커서의 위치 고정을 풀고 보이도록 
    public void unlockCursor(){
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    //커서의 위치를 고정하고 보이지 않도록
    public void lockCursor(){
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // OnGUI는 UI 요소를 그릴 때 사용
    void OnGUI()
    {
        //현재 상태에 따라 중앙에 커서 텍스처를 표시할지 결정
        if (!isPhoneUp && !isBoxOpen && cursorTexture != null)
        {
            // 화면의 중앙 좌표 계산
            Vector2 cursorPosition = new Vector2(Screen.width / 2f, Screen.height / 2f);

            // 중앙에 커서 이미지를 그리기
            GUI.DrawTexture(new Rect(cursorPosition.x - (cursorTexture.width / 2),
                                     cursorPosition.y - (cursorTexture.height / 2),
                                     cursorTexture.width, cursorTexture.height), cursorTexture);
        }
    }
}