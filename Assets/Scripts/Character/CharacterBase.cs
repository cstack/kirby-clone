using UnityEngine;
using System.Collections;

public abstract class CharacterBase : StateMachineBase {
	public Direction dir;
	public enum Direction {
		Left, Right
	}
	public bool slowingDown;
	public Ability ability;
	protected Ability attack;

	protected Transform sprite;

	public void Start() {
		sprite = transform.Find("Sprite");
	}

	protected void UseAbility() {
		UseAbility(false);
	}

	protected void UseAbility(bool friendly) {
		attack = Instantiate (ability) as Ability;
		attack.friendly = friendly;
		attack.transform.parent = transform;
		attack.transform.localPosition = new Vector3 (0.5f, 0.5f, 0f);
		if (dir == Direction.Right) {
			attack.faceRight = true;
		}
		attack.init();
	}

	protected void StopAbility() {
		if (attack != null) {
			Destroy (attack.gameObject);
			attack = null;
		}
	}

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
		updateXVelocity(rigidbody2D, x);
	}

	protected void updateXVelocity(Rigidbody2D obj, float x) {
		Vector2 vel = obj.velocity;
		vel.x = x;
		obj.velocity = vel;
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
		yield return StartCoroutine(SlowDown(this.rigidbody2D, seconds));
	}

	protected IEnumerator SlowDown(Rigidbody2D obj, float seconds) {
		slowingDown = true;
		float velX = rigidbody2D.velocity.x;
		int iterations = 20;
		float delay = seconds/iterations;
		for (int i = 0; i < iterations; i++) {
			if (obj == null) continue;
			velX *= 0.5f;
			updateXVelocity(obj, velX);
			yield return new WaitForSeconds(delay);
		}

		if (obj != null) {
			updateXVelocity(obj, 0f);
		}
		slowingDown = false;
	}
}
