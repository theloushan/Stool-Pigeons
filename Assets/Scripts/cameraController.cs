using UnityEngine;
using System.Collections;

// This script handles the switching between different menus
// To keep the game without loading screens, it simply uses multiple cameras and canvases to simulate there being different menus being loaded
// Each menu is made on the same basic background, but has different UI elements based off of the specific camera queued up to that camera
// However, Unity breaks when using more than one canvas at a time, so shutting them all off and turning one specific one on is required for it to run properly
// This script is mainly used to have specific behavior for each button to call, thus necessitating a specific function for each button
// In addition, this handles the ending of the application, given that that is a specific button function as well
public class cameraController : MonoBehaviour {

	public Camera [] cams; // Storage array for all the different cameras in the game
	public Canvas[] canvas; //Storage array for all the different UI canvases in the game
	public AudioClip menuMusic; // Storage variable for the music file to play for the menu music
	AudioSource menuAudio; // Storage variable for the AudioSource component so I don't have to constantly use GetComponent

	void Start () {
		menuAudio = GetComponent <AudioSource> (); // Store the AudioSource
	}

	// When the game wants return back to the main menu, turn on just the Main Menu camera and the Main Menu canvas
	public void switchToMenu () {
		cams [0].enabled = true;
		cams [1].enabled = false;
		cams [2].enabled = false;
		cams [3].enabled = false;

		canvas [0].enabled = true;
		canvas [1].enabled = false;
		canvas [2].enabled = false;
	}

	// When the game wants to go to the credits screen, turn on just the Credits camera and the Credits canvas
	public void switchToCredits () {
		cams [0].enabled = false;
		cams [1].enabled = true;
		cams [2].enabled = false;
		cams [3].enabled = false;

		canvas [0].enabled = false;
		canvas [1].enabled = true;
		canvas [2].enabled = false;
	}

	// When the game wants to go to the character select screen, turn on just the Characters camera and the Characters canvas
	public void switchToCharacters () {
		cams [0].enabled = false;
		cams [1].enabled = false;
		cams [2].enabled = true;
		cams [3].enabled = false;

		canvas [0].enabled = false;
		canvas [1].enabled = false;
		canvas [2].enabled = true;

		// Reset the music back to the menu music if returning from a game
		if (menuAudio.clip != menuMusic) {
			menuAudio.clip = menuMusic;
			menuAudio.Play ();
		}
	}

	// When the game wants to start gameplay, turn on just the Game camera and turn off all UI canvases
	public void switchToGame () {
		cams [0].enabled = false;
		cams [1].enabled = false;
		cams [2].enabled = false;
		cams [3].enabled = true;

		canvas [0].enabled = false;
		canvas [1].enabled = false;
		canvas [2].enabled = false;
	}

	// When the game is meant to end, tell the application to quit
	public void endGame () {
		Application.Quit ();
	}
}
