using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    private AudioSource audioSource;
    public List<AudioClip> clips;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void PlayAudioClip(string clipName){
        foreach (var item in clips){
            if(item.name == clipName){
                audioSource.clip = item;
                audioSource.Play();
            }
        }
    }
}
