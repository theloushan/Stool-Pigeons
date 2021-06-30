using UnityEngine;
using System.Collections;

// This script controls the behavior of the explosion object that spawns when a player dies
// Since the explosion does nothing but animate before being deleted, it's a relatively simple script
// It plays the explosion sound effect when the object spawns and then plays the explosion animation
// Once the animation plays in its full, it waits for 3 seconds before deleting the object from the game
public class explosionController : MonoBehaviour {

	Animator anim; // Storage for the Animator component so I don't have to constantly use GetComponent
	bool shouldDelete = false; // Determines if the object should be deleted
	int animationTimer = 3; // Number of seconds until the object is cleared from the stage
	int timeCounter = 0; // Number of frames that have passed since the projectile has finished animating
	AudioSource explosionAudio; // Storage variable for the AudioSource component so I don't have to constantly use GetComponent
	public AudioClip explosionSound; // Storage variable for the explosion sound effect

	// Use this for initialization
	void Start () {
		anim = GetComponent<Animator> (); // Store the Animator
		animationTimer *= 60; // Multiply the time to keep on hte field by the number of frames per second
		explosionAudio = GetComponent<AudioSource> (); // Store the AudioSource
		explosionAudio.clip = explosionSound; // Set the explosion sound effect to the active sound
		explosionAudio.Play (); // Play the sound effect
	}
	
	// Update is called once per frame
	void Update () {
		// Check to see if the explosion has played its full animation and set the status appropriately
		if (anim.GetCurrentAnimatorStateInfo (0).IsName ("explosionEnd") == true) {
			shouldDelete = true;
		}

		// Check to see if the explosion should be deleted, if it should, increment the timer up by 1
		if (shouldDelete == true) {
			timeCounter++;
		}

		// If the number of frames to wait for has passed, delete the object
		if (timeCounter == animationTimer) {
			Destroy (gameObject);
		}
	}
}
