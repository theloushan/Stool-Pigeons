using UnityEngine;
using System.Collections;

// This script controls the game's behavior in running the game loop
// It spawns the characters and stage based off of what the player has chosen
// Also clears out the stage and players once the game has ended
// It interprets what characters were chosen and assigns them to the appropriate players for controls
// Switches the music based on if the player is currently in the menus or in the game proper
// Controls the stage select display based off of what stage is currently chosen
// Determines whether or not the game is being played or can even be started based off specific conditions
// Game can only be started if there have been at least two players chosen once the start button has been pressed
// Game ends when there is only one player left standing
public class gameController : MonoBehaviour {

	cameraController cameras; // Storage for the cameraController script so I don't have to constantly use GetComponent
	public GameObject[] characters; // Public variable that stores all the potential characters to choose from
	public GameObject[] stages; // Public variable that stores all the potential stages to choose from
	int totalStages = 0; // Storage for the number of playable stages
	int totalCharacters = 0; // Storage for the number of playable characters
	int numPlayers = 0; // Storage for the number of active players in-game
	int maxPlayers = 4; // Storage for the number of maximum players that can be in a single game
	bool isPlaying = false; // Determines if the game is currently being actively played
	bool canStart = false; // Determines if the game can be started yet or not
	GameObject player1; // Stores the character player 1 has chosen
	GameObject player2; // Stores the character player 2 has chosen
	GameObject player3; // Stores the character player 3 has chosen
	GameObject player4; // Stores the character player 4 has chosen
	GameObject createPlayer1; // Stores the character spawned for player 1
	GameObject createPlayer2; // Stores the character spawned for player 2
	GameObject createPlayer3; // Stores the character spawned for player 3
	GameObject createPlayer4; // Stores the character spawned for player 4
	public GameObject chosenStage; // Public variable that stores the chosen stage, is public so can be used elsewhere
	GameObject createStage; // Stores the stage spawned for the game
	int currentStageNum = 0; // Stores the position in the array for the current stage
	public AudioClip stageMusic; // Public variable that determines the music to play during gameplay
	AudioSource gameAudio; // Storage for the AudioSource component so I don't have to constantly use GetComponent
	public int endDelay = 3; // Public variable that determines the time needed to wait before ending the game
	int endCounter = 0; // Number of frames that have passed since the game's ending status began
	public GameObject fountainDisplay; // Public variable for the image to display on the stage select for the fountain
	public GameObject deckDisplay; // Public variable for the image to display on the stage select for the parking deck
	public GameObject sfxManager; // Public variable for the child object that controls the game's sound effects

	// Use this for initialization
	void Start () {
		cameras = GetComponent <cameraController> (); // Store the cameraController
		totalStages = stages.Length; // Determine the number of playable stages
		totalCharacters = characters.Length; // Determine the number of playable characters
		gameAudio = GetComponent <AudioSource> (); // Store the AudioSource
		endDelay *= 60; // Multiply the time to wait to end the game by the number of frames per second
		fountainDisplay.SetActive (true); // Set the current stage select display to be the Fountain by default
		deckDisplay.SetActive (false); // Turn off the Deck for the stage select display to make sure it doesn't appear instead
	}
	
	// Update is called once per fixed frame-rate frame
	void FixedUpdate () {
		// Check to see if the game is in the "currently being played" status
		if (isPlaying == true) {
			// Since the gameManger creates all game objects as its children, we can check the number of children to end the game
			// It always has the sfx manager and the game manager under it, the stage and the players spawn when the game starts
			// When there is only 1 player left there will be only 4 children, so when that number is hit, we can end the game
			if (this.transform.childCount == 4) {
				endCounter++; // Increment the end game counter by 1

				if (endCounter == endDelay) {
					cameras.switchToCharacters (); // Switch back to the character select screen

					// Determine what player hasn't been killed and destroy that player object
					if (createPlayer1 != null) {
						Destroy (createPlayer1);
					} else if (createPlayer2 != null) {
						Destroy (createPlayer2);
					} else if (createPlayer3 != null) {
						Destroy (createPlayer3);
					} else if (createPlayer4 != null) {
						Destroy (createPlayer4);
					}

					Destroy (createStage); // Destroy the stage object
					numPlayers = 0; // Reset the number of players in the game
					endCounter = 0; // Reset the number of frames to wait until ending the game
					isPlaying = false; // Set the isPlaying status to false
					canStart = false; // Set the canStart status to false
				}
			}
		} else {
			// Check to see if there are at least two players and set the canStart status appropriately
			if (numPlayers < 2) {
				canStart = false;
			} else {
				canStart = true;
			}
		}
	}

	// Function that determines if the game is in a state where it can be started and starts the game if it can
	public void startGame () {
		// Check to see if the game can be started
		if (canStart == true) {
			sfxManager.GetComponent<sfxController> ().startClick (); // Call the start game sound effect function

			// Create the level object and make it a child of the gameManager
			createStage = Instantiate (chosenStage, this.transform.position, this.transform.rotation) as GameObject;
			createStage.transform.parent = this.transform;

			// Loop through and spawn each player in a specific location based off of how many players there are
			for (int i = 1; i <= numPlayers; i++) {
				if (i == 1) {
					Vector3 spawnPosition1 = new Vector3 (this.transform.position.x - 10, this.transform.position.y + 4);
					createPlayer1 = Instantiate (player1, spawnPosition1, this.transform.rotation) as GameObject;
					createPlayer1.transform.parent = this.transform;
					createPlayer1.GetComponent<playerController> ().playerNum = 1;
				} else if (i == 2) {
					Vector3 spawnPosition2 = new Vector3 (this.transform.position.x - 5, this.transform.position.y + 4);
					createPlayer2 = Instantiate (player2, spawnPosition2, this.transform.rotation) as GameObject;
					createPlayer2.transform.parent = this.transform;
					createPlayer2.GetComponent<playerController> ().playerNum = 2;
				} else if (i == 3) {
					Vector3 spawnPosition3 = new Vector3 (this.transform.position.x + 5, this.transform.position.y + 4);
					createPlayer3 = Instantiate (player3, spawnPosition3, this.transform.rotation) as GameObject;
					createPlayer3.transform.parent = this.transform;
					createPlayer3.GetComponent<playerController> ().playerNum = 3;
				} else if (i == 4) {
					Vector3 spawnPosition4 = new Vector3 (this.transform.position.x + 10, this.transform.position.y + 4);
					createPlayer4 = Instantiate (player4, spawnPosition4, this.transform.rotation) as GameObject;
					createPlayer4.transform.parent = this.transform;
					createPlayer4.GetComponent<playerController> ().playerNum = 4;
				}
			}

			gameAudio.clip = stageMusic; // Set the music to the stage music
			gameAudio.Play (); // Play the stage music

			isPlaying = true; // Set the game's playing status to true

			cameras.switchToGame (); // Switch to the game camera
		}
	}

	// Function that sets the stage to play
	void stagePicker () {
		chosenStage = stages [currentStageNum]; // Set the chosen stage to the given stage in the storage array

		// Check to see what stage was chosen and change the stage select display to the appropriate one
		if (currentStageNum == 0) {
			fountainDisplay.SetActive (true);
			deckDisplay.SetActive (false);
		} else {
			fountainDisplay.SetActive (false);
			deckDisplay.SetActive (true);
		}
	}

	// Function that controls the behavior of the storage array if the up arrow is hit
	public void stageUp () {
		// Check to see if the player is at the last available stage and loop them back around to the first one
		if (currentStageNum == totalStages - 1) {
			currentStageNum = 0; // Reset the current stage to 0
		} else {
			currentStageNum++; // Increment the stage number by 1
		}

		stagePicker (); // Call the stagePicker function to set the stage
	}

	// Function that controls the behavior of the storage array if the down arrow is hit
	public void stageDown () {
		// Check to see if the player is at the first available stage and loop back around to the last one
		if (currentStageNum == 0) {
			currentStageNum = totalStages - 1; // Reset the current stage to the last available stage
		} else {
			currentStageNum--; // Decrement the stage number by 1
		}

		stagePicker (); // Call the stagePicker function to set the stage
	}

	// Function that sets the player's character based off how many players have picked thus far
	void setCharacter (GameObject chosenCharacter) {
		// Check to see if the maximum number of players in a single game have been reached
		if (numPlayers < maxPlayers) {
			numPlayers++; // Increment the current number of players by 1

			// Check to see how many players have picked already and set the next player to pick's character
			if (numPlayers == 1) {
				player1 = chosenCharacter;
			} else if (numPlayers == 2) {
				player2 = chosenCharacter;
			} else if (numPlayers == 3) {
				player3 = chosenCharacter;
			} else {
				player4 = chosenCharacter;
			}
		}
	}

	// Simple function that takes the specific pigeon object stored in the array and calls the setCharacter function with it
	public void choosePigeon () {
		GameObject characterStorage = characters [0];
		setCharacter (characters [0]);
	}

	// Simple function that takes the specific seagull object stored in the array and calls the setCharacter function with it
	public void chooseSeagull () {
		GameObject characterStorage = characters [1];
		setCharacter (characterStorage);
	}

	// Simple function that takes the specific raven object stored in the array and calls the setCharacter function with it
	public void chooseRaven () {
		GameObject characterStorage = characters [2];
		setCharacter (characterStorage);
	}

	// Simple function that takes the specific cardinal object stored in the array and calls the setCharacter function with it
	public void chooseCardinal () {
		GameObject characterStorage = characters [3];
		setCharacter (characterStorage);
	}
}
