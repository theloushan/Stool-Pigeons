using UnityEngine;
using System.Collections;

// This script handles the behavior of the bird's poop projectiles once they have been created
// Essentially, the poop should be ejected from the back of the player character at the same speed they are moving
// The poop should start at a size that the player cannot see and grow to its normal height to make it appear as though it is coming out of the bird
// As it moves, the poop should always face the direction it is moving so that the animations play properly
// The poop should play a "falling" animation when falling, and a "splash" animation once it hits the ground
// Once it has landed, the poop should stop moving/rotating entirely
// It should also around for a bit to show accumulation of the poop on the ground over the course of the match
// The poop should not cause any damage to players once it has hit the ground to show that it is not an active projectile anymore
public class pooController : MonoBehaviour {

	float size = 0.01f; // Determines the starting size of the poop projectile
	public float max = 1f; // Determines the max size of the poop projectile, as per the character's stats
	Rigidbody2D rigid; // Storage for the Rigidbody2D component so I don't have to constantly use GetComponent
	bool shouldDelete = false; // Determines if the object should be deleted
	int deleteTime = 5; // Number of seconds until the object is cleared from the stage
	int deleteCounter = 0; // Number of frames that have passed since the projectile has hit the floor
	public LayerMask isPlatform; // LayerMask for the platforms
	public LayerMask isGround; // LayerMask for the ground
	public LayerMask isPlayer; // LayerMask for the player characters
	bool touchPlatform = false; // Determines if the projectile has collided with a platform
	bool touchGround = false; // Determines if the projectile has collided with the ground 
	bool touchPlayer = false; // Determines if the projectile has collided with a player character
	Vector2 movement; // Vector for determining where the projectile should go
	Vector2 resetVector; // Honestly this is just here cause I couldn't use movement = null and I needed to compare it to something to set it to direction
	public Vector2 playerDirection; // Vector for storing the player who fired this projectile's movement
	Animator anim; // Storage for the Animator component so I don't have to constantly use GetComponent
	bool hasCollision = true; // Determines if the projectile still has its collision component active
	AudioSource poopAudio; // Storage variable for the AudioSource component so I don't have to constantly use GetComponent
	public AudioClip poopSound; // Storage variable for the impact sound effect
	bool playedSound = false; // Determines if the sound has been played or not, game wants to play it over and over if this check isn't enabled

	// Use this for initialization
	void Start () {
		anim = GetComponent<Animator> (); // Store the Animator
		rigid = GetComponent<Rigidbody2D> (); // Store the Rigidbody2D
		deleteTime *= 60; // Multiply the time to keep on the field by the number of frames per second
		resetVector = new Vector2 (-30, -30); // Set the resetVector to an arbitrary point way off the map
		movement = resetVector; // Set movement to the same value as resetVector
		poopAudio = GetComponent<AudioSource> ();
	}

	// Called once per fixed frame-rate frame
	void FixedUpdate () {
		// Check if the movement vector is the same as the resetVector (SHOULD ONLY PROC ONCE)
		if (movement == resetVector) {
			movement = new Vector2 (-playerDirection.x * 2, rigid.velocity.y); // Set the movement to the opposite of the player's x direction
			GetComponent<Rigidbody2D> ().AddForce (movement); // Fire the projectile in the movement direction
		}

		// To avoid errors, check to see if the object still has a collider to work with
		if (hasCollision == true) {
			// Set the touch values based off of what layers are being touched, should only be touching one at a time
			touchPlatform = GetComponent<Collider2D> ().IsTouchingLayers (isPlatform);
			touchGround = GetComponent<Collider2D> ().IsTouchingLayers (isGround);
			touchPlayer = GetComponent<Collider2D> ().IsTouchingLayers (isPlayer);
		}

		// Check to see if the projectile is touching either a platform or the ground
		if (touchPlatform == true || touchGround == true) {
			rigid.constraints = RigidbodyConstraints2D.FreezeAll; // Freeze the projectile's position and orientation
			Destroy (GetComponent<CircleCollider2D>()); // Delete the object's collider so it doesn't have collision anymore
			anim.SetBool ("Landing", true); // Tell the Animator to play the poopSplash animation
			shouldDelete = true; // Set the delete value to true
			hasCollision = false; // Make note that the object no longer has colliders
		} else if (touchPlayer == true) {
			Destroy (gameObject); // If the projectile touches a player character, just delete it
		}

		// Check to see if the object should be deleted and if its sound should be played yet
		if (shouldDelete == true && playedSound == false) {
			poopAudio.clip = poopSound; // Set the sound effect
			poopAudio.Play (); // Play the sound effect
			playedSound = true; // Set the sound's status to played
		}

		// Check to see if the projectile has entered the "deletion" status
		if (shouldDelete == true) {
			// Check to see if the number of seconds to wait before clearing the projectile has passed
			if (deleteCounter == deleteTime) {
				Destroy (gameObject); // Delete the object if the time has passed
			} else {
				deleteCounter++; // Increment the number of frames by 1
			}
		} else {
			// Check to see if the size of the projectile has reached its maximum
			if (size < max) {
				size += 0.01f; // Increment the size by 0.01
				transform.localScale = new Vector3 (size, size, 1); // Scale up the projectile by the new size
			}

			Vector2 direction = rigid.velocity; // Store the direction the projectile is heading
			float angle = Mathf.Atan2 (direction.y, direction.x) * Mathf.Rad2Deg; // Calculate the angle that the projectile is facing
			transform.rotation = Quaternion.AngleAxis (angle, Vector3.forward); // Orient the projectile so that it is facing the direction it is heading
		}
	}
}