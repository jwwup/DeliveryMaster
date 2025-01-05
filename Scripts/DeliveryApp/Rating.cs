using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Rating : MonoBehaviour
{

    [SerializeField]
    private PlayerStats playerStats;
    [SerializeField]
    private Image[] stars;


    // Start is called before the first frame update
    void Start()
    {
        UpdateRatingStar();
    }

    public void UpdateRatingStar()
    {
        // 별의 수와 상태를 초기화
        foreach (var star in stars)
        {
            star.fillAmount = 0;
        }

        int rating = playerStats.rating; 

        // 평점에 따라 별을 채우기
        for (int i = 0; i < stars.Length; i++)
        {
            if (rating >= 20)
            {
                stars[i].fillAmount = 1.0f; 
                rating -= 20; 
            }
            else if (rating >= 10)
            {
                stars[i].fillAmount = 0.5f; 
                rating -= 10; 
            }
            else
            {
                break; 
            }
        }
    }
}
