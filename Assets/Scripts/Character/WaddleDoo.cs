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
	
	private enum State {
		WalkLeft, Charge, Attack, Jump
	}
	
	bool canAttack = true;

	IEnumerator WalkLeftEnterState() {
		updateXVelocity (-1 * speed);
		yield return null;
	}

	void WalkLeftUpdate() {
		updateXVelocity (-1 * speed);
		if (canAttack && distanceToKirby() <= range) {
			if (Random.value < 0.5) {
				CurrentState = State.Charge;
			} else {
				CurrentState = State.Jump;
			}
		}
	}

	IEnumerator ChargeEnterState() {
		updateXVelocity (0f);
		yield return new WaitForSeconds(2f);
		CurrentState = State.Attack;
	}

	IEnumerator AttackEnterState() {
		StartCoroutine(UseAbility());
		StartCoroutine (CoolDown ());
		yield return null;
	}

	protected override void OnAbilityFinished() {
		CurrentState = State.WalkLeft;
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
			CurrentState = State.WalkLeft;
		}
	}

	void JumpUpdate() {
		am.animate (rigidbody2D.velocity.y > 0 ? (int) Jump.Upward : (int) Jump.Downward);
	}

	#endregion

	#region implemented abstract members of EnemyBase

	protected override void goToDefaultState() {
		CurrentState = State.WalkLeft;
	}

	#endregion


}
