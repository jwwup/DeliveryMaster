using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Portal : MonoBehaviour
{
    
    public Transform targetPortal; // 텔레포트할 포탈 
    private CharacterControlable character;
    
    public void Teleport(GameObject target)
    {
        target.transform.position = targetPortal.position;
        character = target.GetComponent<CharacterControlable>();
        if(gameObject.CompareTag("PortalIn")) //내부로 들어가는 포탈이면 카메라의 위치를 바꿔 1인칭으로 
        {
           character.camera.localPosition = new Vector3(0,0,0);
        }
        else //외부로 나가는 포탈이면 카메라의 위치를 바꿔 3인칭으로 
        {
            character.camera.localPosition = new Vector3(0,1,-2.7f);
        }
        
    }
}
