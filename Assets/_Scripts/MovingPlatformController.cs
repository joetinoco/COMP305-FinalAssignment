﻿using UnityEngine;
using System.Text.RegularExpressions;

/*

MOVING PLATFORM CONTROLLER
==========================

Makes a platform move back and forth between two points.

Created by: Jose Tinoco
Last Modified: Dec. 13 by Winnie Chung
Revision History:
 Nov. 28: File creation
 Dec. 13: Modified header
*/

public class MovingPlatformController : MonoBehaviour {

	// ==========================================
	// Attributes
	// ==========================================

	// References to platform components
	private Transform mpTransform;
	
	// Movement flags and attributes
	private bool reverseMovement = false;
	private bool isTransportingPlayer = false;
	public float speed = 1f;

	// Platform displacement per frame
	private Vector2 positionDelta;
	private Vector3 platformDisplacement;
	
	// Those two transforms mark the line where the platform will travel.
	// They are set on Unity's GUI for each prefab instance, 
	// to allow easy reuse across the level.
	public Transform movementStart;
	public Transform movementEnd;

	private Vector2 movStart, movEnd;


	// ==========================================
	// Object initialization
	// ==========================================
	void Start () {
		this.mpTransform = GetComponent<Transform>();
		
		// Copy the start/end coordinates into fixed vectors, for updating
		this.movStart = new Vector2(
			this.movementStart.position.x,
			this.movementStart.position.y
		);
		this.movEnd = new Vector2(
			this.movementEnd.position.x,
			this.movementEnd.position.y
		);
	}
	
	// ==========================================
	// Game Lifecycle Updates
	// ==========================================
	void FixedUpdate () {
		
		// Prevent movement (and runtime errors)
		// if the platform has no waypoints set
		if (!this.movementStart || !this.movementEnd) return;
		
		// Select a starting (or ending) point to 
		// calculate movement towards.
		Vector2 movementDestination;
		if (reverseMovement == false) {
			movementDestination = this.movEnd;
		} else {
			movementDestination = this.movStart;
		}
		
		// Move the platform towards it
		this.positionDelta = Vector2.MoveTowards(
			new Vector2(this.mpTransform.position.x, this.mpTransform.position.y),
			movementDestination,
			this.speed * Time.deltaTime
		);

		// Store the platform displacement, so it can be
		// applied to whatever is on top of it
		this.platformDisplacement = new Vector3(
					this.positionDelta.x - this.mpTransform.position.x,
					this.positionDelta.y - this.mpTransform.position.y,
					0);

		// If the platform is moving down and transporting a player,
		// the player must be moved first (in OnCollisionStay2D),
		// otherwise it will stop colliding for a fraction of second
		// and chaos will ensue.
		if (!isTransportingPlayer) this.mpTransform.position = this.positionDelta;

		// Flip the start/end waypoints if the platform reaches either
		if (this.mpTransform.position.x == movementDestination.x && this.mpTransform.position.y == movementDestination.y) {
			reverseMovement = !reverseMovement;
		}

	}

	// Collision detection methods
	// =================================================

	private void OnCollisionEnter2D(Collision2D other) {
		if (other.gameObject.CompareTag("Player")) isTransportingPlayer = true;
	}

	private void OnCollisionExit2D(Collision2D other) {
		if (other.gameObject.CompareTag("Player")) isTransportingPlayer = false;
	}

	private void OnCollisionStay2D(Collision2D other) {
		if (other.gameObject.CompareTag("Player")) {
			// Only act on player if it is over the platform
			if (other.transform.position.y > this.mpTransform.position.y) {
				// Apply the same platform displacement to the player...
				other.gameObject.SendMessage("Displace", this.platformDisplacement);
				// ...then to the platform.
				// This must happen in THIS ORDER to prevent bugs
				// when the platform is moving down.
				this.mpTransform.position = this.positionDelta;
			}
		}
	}

}
