using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionManager : MonoBehaviour
{

    public GameObject ParticleSystem;

    public AudioClip AudioClip;

    public float PickupDurationSeconds = 2; // seconds

    private bool isPickedUp = false;

    // Use this for initialization
    void Start ()
    {
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

            // Play animation
            Instantiate(ParticleSystem, transform.position, transform.rotation);

            // start the timer countdown for gameobject deactivation
            isPickedUp = true;
        }
    }
}
