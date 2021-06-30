using UnityEngine;
using System.Collections;

// This script handles the sound effects of the game's menus
// This is done by affecting the given object's audio source
// Just a few simple functions that change to a specific sound effect and then immediately play said sound effect
// Used in the same manner of cameraController, behaving based off of specific button presses and having a specific function in this script for each type of button
public class sfxController : MonoBehaviour {

	AudioSource menuSfx; // Storage variable for the AudioSource component so I don't have to constantly use GetComponent
	public AudioClip confirmSound; // Storage variable for the confirmation sound effect
	public AudioClip backSound; // Storage variable for the back sound effect
	public AudioClip startSound; // Storage variable for the start sound effect


	// Use this for initialization
	void Start () {
		menuSfx = GetComponent<AudioSource> (); // Store the AudioSource component so that it can be affected later
	}

	// If a button is clicked to confirm a menu element, play the appropriate sound effect
	public void confirmClick () {
		menuSfx.clip = confirmSound;
		menuSfx.Play ();
	}

	// If a button is clicked to go back from a menu element, play the appropriate sound effect
	public void backClick () {
		menuSfx.clip = backSound;
		menuSfx.Play ();
	}

	// If a button is clicked to start the game, play the appropriate sound effect
	public void startClick () {
		menuSfx.clip = startSound;
		menuSfx.Play ();
	}
}
