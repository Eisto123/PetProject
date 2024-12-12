using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    private AudioSource audioSource;
    public List<AudioClip> clips;
    private float pitch = 1;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void PlayAudioClip(string clipName){
        foreach (var item in clips){
            if(item.name == clipName){
                audioSource.clip = item;
                audioSource.pitch = pitch;
                audioSource.Play();
            }
        }
    }
    public void PlayAudioWithRandomPitch(string clipName){
        foreach (var item in clips){
            if(item.name == clipName){
                audioSource.clip = item;
                if(item.name == "roar"){
                    audioSource.pitch = Random.Range(1.2f, 1.6f);
                }
                else{
                    audioSource.pitch = Random.Range(0.8f, 1.2f);
                }
                audioSource.Play();
            }
        }
    }
}
