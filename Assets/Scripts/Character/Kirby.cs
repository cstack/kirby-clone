using AnimationEnums;
using System.Collections;
using System.Globalization;
using UnityEngine;

namespace AnimationEnums {
	public enum IdleOrWalking {
		Idle, Walking
	}
	
	public enum Jumping {
		Jumping, Spinning
	}
}

public class Kirby : StateMachineBase {
	public float speed = 6f;
	public float jumpSpeed = 12.5f;
	public float flySpeed = 7f;

	public float knockbackSpeed = 8f;
	public float knockbackTime = 0.2f;

	private bool isSpinning = false;

	private Direction dir;
	private enum Direction {
		Left, Right
	}

	// TODO: This is a bad way of doing this. See KnockbackEnterState
	private Collision2D enemyOther;

	public enum State {
		IdleOrWalking, Jumping, Flying, Knockback, Sliding, Inhaling, Inhaled
	}

	void Start() {
		CurrentState = State.Jumping;
		dir = Direction.Right;
	}

	void CommonOnCollisionEnter2D(Collision2D other) {
		if (other.gameObject.tag == "ground") {
			CurrentState = State.IdleOrWalking;
		} else if (other.gameObject.tag == "enemy") {
			enemyOther = other;
			Destroy(other.gameObject);
			CurrentState = State.Knockback;
		}
	}

	void HandleHorizontalMovement(ref Vector2 vel) {
		float h = Input.GetAxis("Horizontal");
		if (h > 0 && dir != Direction.Right) {
			Flip();
		} else if (h < 0 && dir != Direction.Left) {
			Flip();
		}
		vel.x = h * speed;
	}

	void Flip() {
		dir = (dir == Direction.Right) ? Direction.Left : Direction.Right;

		Vector3 scale = transform.localScale;
		scale.x *= -1;
		transform.localScale = scale;
	}

	#region IDLE_OR_WALKING

	void IdleOrWalkingUpdate() {
		Vector2 vel = rigidbody2D.velocity;
		HandleHorizontalMovement(ref vel);
		if (Input.GetKey(KeyCode.X)) {
			vel.y = jumpSpeed;
			CurrentState = State.Jumping;
		} else if (Input.GetKey(KeyCode.UpArrow)) {
			vel.y = flySpeed;
			CurrentState = State.Flying;
		} else {
			if (vel.x == 0) {
				am.animate((int) IdleOrWalking.Idle);
			} else {
				am.animate((int) IdleOrWalking.Walking);
			}
		}
		rigidbody2D.velocity = vel;
	}

	void IdleOrWalkingOnCollisionEnter2D(Collision2D other) {
		CommonOnCollisionEnter2D(other);
	}

	#endregion

	#region JUMPING

	void JumpingUpdate() {
		Vector2 vel = rigidbody2D.velocity;
		HandleHorizontalMovement(ref vel);
		if (Input.GetKeyUp(KeyCode.X)) {
			vel.y = Mathf.Min(vel.y, 0);
		}
		if (!isSpinning && Mathf.Abs(vel.y) < 0.4) {
			isSpinning = true;
			StartCoroutine(SpinAnimation());
		}
		rigidbody2D.velocity = vel;
	}

	IEnumerator SpinAnimation() {
		am.animate((int) Jumping.Spinning);
		yield return new WaitForSeconds(0.2f);
		am.animate((int) Jumping.Jumping);
		isSpinning = false;
	}

	void JumpingOnCollisionEnter2D(Collision2D other) {
		CommonOnCollisionEnter2D(other);
	}

	#endregion

	#region FLYING

	void FlyingUpdate() {
		Vector2 vel = rigidbody2D.velocity;
		HandleHorizontalMovement(ref vel);
		if (Input.GetKey(KeyCode.X) || Input.GetKey(KeyCode.UpArrow)) {
			vel.y = flySpeed;
		} else {
			vel.y = Mathf.Max(vel.y, -1 * flySpeed);
		}
		rigidbody2D.velocity = vel;
	}

	void FlyingOnCollisionEnter2D(Collision2D other) {
		CommonOnCollisionEnter2D(other);
	}

	#endregion

	#region KNOCKBACK

	IEnumerator KnockbackEnterState() {
		float xVel = knockbackSpeed;
		if (enemyOther.transform.position.x > transform.position.x) {
			xVel *= -1;
		}
		rigidbody2D.velocity = new Vector2(xVel, 0);
		yield return new WaitForSeconds(knockbackTime);
		CurrentState = State.IdleOrWalking;
		rigidbody2D.velocity = Vector2.zero;
	}

	#endregion

	#region SLIDING
	#endregion
}
