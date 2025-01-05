using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class ScrollviewController : MonoBehaviour
{
    public ScrollRect scrollRect; //스크롤 가능한 영역 관리
    public float space = 20f;  // 메세지 간의 간격
    public List<RectTransform> requestMessages = new List<RectTransform>(); // 화면에 표시될 메세지들
    public ScrollviewController nextScrollViewController; // 메세지를 넘길 다음 화면 지정

    void Awake()
    {
        scrollRect = GetComponent<ScrollRect>();
    }

    public abstract void HandleMessageSpecificLogic(RectTransform message); // 각 화면마다 추가로 필요한 로직

    //지정된 다음화면으로 해당 메세지를 보내기
    public void toNextScreen(RectTransform message)
    {
        if (requestMessages.Remove(message))
        {
            nextScrollViewController.AddDeliverMessage(message);
            var deliveryMessage = message.GetComponent<DeliveryMessage>();
            if (deliveryMessage != null)
                deliveryMessage.currentScreen = nextScrollViewController;

            UpdateScrollView(); //보낸 뒤 다시 화면 업데이트
        }
    }

    // 리스트에 메세지 추가 후 스크롤 뷰에 넣기
    public void AddDeliverMessage(RectTransform message)
    {
        requestMessages.Add(message);
        message.SetParent(scrollRect.content);
        HandleMessageSpecificLogic(message);
        UpdateScrollView();
    }

    //메세지를 리스트에서 뺀 뒤 비활성화
    public void DeleteDeliveryMessage(DeliveryMessage message)
    {
        var rectTransform = message.GetComponent<RectTransform>();
        if (rectTransform != null && requestMessages.Remove(rectTransform))
        {
            rectTransform.gameObject.SetActive(false);
            UpdateScrollView(); // 메세지를 비활성화하고 다시 화면 업데이트
        }
    }

    // 리스트에 있는 메세지들을 간격을 두고 밑으로 쌓이도록 업데이트
    protected virtual void UpdateScrollView()
    {
        float y = 0f;
        foreach (var message in requestMessages)
        {
            if (message == null) continue;
            message.anchoredPosition = new Vector2(5f, -y);
            y += message.sizeDelta.y + space;
        }

        scrollRect.content.sizeDelta = new Vector2(scrollRect.content.sizeDelta.x, y);
    }
}