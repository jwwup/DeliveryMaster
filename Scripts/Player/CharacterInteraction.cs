using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CharacterInteraction : MonoBehaviour
{
    public Animator animator;
    public Transform rightHandTransform;
    public bool boxOpen; //배달 박스를 열었는지 확인
    public GameObject grabbedFood; //플레이어가 잡고 있는 오브젝트(음식)
    private Elevator elevator;
    [SerializeField]
    private float maxRayDistance; //상호작용 가능한 최대거리
    public LayerMask interactLayer; // 상호작용 할 레이어
    [SerializeField]
    private GameObject closeButton; //상자 닫기 버튼
    [SerializeField]
    private CursorManager cursorManager;
    [SerializeField]
    private Camera boxCamera; //박스 내부를 찍는 카메라 
    [SerializeField]
    private Camera mainCamera; //원래의 게임 뷰를 찌는 메인 카메라 

    [SerializeField]
    private AudioClip throwSound;
    [SerializeField]
    private AudioClip grabSound;

    public Transform charBody;
    private CharacterControlable controlable;
    private Mechanic mechanic;

    
    void Awake()
    {
        controlable = GetComponent<CharacterControlable>();
        boxOpen=false;
    }


    void Start()
    {
        rightHandTransform = animator.GetBoneTransform(HumanBodyBones.RightMiddleDistal);
        animator = charBody.GetComponent<Animator>();
    }

    public void HandleBoxOpenedActions()
    {

        if(grabbedFood != null){  // 손에 음식이 있는 경우
            if (!EventSystem.current.IsPointerOverGameObject()) //ui요소 위에 마우스가 위치하지 않는 경우
            {
                PlaceFoodInBox();
            }   
        }
    }

    public void PlaceFoodInBox()  //배달 박스에 음식 놓는 메서드
    { 
        Collider foodCollider = grabbedFood.GetComponent<Collider>();
        Rigidbody foodRigidbody = grabbedFood.GetComponent<Rigidbody>();

        if (foodCollider != null) foodCollider.isTrigger = false;
        if (foodRigidbody != null) foodRigidbody.isKinematic = false;

        //마우스의 위치로 레이캐스트를 하여 음식 놓기
        Ray ray = boxCamera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            grabbedFood.transform.position = hit.point;
            grabbedFood.transform.parent = null;
            grabbedFood = null;
        }
    }

    public void HandleElevatorInteraction() //엘리베이터와의 상호작용을 관리하는 메서드
    {
        if (elevator == null) return;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, maxRayDistance, interactLayer))
        {
            switch (hit.collider.tag)  //클릭한 엘리베이터의 버튼에 따라서 실행
            {
                case "First":
                    elevator.MoveFloor(Elevator.Floor.first);
                    break;
                case "Second":
                    elevator.MoveFloor(Elevator.Floor.second);
                    break;
                case "Third":
                    elevator.MoveFloor(Elevator.Floor.third);
                    break;
                case "Fourth":
                    elevator.MoveFloor(Elevator.Floor.fourth);
                    break;
                case "Fifth":
                    elevator.MoveFloor(Elevator.Floor.fifth);
                    break;
                case "Open":
                    elevator.openDoor();
                    break;
                case "Close":
                    elevator.closeDoor();
                    break;
                case "UpDown":
                    elevator.UpDown();
                    break;
            }
        }
    }

    public void HandleFoodGrabAndDrop()
    {
        if (grabbedFood == null)
        {
            TryGrabFood(); //음식을 잡고 있지 않을 경우 음식 잡는 메서드 호출
        }
        else
        {
            if (!boxOpen)
            {
                TryThrowFood(); //음식을 잡고 있을 경우 음식을 던지는 메서드 호출
            }
        }
    }

    private void TryGrabFood()
    {
        //레이캐스트를 해서 음식이 있을 경우에 음식을 오른손의 자식으로 설정
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, maxRayDistance, interactLayer) && hit.collider.CompareTag("Food"))
        {
            animator.SetTrigger("Grab");
            SoundManager.Instance.PlaySound(grabSound);
            grabbedFood = hit.collider.gameObject;

            Collider foodCollider = grabbedFood.GetComponent<Collider>();
            Rigidbody foodRigidbody = grabbedFood.GetComponent<Rigidbody>();

            if (foodCollider != null) foodCollider.isTrigger = true;
            if (foodRigidbody != null) foodRigidbody.isKinematic = true;

            grabbedFood.transform.position = rightHandTransform.position;
            grabbedFood.transform.rotation = rightHandTransform.rotation;
            grabbedFood.transform.parent = rightHandTransform;
        }
    }

    private void TryThrowFood()
    {   
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, maxRayDistance, interactLayer))
        {
         // interactLayer에 속하는 객체가 감지되면 던지기 방지
         return;
        }    
    
        if (!controlable.isPhoneUp) //핸드폰 UI가 올라온 상태가 아닌 경우에만
        {
            Collider foodCollider = grabbedFood.GetComponent<Collider>();
            Rigidbody foodRigidbody = grabbedFood.GetComponent<Rigidbody>();
            if (foodCollider != null) foodCollider.isTrigger = false;
            if (foodRigidbody != null) foodRigidbody.isKinematic = false;

            //캐릭터의 앞쪽으로 던지고 부모 오브젝트에서 해제
            animator.SetTrigger("Throw");
            foodRigidbody.AddForce(charBody.forward * 5f, ForceMode.Impulse);
            foodRigidbody.AddForce(Vector3.up * 3f, ForceMode.Impulse);
            grabbedFood.transform.parent = null;
            grabbedFood = null;

            SoundManager.Instance.PlaySound(throwSound);
        }
    }

    public void HandleObjectInteractions()  // 기타 오브젝트와의 상호작용 관리
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, maxRayDistance, interactLayer))
        {
            switch (hit.collider.tag)
            {
            
                case "Box":
                    openBox();
                    break;
                
                case "Mechanic":
                    mechanic?.startTalking();
                    cursorManager.unlockCursor();
                    break;

                case "Door":
                    BellKnockCheck doorCheck = hit.collider.GetComponent<BellKnockCheck>();
                    doorCheck?.knockDoor();
                    break;

                case "Bell":
                    BellKnockCheck bellCheck = hit.collider.GetComponent<BellKnockCheck>();
                    bellCheck?.ringBell();
                    break;
            }
        }
    }

    private void openBox()  // 메인카메라를 비활성화하고 박스 내부를 찍는 카메라를 활성화
    {
        closeButton.SetActive(true);
        Debug.Log("open box");
        Camera.main.gameObject.SetActive(false);
        boxCamera.gameObject.SetActive(true);
        boxOpen=true;
        cursorManager.SetBoxOpenState(boxOpen); // 마우스 커서 잠금 해제
    }

    public void OnCloseButtonClicked() // 배달박스 닫는 버튼 눌렀을 경우 다시 원래의 카메라 활성화
    {
        closeButton.SetActive(false);
        boxCamera.gameObject.SetActive(false);
    
    
        if (mainCamera != null)
        {
            mainCamera.gameObject.SetActive(true);
        }
        else
        {
            Debug.LogError("Main Camera not found. Make sure the main camera has the 'MainCamera' tag.");
        }
    
        boxOpen = false;
        cursorManager.SetBoxOpenState(boxOpen); // 마우스 커서 다시 잠금
    }

    void OnTriggerEnter(Collider collider)  //메카닉이나 엘리베이터 근처로 갔을시에 참조
    {
        if(collider.CompareTag("Mechanic"))   
        {
            mechanic=collider.gameObject.GetComponent<Mechanic>();
        }

        if(collider.CompareTag("Elevator"))
        {
            elevator=collider.gameObject.GetComponent<Elevator>();
        }
    }

    void OnTriggerExit(Collider collider)
    {
        if(collider.CompareTag("Mechanic"))
        {
            mechanic = null;
        }

        if(collider.CompareTag("Elevator"))
        {
            elevator=null;
        }
    }
    
}
