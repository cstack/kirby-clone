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

	public enum Inhaling {
		Inhaling, FinishInhaling
	}

	public enum Inhaled {
		Idle, Walking
	}

	public enum InhaledJumping {
		Jumping
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
	private EnemyBase inhaledEnemy;

	bool invulnurable;

	private Animator animator;
	public StarProjectile starProjectilePrefab;

	// TODO: This is a bad way of doing this. See KnockbackEnterState
	private GameObject enemyOther;

	public enum State {
		IdleOrWalking, Jumping, Flying, Knockback, Ducking, Sliding, Inhaling, Inhaled, Die,
		InhaledJumping, InhaledKnockback, Shooting, Swallowing, UsingAbility
	}
	
	new public void Start() {
		base.Start();
		GameObject.Find("LivesRemaining").GetComponent<LivesRemaining>().setLivesRemaining(livesRemaining);
		animator = GetComponentInChildren<Animator>();
		CurrentState = State.Jumping;
		dir = Direction.Right;
		inhaleArea = transform.Find("Sprite/InhaleArea").gameObject;
		inhaleArea.SetActive(false);
	}

	private void OnCollideWithEnemy(GameObject enemy) {
		enemyOther = enemy;
		Destroy(enemy);
		TakeDamage();
		CurrentState = (inhaledEnemy == null) ? State.Knockback : State.InhaledKnockback;
	}

	private void CommonOnCollisionEnter2D(Collision2D other) {
		if (other.gameObject.tag == "enemy") {
			OnCollideWithEnemy(other.gameObject);
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
			if (ability == null) {
				CurrentState = State.Inhaling;
			} else {
				CurrentState = State.UsingAbility;
			}
		} else if (Input.GetKey(KeyCode.UpArrow)) {
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

	#region Inhaled
	
	private void InhaledUpdate() {
		Vector2 vel = rigidbody2D.velocity;
		HandleHorizontalMovement(ref vel);
		if (Input.GetKey (KeyCode.X)) {
			vel.y = jumpSpeed;
			CurrentState = State.InhaledJumping;
		} else if (Input.GetKeyDown(KeyCode.Z)) {
			CurrentState = State.Shooting;
		} else if (Input.GetKey(KeyCode.DownArrow)) {
			CurrentState = State.Swallowing;
		} else {
			if (vel.x == 0) {
				am.animate((int) Inhaled.Idle);
			} else {
				am.animate((int) Inhaled.Walking);
			}
		}
		rigidbody2D.velocity = vel;
	}

	private void InhaledOnCollisionEnter2D(Collision2D other) {
		CommonOnCollisionEnter2D(other);
	}

	#endregion

	#region Swallowing
	
	private IEnumerator SwallowingEnterState() {
		ability = inhaledEnemy.ability;
		yield return new WaitForSeconds (0.5f);
		CurrentState = State.IdleOrWalking;
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
			} else if (Input.GetKey(KeyCode.Z)) {
				if (ability == null) {
					CurrentState = State.Inhaling;
				} else {
					CurrentState = State.UsingAbility;
				}
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

	#region Shooting

	private IEnumerator ShootingEnterState() {
		StarProjectile star = Instantiate (starProjectilePrefab) as StarProjectile;
		star.gameObject.transform.position = transform.position + new Vector3(0f, 0.1f, 0);
		star.direction = (dir == Direction.Right ? 1 : -1);
		yield return new WaitForSeconds (0.5f);
		inhaledEnemy = null;
		CurrentState = State.Jumping;
	}

	#endregion

	#region Shooting
	
	private IEnumerator UsingAbilityEnterState() {
		StartCoroutine(UseAbility(true));
		yield return null;
	}

	protected override void OnAbilityFinished() {
		CurrentState = State.IdleOrWalking;
	}
	
	#endregion

	#region InhaledJumping
	
	private void InhaledJumpingUpdate() {
		Vector2 vel = rigidbody2D.velocity;
		HandleHorizontalMovement(ref vel);
		if (Input.GetKeyDown(KeyCode.Z)) {
			CurrentState = State.Shooting;
		}
		rigidbody2D.velocity = vel;
	}

	private void InhaledJumpingOnCollisionEnter2D(Collision2D other) {
		CommonOnCollisionEnter2D(other);
	}
	
	private void InhaledJumpingOnCollisionStay2D(Collision2D other) {
		if (other.gameObject.tag == "ground") {
			if (other.contacts.Length > 0 &&
			    Vector2.Dot(other.contacts[0].normal, Vector2.up) > 0.5) {
				// Collision was on bottom
				CurrentState = State.Inhaled;
			}
		}
	}
	
	#endregion

	#region FLYING

	private IEnumerator FlyingEnterState() {
		speed -= 2;
		yield break;
	}

	private IEnumerator FlyingExitState() {
		speed += 2;
		AnimatorStateInfo info = animator.GetCurrentAnimatorStateInfo(0);
		animator.speed = 1f;
		yield break;
	}

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

	private void TakeDamage() {
		GameObject go = GameObject.Find("HealthBarItem" + health);
		Animator animator = go.GetComponent<Animator>();
		animator.SetBool("Remove", true);
		
		health -= 1;
		if (health == 0) {
			CurrentState = State.Die;
			return;
		}
	}

	#region KNOCKBACK

	private IEnumerator KnockbackEnterState() {
		float xVel = knockbackSpeed * (enemyOther.transform.position.x > transform.position.x ? -1 : 1);
		updateXVelocity(xVel);
		yield return new WaitForSeconds(knockbackTime);
		CurrentState = State.IdleOrWalking;
		rigidbody2D.velocity = Vector2.zero;
	}

	#endregion

	#region KNOCKBACK
	
	private IEnumerator InhaledKnockbackEnterState() {
		float xVel = knockbackSpeed * (enemyOther.transform.position.x > transform.position.x ? -1 : 1);
		updateXVelocity(xVel);
		yield return new WaitForSeconds(knockbackTime);
		CurrentState = State.Inhaled;
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

	public IEnumerator ComeToHalt() {
		for (int i = 0; i < 10; i++) {
			updateXVelocity(rigidbody2D.velocity.x * 0.9f);
			yield return new WaitForSeconds(0.1f);
		}
		updateXVelocity(0);
	}

	#region INHALING
	private IEnumerator InhalingEnterState() {
		inhaleArea.SetActive(true);
		StartCoroutine("ComeToHalt");
		yield return null;
	}

	private IEnumerator InhalingExitState() {
		inhaleArea.SetActive(false);
		yield return null;
	}
		
	public void InhalingUpdate() {
		if (!Input.GetKey(KeyCode.Z) && am.SubState != (int) Inhaling.FinishInhaling) {
			CurrentState = State.IdleOrWalking;
		}
	}

	private void InhalingOnCollisionEnter2D(Collision2D other) {
		if (other.gameObject.tag == "enemy") {
			inhaledEnemy = other.gameObject.GetComponent<EnemyBase>();
			enemyOther = other.gameObject;
			Destroy(enemyOther);
			am.animate((int) Inhaling.FinishInhaling);
		}
	}
	#endregion

	public void OnFinishedInhaling() {
		CurrentState = Kirby.State.Inhaled;
	}

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

	public void TakeHit(GameObject particle) {
		if (invulnurable) {
			return;
		}
		TakeDamage();
		enemyOther = particle;
		CurrentState = (inhaledEnemy == null) ? State.Knockback : State.InhaledKnockback;
		StartCoroutine("Invulnerability");
	}

	public IEnumerator Invulnerability() {
		invulnurable = true;
		yield return new WaitForSeconds (2f);
		invulnurable = false;
	}
}
