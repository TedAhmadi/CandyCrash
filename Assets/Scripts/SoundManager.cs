using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Clip { Select, Clear, gameSound };

public class SoundManager : MonoBehaviour {

    public static SoundManager instance;
    private AudioSource[] sound;

    private bool Mute = false;

    // Use this for initialization
    void Start () {
        instance = GetComponent<SoundManager>();
        sound = GetComponents<AudioSource>();
    }

    public void PlaySound(Clip audioClip)
    {
        if (!Mute)
        {
            sound[(int)audioClip].Play();
        }
    }

    private void StopSound(Clip audioClip)
    {
        sound[(int)audioClip].Stop();
    }

    public void OnMute() //control the OnMute button
    {
        Mute = !Mute;
        if (Mute)
        {
            StopSound(Clip.gameSound);
            GUIManager.instance.MuteText("UnMute");
        }
        else
        {
            PlaySound(Clip.gameSound);
            GUIManager.instance.MuteText("Mute");
        }
    }
    // Update is called once per frame
    void Update () {
		
	}
}
