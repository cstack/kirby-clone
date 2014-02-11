using UnityEngine;
using System.Collections;
using AnimationEnums;

namespace AnimationEnums {
	public enum Jump {
		Upward, Downward
	}
}

public class WaddleDoo : EnemyBase {
	public float speed = 2f;
	public float range = 3f;
	public float timeBetweenAttacks = 3f;
	public float jumpSpeed = 8f;

	public float chanceOfCharge = 0.8f;
	
	private enum State {
		Walk, Charge, Attack, Jump
	}

	protected override void setPoints() {
		points = 300;
	}

	bool canAttack = true;

	IEnumerator WalkLeftEnterState() {
		updateXVelocity (-1 * speed);
		yield return null;
	}

	void WalkUpdate() {
		updateXVelocity ((dir == Direction.Left ? -1 : 1) * speed);
		if (canAttack && distanceToKirby() <= range) {
			if (Random.value < chanceOfCharge) {
				CurrentState = State.Charge;
			} else {
				CurrentState = State.Jump;
			}
		}
		BounceOffWalls ();
	}

	IEnumerator ChargeEnterState() {
		updateXVelocity (0f);
		yield return new WaitForSeconds(1.5f);
		CurrentState = State.Attack;
	}

	IEnumerator AttackEnterState() {
		UseAbility();
		StartCoroutine (CoolDown ());
		yield return new WaitForSeconds(0.5f);
		CurrentState = State.Walk;
	}

	IEnumerator AttackExitState() {
		StopAbility();
		yield return null;
	}

	IEnumerator CoolDown() {
		canAttack = false;
		yield return new WaitForSeconds (timeBetweenAttacks);
		canAttack = true;
	}

	#region JUMP

	IEnumerator JumpEnterState() {
		updateYVelocity (jumpSpeed);
		yield return null;
	}

	void JumpOnCollisionEnter2D(Collision2D other) {
		if (other.gameObject.tag == "ground") {
			StartCoroutine(CoolDown());
			CurrentState = State.Walk;
		}
	}

	void JumpUpdate() {
		am.animate (rigidbody2D.velocity.y > 0 ? (int) Jump.Upward : (int) Jump.Downward);
	}

	#endregion

	#region implemented abstract members of EnemyBase

	protected override void goToDefaultState() {
		CurrentState = State.Walk;
	}

	#endregion


}
