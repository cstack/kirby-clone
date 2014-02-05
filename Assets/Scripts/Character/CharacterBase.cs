using UnityEngine;
using System.Collections;

public abstract class CharacterBase : StateMachineBase {
	public Direction dir;
	public enum Direction {
		Left, Right
	}
	public Ability ability;

	private Transform sprite;

	public void Start() {
		sprite = transform.Find("Sprite");
	}

	protected IEnumerator UseAbility() {
		yield return StartCoroutine(UseAbility(false));
	}

	protected IEnumerator UseAbility(bool friendly) {
		Ability attack = Instantiate (ability) as Ability;
		attack.friendly = friendly;
		if (dir == Direction.Right) {
			attack.faceRight = true;
		}
		attack.transform.parent = transform;
		attack.transform.localPosition = new Vector3 (0.5f, 0.5f, 0f);
		attack.init();
		yield return new WaitForSeconds(attack.getDuration());
		Destroy (attack.gameObject);
		OnAbilityFinished();
	}

	protected virtual void OnAbilityFinished() {}

	protected void Flip() {
		dir = (dir == Direction.Right) ? Direction.Left : Direction.Right;
		
		// Flip the sprite over the anchor point
		Vector3 scale = sprite.localScale;
		scale.x *= -1;
		sprite.localScale = scale;
		
		/*
		 * Since the flip flips over the anchor point which is the bottom left of the sprite, we need to shift the
		 * sprite to make it look like we flipped over the vertical center axis of the sprite.
		 */
		Vector3 position = sprite.position;
		position.x -= scale.x;
		sprite.position = position;
	}

	protected void updateXVelocity(float x) {
		Vector2 vel = rigidbody2D.velocity;
		vel.x = x;
		rigidbody2D.velocity = vel;
	}
	
	protected void updateYVelocity(float y) {
		Vector2 vel = rigidbody2D.velocity;
		vel.y = y;
		rigidbody2D.velocity = vel;
	}

	protected int forwardRaycast(RaycastHit2D[] hits, float range) {
		float delta = 0.1f;
		Vector3 origin = transform.position + new Vector3 (0, 0.5f, 0);
		if (dir == Direction.Right) {
			origin += new Vector3(1, 0, 0);
		}
		origin += delta * Vector3.right * (dir == Direction.Right ? 1 : -1);
		return Physics2D.RaycastNonAlloc (origin, rigidbody2D.velocity, hits, range);
	}

	protected IEnumerator SlowDown(float seconds) {
		int iterations = 5;
		float delay = seconds/iterations;
		for (int i = 0; i < iterations; i++) {
			updateXVelocity(rigidbody2D.velocity.x * 0.5f);
			yield return new WaitForSeconds(delay);
		}
		updateXVelocity(0f);
	}


}
