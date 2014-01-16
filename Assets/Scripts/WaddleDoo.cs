using UnityEngine;
using System.Collections;

public class WaddleDoo : MonoBehaviour {
	public float speed = 2f;
	public float range = 2f;

	private enum State {
		WALK_LEFT, WALK_RIGHT, ATTACK
	}

	private State state = State.WALK_LEFT;
	private Transform target;

	// Use this for initialization
	void Start () {
		target = GameObject.Find ("Kirby").transform;
	}
	
	// Update is called once per frame
	void Update () {
		UpdateState ();
		updateVelocity ();
	}

	void UpdateState() {
		float myX = transform.position.x;
		if (Mathf.Abs (myX - target.position.x) < range) {
			state = State.ATTACK;
		} else if (target.position.x < myX) {
			state = State.WALK_LEFT;
		} else {
			state = State.WALK_RIGHT;
		}

	}

	void updateVelocity() {
		float vel_x = 0f;
		switch (state) {
		case (State.WALK_LEFT):
			vel_x = -1 * speed;
			break;
		case (State.WALK_RIGHT):
			vel_x = speed;
			break;
		}
		Vector2 vel = rigidbody2D.velocity;
		vel.x = vel_x;
		rigidbody2D.velocity = vel;
	}
}
