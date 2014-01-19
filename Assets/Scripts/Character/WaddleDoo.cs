using UnityEngine;
using System.Collections;

public class WaddleDoo : StateMachineBase {
	public float speed = 2f;
	public float range = 2f;

	private enum State {
		WALK_LEFT, CHARGE, ATTACK
	}

	private State state = State.WALK_LEFT;
	private Transform target;
	private AnimationManager am;

	void setState(State state) {
		CurrentState = state;
		am.State = (int) state;
	}

	void updateXVelocity(float x) {
		Vector2 vel = rigidbody2D.velocity;
		vel.x = x;
		rigidbody2D.velocity = vel;
	}
	
	void Start() {
		target = GameObject.Find ("Kirby").transform;
		am = new AnimationManager(this.GetComponent<Animator>());
		setState (State.WALK_LEFT);
	}

	IEnumerator WalkLeftEnterState() {
		updateXVelocity (-1 * speed);
		yield return null;
	}

	float distanceToTarget() {
		return Vector2.Distance (transform.position, target.position);
	}

	void WalkLeftUpdate() {
		if (distanceToTarget () <= range) {
			setState (State.CHARGE);
		}
	}

	void ChargeEnterState() {
		updateXVelocity (0);
	}
}
