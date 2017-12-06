using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnowManCollisionManager : MonoBehaviour {

    public AudioClip AudioClip;

    public ParticleSystem Snow;

    public float PickupDurationSeconds = 2.0f; // seconds

    private bool isPickedUp = false;

    // Use this for initialization
    void Start () {
		
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

            // Play the snow particle system at the player position
            Instantiate(Snow, other.gameObject.transform.position + new Vector3(0, 1, 0), Quaternion.identity, other.gameObject.transform);

            // start the timer countdown for gameobject deactivation
            isPickedUp = true;
            GameManager.GameManager_instance.PickupSnowmanTrigger();
        }
    }
}
