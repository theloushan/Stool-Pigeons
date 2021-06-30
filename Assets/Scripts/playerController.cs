using UnityEngine;
using System.Collections;

// This script controls the player's behavior during the game itself
// It waits for the player to press a control and behaves according to what button was pressed and what the character's stats are
// It stores the character's stats in public variables so that they can be changed easily for each character within Unity itself
// The character will die when hit by a projectile, and will spawn an explosion in front to mask the character's deletion
// The character will spawn a projectile behind it when the poop button is pressed
// The character has a number of jumps and a jump force based off the character's stats
// Before the character spawns an explosion, it will bounce around the screen for a few seconds
// In addition, each behavior the character takes will load a different animation and sound effect to play
public class playerController : MonoBehaviour {

	public int playerNum = 1; // Public variable that determines what the numbered player the character represents
	public float maxSpeed = 10f; // Public variable that determines the speed at which the character moves horizontally
	public float projectileSize = 0.5f; // Public variable that determines the maximum size of the character's projectiles
	public int projectileTime = 2; // Public variable that determines the time needed to wait before the character can fire another projectile
	int projectileCounter = 0; // Number of frames that have passed since the last projectile has been created
	bool hasFired = false; // Determines if a projectile has been fired or not
	//public int health = 1; // Public variable that determines the number of projectiles the character can be hit by before dying
	bool facingRight = true; // Determines if the character is facing the right or the left
	bool onGround = false; // Determines if the character is currently on the ground
	bool onPlatform = false; // Determines if the character is currently on a platform
	public LayerMask isGround; // LayerMask for the ground
	public LayerMask isPlatform; // LayerMask for the platforms
	public LayerMask isProjectile; // LayerMask for the projectiles
	public float jumpForce = 700f; // Public variable that determines the amount of force with which the character jumps
	public int totalFlaps = 3; // Public variable that determines the number of jumps the character can make before they have to touch the ground again
	int flaps = 0; // Number of jumps that the character has done so far
	public Transform feet; // Public variable for the child object representing the character's feet
	Animator anim; // Storage for the Animator component so I don't have to constantly use GetComponent
	PolygonCollider2D collide; // Storage for the Collider component so I don't have to constantly use GetComponent
	public PhysicsMaterial2D bouncer; // Public variable that stores the PhysicsMaterial to use when the character dies
	public GameObject projectile; // Public variable that stores the object to spawn when creating a new projectile
	public GameObject death; // Public variable that stores the object to spawn to hide the character's deletion once they dies
	public float screenX = 20f; // Public variable that stores the size of the stage's horizontal component
	public float screenY = 20f; // Public variable that stores the size of the stage's vertical component
	bool wasHit = false; // Determines if the character has been hit by a projectile or not
	int deathTimer = 2; // Number of seconds until the character is cleared from the stage
	int deathCounter = 0; // Number of frames that have passed since the character has been hit by a projectile
	public float projectileOffset = 0.5f; // Public variable that stores the amount the projectile's spawn should be offset by to spawn properly
	AudioSource birdAudio; // Storage for the AudioSource component so I don't have to constantly use GetComponent
	public AudioClip poopSound; // Public variable that stores the pooping sound effect
	public AudioClip jumpSound; // Public variable that stores the jumping sound effect

	// Use this for initialization
	void Start () {
		anim = GetComponent<Animator> ();
		collide = GetComponent<PolygonCollider2D> (); // Store the Collider
		projectileTime *= 60; // Multiply the time to keep on the field by the number of frames per second
		deathTimer *= 60; // Multiply the time to keep on the field by the number of frames per second
		birdAudio = GetComponent<AudioSource> (); // Store the AudioSource
	}

	// Called once per fixed frame-rate frame
	void FixedUpdate () {
		if (wasHit == true /* && health == 0*/) {
			anim.SetBool ("Dying", wasHit); // Set the animation to the Death Animation

			// Check to see if the Dying sequence has just started
			if (deathCounter == 0) {
				collide.sharedMaterial = bouncer; // Set the character's physics material
			}

			// If the number of frames to wait for has passed, spawn an explosion and delete the character
			if (deathCounter == deathTimer) {
				GameObject deathExplosion = Instantiate (death, this.transform.position, this.transform.rotation) as GameObject;
				Destroy (gameObject);
			} else {
				deathCounter++; // Otherwise, increment the number of frames waited by 1
			}
		} else {
			wasHit = collide.IsTouchingLayers (isProjectile); // Check to see if the character has touched a projectile
			onGround = feet.GetComponent<Collider2D> ().IsTouchingLayers (isGround); // Check to see if the character is touching the ground
			onPlatform = feet.GetComponent<Collider2D> ().IsTouchingLayers (isPlatform); // Check to see if the character is touching a platform

			// Determine if the player has pressed any of their controls, based off what number player they are
			float move = Input.GetAxis ("Horizontal" + playerNum);
			bool jump = Input.GetButtonDown ("Jump" + playerNum);
			bool drop = Input.GetButtonDown ("Down" + playerNum);
			bool poop = Input.GetButtonDown ("Poop" + playerNum);

			// Set the animation to the proper one based off what buttons were pressed
			anim.SetFloat ("Speed", move);
			anim.SetBool ("Jumping", jump);
			anim.SetBool ("Pooping", poop);

			// Check to see if the character was on a platform and set the flaps and anim.onGround variables appropriately
			if (onGround == true || onPlatform == true) {
				flaps = 0;
				anim.SetBool ("onGround", true);
			} else {
				anim.SetBool ("onGround", false);
			}

			// Check to see if the player is on a platform and has hit the button to drop through the platform
			if (onPlatform == true && drop == true) {
				feet.GetComponent<Collider2D> ().isTrigger = true; // Turning the feet collider to a trigger allows it to pass through other colliders
			}

			// If the character is touching the ground, turn the feet collider back to normal
			if (onGround == true) {
				feet.GetComponent<Collider2D> ().isTrigger = false;
			}

			// Determine the player's new speed and move them appropriately
			Vector2 direction = new Vector2 (move * maxSpeed, GetComponent<Rigidbody2D>().velocity.y);
			GetComponent<Rigidbody2D>().velocity = direction;

			// If the poop button has been pressed, check to see if the player has recently fired
			if (poop == true && hasFired == false) {
				Vector3 spawnPosition; // Variable to store the position to spawn in

				// Check to see which direction the character is facing and set the spawnPosition appropriately
				if (facingRight == true) {
					spawnPosition = new Vector3 (this.transform.position.x - projectileOffset, this.transform.position.y - projectileOffset);
				} else {
					spawnPosition = new Vector3 (this.transform.position.x + projectileOffset, this.transform.position.y - projectileOffset);
				}

				// Create a new poop projectile at the determined spawnPosition
				GameObject createPoop = Instantiate (projectile, spawnPosition, this.transform.rotation) as GameObject;
				createPoop.GetComponent <pooController> ().max = projectileSize; // Set the poop's size
				createPoop.GetComponent <pooController> ().playerDirection = direction; // Set the poop's direction
				hasFired = true; // Set the fired status to true
				birdAudio.clip = poopSound; // Set the pooping sound
				birdAudio.Play (); // Play the pooping sound
			} else if (hasFired == true && projectileCounter != projectileTime) {
				projectileCounter++; // Increment the number of frames that have passed by one if the player has recently fired
			} else {
				hasFired = false; // Reset the player's fired status to false
				projectileCounter = 0; // Reset the number of frames waited so far
			}

			// Behavior to invoke if the jump button has been pressed
			if (jump == true) {
				// Check to see if the player is on the ground or a platform and behave appropriately
				if (onGround == true && onPlatform == false) {
					GetComponent<Rigidbody2D> ().AddForce (new Vector2 (0, jumpForce * 2)); // Add a force upward to the character
					onGround = false; // Set the onGround status to false
					flaps++; // Increment the number of jumps by 1
				} else if (onPlatform == true && onGround == false) {
					GetComponent<Rigidbody2D> ().AddForce (new Vector2 (0, jumpForce * 2)); // Add a force upward to the character
					onPlatform = false; // Set the onPlatform status to false
					flaps++; // Increment the number of jumps by 1
				} else if (onGround == false && onPlatform == false && flaps < totalFlaps) {
					flaps++; // Increment the number of jumps by 1
					GetComponent<Rigidbody2D> ().AddForce (new Vector2 (0, jumpForce * 2)); // Add a force upward to the character
				}

				birdAudio.clip = jumpSound; // Set the jumping sound
				birdAudio.Play (); // Play the jumping sound
			}

			// Check what direction the character is facing and moving, if those directions are contradictory, call the Flip function
			if (move > 0 && !facingRight) {
				Flip ();
			} else if (move < 0 && facingRight) {
				Flip ();
			}

			// Check to see if the chracter has moved off the stage's x boundary, if so call the ResetX function
			if (this.transform.position.x < -screenX || this.transform.position.x > screenX) {
				ResetX ();
			}

			// Check to see if the character has fallen off the bottom of the stage, if so reset them to above the stage in the same x position
			if (this.transform.position.y < -screenY) {
				this.transform.position = new Vector3 (this.transform.position.x, screenY, this.transform.position.z);
			}
		}
	}

	// Basic function to flip the direction the sprite is facing
	void Flip() {
		facingRight = !facingRight; // Change the facingRight status to the opposite
		Vector3 scale = transform.localScale; // Fetch the object's scale
		scale.x *= -1; // Flip the object's x scale
		transform.localScale = scale; // Change the scale to the new one
	}

	// Basic function to change the character's x position to the other side of the screen if they walk off a side
	void ResetX() {
		// Check to see what side the character has walked off, and reset their x position to the opposite side of the stage
		if (this.transform.position.x < 0f) {
			this.transform.position = new Vector3 (screenX, this.transform.position.y, this.transform.position.z);
		} else {
			this.transform.position = new Vector3 (-screenX, this.transform.position.y, this.transform.position.z);
		}
	}
}
