using UnityEngine;
using System.Collections;

public class Kirby : MonoBehaviour {
	public float speed = 10f;
	public float jumpSpeed = 10f;

	public float knockBackSpeed = 6;
	public float knockBackTime = 0.5f;
	public float lastKnockBack = 0;

	/*
	 * All Vertical/Inhale state combinations are valid except for
	 * Flying/Inhaling and Flying/Inhaled
	 */
	public VerticalState verticalState = VerticalState.JUMPING;
	private InhaleState inhaleState = InhaleState.NOT_INHALING;
	private Animator animator;

	public enum VerticalState {
		GROUND, JUMPING, FLYING, KNOCKBACK
	}
	private enum InhaleState {
		NOT_INHALING, INHALING, INHALED
	}

	void Start() {
		animator = this.GetComponent<Animator> ();
	}

	void FixedUpdate() {
		HandleKnockBack();
	}

	void HandleKnockBack() {
		if (verticalState == VerticalState.KNOCKBACK) {
			if (Time.time > lastKnockBack + knockBackTime) {
				verticalState = VerticalState.GROUND;
				rigidbody2D.velocity = new Vector2(0, 0);
			}
		}
	}

	void Update () {
		HandleInhaling(); // This must come before other handlers
		HandleHorizontalMovement();
		HandleJumping();
		HandleFlying();
	} 

	void KnockBack(Collision2D other) {
		verticalState = VerticalState.KNOCKBACK;
		float xVel = knockBackSpeed;
		if (other.transform.position.x > transform.position.x) {
			xVel *= -1;
		}
		lastKnockBack = Time.time;
		rigidbody2D.velocity = new Vector2(xVel, 0);
	}

	void OnCollisionEnter2D(Collision2D collision) {
		if (collision.gameObject.tag == "enemy") {
			Destroy (collision.gameObject);
			KnockBack(collision);
		} else {
			verticalState = VerticalState.GROUND;
		}
	}

	void HandleInhaling() {
		// TODO
	}

	void HandleHorizontalMovement() {
		if (inhaleState == InhaleState.INHALING ||
		    verticalState == VerticalState.KNOCKBACK) {
			return;
		}

		float h = Input.GetAxis("Horizontal");

		if (h > 0) {
			animator.SetInteger ("Direction", 1);
		} else if (h < 0) {
			animator.SetInteger("Direction", 0);
		}

		Vector2 vel = rigidbody2D.velocity;
		vel.x = h * speed;
		rigidbody2D.velocity = vel;
	}

	void HandleJumping() {
		if (inhaleState == InhaleState.INHALING
		    || verticalState == VerticalState.KNOCKBACK) {
			return;
		}

		Vector2 vel = rigidbody2D.velocity;
		if (Input.GetKey(KeyCode.X)) {
			if (verticalState == VerticalState.GROUND) {
				vel.y = jumpSpeed;
			}
			verticalState = VerticalState.JUMPING;
		}
		if (Input.GetKeyUp(KeyCode.X)) {
			if (verticalState == VerticalState.JUMPING) {
				vel.y = Mathf.Min(vel.y, 0);
			}
		}
		rigidbody2D.velocity = vel;
	}
	

	void HandleFlying() {
		if (inhaleState == InhaleState.INHALING ||
		    inhaleState == InhaleState.INHALED ||
		    verticalState == VerticalState.KNOCKBACK) {
			return;
		}
		// TODO
		if (Input.GetKey(KeyCode.UpArrow)) {
			Vector2 vel = rigidbody2D.velocity;
			vel.y = jumpSpeed;
			verticalState = VerticalState.FLYING;
			rigidbody2D.velocity = vel;
		}
	}


}
