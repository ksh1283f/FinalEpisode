using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public enum E_SceneType
{
    None,
    Start,
    Lobby,
    Battle,
    Opening,
    Ending,
}

public class SoundManager : Singletone<SoundManager>
{
    [SerializeField]private E_SceneType sceneType = E_SceneType.None;
    public E_SceneType SceneType
    {
        get { return sceneType;}
        set
        {
            if (value == sceneType)
                return;

            sceneType = value;
            switch (sceneType)
            {
                case E_SceneType.Start:
                case E_SceneType.Opening:
                    StopSound();
                    PresentSound = StartBGM;
                    PlaySound();
                    break;

                case E_SceneType.Battle:
                case E_SceneType.Ending:
                    StopSound();
                    PresentSound = BattleBGM;
                    PlaySound();
                    break;
    
                case E_SceneType.Lobby:
                    StopSound();
                    PresentSound = LobbyBGM;
                    PlaySound();
                    break;   
            }
        }
    }

    [SerializeField] AudioSource StartBGM;
    [SerializeField] AudioSource LobbyBGM;
    [SerializeField] AudioSource BattleBGM;
    [SerializeField] List<AudioSource> soundList = new List<AudioSource>();
    public AudioSource PresentSound = null;
    public AudioSource buttonEffectSound;
    public AudioSource explosionSound;
    public AudioSource synthEffectSound;
    public AudioSource screamSound;
    public AudioSource playerHitSound;

    void Start()
    {
        SceneType = E_SceneType.Start;
        soundList = GetComponents<AudioSource>().ToList();
    }

    public void SetSoundVolume(float value)
    {
        if(PresentSound == null)
            return;

        for (int i = 0; i < soundList.Count; i++)
            soundList[i].volume = value;
    }

    public void PlaySound()
    {
        if(PresentSound == null)
            return;

        StopSound();
        PresentSound.Play();
    }

    public void StopSound()
    {
        if(PresentSound == null)
            return;

        PresentSound.Stop();
    }

    public void PlayButtonSound()
    {
        if(buttonEffectSound == null)
            return;

        buttonEffectSound.Play();
    }

    public void PlayExplosionSound()
    {
        if (explosionSound == null)
            return;

        explosionSound.Play();
    }

    public void PlaySynthEffectSound()
    {
        if (synthEffectSound == null)
            return;

        synthEffectSound.Play();
    }

    public void PlayScreamSound()
    {
        if (screamSound == null)
            return;

        screamSound.Play();
    }

    public void PlayHitSound()
    {
        if (playerHitSound == null)
            return;

        playerHitSound.Play();
    }
}
