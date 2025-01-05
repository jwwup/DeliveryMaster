using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Mechanic : MonoBehaviour
{

    private Animator animator;
    private Transform rightHandTransform;
    [SerializeField]
    private GameObject Wrench;

    [SerializeField]
    private CursorManager cursorManager;

    [SerializeField]
    private VehicleControlable vehicle;
    [SerializeField]
    private PlayerStats playerStats;

    private string text;    //질문
    private string text2;   //거절했을때
    private string text3;   //수락했을때
    private string text4; //돈이 없을 경우
    public TMP_Text askingText;
    public TMP_Text noText;
    public TMP_Text yesText;
    public TMP_Text noMoneyText;
    private float delay = 0.125f;
    [SerializeField]
    private GameObject yesButton;
    [SerializeField]
    private GameObject noButton;
    

    private bool asked; //이미 대화중일때 중복해서 대화를 시작하지 않도록 


    void Awake()
    {
        animator = GetComponent<Animator>();
    }

    // Start is called before the first frame update
    void Start()
    {
        //렌치 오브젝트 오른손에 고정
        rightHandTransform = animator.GetBoneTransform(HumanBodyBones.RightMiddleProximal);
        Wrench.transform.position = rightHandTransform.position;
        Wrench.transform.parent = rightHandTransform;
        Wrench.transform.localPosition = new Vector3(-0.02f, 0.002f, 0.043f);


        text = askingText.text.ToString();
        askingText.text = " ";

        text2 = noText.text.ToString();
        noText.text = " ";

        text3 = yesText.text.ToString();
        yesText.text = " ";

        text4 = noMoneyText.text.ToString();
        noMoneyText.text = " ";

        askingText.gameObject.SetActive(false);
        noText.gameObject.SetActive(false);
        yesText.gameObject.SetActive(false);
        noMoneyText.gameObject.SetActive(false);
        
    }

    public IEnumerator askTextPrint() //플레이어가 대화 시작 했을 경우 질문 텍스트 출력
    {
        asked=true; //새로운 대화 시작 못하도록 
        int count = 0;
        askingText.gameObject.SetActive(true);
        askingText.text = " ";
        yesButton.SetActive(true);
        noButton.SetActive(true);

        while (count != text.Length) //한 글자씩 출력
        {
            if (count < text.Length)
            {
                askingText.text += text[count].ToString();
                count++;
            }

            yield return new WaitForSeconds(delay);
        }
    }


    public IEnumerator noTextPrint() //플레이어가 거절 버튼을 눌렀을 경우의 텍스트 출력
    {
        int count = 0;
        noText.gameObject.SetActive(true);
        while (count != text2.Length)
        {
            if (count < text2.Length)
            {
                noText.text += text2[count].ToString();
                count++;
            }

            yield return new WaitForSeconds(delay);
        }

        yield return new WaitForSeconds(1f);
        noText.text = " ";
        noText.gameObject.SetActive(false);
        

        asked=false;
    }


    public IEnumerator yesTextPrint() //플레이어가 수락 버튼을 눌렀을 경우의 텍스트 출력
    {
        int count = 0;
        yesText.gameObject.SetActive(true);
        while (count != text3.Length)
        {
            if (count < text3.Length)
            {
                yesText.text += text3[count].ToString();
                count++;
            }

            yield return new WaitForSeconds(delay);
        }

        yield return new WaitForSeconds(1f);
        yesText.text = " ";
        yesText.gameObject.SetActive(false);
        

        asked=false;
    }

    public IEnumerator noMoney() //플레이어가 수락을 눌렀는데 돈이 없을 경우 출력
    {
        int count = 0;
        noMoneyText.gameObject.SetActive(true);
        while (count != text4.Length)
        {
            if (count < text4.Length)
            {
                noMoneyText.text += text4[count].ToString();
                count++;
            }

            yield return new WaitForSeconds(delay);
        }

        yield return new WaitForSeconds(1f);
        noMoneyText.text = " ";
        noMoneyText.gameObject.SetActive(false);
        

        asked=false;
    }



    public void startTalking()
    {
        if(!asked)  //진행중인 대화가 없을 경우 대화 시작
        {
            StartCoroutine(askTextPrint());
        }
    }


    public void OnYesButtonClicked() //플레이어가 수락 버튼을 눌렀을 경우 호출
    {
        yesButton.SetActive(false);
        noButton.SetActive(false);
        askingText.text = " ";
        askingText.gameObject.SetActive(false);
        
        
        animator.SetTrigger("Annoy");
        cursorManager.lockCursor();

        if(vehicle!=null&&playerStats.money>=10.0) //돈이 있을 경우 오토바이의 내구도 채우기
        {   
            StartCoroutine(yesTextPrint());
            playerStats.changeMoney(-6.0f);
            vehicle.durability = 10;
            vehicle.UpdateDurabilityText();
        }else
        {
            StartCoroutine(noMoney()); // 돈이 없을 경우
        }

    }

    public void OnNoButtonClicked() //플레이어가 거절 버튼을 눌렀을 경우 호출
    {

        yesButton.SetActive(false);
        noButton.SetActive(false);
        askingText.text = " ";

        askingText.gameObject.SetActive(false);
        
        StartCoroutine(noTextPrint());
        animator.SetTrigger("Angry");
        cursorManager.lockCursor();
    }



}
