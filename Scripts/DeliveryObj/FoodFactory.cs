using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodFactory : Factory
{

    [SerializeField]
    private GameObject[] foodObjects;


    public override IProduct GetProduct(){

         // 비활성화된 음식 아이템들을 담을 리스트 생성
        List<GameObject> inactiveFoods = new List<GameObject>();

        // foodMenu 배열을 순회하며 비활성화된 아이템들을 리스트에 추가
        foreach (GameObject food in foodObjects)
        {
            if (!food.activeInHierarchy) // 음식 아이템이 비활성화된 상태인지 확인
            {
                inactiveFoods.Add(food);
            }
        }

        // 비활성화된 음식이 없으면 경고 메시지를 출력
        if (inactiveFoods.Count == 0)
        {
            Debug.Log("조리할 수 있는 비활성화된 음식 아이템이 없습니다.");
        }

        // 랜덤으로 비활성화된 음식 아이템을 선택
        int randomIndex = Random.Range(0, inactiveFoods.Count);
        GameObject selectedFood = inactiveFoods[randomIndex];

        // 선택된 음식 아이템을 활성화 리턴
        selectedFood.SetActive(true);
        Food deliverFood = selectedFood.GetComponent<Food>();

        return deliverFood;

    }

    void Start()
    {
        
        foreach (GameObject foodObject in foodObjects)
        {
            foodObject.SetActive(false);
        }
    }





}
