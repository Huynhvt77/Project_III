using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;
    public AudioSource[] soundEffects;

    public bool isOn = true;

    private void Awake() 
    {
        if (instance == null){
            instance = this;
            DontDestroyOnLoad(gameObject);
        }    
        else 
        {
            Destroy(gameObject);
        }
    }

    public void PlaySFX(int sfxNumber)
    {
        soundEffects[sfxNumber].Stop();
        soundEffects[sfxNumber].Play();
    }
}
