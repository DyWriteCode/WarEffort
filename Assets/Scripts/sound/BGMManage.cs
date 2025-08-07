using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BGMManage
{
    private AudioSource audio;

    private Dictionary<string, AudioClip> bgmClips;

    private bool isStop;

    public bool IsStop
    {
        get
        {
            return isStop;
        }
        set
        {
            isStop = value;
            if (isStop == true)
            {
                audio.Pause();
            }
            else
            {
                audio.Play();
            }
        }
    }
    private float bgmVolume;

    public float BgmVolume
    {
        get
        {
            return bgmVolume;
        }
        set
        {
            bgmVolume = value;
            audio.volume = bgmVolume;
        }
    }
    private float effectVolume;

    public float EffectVolume
    {
        get
        {
            return effectVolume;
        }
        set
        {
            effectVolume = value;
        }
    }
    public BGMManage()
    {
        bgmClips = new Dictionary<string, AudioClip>();
        audio = GameObject.Find("game").GetComponent<AudioSource>();

        //��ʼ��ֵ
        IsStop = false;
        BgmVolume = 1;
        EffectVolume = 1;
    }

    public void playBGM(string name)
    {
        if (isStop == true)
        {
            return;
        }
        if (!bgmClips.ContainsKey(name))
        {
            AudioClip audioClip=Resources.Load<AudioClip>($"Sounds/{name}");
            bgmClips.Add(name, audioClip);
        }
        audio.clip = bgmClips[name];
        audio.Play();
    }

    public void PlayEffect(string name,Vector3 pos)
    {
        if(isStop == true)
        {
            return;
        }
        AudioClip clip = null;
        if (bgmClips.ContainsKey(name) == false)
        {
            clip=Resources.Load<AudioClip>($"Sounds/{name}");
            bgmClips.Add(name,clip);
        }
        AudioSource.PlayClipAtPoint(bgmClips[name], pos);
    }
}
