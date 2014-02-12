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
	public bool inhaleStarted = false;

	public Sprite defaultAbilityCard;

	public GameObject airPrefab;

	public static int score = 0;

	public static int health = 6;
	public static int livesRemaining = 4;

	public StarProjectile starProjectilePrefab;
	public GameObject slideSmokePrefab;
	public AbilityStar abilityStarPrefab;

	private bool isSpinning = false;
	private GameObject inhaleArea;

	public Ability inhaledAbility;
	public static Ability persistantAbility; 
	public static Sprite abilityCard;

	public bool inhaledEnemy;
	public bool onDoor;

	bool invulnurable;

	private Animator animator;

	// TODO: This is a bad way of doing this. See KnockbackEnterState
	private GameObject enemyOther;

	public enum State {
		IdleOrWalking, Jumping, Flying, Knockback, Ducking, Sliding, Inhaling, Inhaled, Die,
		InhaledJumping, InhaledKnockback, Shooting, Swallowing, UsingAbility, Frozen
	}
	
	new public void Start() {
		base.Start();
		GameObject.Find("LivesRemaining").GetComponent<LivesRemaining>().setLivesRemaining(livesRemaining);
		GameObject.Find("Score").GetComponent<Score>().updateScore(Kirby.score);
		for (int i = 6; i > health; i--) {
			RemoveHealthBarItem(i);
		}
		ability = persistantAbility;
		if (abilityCard != null) {
			GameObject.Find("Ability").GetComponent<SpriteRenderer>().sprite = abilityCard;
		}

		animator = GetComponentInChildren<Animator>();
		CurrentState = State.Jumping;
		dir = Direction.Right;
		inhaleArea = transform.Find("Sprite/InhaleArea").gameObject;
		inhaleArea.SetActive(false);
	}

	// Called on collide from inhaling
	public void enemyCollisionCallback(GameObject enemy) {
		EnemyBase enemyBase = enemy.GetComponent<EnemyBase>();
		InhaleAbility(enemyBase.ability, enemyBase.abilityCard);
		EnemyBase.killEnemy(enemy, false);
	}

	public void InhaleAbility(Ability ability, Sprite abilityCard) {
		Kirby.abilityCard = abilityCard;
		inhaledAbility = ability;
		inhaledEnemy = true;
		CurrentState = Kirby.State.Inhaled;
	}

	private void OnCollideWithEnemy(GameObject enemy) {
		enemyOther = enemy;
		EnemyBase.killEnemy(enemy, true);
		if (invulnurable) {
			return;
		}
		TakeDamage();
		LoseAbility();
		StartCoroutine(Invulnerability());
		CurrentState = (inhaledEnemy == false) ? State.Knockback : State.InhaledKnockback;
	}

	private void CommonOnCollisionEnter2D(Collision2D other) {
		if (other.gameObject.tag == "enemy") {
			OnCollideWithEnemy(other.gameObject);
		}
	}

	private IEnumerator ShowSmoke() {
		int smokeDir = dir == Direction.Right ? -1 : 1;
		GameObject smoke = CreateSmoke(smokeDir * 2);
		yield return new WaitForSeconds(0.2f);
		Destroy(smoke);
	}

	private void HandleHorizontalMovement(ref Vector2 vel) {
		float h = Input.GetAxis("Horizontal");
		if ((h > 0 && dir != Direction.Right) || (h < 0 && dir != Direction.Left)) {
			if (CurrentState.ToString() == State.IdleOrWalking.ToString()) {
				StartCoroutine(ShowSmoke());
			}
			Flip();
		}
		vel.x = h * speed;
	}

	private void CommonUpdate() {
		if (Input.GetKeyDown(KeyCode.LeftControl) ||
		    Input.GetKeyDown(KeyCode.RightControl)) {
			LoseAbility();
		}
	}
	
	#region IDLE_OR_WALKING

	private void IdleOrWalkingUpdate() {
		CommonUpdate();
		Vector2 vel = rigidbody2D.velocity;
		HandleHorizontalMovement(ref vel);
		if (Input.GetKeyDown(KeyCode.X)) {
			vel.y = jumpSpeed;
			CurrentState = State.Jumping;
		} else if (Input.GetKey(KeyCode.Z) && ability == null) {
			CurrentState = State.Inhaling;
		} else if (Input.GetKeyDown(KeyCode.Z) && ability != null) {
			CurrentState = State.UsingAbility;
		} else if (Input.GetKeyDown(KeyCode.UpArrow) && !onDoor) {
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
		CommonUpdate();
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
		ability = inhaledAbility;
		persistantAbility = ability;
		inhaledAbility = null;
		inhaledEnemy = false;
		if (abilityCard != null) {
			GameObject.Find("Ability").GetComponent<SpriteRenderer>().sprite = abilityCard;
		}
		yield return new WaitForSeconds (0.5f);
		CurrentState = State.IdleOrWalking;
	}

	public void SwallowingUpdate() {
		updateXVelocity(0f);
	}
	
	#endregion

	#region JUMPING

	private void JumpingUpdate() {
		CommonUpdate();
		Vector2 vel = rigidbody2D.velocity;
		HandleHorizontalMovement(ref vel);
		if (Input.GetKeyDown(KeyCode.UpArrow) && !onDoor) {
			CurrentState = State.Flying;
		} else {
			if (Input.GetKeyUp(KeyCode.X)) {
				vel.y = Mathf.Min(vel.y, 0);
			} else if (Input.GetKeyDown(KeyCode.Z)) {
				Debug.Log("Pressed");
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
			if (other.contacts.Length > 0 && rigidbody2D.velocity.y <= 0 &&
			    Vector2.Dot(other.contacts[0].normal, Vector2.up) > 0.5) {
				// Collision was on bottom
				CurrentState = State.IdleOrWalking;
			}
		}
	}

	#endregion

	#region Shooting

	private IEnumerator ShootingEnterState() {
		StarProjectile star = Instantiate(starProjectilePrefab) as StarProjectile;
		star.gameObject.transform.position = transform.position + Vector3.up * 0.1f; // Don't touch the ground
		star.direction = (dir == Direction.Right ? 1 : -1);
		inhaledAbility = null;
		inhaledEnemy = false;
		yield return new WaitForSeconds(0.5f);
		CurrentState = State.Jumping;
	}

	#endregion

	#region Shooting
	
	private IEnumerator UsingAbilityEnterState() {
		UseAbility(true);
		StartCoroutine(SlowDown(0.5f));
		yield return null;
	}

	private void UsingAbilityUpdate() {
		if (!Input.GetKey(KeyCode.Z) || attack == null) {
			CurrentState = State.IdleOrWalking;
		}
	}

	private IEnumerator UsingAbilityExitState() {
		StopAbility();
		yield return null;
	}
	
	#endregion

	#region InhaledJumping
	
	private void InhaledJumpingUpdate() {
		CommonUpdate();
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
		animator.speed = 1f;
		yield break;
	}

	private void FlyingUpdate() {
		CommonUpdate();
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
		GameObject air = Instantiate(airPrefab) as GameObject;
		air.transform.parent = transform;
		air.transform.position = transform.position;

		if (dir == Direction.Left) {
			Vector2 scale = air.transform.localScale;
			scale.x *= -1;
			air.transform.localScale = scale;

		}
		Vector2 pos = air.transform.position;
		pos += Vector2.up * 0.4f;
		air.transform.position = pos;

		int airDir = Direction.Right == dir ? 1 : -1;
		air.rigidbody2D.velocity = Vector2.right * airDir * 35;
		StartCoroutine(SlowDown(air.rigidbody2D, 0.4f));
		am.animate((int) Flying.Exhaling);
		yield return new WaitForSeconds(0.4f);
		
		CurrentState = State.Jumping;
		isExhaling = false;

		if (air != null) {
			Destroy(air);
		}
	}
	
	private void FlyingOnCollisionEnter2D(Collision2D other) {
		CommonOnCollisionEnter2D(other);
	}

	#endregion

	private void RemoveHealthBarItem(int healthItem) {
		GameObject go = GameObject.Find("HealthBarItem" + healthItem);
		Animator animator = go.GetComponent<Animator>();
		animator.SetBool("Remove", true);
	}

	private void TakeDamage() {
		RemoveHealthBarItem(health);
		health -= 1;
		if (health == 0) {
			CurrentState = State.Die;
			health = 6;
			return;
		}
	}

	private void LoseAbility() {
		if (ability != null) {
			AbilityStar star = Instantiate(abilityStarPrefab) as AbilityStar;
			star.ability = ability;
			star.abilityCard = abilityCard;
			ability = null;
			persistantAbility = null;
			star.transform.position = transform.position;
			if (enemyOther != null && enemyOther.transform.position.x < transform.position.x) {
				star.goRight = true;
			}

			abilityCard = null;
			GameObject.Find("Ability").GetComponent<SpriteRenderer>().sprite = defaultAbilityCard;
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

	#region Ducking

	public IEnumerator DuckingEnterState() {
		StartCoroutine(SlowDown(0.5f));
		yield return null;
	}

	public void DuckingUpdate() {
		CommonUpdate();
		if (!slowingDown) {
			updateXVelocity(0f);
		}
		if (!Input.GetKey(KeyCode.DownArrow)) {
			CurrentState = State.IdleOrWalking;
		}
		if (Input.GetKey(KeyCode.UpArrow)) {
			CurrentState = State.Flying;
		}
		if (Input.GetKeyDown(KeyCode.Z) || Input.GetKeyDown(KeyCode.X)) {
			CurrentState = State.Sliding;
		}
	}

	#endregion

	#region Sliding

	GameObject CreateSmoke(int smokeDir) {
		GameObject smoke = Instantiate(slideSmokePrefab) as GameObject;
		smoke.transform.parent = transform;

		Vector3 offset = 0.5f * Vector3.left * smokeDir;
		if (Direction.Left == dir) {
			offset += Vector3.right * 0.5f;
		}
		
		smoke.transform.position = transform.position + offset;

		return smoke;
	}
	public IEnumerator SlidingEnterState() {
		int slideDir = dir == Direction.Right ? 1 : -1;
		GameObject smoke = CreateSmoke(slideDir);

		updateXVelocity(11 * slideDir);
		yield return new WaitForSeconds(0.4f);
		StartCoroutine(SlowDown(0.2f));
		yield return new WaitForSeconds(0.2f);
		CurrentState = State.Ducking;
		Destroy(smoke);
	}

	private void SlidingOnCollisionEnter2D(Collision2D other) {
		if (other.gameObject.tag == "enemy") {
			EnemyBase.killEnemy(other.gameObject, true);
		}
	}

	#endregion
	
	#region Inhaling

	private IEnumerator InhalingEnterState() {
		Physics.IgnoreLayerCollision(LayerMask.NameToLayer("kirby"), LayerMask.NameToLayer("enemy"));
		inhaleArea.SetActive(true);
		StartCoroutine(SlowDown(0.5f));
		yield return new WaitForSeconds(0.5f);
		inhaleStarted = true;
	}

	private IEnumerator InhalingExitState() {
		Physics.IgnoreLayerCollision(LayerMask.NameToLayer("kirby"), LayerMask.NameToLayer("enemy"), false);
		inhaleArea.SetActive(false);
		inhaleStarted = false;
		yield break;
	}
		
	public void InhalingUpdate() {
		CommonUpdate();
		if (!slowingDown) {
			updateXVelocity(0f);
		}
		if (inhaleStarted && !Input.GetKey(KeyCode.Z)) {
			CurrentState = State.IdleOrWalking;
		}
	}

	public void OnFinishedInhaling() {
		CurrentState = Kirby.State.Inhaled;
	}

	#endregion

	#region DIE

	private IEnumerator DieEnterState() {
		livesRemaining -= 1;
		if (livesRemaining < 0) {
			Application.Quit(); // TODO
		} else {
			Application.LoadLevel(Application.loadedLevelName);
		}
		yield break;
	}

	#endregion

	public void TakeHit(GameObject particle) {
		if (invulnurable) {
			return;
		}
		enemyOther = particle;
		CurrentState = (inhaledEnemy == false) ? State.Knockback : State.InhaledKnockback;
		TakeDamage();
		LoseAbility();
		StartCoroutine(Invulnerability());
	}

	public IEnumerator Invulnerability() {
		invulnurable = true;
		float duration = 2f;
		float timestep = 0.1f;
		for (float time = 0f; time < duration; time += timestep*2) {
			sprite.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 0.5f, 1f);
			yield return new WaitForSeconds (timestep);
			sprite.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 1f);
			yield return new WaitForSeconds(timestep);
		}
		invulnurable = false;
	}

	public void OnBossDeath() {
		CurrentState = State.Frozen;
	}
}
