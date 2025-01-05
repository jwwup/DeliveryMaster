using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ProgressScrollViewController : ScrollviewController
{
    //진행 스크롤뷰에 왔을때 타이머 시작
    public override void HandleMessageSpecificLogic(RectTransform message)
    {
        var deliveryMessage = message.GetComponent<DeliveryMessage>();
        if (deliveryMessage != null)
        {
            deliveryMessage.Timer.SetActive(true);
            deliveryMessage.startTimer();
        }
    }
}