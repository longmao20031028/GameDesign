using System.Collections;
using System.Collections.Generic;
using System.Net.Http;
using UnityEngine;

public class Sound : MonoBehaviour 
{
    AudioSource audioSource;
    public AudioClip clip1;
    public AudioClip clip2;
    // Start is called before the first frame update
    void Start()
    {
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        clip1 = Resources.Load<AudioClip>("sound");
        clip2 = Resources.Load<AudioClip>("wow");
        
        audioSource.transform.position = Camera.main.transform.position;
    }

    public void playSound(int n)
    {
        //switch(n)
        //{
        //    case 1: 
                audioSource.PlayOneShot(clip1);
        //        break;
        //    case 2: 
        //        audioSource.PlayOneShot(clip2,0.5f);
        //        break;
        //}
        
    }
   
}
