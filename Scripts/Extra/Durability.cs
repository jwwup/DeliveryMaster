using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


public class Durability : MonoBehaviour
{

    [SerializeField]
    private GameObject text;
    [SerializeField]
    private GameObject player;

    // Start is called before the first frame update
    void Start()
    {
        text.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
       FacePlayer();
    }

    // 플레이어 방향을 보고 있도록
    void FacePlayer()
    {
        Vector3 directionToPlayer = player.transform.position - text.transform.position;
        directionToPlayer.y = 0;
        Quaternion rotation = Quaternion.LookRotation(-1*directionToPlayer);
        text.transform.rotation = rotation;
    }

    //플레이어가 근처에 올때만 텍스트 보이게 
    void OnTriggerEnter(Collider collider)
    {
        if(collider.CompareTag("Character"))
        {
            text.SetActive(true);
        }
   }



   void OnTriggerExit(Collider collider)
   {
        if(collider.CompareTag("Character"))
        {
            text.SetActive(false);
        }
   }
}
