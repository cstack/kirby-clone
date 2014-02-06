using UnityEngine;
using System.Collections;

public class AbilityStar : StateMachineBase {
	public float horizontalSpeed = 4f;
	public float verticalSpeed = 8f;
	public bool goRight;
	public float lifespan = 5f;
	
	private float startTime;
	
	private enum State {
		Bouncing, BeingInhaled
	}
	
	// Use this for initialization
	void Start () {
		rigidbody2D.velocity = new Vector2 (horizontalSpeed * (goRight ? 1 : -1), verticalSpeed);
		startTime = Time.time;
		CurrentState = State.Bouncing;
	}
	
	public void BouncingUpdate() {
		if (Time.time - startTime > lifespan) {
			Destroy(gameObject);
		}
	}
	
	public void BouncingOnCollisionEnter2D(Collision2D other) {
		if (other.gameObject.tag == "ground") {
			Vector2 vel = rigidbody2D.velocity;
			if (other.contacts.Length > 0) {
				if (Vector2.Dot(other.contacts[0].normal, Vector2.up) > 0.75) {
					// Collision was on bottom
					vel.y = verticalSpeed;
				} else if (Vector2.Dot(other.contacts[0].normal, Vector2.right) > 0.75) {
					// Collision was on left
					vel.x = horizontalSpeed;
				} else if (Vector2.Dot(other.contacts[0].normal, Vector2.right) < 0.25) {
					// Collision was on right
					vel.x = horizontalSpeed * -1;
				}
			}
			rigidbody2D.velocity = vel;
		}
	}
	
	public void BouncingOnTriggerEnter2D(Collider2D other) {
		base.OnTriggerEnter2D(other);
		if (other.gameObject.tag == "inhale") {
			CurrentState = State.BeingInhaled;
		}
	}
}

