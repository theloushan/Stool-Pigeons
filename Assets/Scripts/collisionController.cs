using UnityEngine;
using System.Collections;

// This script handles the player character's behavior when it interacts with a platform
// This is done by creating a child object on the "feet" layer with its own collider
// By doing this and setting the collision matrix appropriately, this collider is the only thing that collides with platforms
// This allows for us to control our collision with the platforms without disrupting the player's vulnerability to projectiles
// We need to set the trigger of the collider to true if coming from below so that it can pass through platforms
// We need to set the trigger of the collider to false if coming from above so that it will stand on platforms
public class collisionController : MonoBehaviour {

	public float colliderRadius = 0f; // Determines the size of the feet collider
	public LayerMask isPlatform; // LayerMask for the platforms, since that's all we care about colliding with
	public string platformTag; // Tag for the platforms, used to compile all of the ones in the level
	bool inRadius = false; // Determines if the collider has neared a platform
	bool isCalculated = false; // Determines if the closest platform has been determined

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
	}

	// Function to determine the closest platform in the level
	Transform getPlatform() {
		Transform nearestPlatform = null; // Create an empty transform to store the nearest platform in
		GameObject[] taggedPlatforms = GameObject.FindGameObjectsWithTag (platformTag); // Create an array of all platforms in the level
		float nearestDistance = Mathf.Infinity; // Create a float to store the distance to the closest platform in, and set that distance as infinite

		int size = taggedPlatforms.Length; // Store the number of platforms in the array
		float testDistance = 0f; // Set the test distance to 0
		GameObject testPlatform = null; // Create a storage for the platform to test
		Vector3 testPosition = new Vector3 (0, 0, 0); // Create a storage for the distance to test

		// Iterate through every single platform in the array
		for (int i = 0; i < size; i++) {
			testPlatform = taggedPlatforms [i]; // Store the current platform to test

			testPosition = testPlatform.transform.position; // Store the current position to test

			testDistance = (testPosition - transform.position).sqrMagnitude; // Calculate the distance between the player's feet and the tested platform

			// Check to see if the tested distance is smaller than the current smallest distance
			if (testDistance < nearestDistance) {
				nearestPlatform = testPlatform.transform; // Set the nearest platform if the tested platform is closer
				nearestDistance = testDistance; // Set the nearest distance if the tested distance is closer
			}
		}

		return nearestPlatform; // Return the transform of the closest platform to the player character
	}

	// Called once per fixed frame-rate frame
	void FixedUpdate() {
		inRadius = Physics2D.OverlapCircle (this.transform.position, colliderRadius + 0.1f, isPlatform); // Create an overlap circle slightly bigger than the player's feet collider

		// Check to see if a platform has entered the OverlapCircle and if the distances have been calculated
		if (inRadius == true && isCalculated == false) {
			Transform calculatedPlatform = getPlatform (); // Determine the nearest platform
			float contactY = this.transform.position.y - colliderRadius; // Store the y position of the bottom of the feet

			// Check to see if the bottom of the feet are lower than or even with the platform's y position
			if (contactY <= calculatedPlatform.position.y) {
				GetComponent<Collider2D> ().isTrigger = true; // Turn off the collider if below
			} else {
				GetComponent<Collider2D> ().isTrigger = false; // Turn on the collider if above
			}

			isCalculated = true; // Set the calculated status to true
		} else if (inRadius == false && isCalculated == true) {
			isCalculated = false; // If the player character is no longer near a platform, reset the calculation status
			GetComponent<Collider2D> ().isTrigger = false; // Turn on the collider just in case
		}
	}
}
