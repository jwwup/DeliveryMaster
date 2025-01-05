using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeliverySystemTest : MonoBehaviour
{   

    [SerializeField]
    private DeliveryHome[] houses; //배달할 집 목록


    void Start()
    {
        StartCoroutine(InvokeRandomOrders());
    }

    
    // 계속해서 시간 간격을 주고 배달 집의 Order호출
    private IEnumerator InvokeRandomOrders()
    {
        while(true)
        {
                // 랜덤한 간격으로 멈추기 
                float waitTime = Random.Range(5f,10f);
                yield return new WaitForSeconds(waitTime);

                // houses 배열 중 랜덤으로 선택
                int randomIndex = Random.Range(0, houses.Length);
                DeliveryHome randomHouse = houses[randomIndex];

                if(!randomHouse.ordered)
                {
                    // 선택된 house에서 Order() 함수 호출
                    randomHouse.Order();
                    randomHouse.ordered=true;
                }
                
           
        }

    }


}
