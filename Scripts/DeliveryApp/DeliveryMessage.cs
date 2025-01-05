using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DeliveryMessage : MonoBehaviour, IProduct 
{
   
    
    [SerializeField]
    private GameObject acceptButton;
    [SerializeField]
    private GameObject refuseButton;
    public ScrollviewController currentScreen;
    private RectTransform messageTransform;
    public TextMeshProUGUI textComponent;

    public int willDeliver;

    public GameObject Timer;
    [SerializeField]
    private TMP_Text[] timeTexts;

    public float timeLimit;

    public Food food { get; set; }

    public DeliveryHome home;

    private Coroutine timerCoroutine;

    public bool knock;
    public bool bell;

    public void Initialize() 
    {
        // 기본 초기화
    }

    //다른 매개변수가 필요하므로 오버로딩(식당 이름, 거리 , 주소 , 요청사항, 배달 목적지, 현재 메세지의 화면)
    public void Initialize(string restaurantName, float distance, string address, bool bell, bool knock,DeliveryHome home, ScrollviewController currentScreen)
    {
        if (textComponent != null)
        {
            string bellStatus = bell ? "벨O" : "벨X";
            string knockStatus = knock ? "노크O" : "노크X";
            textComponent.text = restaurantName + "\n" + address + "\n" +"요청사항:" +bellStatus + knockStatus+ "\n" +distance.ToString("F2") + "$";
        }

        
        SetLimitTime(Mathf.Round(distance * 100f) / 100f);
        this.knock=knock;
        this.bell=bell;
        this.currentScreen = currentScreen;
        this.home=home;
        
    }


    void Awake()
    {
        willDeliver=0;
        messageTransform = GetComponent<RectTransform>();
    }
    // Start is called before the first frame update
    void Start()
    {
        Timer.SetActive(false);
    }

    //배달 요청 메세지의 수락/거절 버튼을 눌렀을 경우의 이벤트
    public void OnAcceptButtonClicked()
    {
         currentScreen.toNextScreen(messageTransform);
         acceptButton.SetActive(false);
         refuseButton.SetActive(false);
         willDeliver=1;
    }

    public void OnRefuseButtonClicked()
    {
        willDeliver=2;
        acceptButton.SetActive(false);
        refuseButton.SetActive(false);
        home.ordered = false;
    }

    //제한 시간 설정
    public void SetLimitTime(float time)
    {

        timeLimit = time;
        Debug.Log(timeLimit);
    }

    public void startTimer()
    {
       timerCoroutine = StartCoroutine(TimerWorking()); 
    }

    public void stopTimer()
    {
        //타이머 코루틴이 실행(참조) 중일 때만 멈추기
        if (timerCoroutine != null)  
        {
            StopCoroutine(timerCoroutine);  
            timerCoroutine = null;  
        }
        Timer.SetActive(false);  
    }



    private IEnumerator TimerWorking()
    {
        // 정수 부분을 분으로 설정/ 만약 정수 부분이 0이라면, 최소 2분으로 설정
        int minutes = Mathf.Max(2, Mathf.FloorToInt(timeLimit*3f));
        int seconds = 0;

        
        Timer.SetActive(true);

        
        while (minutes > 0 || seconds > 0)
        {
            // 시간 텍스트를 항상 두자리수로 고정
            timeTexts[0].text = minutes.ToString("00");  
            timeTexts[1].text = seconds.ToString("00");  

            
            yield return new WaitForSeconds(1f);

            if (seconds > 0)
            {
                seconds--;
            }
            else
            {
                
                if (minutes > 0)
                {
                    minutes--;
                    seconds = 59;
                }
            }
        }

        // 제한 시간이 끝나면 OnDeliveryLate호출
        Timer.SetActive(false);
        if(food!=null)
        {
            if(!food.deliverd)
            {
                OnDeliveryLate();
            }
        }
        

    }

    // 음식을 비활성화하고 배달한 집의 컴플레인 메서드 호출
    public void OnDeliveryLate()
    {
      
        if(food!=null)
        {
            food.gameObject.SetActive(false);
        }
      
        if(home!=null)
        {
            home.Complain();
        }
        
        //메세지 텍스트 수정 및 다음 화면으로 넘기기 
        textComponent.text +="\n"+ "배달 취소";
        currentScreen.toNextScreen(messageTransform);

        
    }

    

}
