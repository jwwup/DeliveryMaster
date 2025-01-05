using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BellKnockCheck : MonoBehaviour
{
    // 플레이어의 노크와 초인종 유무 체크

    [SerializeField]
    private DeliveryHome home;
    [SerializeField]
    private AudioClip bellSound;
    [SerializeField]
    private AudioClip knockSound;
 

    public void knockDoor(){
        if(home!=null){
            home.doorKnocked=true;
            SoundManager.Instance.PlaySound(knockSound);
        }
   
    }

    public void ringBell(){
        if(home!=null){
            home.ringBell=true;
            SoundManager.Instance.PlaySound(knockSound);
        }
     
    }

}
