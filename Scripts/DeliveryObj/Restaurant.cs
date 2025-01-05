using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Restaurant : MonoBehaviour
{
    [SerializeField]
    private string restaurantName;

    [SerializeField]
    private RequestScrollViewController scrollView; //배달 요청 메세지 넣을 스크롤뷰

    [SerializeField]
    private Transform spawnPlace; // 음식 생성 위치 

    //오브젝트 생성하는 팩토리들
    private FoodFactory foodFactory;
    private MessageFactory messageFactory;


    void Awake(){
        foodFactory = GetComponent<FoodFactory>();
        messageFactory = GetComponent<MessageFactory>();
    }



    public void DeliveryRequest(float distance, string address,bool bell, bool knock, DeliveryHome house)
    {   
       //스크롤 뷰에 메세지가 5개 이상 되지 않도록
         if (scrollView.requestMessages.Count >= 5)
        {
            Debug.Log("요청 메시지가 너무 많습니다. 더 이상 추가할 수 없습니다.");
            return;
        }
        else
        {
            //메세지 팩토리로부터 메세지를 받고 초기화
            DeliveryMessage deliveryMessage = (DeliveryMessage)messageFactory.GetProduct();
            deliveryMessage.Initialize(restaurantName,distance,address,bell,knock,house,scrollView);
            //생성한 메세지를 스크롤뷰에 추가
            RectTransform messageRectTransform = deliveryMessage.gameObject.GetComponent<RectTransform>();
            scrollView.AddDeliverMessage(messageRectTransform);


            //수락 버튼이 눌리면 음식을 생성하도록            
            StartCoroutine(Cook(deliveryMessage, Mathf.Round(distance * 100f) / 100f));

        }

    }



    private IEnumerator Cook(DeliveryMessage message, float fee)
    {
         while (true) // 무한 루프를 통해 계속 대기
        {
            if (message.willDeliver == 1) // 플레이어가 수락 버튼을 누르면 음식 생성
            {
                Food food = (Food)foodFactory.GetProduct();
                // 배달료 할당 및 메세지와 음식 서로 연결
                food.Initialize(fee);  
                food.message=message;
                message.food = food; 
                
                food.gameObject.transform.position = spawnPlace.position;
                yield break; 
            }
            else if (message.willDeliver == 2) // 플레이어가 거절 버튼 누르면 메세지 삭제
            {
                scrollView.DeleteDeliveryMessage(message);
                yield break; 
            }
            else
            {
                // 1도 2도 아닌 경우 계속 대기
                yield return null;
            }
        }
        
    }







}
