using System.Collections;
using UnityEngine;
using TMPro;

public class InitialMent : MonoBehaviour
{
    //인트로 영상 밑에 생성될 대사

    [SerializeField]
    private TMP_Text tmp;  

    public float typingSpeed = 0.1f;

    public void startMent1(){
        StartCoroutine(TypeMent1()); 
    }

    public void startMent2(){
        StartCoroutine(TypeMent2()); 
    }

    public void startMent3(){
        StartCoroutine(TypeMent3()); 
    }
    
    //대사를 한 글자씩 출력하도록
    private IEnumerator TypeMent1()
    {
        tmp.text = "";  
        string message1 = "하... 돈이 필요한데"; 
        foreach (char letter in message1)
        {
            tmp.text += letter;  
            yield return new WaitForSeconds(typingSpeed); 
        }
        yield return new WaitForSeconds(1f);  
       
        
    }

   
    private IEnumerator TypeMent2()
    {
        tmp.text = "";  
        string message2 = "!!!";  
        foreach (char letter in message2)
        {
            tmp.text += letter;  
            yield return new WaitForSeconds(typingSpeed); 
        }
        yield return new WaitForSeconds(1f);  
        
        
    }

 
    private IEnumerator TypeMent3()
    {
        tmp.text = "";  
        string message3 = "배달?!";  
        foreach (char letter in message3)
        {
            tmp.text += letter;  
            yield return new WaitForSeconds(typingSpeed);  
        }
    }
}