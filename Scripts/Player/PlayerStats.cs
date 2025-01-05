using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class PlayerStats : MonoBehaviour
{
    public float maxHp;
    public float curHp; 
    public int rating; 
    public float money; 
    [SerializeField]
    private float targetMoney; //목표 돈
  

    [SerializeField]
    private TMP_Text moneyText; //돈을 나타내는 ui
    [SerializeField]
    private Slider hpSlider; //플레이어의 체력을 나타내는 슬라이더
  
    
    [SerializeField]
    private Rating ratingSystem; 
    [SerializeField]
    private GameOverManager gameoverManager;

    //게임오버,클리어 이벤트
    public event Action<string> OnGameOver;
    public event Action OnGameClear;
    
    void Awake()
    {
        maxHp=100;
        curHp = maxHp;
    }

    // Start is called before the first frame update
    void Start()
    {   
        changeMoney(3);
        changeRating(10);
        UpdateHpSlider();
    }

    public void changeMoney(float value) //플레이어의 돈과 관련 ui 변경하는 메서드
    {
        money+=value;
        moneyText.text = money.ToString() + "$";
        if(money>=targetMoney){
            OnGameClear?.Invoke(); //목표 달성시 게임클리어 이벤트 호출
        }
    }

    public void changeRating(int value) //플레이어의 평점과 관련 ui 변경하는 메서드
    {
        rating += value;
        if(rating<=0){
            OnGameOver?.Invoke("해고"); //지정한 평점 이하로 낮아지면 게임오버 이벤트 호출
        }
        ratingSystem.UpdateRatingStar();
    }

    public void changeHP(int value) //플레이어의 체력과 관련 ui 변경하는 메서드
    {
        curHp+=value;
        if(curHp>maxHp) curHp=maxHp;

        if(curHp<=0f){
            OnGameOver?.Invoke("충돌사고"); //체력이 없어지면 게임오버 이벤트 호출
        }
        UpdateHpSlider(); 
    }

    private void UpdateHpSlider() //최대체력 대비 현재체력의 값으로 슬라이더 표현
    {
        if (hpSlider != null)
        {
            hpSlider.value = curHp / maxHp;
        }
    }

    
}
