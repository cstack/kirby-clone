using UnityEngine;
using System.Collections;

public class Sparky : EnemyBase {
	public float jumpSpeedY = 3f;
	public float jumpHighSpeedY = 5f;
	public float jumpSpeedX = 2f;
	public float jumpForwardChance = 0.5f;
	public float jumpHighChance = 0.5f;
	public float attackDuration = 2f;
	public float attackRechargeTime = 3f;
	public SparkAbility sparkAbilityPrefab;

	private float lastAttackTime;
	private SparkAbility sparkAbility;

	private enum State {
		Slide, Jump, Attack
	}

	#region Slide
	
	private IEnumerator SlideEnterState() {
		StartCoroutine(SlowDown());
		yield return new WaitForSeconds (0.5f);
		TakeAction();
	}

	private IEnumerator SlowDown() {
		for (int i = 0; i < 5; i++) {
			updateXVelocity(rigidbody2D.velocity.x * 0.5f);
			yield return new WaitForSeconds (0.1f);
		}
	}
	
	#endregion

	#region Slide
	
	private IEnumerator AttackEnterState() {
		sparkAbility = Instantiate (sparkAbilityPrefab) as SparkAbility;
		sparkAbility.gameObject.transform.position = transform.position;
		sparkAbility.duration = attackDuration;
		yield return new WaitForSeconds (attackDuration);
		lastAttackTime = Time.time;
		TakeAction();
	}

	private IEnumerator AttackExitState() {
		if (sparkAbility != null) {
			Destroy(sparkAbility.gameObject);
			sparkAbility = null;
		}
		yield return null;
	}

	#endregion

	public void TakeAction() {
		if (dir == Direction.Left && kirby.transform.position.x > transform.position.x) {
			Flip();
			CurrentState = State.Slide;
		} else if (dir == Direction.Right && kirby.transform.position.x < transform.position.x) {
			Flip();
			CurrentState = State.Slide;
		} else if (distanceToKirby() < 4f && Time.time - lastAttackTime > attackRechargeTime) {
			CurrentState = State.Attack;
		} else {
			CurrentState = State.Jump;
		}
	}

	#region Jump
	
	private IEnumerator JumpEnterState() {
		int sign = (dir == Direction.Left ? -1 : 1);
		float speedX = 0f;
		if (Random.value < jumpForwardChance) {
			speedX = jumpSpeedX * sign;
		}
		float speedY = jumpSpeedY;
		if (Random.value < jumpHighChance) {
			speedY = jumpHighSpeedY;
		}
		rigidbody2D.velocity = new Vector2 (speedX, speedY);
		yield return null;
	}

	private void JumpOnCollisionEnter2D(Collision2D other) {
		if (other.gameObject.tag == "ground") {
			if (other.contacts.Length > 0 &&
			    Vector2.Dot(other.contacts[0].normal, Vector2.up) > 0.5) {
				// Collision was on bottom
				CurrentState = State.Slide;
			}
		}
	}
	
	#endregion
	
	protected override void goToDefaultState()
	{
		CurrentState = State.Jump;
	}
}
