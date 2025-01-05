using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerController : Controller
{
    
    [SerializeField]
    private RectTransform phoneUI; //핸드폰UI오브젝트
    private bool isPhoneUp=false;
    private Coroutine movePhoneUICoroutine = null; //핸드폰 이동 코루틴을 참조할 변수

    [SerializeField]
    private CursorManager cursorManager;
    [SerializeField]
    private GameObject Map;
    private bool isMapOpen;

    // Update is called once per frame
    void Update()
    {
        InputRotateAxis();
        InputInteractAction();
        showPhoneUI();
        InputAction();
        OpenMap();
    }

    void FixedUpdate()
    {
        InputMoveAxis();
        InputJumpAction();
    }

    private void InputMoveAxis() //키보드의 이동 관련 인풋을 받아서 컨트롤하는 타겟의 move메서드 실행
    {
        controlTarget.Move(new Vector2(Input.GetAxis("Horizontal"),Input.GetAxis("Vertical")));
    }

    private void InputRotateAxis() //마우스의 인풋을 받아서 컨트롤하는 오브젝트의 rotate메서드 실행
    {
        controlTarget.Rotate(new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y")));
    }

    private void InputInteractAction()
    {
        if(Input.GetKeyDown(KeyCode.F)) //F키를 누르면 컨트롤 하는 오브젝트의 interact메서드 실행
        {
            controlTarget.Interact();
        }
    }

    private void InputJumpAction()
    {
        
        if (Input.GetKey(KeyCode.Space)) //스페이스바를 누르면 컨트롤 오브젝트의 jump메서드 실행
        {
                controlTarget.Jump(); //케릭터일때는 점프, 오토바이 일때는 브레이크
        }

    }

    private void showPhoneUI()
    {
        if (Input.GetKeyDown(KeyCode.E)) //핸드폰 ui 관리
        {
            //핸도폰ui가 올라 오지 않은 상태면 위로 아니라면 밑으로
            Vector2 targetPosition = isPhoneUp ? new Vector2(phoneUI.anchoredPosition.x, -500) : new Vector2(phoneUI.anchoredPosition.x, -20);
            if (movePhoneUICoroutine != null)
            {
                StopCoroutine(movePhoneUICoroutine); //이미 실행중인 코루틴 멈추고 새로 시작
            }
            movePhoneUICoroutine = StartCoroutine(MovePhoneUI(targetPosition));
            isPhoneUp = !isPhoneUp;

            cursorManager.SetPhoneUpState(isPhoneUp);
         
        }
    }

    private IEnumerator MovePhoneUI(Vector2 targetPosition)  //타겟 포지션으로 핸드폰ui 이동
    {
        while (Vector2.Distance(phoneUI.anchoredPosition, targetPosition) > 0.1f)
        {
            phoneUI.anchoredPosition = Vector2.Lerp(phoneUI.anchoredPosition, targetPosition, Time.deltaTime * 5);
            yield return null;
        }
        phoneUI.anchoredPosition = targetPosition;
        movePhoneUICoroutine = null;
    }

    private void InputAction()
    {
        if (Input.GetMouseButtonDown(0))  //마우스 좌클릭시 action메서드 실행
        {
            controlTarget.Action(); //케릭터일 경우에는 여러가지 상호작용, 오토바이일 경우에는 경적 소리
        }

    }

    private void OpenMap()  // 맵ui를 활성화/비활성화
    {
        if(Input.GetKeyDown(KeyCode.Tab))
        {
            if(isMapOpen)
            {
                Map.SetActive(false);
                isMapOpen=false;
            }
            else
            {
                Map.SetActive(true);
                isMapOpen=true;
            }

        }
    }

}
