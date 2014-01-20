using UnityEngine;
using System.Collections;

public class WaddleDoo : StateMachineBase {
	public float speed = 2f;
	public float range = 3f;
	public float timeBetweenAttacks = 3f;

	public EnergyWhip EnergyWhipPrefab;

	private enum State {
		WalkLeft, Charge, Attack
	}

	private Transform target;

	bool canAttack = true;

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
		if (canAttack && distanceToTarget () <= range) {
			CurrentState = State.Charge;
		}
	}

	IEnumerator ChargeEnterState() {
		updateXVelocity (0f);
		yield return new WaitForSeconds(2f);
		CurrentState = State.Attack;
	}

	IEnumerator AttackEnterState() {
		EnergyWhip energyWhip = Instantiate (EnergyWhipPrefab) as EnergyWhip;
		energyWhip.gameObject.transform.position = transform.position;
		yield return new WaitForSeconds(energyWhip.duration);
		Destroy (energyWhip);
		canAttack = false;
		CurrentState = State.WalkLeft;
		yield return new WaitForSeconds(timeBetweenAttacks);
		canAttack = true;
	}
}
