using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Food : MonoBehaviour, IProduct 
{
    //음식의 배달 메세지 
    public DeliveryMessage message;

    // 음식을 클릭하면 생성할 ui 요소 
    [SerializeField]
    private GameObject uiElement;
    //ui 오브젝트 생성할 캔버스
    [SerializeField]
    private Canvas canvas;

    // 플레이어 클래스들
    [SerializeField]
    private CharacterControlable player;
    [SerializeField]
    private CharacterInteraction playerInteraction;
    [SerializeField]
    private PlayerStats playerStats;

    //음식 조각들
    [SerializeField]
    private GameObject[] pieces;
    
    [SerializeField]
    private AudioClip eatingSound;

    private Collider collider;
    private Rigidbody rigidBody; 

    public float deliveryFee;//배달료 
    public bool deliverd; //제한 시간내에 배달 완료 됬는지 확인할 변수

    public void Initialize() 
    {
        // 기본 초기화 
    }

    public void Initialize(float fee) // 배달료 할당하도록 오버로딩
    {
        deliveryFee=fee;
    }

    void Awake()
    {
        deliveryFee=0.0f;
        collider = GetComponent<Collider>();
        rigidBody = GetComponent<Rigidbody>();
    }

    void OnEnable()
    {
        deliverd=false;
    }

    void OnTriggerEnter(Collider collider)
    {
        if (collider.CompareTag("Destination"))
        {
            if (message != null)
            {
                
                gameObject.SetActive(false);
                
                
                // 배달 정확한곳에 했는지 확인
                if (collider.gameObject == message.home.destination)
                { 

                    collider.gameObject.SetActive(false);
                    message.home.ordered=false;

                    // 남아 있는 음식 조각의 비율 확인;
                    float remainingPiecesRatio = CheckFoodPieces();
                    float randomChance = Random.Range(0f, 1f);

                    // 랜덤으로 뽑은 변수가 남아있는 조각 보다 크면 빼 먹은 것을 걸리도록 설정
                    if (randomChance > remainingPiecesRatio+ 0.2f)
                    {
                        message.home.foodComplain();  
                        
                    }

                    //요청사항 지켰는지 확인후 false면 컴플레인
                    if(!CheckRequest()){
                        message.home.requestComplain();
                        deliveryFee=0;
                    } 
                    
                    message.home.ringBell=false;
                    message.home.doorKnocked=false;

                    //플레이어의 돈, 평점 변경
                    playerStats.changeMoney(deliveryFee);
                    playerStats.changeRating(10);
                   

                    deliveryFee=0.0f;
                    deliverd=true;
                    message.stopTimer();
                    message.currentScreen.toNextScreen(message.GetComponent<RectTransform>()); // 메세지를 완료화면으로 보내기
                }
                else
                {
                    // 잘못된 곳에 배달 했으면 패널티 부과 및 텍스트 변경
                    message.home.ordered=false;
                    message.home.destination.SetActive(false);

                    playerStats.changeMoney(-deliveryFee);
                    playerStats.changeRating(-10);
                    

                    deliveryFee=0.0f;
                    deliverd=true;
                    message.stopTimer();

                    message.textComponent.text += "\n Wrong Place";
                    message.currentScreen.toNextScreen(message.GetComponent<RectTransform>());
                }




               
            }
        }
    }

    // 배달 상자 속에서 음식을 마우스로 클릭한 위치에 ui요소 보이도록
    void OnMouseDown()
    {
        if (uiElement != null&&uiElement.activeSelf != true && playerInteraction.boxOpen)
        {
            Vector2 localPoint; // 변환된 로컬 좌표를 저장할 변수
            //화면 상의 마우스 위치를 캔버스의 로컬 위치로 변환
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                canvas.transform as RectTransform,
                Input.mousePosition,
                null, //null로 설정하여 화면 공간 기준으로 처리
                out localPoint 
            );

            // UI 요소의 위치 설정
              uiElement.GetComponent<RectTransform>().anchoredPosition = localPoint;

            // UI 요소 활성화
            uiElement.SetActive(!uiElement.activeSelf);
        }
    }

    // 그랩 버튼을 눌렀을 때 플레이의 손에 음식을 넣기
    public void OnGrabClicked()
    {
        playerInteraction.grabbedFood = gameObject;
        collider.isTrigger = true;
        rigidBody.isKinematic = true;


        transform.position = playerInteraction.rightHandTransform.position;
        transform.rotation = playerInteraction.rightHandTransform.rotation;

        transform.parent = playerInteraction.rightHandTransform; //플레이어 오른손의 자식으로 보내서 따라다니도록

        uiElement.SetActive(false);

    }

    // 먹기 버튼을 눌렀을 때 음식의 조각 중 하나 먹도록
    public void OnEatClicked()
    {
        // 활성화 되어 있는 조각 찾기
        List<GameObject> activePieces = new List<GameObject>();
        foreach (GameObject piece in pieces)
        {
            if (piece.activeSelf)
            {
                activePieces.Add(piece);
            }
        }

        if (activePieces.Count > 0)
        {
            int randomIndex = Random.Range(0, activePieces.Count);
            activePieces[randomIndex].SetActive(false);
            player.eatFood();
            SoundManager.Instance.PlaySound(eatingSound);
        }

        uiElement.SetActive(false);
    }

    //남아 있는(활성화) 조각의 비율 반환
    public float CheckFoodPieces() 
    {
        int activeCount = 0;

        foreach (GameObject piece in pieces)
        {
            if (piece.activeSelf)
            {
                activeCount++;
            }
        }

        float activeRatio = (float)activeCount / pieces.Length;

        return activeRatio;
    }

    //배달 집에서 요청한 요청 사항과 현재 상태가 같은지 확인
    private bool CheckRequest(){
        
        if(message.home.ringBell==message.bell&&message.home.doorKnocked==message.knock){
            return true;
        }
        else
        {
            return false;
        }

    }
}