using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class CharacterControlable : Controlable
{

    [SerializeField]
    private GameObject characterModel;
    public Transform charBody;
    public Animator animator;
    private Rigidbody rigidbody;

    public float jumpForce = 5f; //점프력
    private bool grounded = false; //바닥에 닿고 있는지 체크하는 변수

    private VehicleControlable vehicle; //플레이어가 탈 오토바이
    private Portal portal; 

    [SerializeField]
    private Camera boxCamera; //박스 내부 카메라
    public Transform camera;

    public bool isPhoneUp; //핸드폰ui의 상태 확인 변수

    [SerializeField]
    private CursorManager cursorManager;

    [SerializeField]
    private Transform minimapCameraTransform; //미니맵 카메라의 트랜스폼
    [SerializeField]
    private Transform playerIcon; //미니맵에 표시할 플레이어 아이콘

    [SerializeField]
    private AudioSource audioSource;

    
    private CharacterInteraction characterInteraction;
    private PlayerStats playerStats;

    void Awake()
    {
        rigidbody = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();
        playerStats = GetComponent<PlayerStats>();
        characterInteraction= GetComponent<CharacterInteraction>();  
    }

    void Start()
    {
        animator = charBody.GetComponent<Animator>();
    }


    public override void Interact()
    {
        
        if(vehicle!=null)  //차량 주위에서 Interact하면 차량 탑승
        {
            StartCoroutine(Ride(vehicle));
            animator.SetBool("Drive", true);
        }
       

        if (portal!=null) //포탈 주위에서 Interact하면 텔레포트
        {
            portal.Teleport(this.gameObject);
        } 
    }

    private IEnumerator Ride(VehicleControlable vehicle)
    {
        var controller = FindObjectOfType<Controller>();
        var charBody = GetComponent<Rigidbody>();
        var charCollider = GetComponent<CapsuleCollider>();
        var charCollider2 = GetComponent<BoxCollider>();

        charBody.isKinematic = true;
        charCollider.isTrigger = true;
        charCollider2.isTrigger=true;

        //플레이어가 오토바이를 탑승할 위치로 이동
        while(Vector3.Distance(transform.position, vehicle.ridePosition.position)>0.1f)
        {
            yield return null;
            transform.position += (vehicle.ridePosition.position-transform.position).normalized*Time.deltaTime*3f;
        }
        //플레이어가 앉을 위치로 이동
        while(Vector3.Distance(transform.position,vehicle.characterSeat.position)>0.1f)
        {
            yield return null;
            transform.position += (vehicle.characterSeat.position-transform.position).normalized*Time.deltaTime*3f;
        }


        vehicle.driveCharacter = this;
        //오토바이 시동거는 사운드 실행
        vehicle.audioSource.clip= vehicle.startSound;
        vehicle.audioSource.loop=false;
        vehicle.audioSource.Play();
        
        //탈것의 자식으로 보내서 따라다니도록
        transform.SetParent(vehicle.characterSeat);
        characterModel.transform.rotation=vehicle.transform.rotation;

        //컨트롤러가 컨트롤하는 오브젝트를 캐릭터에서 탈것으로 변경
        controller.ChangeControlTarget(this,vehicle);
    }
    

    public override void Jump()
    {   
        //바닥에 닿고 있을 경우에 점프
        if(grounded)
        {
            animator.SetBool("Jump", true);
            rigidbody.AddForce(Vector3.up*jumpForce,ForceMode.Impulse);
            grounded=false;
        }
        else
        {
            Debug.Log("grounded 아님");
        }
    }


    public override void Move(Vector2 input)
    {
        // 입력된 벡터를 기반으로 이동
        animator.SetFloat("MoveSpeed", input.magnitude);
        Vector3 lookForward = new Vector3(cameraArm.forward.x, 0f, cameraArm.forward.z).normalized;
        Vector3 lookRight = new Vector3(cameraArm.right.x, 0f, cameraArm.right.z).normalized;
        Vector3 moveDir = lookForward * input.y + lookRight * input.x;

        if (input.magnitude != 0)
        {
            charBody.forward = lookForward;
            rigidbody.MovePosition(transform.position + moveDir * Time.deltaTime * 5f);
        }
    }   

    public override void Rotate(Vector2 input)
    {
        //입력을 기반으로 카메라 암(카메라를 가지고 있는 오브젝트)의 회전 결정
        if(cameraArm!=null)
        {
            Vector3 camAngle = cameraArm.rotation.eulerAngles;
            Vector3 minCamAngle = minimapCameraTransform.rotation.eulerAngles;
            Vector3 iconAngle = playerIcon.rotation.eulerAngles;
            float x = camAngle.x - input.y;

            //카메라의 회전 각도 제한
            if(x<180f)
            {
                x=Mathf.Clamp(x,-1f,70f);
            }
            else
            {
                x = Mathf.Clamp(x,335f,361f);
            }
            
            cameraArm.rotation = Quaternion.Euler(x,camAngle.y+input.x,camAngle.z);
            //미니맵 요소들도 회전 반영
            minimapCameraTransform.rotation = Quaternion.Euler(minCamAngle.x, minCamAngle.y, minCamAngle.z-input.x); 
            playerIcon.rotation = Quaternion.Euler(iconAngle.x,iconAngle.y,iconAngle.z-input.x); 

        }
    }

    public override void Action()  //플레이어 상호작용 관련 메서드들 호출
    {
        if (characterInteraction.boxOpen)
        {
            characterInteraction.HandleBoxOpenedActions();
        }
        else
        {
            characterInteraction.HandleElevatorInteraction();
            characterInteraction.HandleFoodGrabAndDrop();
            characterInteraction.HandleObjectInteractions();
        }

    }


    public void fall() //차량과 충돌 했을 때 호출
    {
        animator.SetBool("Drive", false);
        animator.SetTrigger("Fall");
        playerStats.changeHP(-5);
        
    }

    public void eatFood() //음식 빼먹었을 때 호출
    {
        playerStats.changeHP(7);
    }

    public void violationDetected() //신호 위반 감지 했을 때
    {
        playerStats.changeMoney(-4);
    }



   void OnCollisionEnter(Collision collision)
   {
        if (collision.gameObject.CompareTag("Ground"))
        {
            grounded= true;
            animator.SetBool("Jump", false);
        }
   }

   void OnTriggerEnter(Collider collider)
   {
        if(collider.CompareTag("PlayerVehicle"))
        {
            vehicle=collider.gameObject.GetComponent<VehicleControlable>();
        }

        if(collider.CompareTag("PortalIn")||collider.CompareTag("PortalOut"))
        {
            portal=collider.gameObject.GetComponent<Portal>();
        }
   }

   void OnTriggerExit(Collider collider)
   {
        if(collider.CompareTag("PlayerVehicle"))
        {
            vehicle=null;
            
        }

        if(collider.CompareTag("PortalIn")||collider.CompareTag("PortalOut"))
        {
            portal = null;
        }
   }

}
