using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class WarningSignView : MonoBehaviour
{
  
    public List<RectTransform> warningSigns = new List<RectTransform>(); 
    [SerializeField]
    private float space = 25f;

    public void AddWarningSign(RectTransform message) //리스트의 뒤에 경고 메세지 추가 
    {
    
        warningSigns.Insert(warningSigns.Count, message); 

        UpdateScrollView();
    }

    public void RemoveWarningSign(RectTransform message) //리스트에서 경고 메세지 삭제
    {
        
        warningSigns.Remove(message);
     
        UpdateScrollView();
    }

    private void UpdateScrollView()
    {
        float y = -95f;
        for (int i = 0; i < warningSigns.Count; i++)
        {
            
            warningSigns[i].anchoredPosition = new Vector2(0f, y);  // 메시지를 위로부터 배치
              y -=  space;  
        }
    }
}
