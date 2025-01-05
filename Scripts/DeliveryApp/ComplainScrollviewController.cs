using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ComplainScrollviewController : MonoBehaviour
{
    public ScrollRect scrollRect;
    public List<RectTransform> complainMessages = new List<RectTransform>(); 
    public float space = 20f;
    private int maxMessages = 7;  // 메시지의 최대 개수


    void Awake()
    {
        scrollRect = GetComponent<ScrollRect>();
    }

    //스크롤뷰에 메세지 추가
    public void AddDeliverMessage(RectTransform message)
    {
        message.SetParent(scrollRect.content, false); 
        // 새로 추가된 컴플레인 메세지 맨 앞으로 추가
        complainMessages.Insert(0, message);

        // 메시지가 최대 개수를 초과하지 않도록 
        if (complainMessages.Count > maxMessages)
        {
            RectTransform lastMessage = complainMessages[complainMessages.Count - 1];
            complainMessages.RemoveAt(complainMessages.Count - 1);  
            Destroy(lastMessage.gameObject);  
        }

        UpdateScrollView();
    }

    private void UpdateScrollView()
    {
        float y = 0f;
        for (int i = 0; i < complainMessages.Count; i++)
        {
            complainMessages[i].anchoredPosition = new Vector2(0f, y);  // 메세지를 위에서부터 쌓기
            y -= complainMessages[i].sizeDelta.y + space;  
        }

        // 메세지의 개수에 따라 스크롤뷰의 콘텐츠 높이를 업데이트
        scrollRect.content.sizeDelta = new Vector2(scrollRect.content.sizeDelta.x, Mathf.Abs(y));
    }
}
