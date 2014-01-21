using UnityEngine;
using System.Collections;

public abstract class CharacterBase : StateMachineBase {
	public Direction dir;
	public enum Direction {
		Left, Right
	}

	protected void Flip() {
		dir = (dir == Direction.Right) ? Direction.Left : Direction.Right;
		
		// Flip the sprite over the anchor point
		Transform sprite = transform.Find ("Sprite");
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

}
