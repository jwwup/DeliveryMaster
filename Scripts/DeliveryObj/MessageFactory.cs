using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MessageFactory : Factory
{
   
    [SerializeField]
    private GameObject[] deliveryMessageObjects; //배달 메세지 오브젝트들 

   public override IProduct GetProduct(){

         // 비활성화된 음식 아이템들을 담을 리스트 생성
        List<GameObject> inactiveMessages = new List<GameObject>();

        // foodMenu 배열을 순회하며 비활성화된 아이템들을 리스트에 추가
        foreach (GameObject message in deliveryMessageObjects)
        {
            if (!message.activeInHierarchy) // 음식 아이템이 비활성화된 상태인지 확인
            {
                inactiveMessages.Add(message);
            }
        }

        // 비활성화된 음식이 없으면 경고 메시지를 출력
        if (inactiveMessages.Count == 0)
        {
            Debug.Log("비활성화된 메세지 아이템이 없습니다.");
        }

        // 랜덤으로 비활성화된 음식 아이템을 선택
        int randomIndex = Random.Range(0, inactiveMessages.Count);
        GameObject selectedMessage = inactiveMessages[randomIndex];

        // 선택된 음식 아이템을 활성화하고 리턴
        selectedMessage.SetActive(true);
        DeliveryMessage deliveryMessage = selectedMessage.GetComponent<DeliveryMessage>();

        return deliveryMessage;

    }

    void Start()
    {
        
        foreach (GameObject message in deliveryMessageObjects)
        {
            message.SetActive(false);
        }
    }

}
