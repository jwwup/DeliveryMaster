using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro; 

public class VehicleControlable : Controlable
{
   
    public Transform characterSeat; //케릭터가 앉을 위치   
    public Transform ridePosition; // 케릭터가 탑승 위치
   

    [SerializeField]
    private WheelCollider[] wheelColliders;
    [SerializeField]
    private GameObject[] wheelMeshes;

    [SerializeField]
    private AudioClip hornSound;

    public CharacterControlable driveCharacter;

    private bool fall=false;

    private float initialZRotationX; //초기 회전값 x,z
    private float initialZRotationZ;

    [SerializeField]
    private float speedThreshold = 7.0f; //제한속도
    private Rigidbody rb;

    //교통 위반 감지할 변수들
    public bool speeding;
    public bool signalViolation;
    public bool reverseLane;

    //최근의 교통 위반 여부
    private bool lastSpeedingState;
    private bool lastSignalViolationState;
    private bool lastReverseLaneState;

    [SerializeField]
    private TMP_Text text; //내구도 관련 택스트

    public int durability; //내구도
    public bool riding;
    private bool showPlayer; //플레이어가 근처에 있는지 확인하는 변수

    [SerializeField]
    private GameObject player;

    [SerializeField]
    private WarningSignView warningSignview; //경고 표시를 넣을 오브젝트

    //경고 표시 ui들
    [SerializeField]
    private RectTransform speedingSign;
    [SerializeField]
    private RectTransform signalSign;
    [SerializeField]
    private RectTransform reverseSign;


    [SerializeField]
    private float speedValue;
    [SerializeField]
    private TMP_Text speedText;

    private bool isEngineRunning = false; //현재 이동중인지 확인하는 변수
    public AudioSource audioSource;
    [SerializeField]
    public AudioClip startSound;
    [SerializeField]
    private AudioClip engineLoop;
    [SerializeField]
    private AudioClip carCrash;
    

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();
        durability=10;
    }

    void Start()
    {
    
        audioSource.Stop();
        rb.centerOfMass = new Vector3(0, 0, 0);

        initialZRotationX = transform.rotation.eulerAngles.x;
        initialZRotationZ = transform.rotation.eulerAngles.z;

        StartCoroutine(CheckStatus());
         
    }                

    public override void Interact()
    {
        driveCharacter.animator.SetBool("Drive", false);
        driveCharacter.animator.SetTrigger("GetOut");
        StartCoroutine(GetOut());
        
    }

    private IEnumerator GetOut() //케릭터가 오토바이에서 내리도록
    {
     
        if(driveCharacter!=null)
        {
            var controller = FindObjectOfType<Controller>();
            var charBody = driveCharacter.GetComponent<Rigidbody>();
            var charCollider = driveCharacter.GetComponent<CapsuleCollider>();
            var charCollider2 = driveCharacter.GetComponent<BoxCollider>();

            //케릭터를 탑승 위치로 이동
            while(Vector3.Distance(driveCharacter.transform.position,ridePosition.position)>0.1f)
            {
                yield return null; 
                if (driveCharacter != null)  
                {
                    driveCharacter.transform.position += (ridePosition.position - driveCharacter.transform.position).normalized * Time.deltaTime * 3f;
                }
                else
                {
                    yield break; 
                }
            }

            foreach (var wheel in wheelColliders)
            {
                Debug.Log("브레이크");
                wheel.brakeTorque = 3000f;
                wheel.motorTorque = 0; // 브레이크를 걸 때는 모터 토크를 0으로 설정
            }

            charBody.isKinematic=false;
            charCollider.isTrigger=false;
            charCollider2.isTrigger=false;

            driveCharacter.transform.SetParent(null);

            controller.ChangeControlTarget(this,driveCharacter); //컨트롤러의 조종할 타겟 변경

        }
        
        driveCharacter=null;

    }

    //탈것을 컨트롤 할 경우에는 점프가 아닌 브레이크 
    public override void Jump()
    {
        float brakeTorque = 3000f; // 브레이크 토크 값 설정

        // 모든 바퀴에 브레이크 토크 적용
        foreach (var wheel in wheelColliders)
        {
            wheel.brakeTorque = brakeTorque;
            wheel.motorTorque = 0; // 브레이크를 걸 때는 모터 토크를 0으로 설정
        }
    }

    public override void Move(Vector2 input) 
    {
        
        float motor = input.y * speedValue*durability; // 뒤 바퀴의 모터 토크
        float steering = input.x * 10f; // 앞바퀴의 조향 각도
       
       // 이동 시 브레이크 해제
        foreach (var wheel in wheelColliders)
        {
            wheel.brakeTorque = 0;
        }

        // 앞바퀴의 조향 각도 설정
        wheelColliders[0].steerAngle = steering;

        // 뒤 바퀴에 모터 토크 적용
        wheelColliders[1].motorTorque = motor;
         
    }


    public override void Rotate(Vector2 input)
    {
        //입력을 기반으로 카메라 암(카메라를 가지고 있는 오브젝트)의 회전 결정
        if(cameraArm!=null)
        {
            Vector3 camAngle = cameraArm.rotation.eulerAngles;
            float x = camAngle.x - input.y;

            if(x<180f)
            {
                x=Mathf.Clamp(x,-1f,70f);
            }
            else
            {
                x = Mathf.Clamp(x,335f,361f);
            }
            
            cameraArm.rotation = Quaternion.Euler(x,camAngle.y+input.x,camAngle.z);
        }
    }

    //탈것의 컨트롤 할 경우 경적 소리 내기
    public override void Action()
    {
        SoundManager.Instance.PlaySound(hornSound);
    }

    void TextFacePlayer() //내구도 택스트가 플레이어를 바라보도록
    {
        Vector3 directionToPlayer = player.transform.position - text.transform.position;
        directionToPlayer.y = 0;
        Quaternion rotation = Quaternion.LookRotation(-1*directionToPlayer);
        text.transform.rotation = rotation;
    }


    void Update()
    {
        TextFacePlayer();

        if(driveCharacter!=null)
        {
            riding=true;

        }
        else
        {
            riding=false;
        }

        if(showPlayer&&!riding) //플레이어가 근처에 있고 타고 있지 않은 경우에만 텍스트 활성화
        {
            text.gameObject.SetActive(true);
        }
        else
        {
            text.gameObject.SetActive(false);
        }
        
        
        
        float speed = rb.velocity.magnitude * 3.6f; // m/s를 km/h로 변환
        speedText.text = $"{speed:F1} km/h";

        if (speed > 1 && !isEngineRunning && driveCharacter != null)
        {
            // 엔진이 꺼진 상태에서 속도가 0 이상일 때만 시작 사운드를 재생
            isEngineRunning = true;
            StartCoroutine(PlayEngineLoopAfterStartSound());
        }
        else if (speed <= 1 && isEngineRunning) // 속도가 0이라면 모든 엔진 소리 정지
        {
            audioSource.Stop();
            isEngineRunning = false;
        }

    }
    
    void FixedUpdate()
    {
        // 현재 오브젝트의 회전값을 가져옴
        Quaternion currentRotation = transform.rotation;
        
        // X, Y 회전값은 그대로 두고 Z 회전값만 초기 값으로 고정
        currentRotation = Quaternion.Euler(initialZRotationX, currentRotation.eulerAngles.y, initialZRotationZ);
        
        // 고정된 회전값을 다시 오브젝트에 적용
        transform.rotation = currentRotation;

        float speed = rb.velocity.magnitude;

        if (speed > speedThreshold)
        {
            speeding=true;
        }
        else
        {
            speeding=false;
        }
  
    }

    void OnTriggerEnter(Collider collider)
   {
        if(collider.CompareTag("Character"))
        {
            showPlayer=true;
        }

    }

   void OnTriggerExit(Collider collider)
   {
        if(collider.CompareTag("Character"))
        {
            showPlayer=false;
        }
   }


    void OnCollisionEnter(Collision collision)
   {
        if ((collision.gameObject.CompareTag("Car")||collision.gameObject.CompareTag("Wall"))&&speeding) //과속 상태에서 차량과 충돌하면 충돌사고 처리
        {
            fallDown(); //플레이어 하차
            if(durability>3) //내구도 감소
            {
                durability-=3;
                UpdateDurabilityText();
                
            }
            audioSource.Stop();
            isEngineRunning = false;
            audioSource.clip = carCrash;
            audioSource.loop = false;
            audioSource.Play();
        }
   }

   private IEnumerator PlayEngineLoopAfterStartSound()
    {
        yield return new WaitWhile(() => audioSource.isPlaying); // startSound가 끝날 때까지 대기

        // 엔진 루프 사운드 재생
        audioSource.clip = engineLoop;
        audioSource.loop = true;
        audioSource.Play();
    }

   public void UpdateDurabilityText()
   {
        text.text=durability + "/10";
   }


    private void fallDown()
    {
        driveCharacter.fall();
        StartCoroutine(GetOut());
    }

    private IEnumerator CheckStatus()  //교통 위반 상태 확인하는 메서드
    {
        while (true)
        {
            // 값이 변경되었을 때만 ui요소 활성화/비활성화
            if (speeding != lastSpeedingState)
            {
                speedingSign.gameObject.SetActive(speeding);
                if(speeding)
                {
                    warningSignview.AddWarningSign(speedingSign);
                }
                else
                {
                    warningSignview.RemoveWarningSign(speedingSign);
                }
                lastSpeedingState = speeding;
            }

            if (signalViolation != lastSignalViolationState)
            {
                signalSign.gameObject.SetActive(signalViolation);
                if(signalViolation)
                {
                    warningSignview.AddWarningSign(signalSign);
                }
                else
                {
                    warningSignview.RemoveWarningSign(signalSign);
                }
                lastSignalViolationState = signalViolation;
            }

            if (reverseLane != lastReverseLaneState)
            {
                reverseSign.gameObject.SetActive(reverseLane);
                if(reverseLane)
                {
                    warningSignview.AddWarningSign(reverseSign);
                }
                else
                {  
                    warningSignview.RemoveWarningSign(reverseSign);
                }
                lastReverseLaneState = reverseLane;
            }

            // 0.1초마다 체크
            yield return new WaitForSeconds(0.1f);
        }
    }

}
