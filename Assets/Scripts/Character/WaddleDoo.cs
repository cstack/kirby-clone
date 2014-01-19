using UnityEngine;
using System.Collections;

public class WaddleDoo : StateMachineBase {
	public float speed = 2f;
	public float range = 3f;

	private enum State {
		WalkLeft, Charge, Attack
	}

	private Transform target;

	void updateXVelocity(float x) {
		Vector2 vel = rigidbody2D.velocity;
		vel.x = x;
		rigidbody2D.velocity = vel;
	}
	
	void Start() {
		target = GameObject.Find ("Kirby").transform;
		CurrentState = State.WalkLeft;
	}

	IEnumerator WalkLeftEnterState() {
		updateXVelocity (-1 * speed);
		yield return null;
	}

	float distanceToTarget() {
		return Vector2.Distance (transform.position, target.position);
	}

	void WalkLeftUpdate() {
		updateXVelocity (-1 * speed);
		if (distanceToTarget () <= range) {
			CurrentState = State.Charge;
		}
	}

	IEnumerator ChargeEnterState() {
		updateXVelocity (0f);
		yield return new WaitForSeconds(2f);
		CurrentState = State.Attack;
	}
}
