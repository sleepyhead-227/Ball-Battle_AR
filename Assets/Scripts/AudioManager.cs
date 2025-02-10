using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public AudioSource BGMSource;
    public AudioSource SFXSource;

    public AudioClip mainmenuBGM;
    public AudioClip playGameBGM;

    public AudioClip whistleSFX;
    public AudioClip kickSFX;
    public AudioClip fallSFX;
    public AudioClip applauseSFX;
    public AudioClip booSFX;
    public AudioClip clickSFX;

    public float BGMVolume = 1.0f;
    public float SFXVolume = 1.0f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PlayBGM(string sceneName)
    {
        switch (sceneName)
        {
            case "Mainmenu":
                BGMSource.clip = mainmenuBGM;
                break;
            case "PlayGame":
                BGMSource.clip = playGameBGM;
                break;
        }

        BGMSource.volume = BGMVolume;
        BGMSource.Play();
    }

    public void PlaySFX(string sfxName)
    {
        switch (sfxName)
        {
            case "whistle":
                SFXSource.clip = whistleSFX;
                break;
            case "kick":
                SFXSource.clip = kickSFX;
                break;
            case "fall":
                SFXSource.clip = fallSFX;
                break;
            case "applause":
                SFXSource.clip = applauseSFX;
                break;
            case "boo":
                SFXSource.clip = booSFX;
                break;
            case "click":
                SFXSource.clip = clickSFX;
                break;
        }

        SFXSource.volume = SFXVolume;
        SFXSource.Play();
    }
}
