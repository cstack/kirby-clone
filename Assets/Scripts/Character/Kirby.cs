using AnimationEnums;
using System.Collections;
using System.Globalization;
using UnityEngine;

namespace AnimationEnums {
	public enum IdleOrWalking {
		Idle, Walking
	}
	
	public enum Jumping {
		Jumping, Spinning, Exhale
	}

	public enum Flying {
		Flying, Exhaling
	}
}

public class Kirby : CharacterBase {
	public float speed = 6f;
	public float jumpSpeed = 12.5f;
	public float flySpeed = 7f;

	public float knockbackSpeed = 8f;
	public float knockbackTime = 0.2f;
	public bool isExhaling = false;

	public int health = 6;
	public static int livesRemaining = 4;

	private bool isSpinning = false;
	private GameObject inhaleArea;

	bool invulnurable;

	private Animator animator;

	// TODO: This is a bad way of doing this. See KnockbackEnterState
	private GameObject enemyOther;

	public enum State {
		IdleOrWalking, Jumping, Flying, Knockback, Ducking, Sliding, Inhaling, Inhaled, Die
	}
	
	void Start() {
		GameObject.Find("LivesRemaining").GetComponent<LivesRemaining>().setLivesRemaining(livesRemaining);
		animator = GetComponentInChildren<Animator>();
		CurrentState = State.Jumping;
		dir = Direction.Right;
		inhaleArea = transform.Find("Sprite/InhaleArea").gameObject;
		inhaleArea.SetActive(false);
	}

	private void CommonOnCollisionEnter2D(Collision2D other) {
		if (other.gameObject.tag == "enemy") {
			enemyOther = other.gameObject;
			Destroy(other.gameObject);
			CurrentState = State.Knockback;
		}
	}

	private void HandleHorizontalMovement(ref Vector2 vel) {
		float h = Input.GetAxis("Horizontal");
		if (h > 0 && dir != Direction.Right) {
			Flip();
		} else if (h < 0 && dir != Direction.Left) {
			Flip();
		}
		vel.x = h * speed;
	}
	
	#region IDLE_OR_WALKING

	private void IdleOrWalkingUpdate() {
		Vector2 vel = rigidbody2D.velocity;
		HandleHorizontalMovement(ref vel);
		if (Input.GetKey (KeyCode.X)) {
			vel.y = jumpSpeed;
			CurrentState = State.Jumping;
		} else if (Input.GetKey(KeyCode.Z)) {
			CurrentState = State.Inhaling;
		}else if (Input.GetKey(KeyCode.UpArrow)) {
			vel.y = flySpeed;
			CurrentState = State.Flying;
		} else if (Input.GetKey(KeyCode.DownArrow)) {
			CurrentState = State.Ducking;
		} else {
			if (vel.x == 0) {
				am.animate((int) IdleOrWalking.Idle);
			} else {
				am.animate((int) IdleOrWalking.Walking);
			}
		}
		rigidbody2D.velocity = vel;
	}

	private void IdleOrWalkingOnCollisionEnter2D(Collision2D other) {
		CommonOnCollisionEnter2D(other);
	}

	#endregion

	#region JUMPING

	private void JumpingUpdate() {
		Vector2 vel = rigidbody2D.velocity;
		HandleHorizontalMovement(ref vel);
		if (Input.GetKey(KeyCode.UpArrow)) {
			CurrentState = State.Flying;
		} else {
			if (Input.GetKeyUp(KeyCode.X)) {
				vel.y = Mathf.Min(vel.y, 0);
			}
			if (!isSpinning && Mathf.Abs(vel.y) < 0.4) {
				isSpinning = true;
				StartCoroutine(SpinAnimation());
			}
		}
		rigidbody2D.velocity = vel;
	}

	private IEnumerator SpinAnimation() {
		am.animate((int) Jumping.Spinning);
		yield return new WaitForSeconds(0.2f);
		am.animate((int) Jumping.Jumping);
		isSpinning = false;
	}

	private void JumpingOnCollisionEnter2D(Collision2D other) {
		CommonOnCollisionEnter2D(other);
	}

	private void JumpingOnCollisionStay2D(Collision2D other) {
		if (other.gameObject.tag == "ground") {
			if (other.contacts.Length > 0 &&
			    Vector2.Dot(other.contacts[0].normal, Vector2.up) > 0.5) {
				// Collision was on bottom
				CurrentState = State.IdleOrWalking;
			}
		}
	}

	#endregion

	#region FLYING
	
	private void FlyingUpdate() {
		Vector2 vel = rigidbody2D.velocity;
		HandleHorizontalMovement(ref vel);
		if (Input.GetKey(KeyCode.X) || Input.GetKey(KeyCode.UpArrow)) {
			vel.y = flySpeed;
			animator.speed = 1f;
		} else {
			vel.y = Mathf.Max(vel.y, -1 * flySpeed * 0.7f);
			AnimatorStateInfo info = animator.GetCurrentAnimatorStateInfo(0);
			if (info.IsName("FlyingMiddle")) {
				animator.speed = 0.3f;
			}
		}
		if (Input.GetKey(KeyCode.Z)) {
			animator.speed = 1f;
			if (!isExhaling) {
				isExhaling = true;
				StartCoroutine(Exhale());
			}
		}
		rigidbody2D.velocity = vel;
	}

	private IEnumerator Exhale() {
		am.animate((int) Flying.Exhaling);
		yield return new WaitForSeconds(0.4f);
		CurrentState = State.Jumping;
		isExhaling = false;
	}
	
	private void FlyingOnCollisionEnter2D(Collision2D other) {
		CommonOnCollisionEnter2D(other);
	}

	#endregion

	#region KNOCKBACK

	private IEnumerator KnockbackEnterState() {
		GameObject go = GameObject.Find("HealthBarItem" + health);
		Animator animator = go.GetComponent<Animator>();
		animator.SetBool("Remove", true);

		health -= 1;
		if (health == 0) {
			CurrentState = State.Die;
			yield break;
		}

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

	#region DUCKING

	public void DuckingUpdate() {
		if (Input.GetKeyUp(KeyCode.DownArrow)) {
			CurrentState = State.IdleOrWalking;
		}
		Vector2 vel = rigidbody2D.velocity;
		vel.x *= 0.9f;
		rigidbody2D.velocity = vel;
	}

	#endregion

	#region SLIDING
	#endregion

	#region INHALING
	private IEnumerator InhalingEnterState() {
		inhaleArea.SetActive(true);
		yield return null;
	}

	private IEnumerator InhalingExitState() {
		inhaleArea.SetActive(false);
		yield return null;
	}
		
	public void InhalingUpdate() {
		if (!Input.GetKey(KeyCode.Z)) {
			CurrentState = State.IdleOrWalking;
		}
	}

	private void InhalingOnCollisionEnter2D(Collision2D other) {
		if (other.gameObject.tag == "enemy") {
			enemyOther = other.gameObject;
			Destroy(enemyOther);
			CurrentState = State.Flying;
		}
	}
	#endregion

	#region DIE

	private IEnumerator DieEnterState() {
		livesRemaining -= 1;
		if (livesRemaining < 0) {
			Application.Quit(); // TODO
		} else {
			Application.LoadLevel("Main");
		}
		yield break;
	}

	#endregion

	public void TakeHit(EnergyWhipParticle particle) {
		if (invulnurable) {
			return;
		}
		enemyOther = particle.gameObject;
		CurrentState = State.Knockback;
		StartCoroutine("Invulnerability");
	}

	public IEnumerator Invulnerability() {
		invulnurable = true;
		yield return new WaitForSeconds (2f);
		invulnurable = false;
	}
}
