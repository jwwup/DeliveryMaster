using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class GameOverManager : MonoBehaviour
{
    [SerializeField]
    private PlayerStats playerStats;
    [SerializeField]
    private GameObject gameoverScreen; //게임 오버 시 생성될 화면
    [SerializeField]
    private GameObject gameClearScreen; //게임 클리어 시 생성될 화면
    [SerializeField]
    private TMP_Text reasonText;
    [SerializeField]
    private GameObject endingView; //게임 클리어 시 생성될 오브젝트
    [SerializeField]
    private GameObject[] willFalseUIs; //게임 종료 시 비활성화 할 UI요소들

    private CursorManager cursorManager;

    void Awake()
    {
        cursorManager = gameObject.GetComponent<CursorManager>();
    }

    // Start is called before the first frame update
    void Start()
    {
        //플레이어의 스탯이 게임오버or게임클리어 됬을 경우 실행할 이벤트 등록
        playerStats.OnGameOver += gameOver;  
        playerStats.OnGameClear += gameClear;

        gameoverScreen.SetActive(false);
        gameClearScreen.SetActive(false);
        endingView.SetActive(false);
    }

    private void OnDestroy()
    {
        // 구독 해제
        playerStats.OnGameOver -= gameOver;
        playerStats.OnGameClear -= gameClear;
    }

    //게임 오버시에 호출 (게임오버 이유와 함께 화면 활성화)
    public void gameOver(string reason){
        gameoverScreen.SetActive(true);
        reasonText.text=":"+ reason;
        cursorManager.unlockCursor();
        foreach(GameObject g in willFalseUIs){
            g.SetActive(false);
        }
       
    }

    //게임 클리어시에 호출 (게임클리어 화면 활성화)
    public void gameClear(){
        gameClearScreen.SetActive(true);
        endingView.SetActive(true);
        cursorManager.unlockCursor();
        foreach(GameObject g in willFalseUIs){
            g.SetActive(false);
        }

    }
}
