using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class SoundManager : MonoBehaviour
{
	static GameObject soundObj = null;

	private static SoundManager soundManager = null;
	//Creating singleton for use in different positions and for making sure only one instace is created
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
	public AudioClip[] musicClips;//In sce attaching the sound clips


	public void playSound (SoundManager.SOUND_ID id, float volume =1f)
	{
		//A new gameobject is creating and attaching audioSource here
        soundObject = new  GameObject ("Sound");
       
		soundObject.transform.SetParent (transform);
		
		soundObject.AddComponent<AudioSource> ();
		AudioSource audioSource = soundObject.GetComponent<AudioSource> ();
		audioSource.clip = musicClips [(int)id];
		audioSource.Play ();

		//If in some place i need custom volume
		if(volume != 1f)
		{
			audioSource.volume = volume;
		}

		//Destroying the sound object after fully playing
		Destroy (soundObject, audioSource.clip.length);
	}
		
		
	//These are the Clip Id
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
