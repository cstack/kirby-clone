using UnityEngine;
using System.Collections;

public class Kirby : MonoBehaviour {
	public float speed = 10f;
	public float jumpSpeed = 10f;

	/*
	 * All Vertical/Inhale state combinations are valid except for
	 * Flying/Inhaling and Flying/Inhaled
	 */
	private VerticalState verticalState = VerticalState.JUMPING;
	private InhaleState inhaleState = InhaleState.NOT_INHALING;

	private enum VerticalState {
		GROUND, JUMPING, FLYING, FALLING
	}
	private enum InhaleState {
		NOT_INHALING, INHALING, INHALED
	}

	void Update () {
		handleInhaling(); // This must come before other handlers
		handleHorizontalMovement();
		handleJumping();
		handleFlying();
	}

	void KnockBack() {
		verticalState = VerticalState.FALLING;
		rigidbody2D.velocity = new Vector2(-3, 3);
	}

	void OnCollisionEnter2D(Collision2D collision) {
		if (collision.gameObject.tag == "enemy") {
			Destroy (collision.gameObject);
			KnockBack();
		} else {
			verticalState = VerticalState.GROUND;
		}
	}

	void handleInhaling() {
		// TODO
	}

	void handleHorizontalMovement() {
		if (inhaleState == InhaleState.INHALING ||
		    verticalState == VerticalState.FALLING) {
			return;
		}

		float h = Input.GetAxis("Horizontal");
		Vector2 vel = rigidbody2D.velocity;
		vel.x = h * speed;
		rigidbody2D.velocity = vel;
	}

	void handleJumping() {
		if (inhaleState == InhaleState.INHALING) {
			return;
		}

		Vector2 vel = rigidbody2D.velocity;
		if (Input.GetKey(KeyCode.X)) {
			vel.y = jumpSpeed;
		}
		rigidbody2D.velocity = vel;
	}

	void handleFlying() {
		if (inhaleState == InhaleState.INHALING
		    || inhaleState == InhaleState.INHALED) {
			return;
		}
		// TODO
	}
}
