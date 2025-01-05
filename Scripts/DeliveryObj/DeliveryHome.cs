using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class DeliveryHome : MonoBehaviour
{

    //식당 목록
    [SerializeField]
    private Restaurant[] restaurants;
    
    public GameObject destination; //플레이어가 음식을 놓을 위치
    [SerializeField]
    private string address; // 주소
    [SerializeField]
    private PlayerStats playerStats; // 플레이어 스탯
    [SerializeField]
    private GameObject complainMessage; //메세지 프리팹
    [SerializeField]
    private ComplainScrollviewController messageScrollview; // 컴플레인 메세지를 놓을 스크롤 뷰 

    //컴플레인 멘트들
    private string[] lateComplainMent;
    private string[] foodComplainMent;
    private string[] requestComplainMent;

    public bool ordered; //이미 배달 요청한 상태에서 다시 하지 않도록 하는 변수

    // 요청사항 완료 됬는지 확인하는 값
    public bool ringBell;
    public bool doorKnocked;


    void Awake()
    {
        ordered =false;
        ringBell=false;
        doorKnocked=false;
        complainMentInitiate();
    }

    // Start is called before the first frame update
    void Start()
    {
       destination.SetActive(false);

    }

    // 배달 요청
    public void Order()
    {
        ringBell = false; doorKnocked=false; 
        int randomIndex = Random.Range(0, restaurants.Length); //식당 리스트 중 랜덤으로 하나 선택
        float distance = Vector3.Distance(transform.position, restaurants[randomIndex].transform.position); //식당과의 거리 계산
        distance = distance*0.01f;
        bool bell = Random.Range(0, 2) == 0; // 초인종, 노크 유무 랜덤으로 설정
        bool knock = Random.Range(0, 2) == 0;
        restaurants[randomIndex].DeliveryRequest(distance, address, bell,knock,this);  //식당의 deliveryRequest 호출
        destination.SetActive(true); // 배달장소 활성화
    }

    public void Complain() // 배달 늦었을때 호출
    {
        destination.SetActive(false);

        if(playerStats!=null)
        {
            playerStats.changeRating(-1); // 플레이어 평점 감소
        }
        
        messageScrollview.AddDeliverMessage(createComplainMessage(lateComplainMent[Random.Range(0,lateComplainMent.Length)])); // 컴플레인 멘트 랜덤으로 선택해서 스크롤뷰에 추가
    }

    public void foodComplain() // 배달 음식 빼먹은거 들켰을때 호출
    {
        if(playerStats!=null)
        {
            playerStats.changeRating(-1); // 플레이어 평점 감소
        }

        messageScrollview.AddDeliverMessage(createComplainMessage(foodComplainMent[Random.Range(0,foodComplainMent.Length)])); // 컴플레인 멘트 랜덤으로 선택해서 스크롤뷰에 추가

    }

    public void requestComplain() // 배달 요청 안지켰을때 호출
    {
        if(playerStats!=null)
        {
            playerStats.changeRating(-1); // 플레이어 평점 감소
        }

        messageScrollview.AddDeliverMessage(createComplainMessage(requestComplainMent[Random.Range(0,requestComplainMent.Length)])); // 컴플레인 멘트 랜덤으로 선택해서 스크롤뷰에 추가

    }

    private RectTransform createComplainMessage(string messageText)
    {
        // 컴플레인 메시지 프리팹을 인스턴스화
        GameObject newMessage = Instantiate(complainMessage);

        // RectTransform으로 변환 (스크롤뷰에 추가할 때 필요)
        RectTransform messageRect = newMessage.GetComponent<RectTransform>();

        // 프리팹 자체에 있는 TextMeshProUGUI 컴포넌트 접근해서 택스트 설정
        TextMeshProUGUI textComponent = newMessage.transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>();
        if (textComponent != null)
        {
            textComponent.text = address + "\n" + messageText;  // 컴플레인 멘트 받아서 주소 밑에 추가
        }
        else
        {
            Debug.LogWarning("TextMeshProUGUI component not found in the message prefab.");
        }

        return messageRect;
    }

    private void complainMentInitiate() // 컴플레인 멘트 초기화
    {
       lateComplainMent = new string[]
        {
            "배달 좀 빨리 해주세요",
            "너무 늦어서 배달 취소합니다",
            "빨리 좀 와라",
            "너무 느려",
            "주문한 지 한참 지나서도 안 오네요"
        };

       foodComplainMent = new string[]
        {
            "음식을 왜 니가 처먹냐?",
            "음식 상태가 별로예요.",
            "내 음식 어딨어?",
            "음식 빼먹었냐?",
            "음식 빼먹고 배달하는 게 말이 돼?"
        };

        requestComplainMent = new string[]
        {
            "요청사항 좀 똑바로 읽어주세요",
            "도대체 요청사항 적는 의미가 있나요?",
            "요청사항 확인 좀요",
            "내가 괜히 적었겠냐? 좀 신경 써라, 어?"
        };
    }
}
