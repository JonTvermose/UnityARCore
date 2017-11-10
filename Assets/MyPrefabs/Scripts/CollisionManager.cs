using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionManager : MonoBehaviour
{

    private ParticleSystem _particleSystem;

    public AudioClip AudioClip;

    public float PickupDurationSeconds = 2.0f; // seconds

    private bool isPickedUp = false;

    // Use this for initialization
    void Start ()
    {
        _particleSystem = gameObject.GetComponentInChildren<ParticleSystem>();
    }

    // Update is called once per frame
    void Update () {
	    if (isPickedUp)
	    {
	        PickupDurationSeconds -= Time.deltaTime;
        }
        if (PickupDurationSeconds < 0)
	    {
	        gameObject.SetActive(false);
	    }
	}

    void OnTriggerEnter(Collider other)
    {
        // Object has been touched by the player
        if (other.gameObject.CompareTag("Player"))
        {
            // Vibrate the phone
            Handheld.Vibrate();

            // Play a sound
            SoundManager.instance.PlayAudioClip(AudioClip);

            if (_particleSystem != null)
            {
                // Play animation
                _particleSystem.Play();
            }

            // start the timer countdown for gameobject deactivation
            isPickedUp = true;
            GameManager.GameManager_instance.PickupTrigger();
        }
    }
}
