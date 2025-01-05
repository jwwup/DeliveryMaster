using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : Singleton<SoundManager>
{
    public AudioSource source;
    
    public void PlaySound(AudioClip _sound)
    {
        if (_sound == null)
        {
            return;
        }

       
        source.PlayOneShot(_sound); //다른 사운드를 중단하지 않고 오디오 클립 재생
    }

    private void Awake()
    {
        
        source = GetComponent<AudioSource>();
        
    }
    
}
