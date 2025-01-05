using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class CompletedScrollViewController : ScrollviewController
{
    public override void HandleMessageSpecificLogic(RectTransform message)
    {
        // 특별한 로직이 필요하지 않음
    }

    // 리스트에 있는 모든 메세지들 전부 비활성화 후 활성화 되기 전 원래의 위치로 보냄
    public void deleteAllMessages()
    {
        foreach (var rectTransform in requestMessages)
        {
            if (rectTransform != null)
            {
                rectTransform.gameObject.SetActive(false);
                toNextScreen(rectTransform);
            }
        }
        requestMessages.Clear();
        UpdateScrollView();
    }
}