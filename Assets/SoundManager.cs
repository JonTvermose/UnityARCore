﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{

    public static SoundManager instance = null;

    private AudioSource _audioSource;

	// Use this for initialization
	void Start ()
	{
	    if (instance == null)
	        instance = this;
	    _audioSource = gameObject.GetComponent<AudioSource>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void PlayAudioClip(AudioClip clip)
    {
        _audioSource.clip = clip;
        _audioSource.loop = false;
        _audioSource.Play();
    }
}