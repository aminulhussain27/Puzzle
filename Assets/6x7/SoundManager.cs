using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class SoundManager : MonoBehaviour
{
	static GameObject soundObj = null;

	private static SoundManager soundManager = null;

	public static SoundManager Instance ()
	{		
		if (soundManager == null)
		{
			if (GameObject.Find ("SoundManager") == null)
			{
				soundObj = new GameObject ("Sounds");
			}
			soundManager = GameObject.Find ("SoundManager").GetComponent<SoundManager> ();
		}
		return soundManager;
	}

	GameObject soundObject = null;
	public AudioClip[] musicClips;

	public void playSound (SoundManager.SOUND_ID id, float volume =1f)
	{
        soundObject = new  GameObject ("Sound");
       
		soundObject.transform.SetParent (transform);
		
		soundObject.AddComponent<AudioSource> ();
		AudioSource audioSource = soundObject.GetComponent<AudioSource> ();
		audioSource.clip = musicClips [(int)id];
		audioSource.Play ();

		if(volume != 1f)
		{
			audioSource.volume = volume;
		}

		//Destroying the sound object after fully playing
		Destroy (soundObject, audioSource.clip.length);
	}

   
    private AudioSource GetAudioSource(int soundID)
    {
        string soundClipName;
        AudioSource _audioSource;

        GameObject soundObjectRoot =  GameObject.Find ("Sounds").gameObject;

        for (int i = 0; i < soundObjectRoot.transform.childCount; i++) 
        {
            soundClipName = "Sound_" + i.ToString();

            if (soundObjectRoot.transform.GetChild(i).name.ToString() == soundClipName)
            {
                _audioSource = soundObjectRoot.transform.GetChild(i).GetComponent<AudioSource>();
                return _audioSource;
            }
            else
            {
                soundClipName = "Sound_" + soundID.ToString();
                if (soundObjectRoot.transform.GetChild(i).name.ToString() == soundClipName)
                {
                    _audioSource = soundObjectRoot.transform.GetChild(i).GetComponent<AudioSource>();
                    return _audioSource;
                }
                return null;
            }
        }
        return null;
    }
		
	public enum SOUND_ID
	{
		NONE = -1,
		LOOP_BACKGROUND = 0,
		SFX_COIN_PICKUP = 1,
		SFX_COLLIDE_WITH_ENEMY = 2,
		SFX_COLLIDE_WITH_BLOCKER = 3,
		SFX_END = 4,

	}
}
